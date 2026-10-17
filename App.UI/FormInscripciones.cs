using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

public partial class FormInscripciones : Form
{
    private System.Windows.Forms.Panel panelRegistro;
    private System.Windows.Forms.Label lblMiembro;
    private System.Windows.Forms.Label lblPlan;
    private System.Windows.Forms.Label lblEntrenador;
    private System.Windows.Forms.Label lblFecha;
    private System.Windows.Forms.Label lblConcepto;
    private System.Windows.Forms.Label lblCantidad;
    private System.Windows.Forms.Label lblPrecioUnitario;
    private System.Windows.Forms.ComboBox cmbMiembro;
    private System.Windows.Forms.ComboBox cmbPlan;
    private System.Windows.Forms.ComboBox cmbEntrenador;
    private System.Windows.Forms.DateTimePicker dtpFechaInscripcion;
    private System.Windows.Forms.TextBox txtConcepto;
    private System.Windows.Forms.NumericUpDown numCantidad;
    private System.Windows.Forms.TextBox txtPrecioUnitario;
    private System.Windows.Forms.Button btnAgregarDetalle;
    private System.Windows.Forms.Button btnQuitarDetalle;
    private System.Windows.Forms.ListBox lstDetalle;
    private System.Windows.Forms.Label lblTotal;
    private System.Windows.Forms.Button btnRegistrar;
    private System.Windows.Forms.Button btnLimpiar;
    private System.Windows.Forms.Label lblPendientes;
    private System.Windows.Forms.Button btnAtenderPendiente;
    private System.Windows.Forms.Label lblFiltro;
    private System.Windows.Forms.ComboBox cmbFiltroEstado;
    private System.Windows.Forms.DataGridView dgvInscripciones;
    private System.Windows.Forms.Button btnEliminar;
    private System.Windows.Forms.Button btnDeshacer;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;

    private readonly InscripcionService _service = new();
    private readonly MiembroService _miembroService = new();
    private readonly PlanService _planService = new();
    private readonly EntrenadorService _entrenadorService = new();
    private readonly bool _soloLectura;

    private readonly List<DetalleInscripcion> _detalleActual = new();
    private int _idEnEdicion = -1;

