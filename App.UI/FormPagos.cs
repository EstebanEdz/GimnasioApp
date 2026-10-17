using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

/// <summary>Control de pagos: saldo de cada inscripcion, registro de pagos/abonos y anulacion con motivo.</summary>
public partial class FormPagos : Form
{
    private System.Windows.Forms.GroupBox gbInscripcion;
    private System.Windows.Forms.Label lblInscripcion;
    private System.Windows.Forms.ComboBox cmbInscripcion;
    private System.Windows.Forms.Label lblTotal;
    private System.Windows.Forms.Label lblPagado;
    private System.Windows.Forms.Label lblSaldo;
    private System.Windows.Forms.Label lblEstadoPago;
    private System.Windows.Forms.GroupBox gbPago;
    private System.Windows.Forms.Label lblMonto;
    private System.Windows.Forms.NumericUpDown nudMonto;
    private System.Windows.Forms.Button btnPagarSaldo;
    private System.Windows.Forms.Label lblMetodo;
    private System.Windows.Forms.ComboBox cmbMetodo;
    private System.Windows.Forms.Label lblReferencia;
    private System.Windows.Forms.TextBox txtReferencia;
    private System.Windows.Forms.Label lblObservacion;
    private System.Windows.Forms.TextBox txtObservacion;
    private System.Windows.Forms.Button btnRegistrar;
    private System.Windows.Forms.Label lblHistorial;
    private System.Windows.Forms.DataGridView dgvPagos;
    private System.Windows.Forms.Button btnAnular;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;

    private readonly PagoService _service = new();
    private readonly int? _inscripcionInicial;
    private bool _cargando;

