using System.Data;
using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

public partial class FormMiembros : Form
{
    private System.Windows.Forms.Label lblNombre;
    private System.Windows.Forms.Label lblDocumento;
    private System.Windows.Forms.Label lblTelefono;
    private System.Windows.Forms.Label lblCorreo;
    private System.Windows.Forms.Label lblFechaIngreso;
    private System.Windows.Forms.Label lblBuscar;
    private System.Windows.Forms.TextBox txtNombre;
    private System.Windows.Forms.MaskedTextBox maskedDocumento;
    private System.Windows.Forms.MaskedTextBox maskedTelefono;
    private System.Windows.Forms.TextBox txtCorreo;
    private System.Windows.Forms.DateTimePicker dtpFechaIngreso;
    private System.Windows.Forms.CheckBox chkActivo;
    private System.Windows.Forms.TextBox txtBuscar;
    private System.Windows.Forms.Button btnGuardar;
    private System.Windows.Forms.Button btnEditar;
    private System.Windows.Forms.Button btnEliminar;
    private System.Windows.Forms.Button btnLimpiar;
    private System.Windows.Forms.Button btnBuscar;
    private System.Windows.Forms.Button btnExportar;
    private System.Windows.Forms.Button btnImportar;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.DataGridView dgvMiembros;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.GroupBox groupBox1;

    private readonly MiembroService _service = new();
    private readonly ImportExportService _importExport = new();
    private readonly bool _soloLectura;
    private int _idSeleccionado = -1;

    public FormMiembros(bool soloLectura = false)
    {
        InitializeComponent();
        this.lblNombre = new System.Windows.Forms.Label();
        this.lblDocumento = new System.Windows.Forms.Label();
        this.lblTelefono = new System.Windows.Forms.Label();
        this.lblCorreo = new System.Windows.Forms.Label();
        this.lblFechaIngreso = new System.Windows.Forms.Label();
        this.lblBuscar = new System.Windows.Forms.Label();
        this.txtNombre = new System.Windows.Forms.TextBox();
        this.maskedDocumento = new System.Windows.Forms.MaskedTextBox();
        this.maskedTelefono = new System.Windows.Forms.MaskedTextBox();
        this.txtCorreo = new System.Windows.Forms.TextBox();
        this.dtpFechaIngreso = new System.Windows.Forms.DateTimePicker();
        this.chkActivo = new System.Windows.Forms.CheckBox();
        this.txtBuscar = new System.Windows.Forms.TextBox();
        this.btnGuardar = new System.Windows.Forms.Button();
        this.btnEditar = new System.Windows.Forms.Button();
        this.btnEliminar = new System.Windows.Forms.Button();
        this.btnLimpiar = new System.Windows.Forms.Button();
        this.btnBuscar = new System.Windows.Forms.Button();
        this.btnExportar = new System.Windows.Forms.Button();
        this.btnImportar = new System.Windows.Forms.Button();
        this.progressBar1 = new System.Windows.Forms.ProgressBar();
        this.dgvMiembros = new System.Windows.Forms.DataGridView();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        ((System.ComponentModel.ISupportInitialize)(this.dgvMiembros)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.groupBox1.SuspendLayout();
        this.SuspendLayout();

        // ---- GroupBox datos del miembro ----
        this.groupBox1.Text = "Datos del Miembro";
        this.groupBox1.Location = new System.Drawing.Point(12, 12);
        this.groupBox1.Size = new System.Drawing.Size(760, 150);

        this.lblNombre.AutoSize = true;
        this.lblNombre.Location = new System.Drawing.Point(15, 30);
        this.lblNombre.Text = "Nombre";
        this.txtNombre.Location = new System.Drawing.Point(110, 27);
        this.txtNombre.Size = new System.Drawing.Size(220, 20);

        this.lblDocumento.AutoSize = true;
        this.lblDocumento.Location = new System.Drawing.Point(360, 30);
        this.lblDocumento.Text = "Documento";
        this.maskedDocumento.Location = new System.Drawing.Point(455, 27);
        this.maskedDocumento.Size = new System.Drawing.Size(150, 20);
        this.maskedDocumento.Mask = "0000000000";

        this.lblTelefono.AutoSize = true;
        this.lblTelefono.Location = new System.Drawing.Point(15, 62);
        this.lblTelefono.Text = "Telefono";
        this.maskedTelefono.Location = new System.Drawing.Point(110, 59);
        this.maskedTelefono.Size = new System.Drawing.Size(150, 20);
        this.maskedTelefono.Mask = "(000) 000-0000";

        this.lblCorreo.AutoSize = true;
        this.lblCorreo.Location = new System.Drawing.Point(360, 62);
        this.lblCorreo.Text = "Correo";
        this.txtCorreo.Location = new System.Drawing.Point(455, 59);
        this.txtCorreo.Size = new System.Drawing.Size(220, 20);

        this.lblFechaIngreso.AutoSize = true;
        this.lblFechaIngreso.Location = new System.Drawing.Point(15, 94);
        this.lblFechaIngreso.Text = "Fecha ingreso";
        this.dtpFechaIngreso.Location = new System.Drawing.Point(110, 91);
        this.dtpFechaIngreso.Size = new System.Drawing.Size(150, 20);
        this.dtpFechaIngreso.Format = System.Windows.Forms.DateTimePickerFormat.Short;

        this.chkActivo.AutoSize = true;
        this.chkActivo.Location = new System.Drawing.Point(360, 94);
        this.chkActivo.Text = "Activo";
        this.chkActivo.Checked = true;

        this.btnGuardar.Location = new System.Drawing.Point(15, 118);
        this.btnGuardar.Size = new System.Drawing.Size(90, 26);
        this.btnGuardar.Text = "Guardar";
        this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

        this.btnEditar.Location = new System.Drawing.Point(112, 118);
        this.btnEditar.Size = new System.Drawing.Size(90, 26);
        this.btnEditar.Text = "Editar";
        this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);

        this.btnEliminar.Location = new System.Drawing.Point(209, 118);
        this.btnEliminar.Size = new System.Drawing.Size(90, 26);
        this.btnEliminar.Text = "Eliminar";
        this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

        this.btnLimpiar.Location = new System.Drawing.Point(306, 118);
        this.btnLimpiar.Size = new System.Drawing.Size(90, 26);
        this.btnLimpiar.Text = "Limpiar";
        this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

        this.btnImportar.Location = new System.Drawing.Point(455, 118);
        this.btnImportar.Size = new System.Drawing.Size(110, 26);
        this.btnImportar.Text = "Importar CSV...";
        this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);

