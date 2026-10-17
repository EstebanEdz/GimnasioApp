using System.Globalization;
using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

public partial class FormPlanes : Form
{
    private System.Windows.Forms.Label lblNombre;
    private System.Windows.Forms.Label lblPrecio;
    private System.Windows.Forms.TextBox txtNombre;
    private System.Windows.Forms.TextBox txtPrecio;
    private System.Windows.Forms.CheckBox chkIncluyeClases;
    private System.Windows.Forms.CheckBox chkActivo;
    private System.Windows.Forms.GroupBox groupBoxDuracion;
    private System.Windows.Forms.RadioButton radMensual;
    private System.Windows.Forms.RadioButton radTrimestral;
    private System.Windows.Forms.RadioButton radAnual;
    private System.Windows.Forms.Button btnGuardar;
    private System.Windows.Forms.Button btnEditar;
    private System.Windows.Forms.Button btnEliminar;
    private System.Windows.Forms.Button btnLimpiar;
    private System.Windows.Forms.DataGridView dgvPlanes;
    private System.Windows.Forms.Label lblBuscar;
    private System.Windows.Forms.TextBox txtBuscar;
    private System.Windows.Forms.Button btnBuscar;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.GroupBox groupBox1;

    private readonly PlanService _service = new();
    private readonly bool _soloLectura;
    private int _idSeleccionado = -1;