    public FormPagos(int? inscripcionId = null)
    {
        InitializeComponent();
        _inscripcionInicial = inscripcionId;

        this.gbInscripcion = new System.Windows.Forms.GroupBox();
        this.lblInscripcion = new System.Windows.Forms.Label();
        this.cmbInscripcion = new System.Windows.Forms.ComboBox();
        this.lblTotal = new System.Windows.Forms.Label();
        this.lblPagado = new System.Windows.Forms.Label();
        this.lblSaldo = new System.Windows.Forms.Label();
        this.lblEstadoPago = new System.Windows.Forms.Label();
        this.gbPago = new System.Windows.Forms.GroupBox();
        this.lblMonto = new System.Windows.Forms.Label();
        this.nudMonto = new System.Windows.Forms.NumericUpDown();
        this.btnPagarSaldo = new System.Windows.Forms.Button();
        this.lblMetodo = new System.Windows.Forms.Label();
        this.cmbMetodo = new System.Windows.Forms.ComboBox();
        this.lblReferencia = new System.Windows.Forms.Label();
        this.txtReferencia = new System.Windows.Forms.TextBox();
        this.lblObservacion = new System.Windows.Forms.Label();
        this.txtObservacion = new System.Windows.Forms.TextBox();
        this.btnRegistrar = new System.Windows.Forms.Button();
        this.lblHistorial = new System.Windows.Forms.Label();
        this.dgvPagos = new System.Windows.Forms.DataGridView();
        this.btnAnular = new System.Windows.Forms.Button();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        ((System.ComponentModel.ISupportInitialize)(this.nudMonto)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.gbInscripcion.SuspendLayout();
        this.gbPago.SuspendLayout();
        this.SuspendLayout();

        // ---- Inscripcion y saldo ----
        this.lblInscripcion.AutoSize = true;
        this.lblInscripcion.Location = new System.Drawing.Point(12, 25);
        this.lblInscripcion.Text = "Inscripcion";
        this.cmbInscripcion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbInscripcion.Location = new System.Drawing.Point(90, 22);
        this.cmbInscripcion.Size = new System.Drawing.Size(640, 21);
        this.cmbInscripcion.SelectedIndexChanged += new System.EventHandler(this.cmbInscripcion_SelectedIndexChanged);

        this.lblTotal.AutoSize = true;
        this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblTotal.Location = new System.Drawing.Point(12, 60);
        this.lblTotal.Text = "Total: $0";
        this.lblPagado.AutoSize = true;
        this.lblPagado.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblPagado.Location = new System.Drawing.Point(200, 60);
        this.lblPagado.Text = "Pagado: $0";
        this.lblSaldo.AutoSize = true;
        this.lblSaldo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblSaldo.Location = new System.Drawing.Point(400, 60);
        this.lblSaldo.Text = "Saldo: $0";
        this.lblEstadoPago.AutoSize = true;
        this.lblEstadoPago.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblEstadoPago.Location = new System.Drawing.Point(600, 60);
        this.lblEstadoPago.Text = "";

        this.gbInscripcion.Controls.Add(this.lblInscripcion);
        this.gbInscripcion.Controls.Add(this.cmbInscripcion);
        this.gbInscripcion.Controls.Add(this.lblTotal);
        this.gbInscripcion.Controls.Add(this.lblPagado);
        this.gbInscripcion.Controls.Add(this.lblSaldo);
        this.gbInscripcion.Controls.Add(this.lblEstadoPago);
        this.gbInscripcion.Location = new System.Drawing.Point(12, 10);
        this.gbInscripcion.Size = new System.Drawing.Size(745, 90);
        this.gbInscripcion.Text = "Inscripcion y saldo";

        // ---- Registrar pago ----
        this.lblMonto.AutoSize = true;
        this.lblMonto.Location = new System.Drawing.Point(12, 28);
        this.lblMonto.Text = "Monto ($)";
        this.nudMonto.Location = new System.Drawing.Point(90, 25);
        this.nudMonto.Size = new System.Drawing.Size(120, 20);
        this.nudMonto.Maximum = 1000000000M;
        this.nudMonto.Minimum = 0M;
        this.nudMonto.Increment = 1000M;
        this.nudMonto.ThousandsSeparator = true;
        this.btnPagarSaldo.Location = new System.Drawing.Point(218, 22);
        this.btnPagarSaldo.Size = new System.Drawing.Size(120, 26);
        this.btnPagarSaldo.Text = "Pagar el saldo";
        this.btnPagarSaldo.Click += new System.EventHandler(this.btnPagarSaldo_Click);

        this.lblMetodo.AutoSize = true;
        this.lblMetodo.Location = new System.Drawing.Point(370, 28);
        this.lblMetodo.Text = "Metodo";
        this.cmbMetodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbMetodo.Location = new System.Drawing.Point(425, 25);
        this.cmbMetodo.Size = new System.Drawing.Size(130, 21);
        this.cmbMetodo.SelectedIndexChanged += new System.EventHandler(this.cmbMetodo_SelectedIndexChanged);

        this.lblReferencia.AutoSize = true;
        this.lblReferencia.Location = new System.Drawing.Point(12, 62);
        this.lblReferencia.Text = "Referencia";
        this.txtReferencia.Location = new System.Drawing.Point(90, 59);
        this.txtReferencia.Size = new System.Drawing.Size(248, 20);
        this.txtReferencia.MaxLength = 50;

        this.lblObservacion.AutoSize = true;
        this.lblObservacion.Location = new System.Drawing.Point(370, 62);
        this.lblObservacion.Text = "Nota";
        this.txtObservacion.Location = new System.Drawing.Point(425, 59);
        this.txtObservacion.Size = new System.Drawing.Size(305, 20);
        this.txtObservacion.MaxLength = 200;

        this.btnRegistrar.Location = new System.Drawing.Point(12, 92);
        this.btnRegistrar.Size = new System.Drawing.Size(150, 30);
        this.btnRegistrar.Text = "Registrar pago";
        this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);

        this.gbPago.Controls.Add(this.lblMonto);
        this.gbPago.Controls.Add(this.nudMonto);
        this.gbPago.Controls.Add(this.btnPagarSaldo);
        this.gbPago.Controls.Add(this.lblMetodo);
        this.gbPago.Controls.Add(this.cmbMetodo);
        this.gbPago.Controls.Add(this.lblReferencia);
        this.gbPago.Controls.Add(this.txtReferencia);
        this.gbPago.Controls.Add(this.lblObservacion);
        this.gbPago.Controls.Add(this.txtObservacion);
        this.gbPago.Controls.Add(this.btnRegistrar);
        this.gbPago.Location = new System.Drawing.Point(12, 108);
        this.gbPago.Size = new System.Drawing.Size(745, 130);
        this.gbPago.Text = "Registrar pago / abono";

        // ---- Historial ----
        this.lblHistorial.AutoSize = true;
        this.lblHistorial.Location = new System.Drawing.Point(12, 248);
        this.lblHistorial.Text = "Historial de pagos de esta inscripcion";
        this.dgvPagos.Location = new System.Drawing.Point(12, 266);
        this.dgvPagos.Size = new System.Drawing.Size(745, 190);
        this.dgvPagos.ReadOnly = true;
        this.dgvPagos.AllowUserToAddRows = false;
        this.dgvPagos.RowHeadersVisible = false;
        this.dgvPagos.MultiSelect = false;
        this.dgvPagos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvPagos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvPagos.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvPagos_DataBindingComplete);

