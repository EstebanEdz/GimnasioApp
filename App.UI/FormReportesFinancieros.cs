using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using App.BLL;

namespace App.UI;

/// <summary>Reportes financieros: ingresos realmente cobrados, estado de las membresias y cartera (morosos).</summary>
public partial class FormReportesFinancieros : Form
{
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabIngresos;
    private System.Windows.Forms.TabPage tabMembresias;
    private System.Windows.Forms.TabPage tabMorosos;

    private System.Windows.Forms.Label lblDesde;
    private System.Windows.Forms.DateTimePicker dtpDesde;
    private System.Windows.Forms.Label lblHasta;
    private System.Windows.Forms.DateTimePicker dtpHasta;
    private System.Windows.Forms.Button btnGenerar;
    private System.Windows.Forms.Label lblCobrado;
    private System.Windows.Forms.Label lblFacturado;
    private System.Windows.Forms.Label lblRecaudo;
    private System.Windows.Forms.Label lblCartera;
    private Chart chartIngresos;
    private System.Windows.Forms.DataGridView dgvMetodo;
    private System.Windows.Forms.DataGridView dgvMes;

    private System.Windows.Forms.Label lblEstados;
    private System.Windows.Forms.DataGridView dgvEstados;
    private System.Windows.Forms.Label lblPorVencer;
    private System.Windows.Forms.NumericUpDown nudDias;
    private System.Windows.Forms.Label lblDiasTexto;
    private System.Windows.Forms.Button btnActualizarMembresias;
    private System.Windows.Forms.DataGridView dgvPorVencer;

    private System.Windows.Forms.Label lblTotalMorosos;
    private System.Windows.Forms.DataGridView dgvMorosos;

    private System.Windows.Forms.Button btnExportar;

    private readonly ReporteService _reportes = new();
    private readonly ImportExportService _importExport = new();

    private DataTable _dtMes = new();
    private DataTable _dtPorVencer = new();
    private DataTable _dtMorosos = new();