    public FormPlanes(bool soloLectura = false)
    {
        InitializeComponent();
        this.lblNombre = new System.Windows.Forms.Label();
        this.lblPrecio = new System.Windows.Forms.Label();
        this.txtNombre = new System.Windows.Forms.TextBox();
        this.txtPrecio = new System.Windows.Forms.TextBox();
        this.chkIncluyeClases = new System.Windows.Forms.CheckBox();
        this.chkActivo = new System.Windows.Forms.CheckBox();
        this.groupBoxDuracion = new System.Windows.Forms.GroupBox();
        this.radMensual = new System.Windows.Forms.RadioButton();
        this.radTrimestral = new System.Windows.Forms.RadioButton();
        this.radAnual = new System.Windows.Forms.RadioButton();
        this.btnGuardar = new System.Windows.Forms.Button();
        this.btnEditar = new System.Windows.Forms.Button();
        this.btnEliminar = new System.Windows.Forms.Button();
        this.btnLimpiar = new System.Windows.Forms.Button();
        this.dgvPlanes = new System.Windows.Forms.DataGridView();
        this.lblBuscar = new System.Windows.Forms.Label();
        this.txtBuscar = new System.Windows.Forms.TextBox();
        this.btnBuscar = new System.Windows.Forms.Button();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        ((System.ComponentModel.ISupportInitialize)(this.dgvPlanes)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.groupBox1.SuspendLayout();
        this.groupBoxDuracion.SuspendLayout();
        this.SuspendLayout();

        this.groupBox1.Text = "Datos del Plan";
        this.groupBox1.Location = new System.Drawing.Point(12, 12);
        this.groupBox1.Size = new System.Drawing.Size(620, 180);

        this.lblNombre.AutoSize = true;
        this.lblNombre.Location = new System.Drawing.Point(15, 30);
        this.lblNombre.Text = "Nombre";
        this.txtNombre.Location = new System.Drawing.Point(110, 27);
        this.txtNombre.Size = new System.Drawing.Size(220, 20);

        this.lblPrecio.AutoSize = true;
        this.lblPrecio.Location = new System.Drawing.Point(360, 30);
        this.lblPrecio.Text = "Precio";
        this.txtPrecio.Location = new System.Drawing.Point(420, 27);
        this.txtPrecio.Size = new System.Drawing.Size(120, 20);

        this.chkIncluyeClases.AutoSize = true;
        this.chkIncluyeClases.Location = new System.Drawing.Point(15, 60);
        this.chkIncluyeClases.Text = "Incluye clases grupales";

        this.chkActivo.AutoSize = true;
        this.chkActivo.Location = new System.Drawing.Point(200, 60);
        this.chkActivo.Text = "Activo";

        this.groupBoxDuracion.Text = "Tipo de duracion";
        this.groupBoxDuracion.Location = new System.Drawing.Point(15, 88);
        this.groupBoxDuracion.Size = new System.Drawing.Size(340, 45);
        this.radMensual.AutoSize = true;
        this.radMensual.Location = new System.Drawing.Point(10, 18);
        this.radMensual.Text = "Mensual";
        this.radTrimestral.AutoSize = true;
        this.radTrimestral.Location = new System.Drawing.Point(110, 18);
        this.radTrimestral.Text = "Trimestral";
        this.radAnual.AutoSize = true;
        this.radAnual.Location = new System.Drawing.Point(220, 18);
        this.radAnual.Text = "Anual";
        this.groupBoxDuracion.Controls.Add(this.radMensual);
        this.groupBoxDuracion.Controls.Add(this.radTrimestral);
        this.groupBoxDuracion.Controls.Add(this.radAnual);

        this.btnGuardar.Location = new System.Drawing.Point(15, 140);
        this.btnGuardar.Size = new System.Drawing.Size(90, 26);
        this.btnGuardar.Text = "Guardar";
        this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

        this.btnEditar.Location = new System.Drawing.Point(112, 140);
        this.btnEditar.Size = new System.Drawing.Size(90, 26);
        this.btnEditar.Text = "Editar";
        this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);

        this.btnEliminar.Location = new System.Drawing.Point(209, 140);
        this.btnEliminar.Size = new System.Drawing.Size(90, 26);
        this.btnEliminar.Text = "Eliminar";
        this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

        this.btnLimpiar.Location = new System.Drawing.Point(306, 140);
        this.btnLimpiar.Size = new System.Drawing.Size(90, 26);
        this.btnLimpiar.Text = "Limpiar";
        this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

        this.groupBox1.Controls.Add(this.lblNombre);
        this.groupBox1.Controls.Add(this.txtNombre);
        this.groupBox1.Controls.Add(this.lblPrecio);
        this.groupBox1.Controls.Add(this.txtPrecio);
        this.groupBox1.Controls.Add(this.chkIncluyeClases);
        this.groupBox1.Controls.Add(this.chkActivo);
        this.groupBox1.Controls.Add(this.groupBoxDuracion);
        this.groupBox1.Controls.Add(this.btnGuardar);
        this.groupBox1.Controls.Add(this.btnEditar);
        this.groupBox1.Controls.Add(this.btnEliminar);
        this.groupBox1.Controls.Add(this.btnLimpiar);

        this.lblBuscar.AutoSize = true;
        this.lblBuscar.Location = new System.Drawing.Point(15, 205);
        this.lblBuscar.Text = "Buscar (nombre o duracion)";
        this.txtBuscar.Location = new System.Drawing.Point(210, 202);
        this.txtBuscar.Size = new System.Drawing.Size(220, 20);
        this.btnBuscar.Location = new System.Drawing.Point(440, 200);
        this.btnBuscar.Size = new System.Drawing.Size(90, 26);
        this.btnBuscar.Text = "Buscar";
        this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);