        this.btnAnular.Location = new System.Drawing.Point(12, 464);
        this.btnAnular.Size = new System.Drawing.Size(170, 28);
        this.btnAnular.Text = "Anular pago seleccionado";
        this.btnAnular.Click += new System.EventHandler(this.btnAnular_Click);

        this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
        this.toolTip1.SetToolTip(this.btnPagarSaldo, "Pone en el monto lo que falta por pagar");
        this.toolTip1.SetToolTip(this.btnAnular, "El pago no se borra: queda anulado con su motivo en el historial");

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(770, 505);
        this.Controls.Add(this.gbInscripcion);
        this.Controls.Add(this.gbPago);
        this.Controls.Add(this.lblHistorial);
        this.Controls.Add(this.dgvPagos);
        this.Controls.Add(this.btnAnular);
        this.Name = "FormPagos";
        this.Text = "Control de Pagos";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        ((System.ComponentModel.ISupportInitialize)(this.nudMonto)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.gbInscripcion.ResumeLayout(false);
        this.gbInscripcion.PerformLayout();
        this.gbPago.ResumeLayout(false);
        this.gbPago.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

        cmbMetodo.Items.AddRange(MetodosPago.Todos);
        cmbMetodo.SelectedIndex = 0;

        // Permisos: sin PAGOS_REGISTRAR no se puede registrar; sin PAGOS_ANULAR no se puede anular.
        gbPago.Enabled = SesionActual.Tiene(Permisos.PagosRegistrar);
        btnAnular.Enabled = SesionActual.Tiene(Permisos.PagosAnular);

        CargarInscripciones(_inscripcionInicial);
    }

    private void CargarInscripciones(int? seleccionarId)
    {
        try
        {
            _cargando = true;
            var lista = _service.ObtenerInscripcionesParaPago();
            cmbInscripcion.Items.Clear();
            foreach (var m in lista) cmbInscripcion.Items.Add(m);

            if (seleccionarId.HasValue)
            {
                var objetivo = lista.FirstOrDefault(m => m.InscripcionId == seleccionarId.Value);
                if (objetivo != null) cmbInscripcion.SelectedItem = objetivo;
            }
            if (cmbInscripcion.SelectedIndex < 0 && cmbInscripcion.Items.Count > 0)
                cmbInscripcion.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Pagos.CargarInscripciones");
        }
        finally
        {
            _cargando = false;
        }
        ActualizarDetalle();
    }

