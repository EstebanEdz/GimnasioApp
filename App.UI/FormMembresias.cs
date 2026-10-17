using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

/// <summary>Control de membresias: estados, vencimientos, renovaciones, congelamientos y saldos.</summary>
public partial class FormMembresias : Form
{
    private System.Windows.Forms.Label lblEstado;
    private System.Windows.Forms.ComboBox cmbEstado;
    private System.Windows.Forms.Label lblBuscar;
    private System.Windows.Forms.TextBox txtBuscar;
    private System.Windows.Forms.CheckBox chkConSaldo;
    private System.Windows.Forms.Button btnBuscar;
    private System.Windows.Forms.DataGridView dgvMembresias;
    private System.Windows.Forms.Button btnRenovar;
    private System.Windows.Forms.Button btnCongelar;
    private System.Windows.Forms.Button btnReactivar;
    private System.Windows.Forms.Button btnCancelarMembresia;
    private System.Windows.Forms.Button btnPagos;
    private System.Windows.Forms.Label lblResumen;
    private System.Windows.Forms.Label lblLeyenda;
    private System.Windows.Forms.ToolTip toolTip1;

    private readonly MembresiaService _service = new();
    private readonly int _diasAviso = Configuracion.DiasAvisoVencimiento;

    public FormMembresias()
    {
        InitializeComponent();
        this.lblEstado = new System.Windows.Forms.Label();
        this.cmbEstado = new System.Windows.Forms.ComboBox();
        this.lblBuscar = new System.Windows.Forms.Label();
        this.txtBuscar = new System.Windows.Forms.TextBox();
        this.chkConSaldo = new System.Windows.Forms.CheckBox();
        this.btnBuscar = new System.Windows.Forms.Button();
        this.dgvMembresias = new System.Windows.Forms.DataGridView();
        this.btnRenovar = new System.Windows.Forms.Button();
        this.btnCongelar = new System.Windows.Forms.Button();
        this.btnReactivar = new System.Windows.Forms.Button();
        this.btnCancelarMembresia = new System.Windows.Forms.Button();
        this.btnPagos = new System.Windows.Forms.Button();
        this.lblResumen = new System.Windows.Forms.Label();
        this.lblLeyenda = new System.Windows.Forms.Label();
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        ((System.ComponentModel.ISupportInitialize)(this.dgvMembresias)).BeginInit();
        this.SuspendLayout();

        // ---- Filtros ----
        this.lblEstado.AutoSize = true;
        this.lblEstado.Location = new System.Drawing.Point(12, 17);
        this.lblEstado.Text = "Estado";
        this.cmbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbEstado.Location = new System.Drawing.Point(60, 13);
        this.cmbEstado.Size = new System.Drawing.Size(130, 21);
        this.cmbEstado.Items.AddRange(new object[] { "Todas", "Vigente", "Por vencer", "Vencida", "Congelada", "Cancelada" });
        this.cmbEstado.SelectedIndex = 0;
        this.cmbEstado.SelectedIndexChanged += new System.EventHandler(this.Filtros_Changed);

        this.lblBuscar.AutoSize = true;
        this.lblBuscar.Location = new System.Drawing.Point(210, 17);
        this.lblBuscar.Text = "Miembro / documento";
        this.txtBuscar.Location = new System.Drawing.Point(335, 13);
        this.txtBuscar.Size = new System.Drawing.Size(180, 20);
        this.txtBuscar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBuscar_KeyPress);

        this.chkConSaldo.AutoSize = true;
        this.chkConSaldo.Location = new System.Drawing.Point(530, 15);
        this.chkConSaldo.Text = "Solo con saldo pendiente";
        this.chkConSaldo.CheckedChanged += new System.EventHandler(this.Filtros_Changed);

