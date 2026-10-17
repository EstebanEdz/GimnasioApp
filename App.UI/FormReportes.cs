using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using App.BLL;

namespace App.UI;

public partial class FormReportes : Form
{
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabSencillo;
    private System.Windows.Forms.TabPage tabGrafico;
    private System.Windows.Forms.TabPage tabCalculado;
    private System.Windows.Forms.DataGridView dgvSencillo;
    private Chart chartInscripciones;
    private System.Windows.Forms.Label lblDesde;
    private System.Windows.Forms.Label lblHasta;
    private System.Windows.Forms.DateTimePicker dtpDesde;
    private System.Windows.Forms.DateTimePicker dtpHasta;
    private System.Windows.Forms.Button btnCalcularIngresos;
    private System.Windows.Forms.Label lblTotalIngresos;
    private System.Windows.Forms.DataGridView dgvIngresos;
    private System.Windows.Forms.Button btnExportar;
    private System.Windows.Forms.ProgressBar progressBar1;

    private readonly ReporteService _reporteService = new();
    private readonly ImportExportService _importExport = new();
    private DataTable _dtActual = new();

    public FormReportes(int pestanaInicial = 0)
    {
        InitializeComponent();
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabSencillo = new System.Windows.Forms.TabPage();
        this.tabGrafico = new System.Windows.Forms.TabPage();
        this.tabCalculado = new System.Windows.Forms.TabPage();
        this.dgvSencillo = new System.Windows.Forms.DataGridView();
        this.chartInscripciones = new Chart();
        this.lblDesde = new System.Windows.Forms.Label();
        this.lblHasta = new System.Windows.Forms.Label();
        this.dtpDesde = new System.Windows.Forms.DateTimePicker();
        this.dtpHasta = new System.Windows.Forms.DateTimePicker();
        this.btnCalcularIngresos = new System.Windows.Forms.Button();
        this.lblTotalIngresos = new System.Windows.Forms.Label();
        this.dgvIngresos = new System.Windows.Forms.DataGridView();
        this.btnExportar = new System.Windows.Forms.Button();
        this.progressBar1 = new System.Windows.Forms.ProgressBar();
        ((System.ComponentModel.ISupportInitialize)(this.dgvSencillo)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.chartInscripciones)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvIngresos)).BeginInit();
        this.tabControl1.SuspendLayout();
        this.tabSencillo.SuspendLayout();
        this.tabGrafico.SuspendLayout();
        this.tabCalculado.SuspendLayout();
        this.SuspendLayout();

        // ---- Tab Sencillo ----
        this.dgvSencillo.Dock = System.Windows.Forms.DockStyle.Fill;
        this.dgvSencillo.ReadOnly = true;
        this.dgvSencillo.AllowUserToAddRows = false;
        this.dgvSencillo.RowHeadersVisible = false;
        this.dgvSencillo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.tabSencillo.Controls.Add(this.dgvSencillo);
        this.tabSencillo.Text = "Miembros por Plan (sencillo)";
        this.tabSencillo.Padding = new System.Windows.Forms.Padding(8);

        // ---- Tab Grafico ----
        var area = new ChartArea("area1");
        this.chartInscripciones.ChartAreas.Add(area);
        this.chartInscripciones.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabGrafico.Controls.Add(this.chartInscripciones);
        this.tabGrafico.Text = "Inscripciones por Mes (grafico)";
        this.tabGrafico.Padding = new System.Windows.Forms.Padding(8);

        // ---- Tab Calculado ----
        this.lblDesde.AutoSize = true;
        this.lblDesde.Location = new System.Drawing.Point(10, 15);
        this.lblDesde.Text = "Desde";
        this.dtpDesde.Location = new System.Drawing.Point(60, 12);
        this.dtpDesde.Size = new System.Drawing.Size(120, 20);
        this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;

        this.lblHasta.AutoSize = true;
        this.lblHasta.Location = new System.Drawing.Point(200, 15);
        this.lblHasta.Text = "Hasta";
        this.dtpHasta.Location = new System.Drawing.Point(250, 12);
        this.dtpHasta.Size = new System.Drawing.Size(120, 20);
        this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;

        this.btnCalcularIngresos.Location = new System.Drawing.Point(390, 10);
        this.btnCalcularIngresos.Size = new System.Drawing.Size(120, 26);
        this.btnCalcularIngresos.Text = "Calcular";
        this.btnCalcularIngresos.Click += new System.EventHandler(this.btnCalcularIngresos_Click);

        this.lblTotalIngresos.AutoSize = true;
        this.lblTotalIngresos.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
        this.lblTotalIngresos.Location = new System.Drawing.Point(530, 15);
        this.lblTotalIngresos.Text = "Total del rango: $0";

        this.dgvIngresos.Location = new System.Drawing.Point(10, 45);
        this.dgvIngresos.Size = new System.Drawing.Size(760, 340);
        this.dgvIngresos.ReadOnly = true;
        this.dgvIngresos.AllowUserToAddRows = false;
        this.dgvIngresos.RowHeadersVisible = false;
        this.dgvIngresos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

        this.tabCalculado.Controls.Add(this.lblDesde);
        this.tabCalculado.Controls.Add(this.dtpDesde);
        this.tabCalculado.Controls.Add(this.lblHasta);
        this.tabCalculado.Controls.Add(this.dtpHasta);
        this.tabCalculado.Controls.Add(this.btnCalcularIngresos);
        this.tabCalculado.Controls.Add(this.lblTotalIngresos);
        this.tabCalculado.Controls.Add(this.dgvIngresos);
        this.tabCalculado.Text = "Ingresos por Rango (calculado)";
        this.tabCalculado.Padding = new System.Windows.Forms.Padding(8);

        this.tabControl1.Controls.Add(this.tabSencillo);
        this.tabControl1.Controls.Add(this.tabGrafico);
        this.tabControl1.Controls.Add(this.tabCalculado);
        this.tabControl1.Location = new System.Drawing.Point(10, 10);
        this.tabControl1.Size = new System.Drawing.Size(800, 430);

        this.btnExportar.Location = new System.Drawing.Point(10, 450);
        this.btnExportar.Size = new System.Drawing.Size(160, 30);
        this.btnExportar.Text = "Exportar (Excel/PDF)...";
        this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);

        this.progressBar1.Location = new System.Drawing.Point(180, 456);
        this.progressBar1.Size = new System.Drawing.Size(300, 18);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(824, 496);
        this.Controls.Add(this.tabControl1);
        this.Controls.Add(this.btnExportar);
        this.Controls.Add(this.progressBar1);
        this.Name = "FormReportes";
        this.Text = "Reportes";
        ((System.ComponentModel.ISupportInitialize)(this.dgvSencillo)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.chartInscripciones)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvIngresos)).EndInit();
        this.tabSencillo.ResumeLayout(false);
        this.tabGrafico.ResumeLayout(false);
        this.tabCalculado.ResumeLayout(false);
        this.tabCalculado.PerformLayout();
        this.tabControl1.ResumeLayout(false);
        this.ResumeLayout(false);
    
        dtpDesde.Value = DateTime.Today.AddMonths(-6);
        dtpHasta.Value = DateTime.Today;

        GenerarReporteSencillo();
        GenerarReporteGrafico();

        SeleccionarPestana(pestanaInicial);
    }

    public void SeleccionarPestana(int indice)
    {
        if (indice >= 0 && indice < tabControl1.TabCount)
            tabControl1.SelectedIndex = indice;
    }

    // ---------------- REPORTE SENCILLO: Miembros por Plan ----------------
    private void GenerarReporteSencillo()
    {
        var datos = _reporteService.MiembrosPorPlan();
        var dt = new DataTable();
        dt.Columns.Add("Plan", typeof(string));
        dt.Columns.Add("Cantidad de inscripciones", typeof(int));
        foreach (var d in datos) dt.Rows.Add(d.Plan, d.Cantidad);
        dgvSencillo.DataSource = dt;
    }

    // ---------------- REPORTE CON GRAFICO: Inscripciones por Mes ----------------
    private void GenerarReporteGrafico()
    {
        var datos = _reporteService.InscripcionesPorMes();

        chartInscripciones.Series.Clear();
        chartInscripciones.ChartAreas.Clear();
        chartInscripciones.ChartAreas.Add(new ChartArea("area1"));

        var serie = new Series("Inscripciones")
        {
            ChartType = SeriesChartType.Column,
            IsValueShownAsLabel = true
        };
        foreach (var d in datos)
            serie.Points.AddXY(d.Mes, d.Cantidad);

        chartInscripciones.Series.Add(serie);
    }

    // ---------------- REPORTE CALCULADO: Ingresos por Rango de Fechas ----------------
    private void btnCalcularIngresos_Click(object sender, EventArgs e)
    {
        if (dtpDesde.Value.Date > dtpHasta.Value.Date)
        {
            MessageBox.Show("La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.",
                "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var (detalle, total) = _reporteService.IngresosPorRango(dtpDesde.Value, dtpHasta.Value);

        var dt = new DataTable();
        dt.Columns.Add("Fecha", typeof(string));
        dt.Columns.Add("Miembro", typeof(string));
        dt.Columns.Add("Plan", typeof(string));
        dt.Columns.Add("Total", typeof(string));
        foreach (var d in detalle)
            dt.Rows.Add(d.Fecha.ToString("yyyy-MM-dd"), d.Miembro, d.Plan, d.Total.ToString("N0"));

        dgvIngresos.DataSource = dt;
        lblTotalIngresos.Text = $"Total del rango: ${total:N0}";
    }

    // ---------------- EXPORTAR (comun a las 3 pestañas) ----------------
    private void btnExportar_Click(object sender, EventArgs e)
    {
        DataTable dt;
        string titulo;

        switch (tabControl1.SelectedIndex)
        {
            case 0:
                dt = (DataTable)dgvSencillo.DataSource;
                titulo = "Miembros por Plan";
                break;
            case 1:
                dt = ObtenerDataTableDesdeGrafico();
                titulo = "Inscripciones por Mes";
                break;
            default:
                dt = dgvIngresos.DataSource as DataTable ?? new DataTable();
                titulo = "Ingresos por Rango de Fechas";
                break;
        }

        if (dt.Rows.Count == 0)
        {
            MessageBox.Show("No hay datos para exportar. Genere el reporte primero.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var saveDialog = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx|PDF (*.pdf)|*.pdf", FileName = titulo };
        if (saveDialog.ShowDialog() != DialogResult.OK) return;

        progressBar1.Value = 0;
        for (int i = 0; i <= 100; i += 20) { progressBar1.Value = i; Application.DoEvents(); }

        try
        {
            if (tabControl1.SelectedIndex == 1)
            {
                // Pestaña con grafico: se exporta la imagen del Chart + la tabla de datos.
                using var msImagen = new MemoryStream();
                chartInscripciones.SaveImage(msImagen, ChartImageFormat.Png);
                byte[] imagenPng = msImagen.ToArray();

                if (saveDialog.FilterIndex == 2)
                    _importExport.ExportarPdfConGrafico(titulo, imagenPng, dt, saveDialog.FileName);
                else
                    _importExport.ExportarExcelConGrafico(imagenPng, dt, saveDialog.FileName, titulo);
            }
            else if (saveDialog.FilterIndex == 2)
            {
                _importExport.ExportarPdf(titulo, dt, saveDialog.FileName);
            }
            else
            {
                _importExport.ExportarExcel(dt, saveDialog.FileName, titulo);
            }

            MessageBox.Show("Reporte exportado exitosamente.", "Exportacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo exportar el reporte.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            progressBar1.Value = 0;
        }
    }

    private DataTable ObtenerDataTableDesdeGrafico()
    {
        var dt = new DataTable();
        dt.Columns.Add("Mes", typeof(string));
        dt.Columns.Add("Cantidad", typeof(string));
        foreach (var d in _reporteService.InscripcionesPorMes())
            dt.Rows.Add(d.Mes, d.Cantidad.ToString());
        return dt;
    }
}