    public FormReportesFinancieros()
    {
        InitializeComponent();
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabIngresos = new System.Windows.Forms.TabPage();
        this.tabMembresias = new System.Windows.Forms.TabPage();
        this.tabMorosos = new System.Windows.Forms.TabPage();
        this.lblDesde = new System.Windows.Forms.Label();
        this.dtpDesde = new System.Windows.Forms.DateTimePicker();
        this.lblHasta = new System.Windows.Forms.Label();
        this.dtpHasta = new System.Windows.Forms.DateTimePicker();
        this.btnGenerar = new System.Windows.Forms.Button();
        this.lblCobrado = new System.Windows.Forms.Label();
        this.lblFacturado = new System.Windows.Forms.Label();
        this.lblRecaudo = new System.Windows.Forms.Label();
        this.lblCartera = new System.Windows.Forms.Label();
        this.chartIngresos = new Chart();
        this.dgvMetodo = new System.Windows.Forms.DataGridView();
        this.dgvMes = new System.Windows.Forms.DataGridView();
        this.lblEstados = new System.Windows.Forms.Label();
        this.dgvEstados = new System.Windows.Forms.DataGridView();
        this.lblPorVencer = new System.Windows.Forms.Label();
        this.nudDias = new System.Windows.Forms.NumericUpDown();
        this.lblDiasTexto = new System.Windows.Forms.Label();
        this.btnActualizarMembresias = new System.Windows.Forms.Button();
        this.dgvPorVencer = new System.Windows.Forms.DataGridView();
        this.lblTotalMorosos = new System.Windows.Forms.Label();
        this.dgvMorosos = new System.Windows.Forms.DataGridView();
        this.btnExportar = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.chartIngresos)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvMetodo)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvMes)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvEstados)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudDias)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvPorVencer)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvMorosos)).BeginInit();
        this.SuspendLayout();

        // ================= Pestaña 1: Ingresos cobrados =================
        this.lblDesde.AutoSize = true;
        this.lblDesde.Location = new System.Drawing.Point(10, 15);
        this.lblDesde.Text = "Desde";
        this.dtpDesde.Location = new System.Drawing.Point(55, 11);
        this.dtpDesde.Size = new System.Drawing.Size(110, 20);
        this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
        this.lblHasta.AutoSize = true;
        this.lblHasta.Location = new System.Drawing.Point(185, 15);
        this.lblHasta.Text = "Hasta";
        this.dtpHasta.Location = new System.Drawing.Point(230, 11);
        this.dtpHasta.Size = new System.Drawing.Size(110, 20);
        this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
        this.btnGenerar.Location = new System.Drawing.Point(360, 8);
        this.btnGenerar.Size = new System.Drawing.Size(100, 26);
        this.btnGenerar.Text = "Generar";
        this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);

        var fuenteKpi = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblCobrado.AutoSize = true;
        this.lblCobrado.Font = fuenteKpi;
        this.lblCobrado.ForeColor = System.Drawing.Color.DarkGreen;
        this.lblCobrado.Location = new System.Drawing.Point(10, 48);
        this.lblCobrado.Text = "Cobrado: $0";
        this.lblFacturado.AutoSize = true;
        this.lblFacturado.Font = fuenteKpi;
        this.lblFacturado.Location = new System.Drawing.Point(230, 48);
        this.lblFacturado.Text = "Facturado: $0";
        this.lblRecaudo.AutoSize = true;
        this.lblRecaudo.Font = fuenteKpi;
        this.lblRecaudo.Location = new System.Drawing.Point(450, 48);
        this.lblRecaudo.Text = "Recaudo: 0%";
        this.lblCartera.AutoSize = true;
        this.lblCartera.Font = fuenteKpi;
        this.lblCartera.ForeColor = System.Drawing.Color.Firebrick;
        this.lblCartera.Location = new System.Drawing.Point(620, 48);
        this.lblCartera.Text = "Cartera pendiente hoy: $0";

        this.chartIngresos.Location = new System.Drawing.Point(10, 80);
        this.chartIngresos.Size = new System.Drawing.Size(500, 330);
        this.chartIngresos.ChartAreas.Add(new ChartArea("area1"));

        ConfigurarGrid(this.dgvMetodo);
        this.dgvMetodo.Location = new System.Drawing.Point(520, 80);
        this.dgvMetodo.Size = new System.Drawing.Size(335, 150);
        ConfigurarGrid(this.dgvMes);
        this.dgvMes.Location = new System.Drawing.Point(520, 240);
        this.dgvMes.Size = new System.Drawing.Size(335, 170);

        this.tabIngresos.Controls.Add(this.lblDesde);
        this.tabIngresos.Controls.Add(this.dtpDesde);
        this.tabIngresos.Controls.Add(this.lblHasta);
        this.tabIngresos.Controls.Add(this.dtpHasta);
        this.tabIngresos.Controls.Add(this.btnGenerar);
        this.tabIngresos.Controls.Add(this.lblCobrado);
        this.tabIngresos.Controls.Add(this.lblFacturado);
        this.tabIngresos.Controls.Add(this.lblRecaudo);
        this.tabIngresos.Controls.Add(this.lblCartera);
        this.tabIngresos.Controls.Add(this.chartIngresos);
        this.tabIngresos.Controls.Add(this.dgvMetodo);
        this.tabIngresos.Controls.Add(this.dgvMes);
        this.tabIngresos.Text = "Ingresos cobrados";

        // ================= Pestaña 2: Membresias =================
        this.lblEstados.AutoSize = true;
        this.lblEstados.Location = new System.Drawing.Point(10, 15);
        this.lblEstados.Text = "Membresias por estado";
        ConfigurarGrid(this.dgvEstados);
        this.dgvEstados.Location = new System.Drawing.Point(10, 38);
        this.dgvEstados.Size = new System.Drawing.Size(230, 190);

        this.lblPorVencer.AutoSize = true;
        this.lblPorVencer.Location = new System.Drawing.Point(260, 15);
        this.lblPorVencer.Text = "Vencen en los proximos";
        this.nudDias.Location = new System.Drawing.Point(392, 12);
        this.nudDias.Size = new System.Drawing.Size(50, 20);
        this.nudDias.Minimum = 1;
        this.nudDias.Maximum = 90;
        this.nudDias.Value = Math.Min(Configuracion.DiasAvisoVencimiento, 90);
        this.lblDiasTexto.AutoSize = true;
        this.lblDiasTexto.Location = new System.Drawing.Point(448, 15);
        this.lblDiasTexto.Text = "dias";
        this.btnActualizarMembresias.Location = new System.Drawing.Point(490, 8);
        this.btnActualizarMembresias.Size = new System.Drawing.Size(100, 26);
        this.btnActualizarMembresias.Text = "Actualizar";
        this.btnActualizarMembresias.Click += new System.EventHandler(this.btnActualizarMembresias_Click);
        ConfigurarGrid(this.dgvPorVencer);
        this.dgvPorVencer.Location = new System.Drawing.Point(260, 38);
        this.dgvPorVencer.Size = new System.Drawing.Size(595, 372);

        this.tabMembresias.Controls.Add(this.lblEstados);
        this.tabMembresias.Controls.Add(this.dgvEstados);
        this.tabMembresias.Controls.Add(this.lblPorVencer);
        this.tabMembresias.Controls.Add(this.nudDias);
        this.tabMembresias.Controls.Add(this.lblDiasTexto);
        this.tabMembresias.Controls.Add(this.btnActualizarMembresias);
        this.tabMembresias.Controls.Add(this.dgvPorVencer);
        this.tabMembresias.Text = "Membresias";

        // ================= Pestaña 3: Morosos =================
        this.lblTotalMorosos.AutoSize = true;
        this.lblTotalMorosos.Font = fuenteKpi;
        this.lblTotalMorosos.ForeColor = System.Drawing.Color.Firebrick;
        this.lblTotalMorosos.Location = new System.Drawing.Point(10, 14);
        this.lblTotalMorosos.Text = "";
        ConfigurarGrid(this.dgvMorosos);
        this.dgvMorosos.Location = new System.Drawing.Point(10, 42);
        this.dgvMorosos.Size = new System.Drawing.Size(845, 368);

        this.tabMorosos.Controls.Add(this.lblTotalMorosos);
        this.tabMorosos.Controls.Add(this.dgvMorosos);
        this.tabMorosos.Text = "Cartera (morosos)";

        // ================= Contenedor =================
        this.tabControl1.Controls.Add(this.tabIngresos);
        this.tabControl1.Controls.Add(this.tabMembresias);
        this.tabControl1.Controls.Add(this.tabMorosos);
        this.tabControl1.Location = new System.Drawing.Point(10, 10);
        this.tabControl1.Size = new System.Drawing.Size(875, 445);

        this.btnExportar.Location = new System.Drawing.Point(10, 465);
        this.btnExportar.Size = new System.Drawing.Size(180, 30);
        this.btnExportar.Text = "Exportar pestaña (Excel/PDF)...";
        this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(900, 505);
        this.Controls.Add(this.tabControl1);
        this.Controls.Add(this.btnExportar);
        this.Name = "FormReportesFinancieros";
        this.Text = "Reportes Financieros";
        ((System.ComponentModel.ISupportInitialize)(this.chartIngresos)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvMetodo)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvMes)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvEstados)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudDias)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvPorVencer)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvMorosos)).EndInit();
        this.ResumeLayout(false);

        dtpDesde.Value = new DateTime(DateTime.Today.Year, 1, 1);
        dtpHasta.Value = DateTime.Today;

        GenerarIngresos();
        GenerarMembresias();
        GenerarMorosos();
    }

    private static void ConfigurarGrid(System.Windows.Forms.DataGridView grid)
    {
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.RowHeadersVisible = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    }

    private void btnGenerar_Click(object? sender, EventArgs e) => GenerarIngresos();

    private void btnActualizarMembresias_Click(object? sender, EventArgs e) => GenerarMembresias();

    // ---------------- Ingresos cobrados ----------------
    private void GenerarIngresos()
    {
        if (dtpDesde.Value.Date > dtpHasta.Value.Date)
        {
            Mensajes.Advertencia(this, "La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.");
            return;
        }

        try
        {
            var resumen = _reportes.ObtenerResumenFinanciero(dtpDesde.Value, dtpHasta.Value);
            lblCobrado.Text = $"Cobrado: ${resumen.Cobrado:N0}";
            lblFacturado.Text = $"Facturado: ${resumen.Facturado:N0}";
            lblRecaudo.Text = $"Recaudo: {resumen.PorcentajeRecaudo}%";
            lblCartera.Text = $"Cartera pendiente hoy: ${resumen.CarteraPendiente:N0} ({resumen.MiembrosConSaldo} miembro(s))";

            var porMes = _reportes.IngresosCobradosPorMes(dtpDesde.Value, dtpHasta.Value);
            _dtMes = new DataTable();
            _dtMes.Columns.Add("Mes", typeof(string));
            _dtMes.Columns.Add("Pagos", typeof(string));
            _dtMes.Columns.Add("Total cobrado", typeof(string));
            foreach (var f in porMes) _dtMes.Rows.Add(f.Mes, f.NumPagos.ToString(), f.Total.ToString("N0"));
            dgvMes.DataSource = _dtMes;

            chartIngresos.Series.Clear();
            chartIngresos.ChartAreas.Clear();
            chartIngresos.ChartAreas.Add(new ChartArea("area1"));
            chartIngresos.Titles.Clear();
            chartIngresos.Titles.Add(new Title("Ingresos cobrados por mes"));
            var serie = new Series("Cobrado") { ChartType = SeriesChartType.Column, IsValueShownAsLabel = true };
            foreach (var f in porMes) serie.Points.AddXY(f.Mes, f.Total);
            chartIngresos.Series.Add(serie);

            var porMetodo = _reportes.IngresosCobradosPorMetodo(dtpDesde.Value, dtpHasta.Value);
            var dtMetodo = new DataTable();
            dtMetodo.Columns.Add("Metodo", typeof(string));
            dtMetodo.Columns.Add("Pagos", typeof(string));
            dtMetodo.Columns.Add("Total", typeof(string));
            dtMetodo.Columns.Add("%", typeof(string));
            foreach (var f in porMetodo) dtMetodo.Rows.Add(f.Metodo, f.NumPagos.ToString(), f.Total.ToString("N0"), f.Porcentaje + "%");
            dgvMetodo.DataSource = dtMetodo;
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "ReportesFinancieros.Ingresos");
        }
    }

    // ---------------- Membresias ----------------
    private void GenerarMembresias()
    {
        try
        {
            var estados = _reportes.MembresiasPorEstado();
            var dtEstados = new DataTable();
            dtEstados.Columns.Add("Estado", typeof(string));
            dtEstados.Columns.Add("Cantidad", typeof(string));
            foreach (var f in estados) dtEstados.Rows.Add(f.Estado, f.Cantidad.ToString());
            dgvEstados.DataSource = dtEstados;

            var porVencer = _reportes.ProximasAVencer((int)nudDias.Value);
            _dtPorVencer = new DataTable();
            _dtPorVencer.Columns.Add("Miembro", typeof(string));
            _dtPorVencer.Columns.Add("Documento", typeof(string));
            _dtPorVencer.Columns.Add("Plan", typeof(string));
            _dtPorVencer.Columns.Add("Vence", typeof(string));
            _dtPorVencer.Columns.Add("Dias", typeof(string));
            _dtPorVencer.Columns.Add("Saldo", typeof(string));
            foreach (var m in porVencer)
                _dtPorVencer.Rows.Add(m.Miembro, m.Documento, m.Plan, m.FechaVencimiento.ToString("yyyy-MM-dd"),
                    m.DiasParaVencer.ToString(), m.Saldo.ToString("N0"));
            dgvPorVencer.DataSource = _dtPorVencer;
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "ReportesFinancieros.Membresias");
        }
    }

    // ---------------- Cartera / morosos ----------------
    private void GenerarMorosos()
    {
        try
        {
            var morosos = _reportes.Morosos();
            _dtMorosos = new DataTable();
            _dtMorosos.Columns.Add("Miembro", typeof(string));
            _dtMorosos.Columns.Add("Documento", typeof(string));
            _dtMorosos.Columns.Add("Plan", typeof(string));
            _dtMorosos.Columns.Add("Estado", typeof(string));
            _dtMorosos.Columns.Add("Total", typeof(string));
            _dtMorosos.Columns.Add("Pagado", typeof(string));
            _dtMorosos.Columns.Add("Saldo", typeof(string));
            foreach (var m in morosos)
                _dtMorosos.Rows.Add(m.Miembro, m.Documento, m.Plan, m.Estado,
                    m.Total.ToString("N0"), m.Pagado.ToString("N0"), m.Saldo.ToString("N0"));
            dgvMorosos.DataSource = _dtMorosos;

            lblTotalMorosos.Text = $"{morosos.Count} membresia(s) con saldo pendiente por un total de ${morosos.Sum(m => m.Saldo):N0}";
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "ReportesFinancieros.Morosos");
        }
    }

    // ---------------- Exportar la pestaña activa ----------------
    private void btnExportar_Click(object? sender, EventArgs e)
    {
        DataTable dt;
        string titulo;
        switch (tabControl1.SelectedIndex)
        {
            case 0: dt = _dtMes; titulo = "Ingresos cobrados por mes"; break;
            case 1: dt = _dtPorVencer; titulo = "Membresias por vencer"; break;
            default: dt = _dtMorosos; titulo = "Cartera - morosos"; break;
        }

        if (dt.Rows.Count == 0)
        {
            Mensajes.Info(this, "No hay datos para exportar en esta pestaña.", "Aviso");
            return;
        }

        using var saveDialog = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx|PDF (*.pdf)|*.pdf", FileName = titulo };
        if (saveDialog.ShowDialog() != DialogResult.OK) return;

        try
        {
            bool esPdf = saveDialog.FilterIndex == 2;
            if (tabControl1.SelectedIndex == 0)
            {
                using var ms = new MemoryStream();
                chartIngresos.SaveImage(ms, ChartImageFormat.Png);
                byte[] png = ms.ToArray();
                if (esPdf) _importExport.ExportarPdfConGrafico(titulo, png, dt, saveDialog.FileName);
                else _importExport.ExportarExcelConGrafico(png, dt, saveDialog.FileName, titulo);
            }
            else if (esPdf)
            {
                _importExport.ExportarPdf(titulo, dt, saveDialog.FileName);
            }
            else
            {
                _importExport.ExportarExcel(dt, saveDialog.FileName, titulo);
            }

            Mensajes.Info(this, "Reporte exportado exitosamente.", "Exportacion");
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "ReportesFinancieros.Exportar");
        }
    }
}