        this.btnBuscar.Location = new System.Drawing.Point(710, 10);
        this.btnBuscar.Size = new System.Drawing.Size(90, 26);
        this.btnBuscar.Text = "Buscar";
        this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);

        // ---- Grid ----
        this.dgvMembresias.Location = new System.Drawing.Point(12, 46);
        this.dgvMembresias.Size = new System.Drawing.Size(960, 360);
        this.dgvMembresias.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.dgvMembresias.ReadOnly = true;
        this.dgvMembresias.AllowUserToAddRows = false;
        this.dgvMembresias.RowHeadersVisible = false;
        this.dgvMembresias.MultiSelect = false;
        this.dgvMembresias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvMembresias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvMembresias.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvMembresias_DataBindingComplete);

        // ---- Acciones ----
        int y = 418;
        this.btnRenovar.Location = new System.Drawing.Point(12, y);
        this.btnRenovar.Size = new System.Drawing.Size(110, 30);
        this.btnRenovar.Text = "Renovar";
        this.btnRenovar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this.btnRenovar.Click += new System.EventHandler(this.btnRenovar_Click);

        this.btnCongelar.Location = new System.Drawing.Point(130, y);
        this.btnCongelar.Size = new System.Drawing.Size(110, 30);
        this.btnCongelar.Text = "Congelar";
        this.btnCongelar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this.btnCongelar.Click += new System.EventHandler(this.btnCongelar_Click);

        this.btnReactivar.Location = new System.Drawing.Point(248, y);
        this.btnReactivar.Size = new System.Drawing.Size(110, 30);
        this.btnReactivar.Text = "Reactivar";
        this.btnReactivar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this.btnReactivar.Click += new System.EventHandler(this.btnReactivar_Click);

        this.btnCancelarMembresia.Location = new System.Drawing.Point(366, y);
        this.btnCancelarMembresia.Size = new System.Drawing.Size(110, 30);
        this.btnCancelarMembresia.Text = "Cancelar";
        this.btnCancelarMembresia.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this.btnCancelarMembresia.Click += new System.EventHandler(this.btnCancelarMembresia_Click);

        this.btnPagos.Location = new System.Drawing.Point(494, y);
        this.btnPagos.Size = new System.Drawing.Size(150, 30);
        this.btnPagos.Text = "Pagos de la membresia...";
        this.btnPagos.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this.btnPagos.Click += new System.EventHandler(this.btnPagos_Click);

        this.lblResumen.AutoSize = true;
        this.lblResumen.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblResumen.Location = new System.Drawing.Point(12, 460);
        this.lblResumen.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this.lblResumen.Text = "";

        this.lblLeyenda.AutoSize = true;
        this.lblLeyenda.ForeColor = System.Drawing.Color.DimGray;
        this.lblLeyenda.Location = new System.Drawing.Point(12, 482);
        this.lblLeyenda.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this.lblLeyenda.Text = $"Rojo = vencida   |   Amarillo = vence en {_diasAviso} dias o menos   |   Azul = congelada   |   Gris = cancelada   |   Texto en negrita = tiene saldo pendiente";

        this.toolTip1.SetToolTip(this.btnRenovar, "Crea una inscripcion nueva con el mismo plan (empieza al vencer la actual, o hoy si ya vencio)");
        this.toolTip1.SetToolTip(this.btnCongelar, "Pausa la membresia; al reactivarla se suman los dias congelados al vencimiento");
        this.toolTip1.SetToolTip(this.btnCancelarMembresia, "Cancela la membresia (pide motivo). El historial de pagos se conserva");

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(984, 510);
        this.Controls.Add(this.lblEstado);
        this.Controls.Add(this.cmbEstado);
        this.Controls.Add(this.lblBuscar);
        this.Controls.Add(this.txtBuscar);
        this.Controls.Add(this.chkConSaldo);
        this.Controls.Add(this.btnBuscar);
        this.Controls.Add(this.dgvMembresias);
        this.Controls.Add(this.btnRenovar);
        this.Controls.Add(this.btnCongelar);
        this.Controls.Add(this.btnReactivar);
        this.Controls.Add(this.btnCancelarMembresia);
        this.Controls.Add(this.btnPagos);
        this.Controls.Add(this.lblResumen);
        this.Controls.Add(this.lblLeyenda);
        this.Name = "FormMembresias";
        this.Text = "Control de Membresias";
        ((System.ComponentModel.ISupportInitialize)(this.dgvMembresias)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

        // Permisos: los botones de gestion solo se habilitan si el perfil puede gestionar membresias / ver pagos.
        bool puedeGestionar = SesionActual.Tiene(Permisos.MembresiasGestionar);
        btnRenovar.Enabled = puedeGestionar;
        btnCongelar.Enabled = puedeGestionar;
        btnReactivar.Enabled = puedeGestionar;
        btnCancelarMembresia.Enabled = puedeGestionar;
        btnPagos.Enabled = SesionActual.Tiene(Permisos.PagosVer);

        CargarGrid();
    }

    private void Filtros_Changed(object? sender, EventArgs e) => CargarGrid();

    private void btnBuscar_Click(object? sender, EventArgs e) => CargarGrid();

    private void txtBuscar_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            e.Handled = true;
            CargarGrid();
        }
    }

    private void CargarGrid(int? seleccionarId = null)
    {
        try
        {
            string seleccion = cmbEstado.Text;
            string? estado = seleccion is "Todas" or "Por vencer" or "" ? null : seleccion;
            int? porVencer = seleccion == "Por vencer" ? _diasAviso : null;

            var lista = _service.Listar(estado, txtBuscar.Text, porVencer, chkConSaldo.Checked);

            dgvMembresias.DataSource = lista.Select(m => new
            {
                Id = m.InscripcionId,
                m.Miembro,
                m.Documento,
                m.Plan,
                Vence = m.FechaVencimiento.ToString("yyyy-MM-dd"),
                m.Estado,
                Dias = m.Estado == EstadosMembresia.Vigente ? m.DiasParaVencer.ToString() : "",
                m.Total,
                m.Pagado,
                m.Saldo
            }).ToList();

            foreach (string columna in new[] { "Total", "Pagado", "Saldo" })
            {
                dgvMembresias.Columns[columna]!.DefaultCellStyle.Format = "N0";
                dgvMembresias.Columns[columna]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dgvMembresias.Columns["Dias"]!.HeaderText = "Dias p/vencer";
            dgvMembresias.Columns["Id"]!.FillWeight = 40;

            decimal pendiente = lista.Where(m => m.Estado != EstadosMembresia.Cancelada).Sum(m => m.Saldo);
            lblResumen.Text = $"{lista.Count} membresia(s)   |   Saldo pendiente en esta lista: ${pendiente:N0}";

            if (seleccionarId.HasValue)
            {
                foreach (DataGridViewRow fila in dgvMembresias.Rows)
                {
                    if (Convert.ToInt32(fila.Cells["Id"].Value) == seleccionarId.Value)
                    {
                        fila.Selected = true;
                        dgvMembresias.CurrentCell = fila.Cells["Miembro"];
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Membresias.Cargar");
        }
    }

    /// <summary>Colorea cada fila segun el estado de la membresia y marca en negrita las que deben dinero.</summary>
    private void dgvMembresias_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        var negrita = new System.Drawing.Font(dgvMembresias.Font, System.Drawing.FontStyle.Bold);

        foreach (DataGridViewRow fila in dgvMembresias.Rows)
        {
            string estado = Convert.ToString(fila.Cells["Estado"].Value) ?? string.Empty;
            decimal saldo = Convert.ToDecimal(fila.Cells["Saldo"].Value);
            _ = int.TryParse(Convert.ToString(fila.Cells["Dias"].Value), out int dias);

            System.Drawing.Color? fondo = estado switch
            {
                EstadosMembresia.Vencida => System.Drawing.Color.MistyRose,
                EstadosMembresia.Congelada => System.Drawing.Color.AliceBlue,
                EstadosMembresia.Cancelada => System.Drawing.Color.Gainsboro,
                EstadosMembresia.Vigente when dias <= _diasAviso => System.Drawing.Color.LemonChiffon,
                _ => null
            };

            if (fondo.HasValue) fila.DefaultCellStyle.BackColor = fondo.Value;
            if (saldo > 0 && estado != EstadosMembresia.Cancelada) fila.DefaultCellStyle.Font = negrita;
        }
    }

    private bool TryObtenerId(out int id)
    {
        id = 0;
        if (dgvMembresias.CurrentRow == null)
        {
            Mensajes.Info(this, "Seleccione una membresia de la lista.", "Aviso");
            return false;
        }
        id = Convert.ToInt32(dgvMembresias.CurrentRow.Cells["Id"].Value);
        return true;
    }

    private void btnRenovar_Click(object? sender, EventArgs e)
    {
        if (!TryObtenerId(out int id)) return;
        if (!Mensajes.Confirmar(this, $"¿Renovar la membresia #{id}?\n\nSe creara una inscripcion nueva con el mismo plan.")) return;

        try
        {
            int nuevoId = _service.Renovar(id);
            Mensajes.Info(this, $"Membresia renovada. Nueva inscripcion #{nuevoId}.\n\nRecuerde registrar el pago.", "Renovacion");
            CargarGrid(nuevoId);
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Membresias.Renovar");
        }
    }

    private void btnCongelar_Click(object? sender, EventArgs e)
    {
        if (!TryObtenerId(out int id)) return;
        if (!Mensajes.Confirmar(this, $"¿Congelar la membresia #{id} desde hoy?")) return;

        try
        {
            _service.Congelar(id);
            CargarGrid(id);
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Membresias.Congelar");
        }
    }

    private void btnReactivar_Click(object? sender, EventArgs e)
    {
        if (!TryObtenerId(out int id)) return;

        try
        {
            _service.Descongelar(id);
            Mensajes.Info(this, "Membresia reactivada. Se sumaron al vencimiento los dias que estuvo congelada.", "Reactivacion");
            CargarGrid(id);
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Membresias.Reactivar");
        }
    }

    private void btnCancelarMembresia_Click(object? sender, EventArgs e)
    {
        if (!TryObtenerId(out int id)) return;

        string? motivo = Mensajes.PedirTexto(this, "Cancelar membresia", $"Motivo de la cancelacion de la membresia #{id}:");
        if (motivo == null) return;

        try
        {
            _service.Cancelar(id, motivo);
            CargarGrid(id);
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Membresias.Cancelar");
        }
    }

    private void btnPagos_Click(object? sender, EventArgs e)
    {
        if (!TryObtenerId(out int id)) return;

        using var pagos = new FormPagos(id);
        pagos.ShowDialog(this);
        CargarGrid(id);   // el saldo pudo cambiar
    }
}