        this.dgvPlanes.Location = new System.Drawing.Point(12, 232);
        this.dgvPlanes.Size = new System.Drawing.Size(620, 300);
        this.dgvPlanes.ReadOnly = true;
        this.dgvPlanes.AllowUserToAddRows = false;
        this.dgvPlanes.RowHeadersVisible = false;
        this.dgvPlanes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvPlanes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvPlanes.DoubleClick += new System.EventHandler(this.dgvPlanes_DoubleClick);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(644, 544);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.lblBuscar);
        this.Controls.Add(this.txtBuscar);
        this.Controls.Add(this.btnBuscar);
        this.Controls.Add(this.dgvPlanes);
        this.Name = "FormPlanes";
        this.Text = "Gestion de Planes";
        ((System.ComponentModel.ISupportInitialize)(this.dgvPlanes)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.groupBoxDuracion.ResumeLayout(false);
        this.groupBoxDuracion.PerformLayout();
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        this.ResumeLayout(false);
    
        _soloLectura = soloLectura;

        toolTip1.SetToolTip(txtNombre, "Nombre comercial del plan");
        toolTip1.SetToolTip(txtPrecio, "Precio en pesos, solo numeros");
        toolTip1.SetToolTip(chkIncluyeClases, "Marque si el plan incluye clases grupales");

        txtPrecio.KeyPress += TxtPrecio_KeyPress;
        radMensual.Checked = true;
        chkActivo.Checked = true;

        if (_soloLectura)
        {
            btnGuardar.Enabled = false;
            btnEliminar.Enabled = false;
            Text += " (solo lectura)";
        }

        CargarGrid();
    }

    // Filtra el campo de precio para aceptar solo digitos y un punto decimal (Ses. 2/3: filtrar con KeyPress).
    private void TxtPrecio_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (char.IsControl(e.KeyChar)) return;
        if (char.IsDigit(e.KeyChar)) return;
        if (e.KeyChar == '.' && !txtPrecio.Text.Contains('.')) return;
        e.Handled = true;
    }

    private void CargarGrid(string filtro = "")
    {
        var lista = _service.ObtenerTodos();
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            lista = lista.Where(p =>
                p.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                p.TipoDuracion.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        dgvPlanes.DataSource = lista.Select(p => new
        {
            p.Id,
            p.Nombre,
            p.TipoDuracion,
            p.Precio,
            Clases = p.IncluyeClases ? "Si" : "No",
            p.Activo
        }).ToList();
    }

    private void btnBuscar_Click(object sender, EventArgs e) => CargarGrid(txtBuscar.Text.Trim());

    private string TipoDuracionSeleccionado() =>
        radMensual.Checked ? "Mensual" : radTrimestral.Checked ? "Trimestral" : "Anual";

    private void btnGuardar_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();

        if (!decimal.TryParse(txtPrecio.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precio))
        {
            errorProvider1.SetError(txtPrecio, "Precio invalido");
            MessageBox.Show("El precio ingresado no es valido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var plan = new Plan
        {
            Id = _idSeleccionado > 0 ? _idSeleccionado : 0,
            Nombre = txtNombre.Text.Trim(),
            TipoDuracion = TipoDuracionSeleccionado(),
            Precio = precio,
            IncluyeClases = chkIncluyeClases.Checked,
            Activo = chkActivo.Checked
        };

        var errores = _service.Validar(plan);
        if (errores.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(plan.Nombre)) errorProvider1.SetError(txtNombre, "Requerido");
            MessageBox.Show("Corrija los siguientes datos:\n\n- " + string.Join("\n- ", errores),
                "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _service.Guardar(plan);
        MessageBox.Show("Plan guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario();
        CargarGrid();
    }

    private void btnEditar_Click(object sender, EventArgs e)
    {
        if (dgvPlanes.CurrentRow == null)
        {
            MessageBox.Show("Seleccione un plan para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int id = Convert.ToInt32(dgvPlanes.CurrentRow.Cells["Id"].Value);
        var p = _service.ObtenerPorId(id);
        if (p == null) return;

        _idSeleccionado = p.Id;
        txtNombre.Text = p.Nombre;
        txtPrecio.Text = p.Precio.ToString(CultureInfo.InvariantCulture);
        chkIncluyeClases.Checked = p.IncluyeClases;
        chkActivo.Checked = p.Activo;
        radMensual.Checked = p.TipoDuracion == "Mensual";
        radTrimestral.Checked = p.TipoDuracion == "Trimestral";
        radAnual.Checked = p.TipoDuracion == "Anual";
    }

    private void btnEliminar_Click(object sender, EventArgs e)
    {
        if (dgvPlanes.CurrentRow == null)
        {
            MessageBox.Show("Seleccione un plan para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("¿Desea eliminar este plan?", "Confirmacion",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        int id = Convert.ToInt32(dgvPlanes.CurrentRow.Cells["Id"].Value);
        try
        {
            _service.Eliminar(id);
            LimpiarFormulario();
            CargarGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo eliminar el plan (puede tener inscripciones asociadas).\n\n" + ex.Message,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnLimpiar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        txtBuscar.Clear();
        CargarGrid();
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = -1;
        txtNombre.Clear();
        txtPrecio.Clear();
        chkIncluyeClases.Checked = false;
        chkActivo.Checked = true;
        radMensual.Checked = true;
        errorProvider1.Clear();
    }

    private void dgvPlanes_DoubleClick(object sender, EventArgs e) => btnEditar_Click(sender, e);
}