        this.btnExportar.Location = new System.Drawing.Point(575, 118);
        this.btnExportar.Size = new System.Drawing.Size(110, 26);
        this.btnExportar.Text = "Exportar...";
        this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);

        this.groupBox1.Controls.Add(this.lblNombre);
        this.groupBox1.Controls.Add(this.txtNombre);
        this.groupBox1.Controls.Add(this.lblDocumento);
        this.groupBox1.Controls.Add(this.maskedDocumento);
        this.groupBox1.Controls.Add(this.lblTelefono);
        this.groupBox1.Controls.Add(this.maskedTelefono);
        this.groupBox1.Controls.Add(this.lblCorreo);
        this.groupBox1.Controls.Add(this.txtCorreo);
        this.groupBox1.Controls.Add(this.lblFechaIngreso);
        this.groupBox1.Controls.Add(this.dtpFechaIngreso);
        this.groupBox1.Controls.Add(this.chkActivo);
        this.groupBox1.Controls.Add(this.btnGuardar);
        this.groupBox1.Controls.Add(this.btnEditar);
        this.groupBox1.Controls.Add(this.btnEliminar);
        this.groupBox1.Controls.Add(this.btnLimpiar);
        this.groupBox1.Controls.Add(this.btnImportar);
        this.groupBox1.Controls.Add(this.btnExportar);

        // ---- Buscar ----
        this.lblBuscar.AutoSize = true;
        this.lblBuscar.Location = new System.Drawing.Point(15, 172);
        this.lblBuscar.Text = "Buscar (nombre o documento)";
        this.txtBuscar.Location = new System.Drawing.Point(210, 169);
        this.txtBuscar.Size = new System.Drawing.Size(220, 20);
        this.btnBuscar.Location = new System.Drawing.Point(440, 167);
        this.btnBuscar.Size = new System.Drawing.Size(90, 26);
        this.btnBuscar.Text = "Buscar";
        this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);

        this.progressBar1.Location = new System.Drawing.Point(540, 172);
        this.progressBar1.Size = new System.Drawing.Size(232, 18);

        // ---- Grid ----
        this.dgvMiembros.Location = new System.Drawing.Point(12, 200);
        this.dgvMiembros.Size = new System.Drawing.Size(760, 330);
        this.dgvMiembros.ReadOnly = true;
        this.dgvMiembros.AllowUserToAddRows = false;
        this.dgvMiembros.RowHeadersVisible = false;
        this.dgvMiembros.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvMiembros.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvMiembros.DoubleClick += new System.EventHandler(this.dgvMiembros_DoubleClick);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(784, 541);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.lblBuscar);
        this.Controls.Add(this.txtBuscar);
        this.Controls.Add(this.btnBuscar);
        this.Controls.Add(this.progressBar1);
        this.Controls.Add(this.dgvMiembros);
        this.Name = "FormMiembros";
        this.Text = "Gestion de Miembros";
        ((System.ComponentModel.ISupportInitialize)(this.dgvMiembros)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    
        _soloLectura = soloLectura;

        dtpFechaIngreso.Value = DateTime.Today;
        chkActivo.Checked = true;

        ConfigurarTooltips();
        AplicarModoSoloLectura();
        CargarGrid();
    }

    private void ConfigurarTooltips()
    {
        toolTip1.SetToolTip(txtNombre, "Nombre completo del miembro");
        toolTip1.SetToolTip(maskedDocumento, "Numero de documento (solo digitos)");
        toolTip1.SetToolTip(maskedTelefono, "Telefono de contacto");
        toolTip1.SetToolTip(txtCorreo, "Correo electronico del miembro");
        toolTip1.SetToolTip(dtpFechaIngreso, "Fecha de ingreso al gimnasio");
        toolTip1.SetToolTip(txtBuscar, "Buscar por nombre o documento");
    }

    private void AplicarModoSoloLectura()
    {
        if (!_soloLectura) return;
        btnGuardar.Enabled = false;
        btnEliminar.Enabled = false;
        btnImportar.Enabled = false;
        Text += " (solo lectura)";
    }

    private void CargarGrid()
    {
        dgvMiembros.DataSource = _service.ObtenerTodos().Select(m => new
        {
            m.Id,
            m.Nombre,
            m.Documento,
            m.Telefono,
            m.Correo,
            m.FechaIngreso,
            m.Activo
        }).ToList();
    }

    private bool ValidarCampos(out Miembro miembro)
    {
        miembro = new Miembro(
            txtNombre.Text.Trim(),
            maskedDocumento.Text.Trim(),
            maskedTelefono.Text.Trim(),
            txtCorreo.Text.Trim(),
            dtpFechaIngreso.Value)
        {
            Id = _idSeleccionado > 0 ? _idSeleccionado : 0,
            Activo = chkActivo.Checked
        };

        errorProvider1.Clear();
        var errores = _service.Validar(miembro);

        if (errores.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(miembro.Nombre)) errorProvider1.SetError(txtNombre, "Requerido");
            else if (errores.Any(er => er.Contains("nombre", StringComparison.OrdinalIgnoreCase)))
                errorProvider1.SetError(txtNombre, "Nombre duplicado");
            if (string.IsNullOrWhiteSpace(miembro.Documento) || maskedDocumento.Text.Contains('_'))
                errorProvider1.SetError(maskedDocumento, "Requerido / incompleto");
            if (string.IsNullOrWhiteSpace(miembro.Telefono) || maskedTelefono.Text.Contains('_'))
                errorProvider1.SetError(maskedTelefono, "Requerido / incompleto");
            if (string.IsNullOrWhiteSpace(miembro.Correo) || !miembro.Correo.Contains('@'))
                errorProvider1.SetError(txtCorreo, "Correo invalido");

            MessageBox.Show("Corrija los siguientes datos antes de guardar:\n\n- " + string.Join("\n- ", errores),
                "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private void btnGuardar_Click(object sender, EventArgs e)
    {
        if (!ValidarCampos(out var miembro)) return;

        _service.Guardar(miembro);
        MessageBox.Show("Miembro guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario();
        CargarGrid();
    }

    private void btnEditar_Click(object sender, EventArgs e)
    {
        if (dgvMiembros.CurrentRow == null)
        {
            MessageBox.Show("Seleccione un miembro para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(dgvMiembros.CurrentRow.Cells["Id"].Value);
        var m = _service.ObtenerPorId(id);
        if (m == null) return;

        _idSeleccionado = m.Id;
        txtNombre.Text = m.Nombre;
        maskedDocumento.Text = m.Documento;
        maskedTelefono.Text = m.Telefono;
        txtCorreo.Text = m.Correo;
        dtpFechaIngreso.Value = m.FechaIngreso;
        chkActivo.Checked = m.Activo;
    }

    private void btnEliminar_Click(object sender, EventArgs e)
    {
        if (dgvMiembros.CurrentRow == null)
        {
            MessageBox.Show("Seleccione un miembro para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(dgvMiembros.CurrentRow.Cells["Id"].Value);
        var resultado = MessageBox.Show("¿Desea eliminar este miembro? Esta accion no se puede deshacer.",
            "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (resultado != DialogResult.Yes) return;

        try
        {
            _service.Eliminar(id);
            LimpiarFormulario();
            CargarGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo eliminar el miembro (puede tener inscripciones asociadas).\n\n" + ex.Message,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnBuscar_Click(object sender, EventArgs e)
    {
        dgvMiembros.DataSource = _service.Buscar(txtBuscar.Text.Trim()).Select(m => new
        {
            m.Id,
            m.Nombre,
            m.Documento,
            m.Telefono,
            m.Correo,
            m.FechaIngreso,
            m.Activo
        }).ToList();
    }

    private void btnLimpiar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        CargarGrid();
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = -1;
        txtNombre.Clear();
        maskedDocumento.Clear();
        maskedTelefono.Clear();
        txtCorreo.Clear();
        dtpFechaIngreso.Value = DateTime.Today;
        chkActivo.Checked = true;
        txtBuscar.Clear();
        errorProvider1.Clear();
    }

    private void dgvMiembros_DoubleClick(object sender, EventArgs e) => btnEditar_Click(sender, e);

    // -------- Exportar (Excel / PDF) --------
    private void btnExportar_Click(object sender, EventArgs e)
    {
        using var saveDialog = new SaveFileDialog
        {
            Filter = "Excel (*.xlsx)|*.xlsx|PDF (*.pdf)|*.pdf",
            FileName = "Miembros"
        };
        if (saveDialog.ShowDialog() != DialogResult.OK) return;

        progressBar1.Value = 0;
        for (int i = 0; i <= 100; i += 25) { progressBar1.Value = i; Application.DoEvents(); }

        var dt = ObtenerDataTableActual();

        try
        {
            if (saveDialog.FilterIndex == 2)
                _importExport.ExportarPdf("Reporte de Miembros", dt, saveDialog.FileName);
            else
                _importExport.ExportarExcel(dt, saveDialog.FileName, "Miembros");

            MessageBox.Show("Archivo exportado exitosamente.", "Exportacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo exportar el archivo.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            progressBar1.Value = 0;
        }
    }

    private DataTable ObtenerDataTableActual()
    {
        var dt = new DataTable();
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Nombre", typeof(string));
        dt.Columns.Add("Documento", typeof(string));
        dt.Columns.Add("Telefono", typeof(string));
        dt.Columns.Add("Correo", typeof(string));
        dt.Columns.Add("FechaIngreso", typeof(string));
        dt.Columns.Add("Activo", typeof(string));

        foreach (var m in _service.ObtenerTodos())
            dt.Rows.Add(m.Id, m.Nombre, m.Documento, m.Telefono, m.Correo, m.FechaIngreso.ToString("yyyy-MM-dd"), m.Activo ? "Si" : "No");

        return dt;
    }

    // -------- Importar CSV (con vista previa) --------
    private void btnImportar_Click(object sender, EventArgs e)
    {
        using var openDialog = new OpenFileDialog { Filter = "CSV (*.csv)|*.csv|TXT (*.txt)|*.txt" };
        if (openDialog.ShowDialog() != DialogResult.OK) return;

        List<string> erroresEstructura;
        DataTable vistaPrevia;
        try
        {
            vistaPrevia = _importExport.GenerarVistaPreviaCsv(openDialog.FileName, out erroresEstructura);
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo leer el archivo.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using var formPreview = new FormVistaPreviaImportacion(vistaPrevia, erroresEstructura);
        if (formPreview.ShowDialog() != DialogResult.OK) return;

        var resultado = _importExport.ConfirmarImportacionMiembros(vistaPrevia);
        CargarGrid();

        string mensaje = $"Importacion finalizada.\n\nRegistros exitosos: {resultado.Exitosos}\nRegistros fallidos: {resultado.Fallidos}";
        if (resultado.Errores.Count > 0)
            mensaje += "\n\nDetalle de fallos:\n- " + string.Join("\n- ", resultado.Errores.Take(10));

        MessageBox.Show(mensaje, "Importacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