    private class ItemCombo
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public override string ToString() => Texto;
    }

    public FormInscripciones(bool soloLectura = false)
    {
        InitializeComponent();
        this.panelRegistro = new System.Windows.Forms.Panel();
        this.lblMiembro = new System.Windows.Forms.Label();
        this.lblPlan = new System.Windows.Forms.Label();
        this.lblEntrenador = new System.Windows.Forms.Label();
        this.lblFecha = new System.Windows.Forms.Label();
        this.lblConcepto = new System.Windows.Forms.Label();
        this.lblCantidad = new System.Windows.Forms.Label();
        this.lblPrecioUnitario = new System.Windows.Forms.Label();
        this.cmbMiembro = new System.Windows.Forms.ComboBox();
        this.cmbPlan = new System.Windows.Forms.ComboBox();
        this.cmbEntrenador = new System.Windows.Forms.ComboBox();
        this.dtpFechaInscripcion = new System.Windows.Forms.DateTimePicker();
        this.txtConcepto = new System.Windows.Forms.TextBox();
        this.numCantidad = new System.Windows.Forms.NumericUpDown();
        this.txtPrecioUnitario = new System.Windows.Forms.TextBox();
        this.btnAgregarDetalle = new System.Windows.Forms.Button();
        this.btnQuitarDetalle = new System.Windows.Forms.Button();
        this.lstDetalle = new System.Windows.Forms.ListBox();
        this.lblTotal = new System.Windows.Forms.Label();
        this.btnRegistrar = new System.Windows.Forms.Button();
        this.btnLimpiar = new System.Windows.Forms.Button();
        this.lblPendientes = new System.Windows.Forms.Label();
        this.btnAtenderPendiente = new System.Windows.Forms.Button();
        this.lblFiltro = new System.Windows.Forms.Label();
        this.cmbFiltroEstado = new System.Windows.Forms.ComboBox();
        this.dgvInscripciones = new System.Windows.Forms.DataGridView();
        this.btnEliminar = new System.Windows.Forms.Button();
        this.btnDeshacer = new System.Windows.Forms.Button();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvInscripciones)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.panelRegistro.SuspendLayout();
        this.SuspendLayout();

        // ---- Panel de registro ----
        this.panelRegistro.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.panelRegistro.Location = new System.Drawing.Point(12, 12);
        this.panelRegistro.Size = new System.Drawing.Size(860, 235);

        this.lblMiembro.AutoSize = true;
        this.lblMiembro.Location = new System.Drawing.Point(10, 12);
        this.lblMiembro.Text = "Miembro";
        this.cmbMiembro.Location = new System.Drawing.Point(100, 9);
        this.cmbMiembro.Size = new System.Drawing.Size(230, 21);
        this.cmbMiembro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

        this.lblPlan.AutoSize = true;
        this.lblPlan.Location = new System.Drawing.Point(345, 12);
        this.lblPlan.Text = "Plan";
        this.cmbPlan.Location = new System.Drawing.Point(400, 9);
        this.cmbPlan.Size = new System.Drawing.Size(230, 21);
        this.cmbPlan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbPlan.SelectedIndexChanged += new System.EventHandler(this.cmbPlan_SelectedIndexChanged);

        this.lblEntrenador.AutoSize = true;
        this.lblEntrenador.Location = new System.Drawing.Point(645, 12);
        this.lblEntrenador.Text = "Entrenador";
        this.cmbEntrenador.Location = new System.Drawing.Point(10, 40);
        this.cmbEntrenador.Size = new System.Drawing.Size(230, 21);
        this.cmbEntrenador.Location = new System.Drawing.Point(720, 9);
        this.cmbEntrenador.Size = new System.Drawing.Size(120, 21);
        this.cmbEntrenador.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

        this.lblFecha.AutoSize = true;
        this.lblFecha.Location = new System.Drawing.Point(10, 44);
        this.lblFecha.Text = "Fecha inscripcion";
        this.dtpFechaInscripcion.Location = new System.Drawing.Point(120, 41);
        this.dtpFechaInscripcion.Size = new System.Drawing.Size(120, 20);
        this.dtpFechaInscripcion.Format = System.Windows.Forms.DateTimePickerFormat.Short;

        this.lblConcepto.AutoSize = true;
        this.lblConcepto.Location = new System.Drawing.Point(10, 82);
        this.lblConcepto.Text = "Concepto";
        this.txtConcepto.Location = new System.Drawing.Point(100, 79);
        this.txtConcepto.Size = new System.Drawing.Size(230, 20);

        this.lblCantidad.AutoSize = true;
        this.lblCantidad.Location = new System.Drawing.Point(345, 82);
        this.lblCantidad.Text = "Cant.";
        this.numCantidad.Location = new System.Drawing.Point(390, 79);
        this.numCantidad.Size = new System.Drawing.Size(60, 20);

        this.lblPrecioUnitario.AutoSize = true;
        this.lblPrecioUnitario.Location = new System.Drawing.Point(465, 82);
        this.lblPrecioUnitario.Text = "Precio unit.";
        this.txtPrecioUnitario.Location = new System.Drawing.Point(540, 79);
        this.txtPrecioUnitario.Size = new System.Drawing.Size(100, 20);

        this.btnAgregarDetalle.Location = new System.Drawing.Point(650, 77);
        this.btnAgregarDetalle.Size = new System.Drawing.Size(90, 24);
        this.btnAgregarDetalle.Text = "Agregar";
        this.btnAgregarDetalle.Click += new System.EventHandler(this.btnAgregarDetalle_Click);

        this.btnQuitarDetalle.Location = new System.Drawing.Point(750, 77);
        this.btnQuitarDetalle.Size = new System.Drawing.Size(90, 24);
        this.btnQuitarDetalle.Text = "Quitar item";
        this.btnQuitarDetalle.Click += new System.EventHandler(this.btnQuitarDetalle_Click);

        this.lstDetalle.Location = new System.Drawing.Point(10, 110);
        this.lstDetalle.Size = new System.Drawing.Size(600, 90);

        this.lblTotal.AutoSize = true;
        this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblTotal.Location = new System.Drawing.Point(650, 115);
        this.lblTotal.Text = "Total: $0";

        this.btnRegistrar.Location = new System.Drawing.Point(650, 150);
        this.btnRegistrar.Size = new System.Drawing.Size(110, 28);
        this.btnRegistrar.Text = "Registrar";
        this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);

        this.btnLimpiar.Location = new System.Drawing.Point(766, 150);
        this.btnLimpiar.Size = new System.Drawing.Size(80, 28);
        this.btnLimpiar.Text = "Limpiar";
        this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

        this.lblPendientes.AutoSize = true;
        this.lblPendientes.Location = new System.Drawing.Point(10, 208);
        this.lblPendientes.Text = "Turnos pendientes en cola: 0";

        this.btnAtenderPendiente.Location = new System.Drawing.Point(220, 202);
        this.btnAtenderPendiente.Size = new System.Drawing.Size(170, 26);
        this.btnAtenderPendiente.Text = "Atender siguiente pendiente";
        this.btnAtenderPendiente.Click += new System.EventHandler(this.btnAtenderPendiente_Click);

        this.panelRegistro.Controls.Add(this.lblMiembro);
        this.panelRegistro.Controls.Add(this.cmbMiembro);
        this.panelRegistro.Controls.Add(this.lblPlan);
        this.panelRegistro.Controls.Add(this.cmbPlan);
        this.panelRegistro.Controls.Add(this.lblEntrenador);
        this.panelRegistro.Controls.Add(this.cmbEntrenador);
        this.panelRegistro.Controls.Add(this.lblFecha);
        this.panelRegistro.Controls.Add(this.dtpFechaInscripcion);
        this.panelRegistro.Controls.Add(this.lblConcepto);
        this.panelRegistro.Controls.Add(this.txtConcepto);
        this.panelRegistro.Controls.Add(this.lblCantidad);
        this.panelRegistro.Controls.Add(this.numCantidad);
        this.panelRegistro.Controls.Add(this.lblPrecioUnitario);
        this.panelRegistro.Controls.Add(this.txtPrecioUnitario);
        this.panelRegistro.Controls.Add(this.btnAgregarDetalle);
        this.panelRegistro.Controls.Add(this.btnQuitarDetalle);
        this.panelRegistro.Controls.Add(this.lstDetalle);
        this.panelRegistro.Controls.Add(this.lblTotal);
        this.panelRegistro.Controls.Add(this.btnRegistrar);
        this.panelRegistro.Controls.Add(this.btnLimpiar);
        this.panelRegistro.Controls.Add(this.lblPendientes);
        this.panelRegistro.Controls.Add(this.btnAtenderPendiente);

        // ---- Filtro y grid ----
        this.lblFiltro.AutoSize = true;
        this.lblFiltro.Location = new System.Drawing.Point(12, 258);
        this.lblFiltro.Text = "Filtrar por estado";
        this.cmbFiltroEstado.Location = new System.Drawing.Point(120, 255);
        this.cmbFiltroEstado.Size = new System.Drawing.Size(120, 21);
        this.cmbFiltroEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbFiltroEstado.SelectedIndexChanged += new System.EventHandler(this.cmbFiltroEstado_SelectedIndexChanged);

        this.btnEliminar.Location = new System.Drawing.Point(660, 253);
        this.btnEliminar.Size = new System.Drawing.Size(100, 26);
        this.btnEliminar.Text = "Eliminar";
        this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

        this.btnDeshacer.Location = new System.Drawing.Point(766, 253);
        this.btnDeshacer.Size = new System.Drawing.Size(106, 26);
        this.btnDeshacer.Text = "Deshacer";
        this.btnDeshacer.Click += new System.EventHandler(this.btnDeshacer_Click);

        this.dgvInscripciones.Location = new System.Drawing.Point(12, 285);
        this.dgvInscripciones.Size = new System.Drawing.Size(860, 260);
        this.dgvInscripciones.ReadOnly = true;
        this.dgvInscripciones.AllowUserToAddRows = false;
        this.dgvInscripciones.RowHeadersVisible = false;
        this.dgvInscripciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvInscripciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvInscripciones.DoubleClick += new System.EventHandler(this.dgvInscripciones_DoubleClick);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(884, 557);
        this.Controls.Add(this.panelRegistro);
        this.Controls.Add(this.lblFiltro);
        this.Controls.Add(this.cmbFiltroEstado);
        this.Controls.Add(this.btnEliminar);
        this.Controls.Add(this.btnDeshacer);
        this.Controls.Add(this.dgvInscripciones);
        this.Name = "FormInscripciones";
        this.Text = "Gestion de Inscripciones";
        ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.dgvInscripciones)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.panelRegistro.ResumeLayout(false);
        this.panelRegistro.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    
        _soloLectura = soloLectura;

        toolTip1.SetToolTip(cmbMiembro, "Miembro que se inscribe");
        toolTip1.SetToolTip(cmbPlan, "Plan a contratar");
        toolTip1.SetToolTip(cmbEntrenador, "Entrenador asignado (opcional)");
        toolTip1.SetToolTip(txtConcepto, "Concepto del item (ej. Plan Mensual, Sesion adicional)");
        toolTip1.SetToolTip(numCantidad, "Cantidad de unidades del item");
        toolTip1.SetToolTip(txtPrecioUnitario, "Precio unitario, solo numeros");

        txtPrecioUnitario.KeyPress += (s, e) =>
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;
            if (e.KeyChar == '.' && !txtPrecioUnitario.Text.Contains('.')) return;
            e.Handled = true;
        };

        numCantidad.Minimum = 1;
        numCantidad.Value = 1;
        dtpFechaInscripcion.Value = DateTime.Today;

        CargarCombosDesdeBd();
        cmbFiltroEstado.Items.AddRange(new object[] { "Todos", "Vigente", "Vencida" });
        cmbFiltroEstado.SelectedIndex = 0;

        if (_soloLectura)
        {
            panelRegistro.Enabled = false;
            btnEliminar.Enabled = false;
            btnDeshacer.Enabled = false;
            Text += " (solo lectura)";
        }

        CargarGrid();
    }

    private void CargarCombosDesdeBd()
    {
        cmbMiembro.Items.Clear();
        foreach (var m in _miembroService.ObtenerTodos().Where(m => m.Activo))
            cmbMiembro.Items.Add(new ItemCombo { Id = m.Id, Texto = $"{m.Nombre} ({m.Documento})" });

        cmbPlan.Items.Clear();
        foreach (var p in _planService.ObtenerTodos(soloActivos: true))
            cmbPlan.Items.Add(new ItemCombo { Id = p.Id, Texto = $"{p.Nombre} - {p.TipoDuracion} - ${p.Precio:N0}" });

        cmbEntrenador.Items.Clear();
        cmbEntrenador.Items.Add(new ItemCombo { Id = 0, Texto = "(Sin entrenador)" });
        foreach (var e in _entrenadorService.ObtenerTodos(soloActivos: true))
            cmbEntrenador.Items.Add(new ItemCombo { Id = e.Id, Texto = $"{e.Nombre} ({e.Especialidad})" });
        cmbEntrenador.SelectedIndex = 0;
    }

    private void cmbPlan_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbPlan.SelectedItem is not ItemCombo item) return;
        var plan = _planService.ObtenerTodos().FirstOrDefault(p => p.Id == item.Id);
        if (plan == null) return;

        txtConcepto.Text = $"Plan {plan.Nombre}";
        txtPrecioUnitario.Text = plan.Precio.ToString("0.##");
        numCantidad.Value = 1;
    }

    private void btnAgregarDetalle_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();

        if (string.IsNullOrWhiteSpace(txtConcepto.Text))
        {
            errorProvider1.SetError(txtConcepto, "Requerido");
            return;
        }
        if (!decimal.TryParse(txtPrecioUnitario.Text, out decimal precio) || precio < 0)
        {
            errorProvider1.SetError(txtPrecioUnitario, "Precio invalido");
            return;
        }

        var item = new DetalleInscripcion
        {
            Concepto = txtConcepto.Text.Trim(),
            Cantidad = (int)numCantidad.Value,
            PrecioUnitario = precio
        };
        _detalleActual.Add(item);
        RefrescarListaDetalle();

        txtConcepto.Clear();
        txtPrecioUnitario.Clear();
        numCantidad.Value = 1;
    }

    private void btnQuitarDetalle_Click(object sender, EventArgs e)
    {
        if (lstDetalle.SelectedIndex < 0) return;
        _detalleActual.RemoveAt(lstDetalle.SelectedIndex);
        RefrescarListaDetalle();
    }

    private void RefrescarListaDetalle()
    {
        lstDetalle.Items.Clear();
        foreach (var d in _detalleActual)
            lstDetalle.Items.Add($"{d.Concepto}  x{d.Cantidad}  =  ${d.Subtotal:N0}");

        lblTotal.Text = $"Total: ${_detalleActual.Sum(d => d.Subtotal):N0}";
    }

    private void btnRegistrar_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();

        if (cmbMiembro.SelectedItem is not ItemCombo miembroSel)
        {
            MessageBox.Show("Seleccione un miembro.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbPlan.SelectedItem is not ItemCombo planSel)
        {
            MessageBox.Show("Seleccione un plan.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var entrenadorSel = cmbEntrenador.SelectedItem as ItemCombo;

        var inscripcion = new Inscripcion
        {
            Id = _idEnEdicion > 0 ? _idEnEdicion : 0,
            MiembroId = miembroSel.Id,
            PlanId = planSel.Id,
            EntrenadorId = entrenadorSel is { Id: > 0 } ? entrenadorSel.Id : null,
            FechaInscripcion = dtpFechaInscripcion.Value.Date,
            Estado = "Vigente",
            Detalle = new List<DetalleInscripcion>(_detalleActual)
        };

        var errores = _service.Validar(inscripcion);
        if (errores.Count > 0)
        {
            MessageBox.Show("Corrija los siguientes datos:\n\n- " + string.Join("\n- ", errores),
                "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Si es un registro nuevo (no una edicion), avisa si el miembro ya tiene un plan
        // vigente, pero deja continuar si el usuario confirma que quiere agregar el plan nuevo.
        if (_idEnEdicion <= 0)
        {
            var vigente = _service.ObtenerVigentePorMiembro(inscripcion.MiembroId);
            if (vigente != null)
            {
                var resp = MessageBox.Show(
                    $"Este miembro ya tiene un plan vigente registrado:\n\n" +
                    $"Plan: {vigente.PlanNombre}\nVence: {vigente.FechaVencimiento:yyyy-MM-dd}\n\n" +
                    "¿Desea registrar igualmente este plan nuevo?",
                    "Plan ya registrado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resp != DialogResult.Yes) return;
            }
        }

        try
        {
            if (_idEnEdicion > 0)
            {
                _service.Actualizar(inscripcion);
                MessageBox.Show($"Inscripcion actualizada correctamente. Total: ${inscripcion.Total:N0}",
                    "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _service.Registrar(inscripcion);
                MessageBox.Show($"Inscripcion registrada correctamente. Total: ${inscripcion.Total:N0}\n" +
                                 "Se agrego a la cola de turnos pendientes por atender en recepcion.",
                    "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ActualizarContadorPendientes();
            }

            LimpiarFormulario();
            CargarGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo guardar la inscripcion.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void dgvInscripciones_DoubleClick(object sender, EventArgs e)
    {
        if (_soloLectura || dgvInscripciones.CurrentRow == null) return;

        int id = Convert.ToInt32(dgvInscripciones.CurrentRow.Cells["Id"].Value);
        var todas = _service.ObtenerTodos();
        var insc = todas.FirstOrDefault(x => x.Id == id);
        if (insc == null) return;

        _idEnEdicion = insc.Id;
        SeleccionarEnCombo(cmbMiembro, insc.MiembroId);
        SeleccionarEnCombo(cmbPlan, insc.PlanId);
        SeleccionarEnCombo(cmbEntrenador, insc.EntrenadorId ?? 0);
        dtpFechaInscripcion.Value = insc.FechaInscripcion;

        _detalleActual.Clear();
        _detalleActual.AddRange(_service.ObtenerDetalle(insc.Id));
        RefrescarListaDetalle();

        btnRegistrar.Text = "Actualizar";
    }

    private static void SeleccionarEnCombo(ComboBox combo, int id)
    {
        for (int i = 0; i < combo.Items.Count; i++)
        {
            if (combo.Items[i] is ItemCombo ic && ic.Id == id)
            {
                combo.SelectedIndex = i;
                return;
            }
        }
    }

    private void btnAtenderPendiente_Click(object sender, EventArgs e)
    {
        var siguiente = _service.AtenderSiguientePendiente();
        ActualizarContadorPendientes();
        if (siguiente == null)
        {
            MessageBox.Show("No hay turnos pendientes en la cola.", "Cola vacia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        MessageBox.Show($"Atendiendo turno:\nMiembro: {siguiente.MiembroNombre}\nPlan: {siguiente.PlanNombre}\nTotal: ${siguiente.Total:N0}",
            "Turno atendido", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ActualizarContadorPendientes() =>
        lblPendientes.Text = $"Turnos pendientes en cola: {_service.ColaPendientes.Count}";

    private void btnEliminar_Click(object sender, EventArgs e)
    {
        if (dgvInscripciones.CurrentRow == null)
        {
            MessageBox.Show("Seleccione una inscripcion para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("¿Desea eliminar esta inscripcion? Podra deshacer esta accion mientras la aplicacion siga abierta.",
            "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        int id = Convert.ToInt32(dgvInscripciones.CurrentRow.Cells["Id"].Value);
        _service.Eliminar(id);
        CargarGrid();
    }

    private void btnDeshacer_Click(object sender, EventArgs e)
    {
        var restaurada = _service.Deshacer();
        if (restaurada == null)
        {
            MessageBox.Show("No hay eliminaciones para deshacer en esta sesion.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        MessageBox.Show($"Se restauro la inscripcion de {restaurada.MiembroNombre}.", "Deshacer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        CargarGrid();
    }

    private void btnLimpiar_Click(object sender, EventArgs e) => LimpiarFormulario();

    private void cmbFiltroEstado_SelectedIndexChanged(object sender, EventArgs e) => CargarGrid();

    private void CargarGrid()
    {
        var lista = _service.ObtenerTodos();
        string filtro = cmbFiltroEstado.SelectedItem?.ToString() ?? "Todos";
        if (filtro != "Todos")
            lista = lista.Where(i => i.Estado == filtro).ToList();

        dgvInscripciones.DataSource = lista.Select(i => new
        {
            i.Id,
            Miembro = i.MiembroNombre,
            Plan = i.PlanNombre,
            Entrenador = i.EntrenadorNombre,
            i.FechaInscripcion,
            i.FechaVencimiento,
            i.Estado,
            i.Total
        }).ToList();
    }

    private void LimpiarFormulario()
    {
        _detalleActual.Clear();
        RefrescarListaDetalle();
        cmbMiembro.SelectedIndex = -1;
        cmbPlan.SelectedIndex = -1;
        cmbEntrenador.SelectedIndex = 0;
        dtpFechaInscripcion.Value = DateTime.Today;
        errorProvider1.Clear();
        _idEnEdicion = -1;
        btnRegistrar.Text = "Registrar";
    }
}