    private void cmbInscripcion_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_cargando) ActualizarDetalle();
    }

    private void cmbMetodo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        bool pideReferencia = MetodosPago.RequiereReferencia(cmbMetodo.Text);
        lblReferencia.Text = pideReferencia ? "Referencia *" : "Referencia";
    }

    /// <summary>Refresca los totales, el monto sugerido y el historial de la inscripcion seleccionada.</summary>
    private void ActualizarDetalle()
    {
        errorProvider1.Clear();

        if (cmbInscripcion.SelectedItem is not Membresia seleccionada)
        {
            lblTotal.Text = "Total: $0";
            lblPagado.Text = "Pagado: $0";
            lblSaldo.Text = "Saldo: $0";
            lblEstadoPago.Text = "";
            dgvPagos.DataSource = null;
            btnRegistrar.Enabled = false;
            return;
        }

        try
        {
            var saldo = _service.ObtenerSaldo(seleccionada.InscripcionId);
            lblTotal.Text = $"Total: ${saldo.Total:N0}";
            lblPagado.Text = $"Pagado: ${saldo.Pagado:N0}";
            lblSaldo.Text = $"Saldo: ${saldo.Saldo:N0}";
            lblEstadoPago.Text = saldo.EstadoPago;
            lblEstadoPago.ForeColor = saldo.EstadoPago == "Pagada" ? System.Drawing.Color.DarkGreen
                : saldo.EstadoPago == "Parcial" ? System.Drawing.Color.DarkOrange : System.Drawing.Color.Firebrick;

            nudMonto.Value = Math.Min(saldo.Saldo > 0 ? saldo.Saldo : 0M, nudMonto.Maximum);
            btnRegistrar.Enabled = saldo.Saldo > 0;
            btnPagarSaldo.Enabled = saldo.Saldo > 0;

            var pagos = _service.ObtenerPorInscripcion(seleccionada.InscripcionId);
            dgvPagos.DataSource = pagos.Select(p => new
            {
                p.Id,
                Fecha = p.FechaPago.ToString("yyyy-MM-dd HH:mm"),
                p.Monto,
                Metodo = p.MetodoPago,
                p.Referencia,
                p.Estado,
                Registro = p.Usuario,
                Nota = p.Observacion,
                Motivo = p.MotivoAnulacion
            }).ToList();

            dgvPagos.Columns["Monto"]!.DefaultCellStyle.Format = "N0";
            dgvPagos.Columns["Monto"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPagos.Columns["Id"]!.FillWeight = 30;
            dgvPagos.Columns["Registro"]!.HeaderText = "Registrado por";
            dgvPagos.Columns["Motivo"]!.HeaderText = "Motivo anulacion";
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Pagos.ActualizarDetalle");
        }
    }

    private void dgvPagos_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        // Los pagos anulados se ven tachados y en gris para que no se confundan con los vigentes.
        var tachado = new System.Drawing.Font(dgvPagos.Font, System.Drawing.FontStyle.Strikeout);
        foreach (DataGridViewRow fila in dgvPagos.Rows)
        {
            if (Convert.ToString(fila.Cells["Estado"].Value) == "Anulado")
            {
                fila.DefaultCellStyle.ForeColor = System.Drawing.Color.Gray;
                fila.DefaultCellStyle.Font = tachado;
            }
        }
    }

    private void btnPagarSaldo_Click(object? sender, EventArgs e)
    {
        if (cmbInscripcion.SelectedItem is not Membresia seleccionada) return;
        try
        {
            var saldo = _service.ObtenerSaldo(seleccionada.InscripcionId);
            nudMonto.Value = Math.Min(Math.Max(saldo.Saldo, 0M), nudMonto.Maximum);
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Pagos.PagarSaldo");
        }
    }

    private void btnRegistrar_Click(object? sender, EventArgs e)
    {
        errorProvider1.Clear();

        if (cmbInscripcion.SelectedItem is not Membresia seleccionada)
        {
            errorProvider1.SetError(cmbInscripcion, "Seleccione una inscripcion.");
            return;
        }

        var pago = new Pago
        {
            InscripcionId = seleccionada.InscripcionId,
            Monto = nudMonto.Value,
            MetodoPago = cmbMetodo.Text,
            Referencia = txtReferencia.Text,
            Observacion = txtObservacion.Text
        };

        var errores = _service.Validar(pago.InscripcionId, pago.Monto, pago.MetodoPago, pago.Referencia);
        if (errores.Count > 0)
        {
            Mensajes.Advertencia(this, "Revise lo siguiente:\n\n- " + string.Join("\n- ", errores));
            return;
        }

        if (!Mensajes.Confirmar(this, $"¿Registrar un pago de ${pago.Monto:N0} por {pago.MetodoPago} a nombre de {seleccionada.Miembro}?")) return;

        try
        {
            int id = _service.Registrar(pago);
            var saldo = _service.ObtenerSaldo(pago.InscripcionId);
            Mensajes.Info(this,
                $"Pago #{id} registrado.\n\nSaldo pendiente: ${saldo.Saldo:N0}" + (saldo.Saldo <= 0 ? "\n\nLa inscripcion quedo pagada por completo." : string.Empty),
                "Pago registrado");

            txtReferencia.Clear();
            txtObservacion.Clear();
            CargarInscripciones(pago.InscripcionId);
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Pagos.Registrar");
            CargarInscripciones(pago.InscripcionId);   // el saldo pudo cambiar mientras tanto
        }
    }

    private void btnAnular_Click(object? sender, EventArgs e)
    {
        if (dgvPagos.CurrentRow == null)
        {
            Mensajes.Info(this, "Seleccione un pago del historial.", "Aviso");
            return;
        }

        int pagoId = Convert.ToInt32(dgvPagos.CurrentRow.Cells["Id"].Value);
        if (Convert.ToString(dgvPagos.CurrentRow.Cells["Estado"].Value) == "Anulado")
        {
            Mensajes.Info(this, "Ese pago ya esta anulado.", "Aviso");
            return;
        }

        string? motivo = Mensajes.PedirTexto(this, "Anular pago", $"Motivo de la anulacion del pago #{pagoId}:");
        if (motivo == null) return;

        try
        {
            _service.Anular(pagoId, motivo);
            int? idInscripcion = (cmbInscripcion.SelectedItem as Membresia)?.InscripcionId;
            CargarInscripciones(idInscripcion);
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "Pagos.Anular");
        }
    }
}
