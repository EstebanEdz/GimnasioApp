using App.BLL;

namespace App.UI;

/// <summary>Bitacora de acciones (quien hizo que) y log de errores del sistema. Solo Administrador.</summary>
public partial class FormAuditoria : Form
{
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabAuditoria;
    private System.Windows.Forms.TabPage tabErrores;
    private System.Windows.Forms.Label lblDesde;
    private System.Windows.Forms.DateTimePicker dtpDesde;
    private System.Windows.Forms.Label lblHasta;
    private System.Windows.Forms.DateTimePicker dtpHasta;
    private System.Windows.Forms.Label lblUsuario;
    private System.Windows.Forms.TextBox txtUsuario;
    private System.Windows.Forms.Label lblAccion;
    private System.Windows.Forms.ComboBox cmbAccion;
    private System.Windows.Forms.Button btnConsultar;
    private System.Windows.Forms.DataGridView dgvAuditoria;
    private System.Windows.Forms.Button btnActualizarErrores;
    private System.Windows.Forms.Label lblNotaErrores;
    private System.Windows.Forms.DataGridView dgvErrores;

    private readonly AuditoriaService _service = new();

    public FormAuditoria()
    {
        InitializeComponent();
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabAuditoria = new System.Windows.Forms.TabPage();
        this.tabErrores = new System.Windows.Forms.TabPage();
        this.lblDesde = new System.Windows.Forms.Label();
        this.dtpDesde = new System.Windows.Forms.DateTimePicker();
        this.lblHasta = new System.Windows.Forms.Label();
        this.dtpHasta = new System.Windows.Forms.DateTimePicker();
        this.lblUsuario = new System.Windows.Forms.Label();
        this.txtUsuario = new System.Windows.Forms.TextBox();
        this.lblAccion = new System.Windows.Forms.Label();
        this.cmbAccion = new System.Windows.Forms.ComboBox();
        this.btnConsultar = new System.Windows.Forms.Button();
        this.dgvAuditoria = new System.Windows.Forms.DataGridView();
        this.btnActualizarErrores = new System.Windows.Forms.Button();
        this.lblNotaErrores = new System.Windows.Forms.Label();
        this.dgvErrores = new System.Windows.Forms.DataGridView();
        ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvErrores)).BeginInit();
        this.tabControl1.SuspendLayout();
        this.tabAuditoria.SuspendLayout();
        this.tabErrores.SuspendLayout();
        this.SuspendLayout();

        // ---- Pestaña Auditoria ----
        this.lblDesde.AutoSize = true;
        this.lblDesde.Location = new System.Drawing.Point(10, 15);
        this.lblDesde.Text = "Desde";
        this.dtpDesde.Location = new System.Drawing.Point(55, 11);
        this.dtpDesde.Size = new System.Drawing.Size(100, 20);
        this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;

        this.lblHasta.AutoSize = true;
        this.lblHasta.Location = new System.Drawing.Point(170, 15);
        this.lblHasta.Text = "Hasta";
        this.dtpHasta.Location = new System.Drawing.Point(212, 11);
        this.dtpHasta.Size = new System.Drawing.Size(100, 20);
        this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;

        this.lblUsuario.AutoSize = true;
        this.lblUsuario.Location = new System.Drawing.Point(328, 15);
        this.lblUsuario.Text = "Usuario";
        this.txtUsuario.Location = new System.Drawing.Point(378, 11);
        this.txtUsuario.Size = new System.Drawing.Size(110, 20);

        this.lblAccion.AutoSize = true;
        this.lblAccion.Location = new System.Drawing.Point(502, 15);
        this.lblAccion.Text = "Accion";
        this.cmbAccion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbAccion.Location = new System.Drawing.Point(545, 11);
        this.cmbAccion.Size = new System.Drawing.Size(190, 21);

        this.btnConsultar.Location = new System.Drawing.Point(750, 8);
        this.btnConsultar.Size = new System.Drawing.Size(90, 26);
        this.btnConsultar.Text = "Consultar";
        this.btnConsultar.Click += new System.EventHandler(this.btnConsultar_Click);

        this.dgvAuditoria.Location = new System.Drawing.Point(10, 44);
        this.dgvAuditoria.Size = new System.Drawing.Size(840, 380);
        this.dgvAuditoria.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.dgvAuditoria.ReadOnly = true;
        this.dgvAuditoria.AllowUserToAddRows = false;
        this.dgvAuditoria.RowHeadersVisible = false;
        this.dgvAuditoria.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

        this.tabAuditoria.Controls.Add(this.lblDesde);
        this.tabAuditoria.Controls.Add(this.dtpDesde);
        this.tabAuditoria.Controls.Add(this.lblHasta);
        this.tabAuditoria.Controls.Add(this.dtpHasta);
        this.tabAuditoria.Controls.Add(this.lblUsuario);
        this.tabAuditoria.Controls.Add(this.txtUsuario);
        this.tabAuditoria.Controls.Add(this.lblAccion);
        this.tabAuditoria.Controls.Add(this.cmbAccion);
        this.tabAuditoria.Controls.Add(this.btnConsultar);
        this.tabAuditoria.Controls.Add(this.dgvAuditoria);
        this.tabAuditoria.Text = "Auditoria de acciones";

        // ---- Pestaña Errores ----
        this.btnActualizarErrores.Location = new System.Drawing.Point(10, 8);
        this.btnActualizarErrores.Size = new System.Drawing.Size(100, 26);
        this.btnActualizarErrores.Text = "Actualizar";
        this.btnActualizarErrores.Click += new System.EventHandler(this.btnActualizarErrores_Click);

        this.lblNotaErrores.AutoSize = true;
        this.lblNotaErrores.ForeColor = System.Drawing.Color.DimGray;
        this.lblNotaErrores.Location = new System.Drawing.Point(125, 14);
        this.lblNotaErrores.Text = "Ultimos 200 errores inesperados. Busque por el \"Codigo de soporte\" que vio el usuario. Detalle tecnico completo: tabla LogErrores y carpeta Logs.";

        this.dgvErrores.Location = new System.Drawing.Point(10, 44);
        this.dgvErrores.Size = new System.Drawing.Size(840, 380);
        this.dgvErrores.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.dgvErrores.ReadOnly = true;
        this.dgvErrores.AllowUserToAddRows = false;
        this.dgvErrores.RowHeadersVisible = false;
        this.dgvErrores.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

        this.tabErrores.Controls.Add(this.btnActualizarErrores);
        this.tabErrores.Controls.Add(this.lblNotaErrores);
        this.tabErrores.Controls.Add(this.dgvErrores);
        this.tabErrores.Text = "Log de errores";

        this.tabControl1.Controls.Add(this.tabAuditoria);
        this.tabControl1.Controls.Add(this.tabErrores);
        this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(870, 480);
        this.Controls.Add(this.tabControl1);
        this.Name = "FormAuditoria";
        this.Text = "Auditoria y Errores (Administrador)";
        ((System.ComponentModel.ISupportInitialize)(this.dgvAuditoria)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvErrores)).EndInit();
        this.tabAuditoria.ResumeLayout(false);
        this.tabAuditoria.PerformLayout();
        this.tabErrores.ResumeLayout(false);
        this.tabErrores.PerformLayout();
        this.tabControl1.ResumeLayout(false);
        this.ResumeLayout(false);

        dtpDesde.Value = DateTime.Today.AddDays(-7);
        dtpHasta.Value = DateTime.Today;

        try
        {
            cmbAccion.Items.Add("(Todas)");
            foreach (string a in _service.ObtenerAcciones()) cmbAccion.Items.Add(a);
            cmbAccion.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Auditoria.Acciones");
        }

        CargarAuditoria();
        CargarErrores();
    }

    private void btnConsultar_Click(object? sender, EventArgs e) => CargarAuditoria();

    private void btnActualizarErrores_Click(object? sender, EventArgs e) => CargarErrores();

    private void CargarAuditoria()
    {
        if (dtpDesde.Value.Date > dtpHasta.Value.Date)
        {
            Mensajes.Advertencia(this, "La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.");
            return;
        }

        try
        {
            string? accion = cmbAccion.SelectedIndex <= 0 ? null : cmbAccion.Text;
            var lista = _service.Consultar(dtpDesde.Value, dtpHasta.Value, txtUsuario.Text, accion);
            dgvAuditoria.DataSource = lista.Select(r => new
            {
                Fecha = r.Fecha.ToString("yyyy-MM-dd HH:mm:ss"),
                r.Usuario,
                r.Accion,
                r.Entidad,
                Id = r.EntidadId,
                r.Detalle
            }).ToList();
            dgvAuditoria.Columns["Id"]!.FillWeight = 30;
            dgvAuditoria.Columns["Detalle"]!.FillWeight = 200;
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Auditoria.Consultar");
        }
    }

    private void CargarErrores()
    {
        try
        {
            dgvErrores.DataSource = _service.ObtenerUltimosErrores().Select(r => new
            {
                r.Codigo,
                Fecha = r.Fecha.ToString("yyyy-MM-dd HH:mm:ss"),
                r.Usuario,
                r.Contexto,
                r.Mensaje
            }).ToList();
            dgvErrores.Columns["Mensaje"]!.FillWeight = 200;
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Auditoria.Errores");
        }
    }
}
