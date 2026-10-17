using System.Data;
using System.Globalization;
using ClosedXML.Excel;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace App.BLL;

public class ResultadoImportacion
{
    public int Exitosos { get; set; }
    public int Fallidos { get; set; }
    public List<string> Errores { get; } = new();
}

public class ImportExportService
{
    private readonly MiembroService _miembroService = new();

    // ---------------- IMPORTAR CSV ----------------

    /// <summary>
    /// Lee el CSV y arma una vista previa (DataTable) validando la estructura
    /// esperada: Nombre;Documento;Telefono;Correo;FechaIngreso(yyyy-MM-dd)
    /// Los renglones con formato invalido quedan marcados en 'Errores' pero
    /// no detienen la vista previa del resto de filas validas.
    /// </summary>
    public DataTable GenerarVistaPreviaCsv(string rutaArchivo, out List<string> errores)
    {
        errores = new List<string>();
        var tabla = new DataTable();
        tabla.Columns.Add("Nombre", typeof(string));
        tabla.Columns.Add("Documento", typeof(string));
        tabla.Columns.Add("Telefono", typeof(string));
        tabla.Columns.Add("Correo", typeof(string));
        tabla.Columns.Add("FechaIngreso", typeof(string));
        tabla.Columns.Add("Valido", typeof(bool));

        var lineas = File.ReadAllLines(rutaArchivo);
        if (lineas.Length == 0)
        {
            errores.Add("El archivo esta vacio.");
            return tabla;
        }

        // Se asume que la primera linea es encabezado (Nombre;Documento;Telefono;Correo;FechaIngreso)
        for (int i = 1; i < lineas.Length; i++)
        {
            var linea = lineas[i];
            if (string.IsNullOrWhiteSpace(linea)) continue;

            var campos = linea.Split(';');
            bool esValido = true;
            string motivo = string.Empty;

            if (campos.Length < 5)
            {
                esValido = false;
                motivo = "Faltan columnas";
            }
            else if (string.IsNullOrWhiteSpace(campos[0]) || string.IsNullOrWhiteSpace(campos[1]))
            {
                esValido = false;
                motivo = "Nombre o documento vacio";
            }
            else if (!DateTime.TryParseExact(campos[4].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                esValido = false;
                motivo = "Fecha invalida (use yyyy-MM-dd)";
            }

            if (!esValido)
                errores.Add($"Fila {i + 1}: {motivo}");

            tabla.Rows.Add(
                campos.Length > 0 ? campos[0] : string.Empty,
                campos.Length > 1 ? campos[1] : string.Empty,
                campos.Length > 2 ? campos[2] : string.Empty,
                campos.Length > 3 ? campos[3] : string.Empty,
                campos.Length > 4 ? campos[4] : string.Empty,
                esValido);
        }

        return tabla;
    }

    /// <summary>Inserta unicamente las filas marcadas como validas de la vista previa ya confirmada por el usuario.</summary>
    public ResultadoImportacion ConfirmarImportacionMiembros(DataTable vistaPrevia)
    {
        var resultado = new ResultadoImportacion();

        foreach (DataRow fila in vistaPrevia.Rows)
        {
            bool esValida = fila.Table.Columns.Contains("Valido") && (bool)fila["Valido"];
            if (!esValida)
            {
                resultado.Fallidos++;
                continue;
            }

            try
            {
                var miembro = new Entidades.Miembro(
                    fila["Nombre"].ToString()!.Trim(),
                    fila["Documento"].ToString()!.Trim(),
                    fila["Telefono"].ToString() ?? string.Empty,
                    fila["Correo"].ToString() ?? string.Empty,
                    DateTime.ParseExact(fila["FechaIngreso"].ToString()!.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture));

                var errores = _miembroService.Validar(miembro);
                if (errores.Count > 0)
                {
                    resultado.Fallidos++;
                    resultado.Errores.Add($"{miembro.Documento}: {string.Join(", ", errores)}");
                    continue;
                }

                _miembroService.Guardar(miembro);
                resultado.Exitosos++;
            }
            catch (Exception ex)
            {
                resultado.Fallidos++;
                resultado.Errores.Add(ex.Message);
            }
        }

        return resultado;
    }

    // ---------------- EXPORTAR EXCEL ----------------

    public void ExportarExcel(DataTable datos, string rutaArchivo, string nombreHoja = "Reporte")
    {
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add(nombreHoja);
        hoja.Cell(1, 1).InsertTable(datos);
        hoja.Columns().AdjustToContents();
        libro.SaveAs(rutaArchivo);
    }

    /// <summary>Exporta a Excel incrustando la imagen del grafico arriba de la tabla de datos.</summary>
    public void ExportarExcelConGrafico(byte[] imagenPng, DataTable datos, string rutaArchivo, string nombreHoja = "Reporte")
    {
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add(nombreHoja);

        using (var msImagen = new MemoryStream(imagenPng))
        {
            hoja.AddPicture(msImagen)
                .MoveTo(hoja.Cell(1, 1))
                .Scale(0.75);
        }

        // Deja espacio debajo de la imagen (~22 filas) antes de la tabla de datos.
        int filaTabla = 24;
        hoja.Cell(filaTabla, 1).InsertTable(datos);
        hoja.Columns().AdjustToContents();
        libro.SaveAs(rutaArchivo);
    }

    // ---------------- EXPORTAR PDF ----------------

    /// <summary>Exporta a PDF incrustando la imagen del grafico arriba de la tabla de datos.</summary>
    public void ExportarPdfConGrafico(string titulo, byte[] imagenPng, DataTable datos, string rutaArchivo)
    {
        using var documento = new PdfDocument();
        var fuenteTitulo = new XFont("Arial", 16, XFontStyleEx.Bold);
        var fuenteEncabezado = new XFont("Arial", 10, XFontStyleEx.Bold);
        var fuenteTexto = new XFont("Arial", 9, XFontStyleEx.Regular);

        int columnas = datos.Columns.Count;

        PdfPage pagina = documento.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(pagina);
        double anchoPagina = pagina.Width.Point;
        double altoPagina = pagina.Height.Point;
        double anchoUtil = anchoPagina - 80;
        double anchoColumna = anchoUtil / Math.Max(columnas, 1);
        double y = 40;

        gfx.DrawString(titulo, fuenteTitulo, XBrushes.Black, new XPoint(40.0, y));
        y += 30;

        // Imagen del grafico, respetando su proporcion original.
        // El stream se mantiene abierto hasta despues de documento.Save() porque
        // PdfSharp puede leer los bytes de la imagen de forma diferida.
        using var msImagen = new MemoryStream(imagenPng);
        XImage imagen = XImage.FromStream(msImagen);
        double altoImagen = anchoUtil * (imagen.PixelHeight / (double)imagen.PixelWidth);
        gfx.DrawImage(imagen, 40.0, y, anchoUtil, altoImagen);
        y += altoImagen + 20;

        void DibujarEncabezados()
        {
            double x = 40;
            for (int c = 0; c < columnas; c++)
            {
                gfx.DrawString(datos.Columns[c].ColumnName, fuenteEncabezado, XBrushes.Black,
                    new XRect(x, y, anchoColumna, 20), XStringFormats.TopLeft);
                x += anchoColumna;
            }
            y += 22;
            gfx.DrawLine(XPens.Black, 40.0, y, 40.0 + anchoUtil, y);
            y += 6;
        }

        DibujarEncabezados();

        foreach (DataRow fila in datos.Rows)
        {
            if (y > altoPagina - 60)
            {
                pagina = documento.AddPage();
                gfx = XGraphics.FromPdfPage(pagina);
                altoPagina = pagina.Height.Point;
                y = 40;
                DibujarEncabezados();
            }

            double x = 40;
            for (int c = 0; c < columnas; c++)
            {
                gfx.DrawString(fila[c]?.ToString() ?? string.Empty, fuenteTexto, XBrushes.Black,
                    new XRect(x, y, anchoColumna, 18), XStringFormats.TopLeft);
                x += anchoColumna;
            }
            y += 18;
        }

        documento.Save(rutaArchivo);
    }

    public void ExportarPdf(string titulo, DataTable datos, string rutaArchivo)
    {
        using var documento = new PdfDocument();
        var fuenteTitulo = new XFont("Arial", 16, XFontStyleEx.Bold);
        var fuenteEncabezado = new XFont("Arial", 10, XFontStyleEx.Bold);
        var fuenteTexto = new XFont("Arial", 9, XFontStyleEx.Regular);

        int columnas = datos.Columns.Count;

        PdfPage pagina = documento.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(pagina);
        double anchoUtil = pagina.Width.Point - 80;
        double anchoColumna = anchoUtil / Math.Max(columnas, 1);
        double y = 40;

        void DibujarTitulo()
        {
            gfx.DrawString(titulo, fuenteTitulo, XBrushes.Black, new XPoint(40, y));
            y += 30;
        }

        void DibujarEncabezados()
        {
            double x = 40;
            for (int c = 0; c < columnas; c++)
            {
                gfx.DrawString(datos.Columns[c].ColumnName, fuenteEncabezado, XBrushes.Black,
                    new XRect(x, y, anchoColumna, 20), XStringFormats.TopLeft);
                x += anchoColumna;
            }
            y += 22;
            gfx.DrawLine(XPens.Black, 40, y, 40 + anchoUtil, y);
            y += 6;
        }

        DibujarTitulo();
        DibujarEncabezados();

        foreach (DataRow fila in datos.Rows)
        {
            if (y > pagina.Height.Point - 60)
            {
                pagina = documento.AddPage();
                gfx = XGraphics.FromPdfPage(pagina);
                y = 40;
                DibujarEncabezados();
            }

            double x = 40;
            for (int c = 0; c < columnas; c++)
            {
                gfx.DrawString(fila[c]?.ToString() ?? string.Empty, fuenteTexto, XBrushes.Black,
                    new XRect(x, y, anchoColumna, 18), XStringFormats.TopLeft);
                x += anchoColumna;
            }
            y += 18;
        }

        documento.Save(rutaArchivo);
    }
}
