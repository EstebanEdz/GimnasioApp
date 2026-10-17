using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

public partial class FormPrincipal : Form
{
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem menuArchivo;
    private System.Windows.Forms.ToolStripMenuItem menuSalir;
    private System.Windows.Forms.ToolStripMenuItem menuModulos;
    private System.Windows.Forms.ToolStripMenuItem menuMiembros;
    private System.Windows.Forms.ToolStripMenuItem menuPlanes;
    private System.Windows.Forms.ToolStripMenuItem menuEntrenadores;
    private System.Windows.Forms.ToolStripMenuItem menuInscripciones;
    private System.Windows.Forms.ToolStripMenuItem menuUsuarios;
    private System.Windows.Forms.ToolStripMenuItem menuUsuariosItem;
    private System.Windows.Forms.ToolStripMenuItem menuPermisosItem;
    private System.Windows.Forms.ToolStripMenuItem menuAuditoriaItem;
    private System.Windows.Forms.ToolStripMenuItem menuMembresias;
    private System.Windows.Forms.ToolStripMenuItem menuPagos;
    private System.Windows.Forms.ToolStripMenuItem menuReporteFinanciero;
    private System.Windows.Forms.ToolStripMenuItem menuCambiarContrasena;
    private System.Windows.Forms.ToolStripButton tsbMembresias;
    private System.Windows.Forms.ToolStripButton tsbPagos;
    private System.Windows.Forms.ToolStripStatusLabel lblAlertas;
    private System.Windows.Forms.ToolStripMenuItem menuReportes;
    private System.Windows.Forms.ToolStripMenuItem menuReporteMiembrosPorPlan;
    private System.Windows.Forms.ToolStripMenuItem menuReporteInscripcionesPorMes;
    private System.Windows.Forms.ToolStripMenuItem menuReporteIngresos;
    private System.Windows.Forms.ToolStripMenuItem menuVentana;
    private System.Windows.Forms.ToolStripMenuItem menuCascada;
    private System.Windows.Forms.ToolStripMenuItem menuMosaicoH;
    private System.Windows.Forms.ToolStripMenuItem menuMosaicoV;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripButton tsbMiembros;
    private System.Windows.Forms.ToolStripButton tsbPlanes;
    private System.Windows.Forms.ToolStripButton tsbEntrenadores;
    private System.Windows.Forms.ToolStripButton tsbInscripciones;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel lblUsuario;
    private System.Windows.Forms.ToolStripStatusLabel lblPerfil;
    private System.Windows.Forms.ToolStripStatusLabel lblHora;
    private System.Windows.Forms.Timer timer1;

    private readonly Usuario _usuario;
    // Cada modulo es de solo lectura si el perfil NO tiene su permiso de edicion (ver tabla Permisos).
    private static bool SoloLectura(string permisoEditar) => !SesionActual.Tiene(permisoEditar);

    public FormPrincipal(Usuario usuario)
    {
        InitializeComponent();
        this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        this.menuArchivo = new System.Windows.Forms.ToolStripMenuItem();
        this.menuSalir = new System.Windows.Forms.ToolStripMenuItem();
        this.menuModulos = new System.Windows.Forms.ToolStripMenuItem();
        this.menuMiembros = new System.Windows.Forms.ToolStripMenuItem();
        this.menuPlanes = new System.Windows.Forms.ToolStripMenuItem();
        this.menuEntrenadores = new System.Windows.Forms.ToolStripMenuItem();
        this.menuInscripciones = new System.Windows.Forms.ToolStripMenuItem();
        this.menuUsuarios = new System.Windows.Forms.ToolStripMenuItem();
        this.menuUsuariosItem = new System.Windows.Forms.ToolStripMenuItem();
        this.menuPermisosItem = new System.Windows.Forms.ToolStripMenuItem();
        this.menuAuditoriaItem = new System.Windows.Forms.ToolStripMenuItem();
        this.menuMembresias = new System.Windows.Forms.ToolStripMenuItem();
        this.menuPagos = new System.Windows.Forms.ToolStripMenuItem();
        this.menuReporteFinanciero = new System.Windows.Forms.ToolStripMenuItem();
        this.menuCambiarContrasena = new System.Windows.Forms.ToolStripMenuItem();
        this.tsbMembresias = new System.Windows.Forms.ToolStripButton();
        this.tsbPagos = new System.Windows.Forms.ToolStripButton();
        this.lblAlertas = new System.Windows.Forms.ToolStripStatusLabel();
        this.menuReportes = new System.Windows.Forms.ToolStripMenuItem();
        this.menuReporteMiembrosPorPlan = new System.Windows.Forms.ToolStripMenuItem();
        this.menuReporteInscripcionesPorMes = new System.Windows.Forms.ToolStripMenuItem();
        this.menuReporteIngresos = new System.Windows.Forms.ToolStripMenuItem();
        this.menuVentana = new System.Windows.Forms.ToolStripMenuItem();
        this.menuCascada = new System.Windows.Forms.ToolStripMenuItem();
        this.menuMosaicoH = new System.Windows.Forms.ToolStripMenuItem();
        this.menuMosaicoV = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStrip1 = new System.Windows.Forms.ToolStrip();
        this.tsbMiembros = new System.Windows.Forms.ToolStripButton();
        this.tsbPlanes = new System.Windows.Forms.ToolStripButton();
        this.tsbEntrenadores = new System.Windows.Forms.ToolStripButton();
        this.tsbInscripciones = new System.Windows.Forms.ToolStripButton();
        this.statusStrip1 = new System.Windows.Forms.StatusStrip();
        this.lblUsuario = new System.Windows.Forms.ToolStripStatusLabel();
        this.lblPerfil = new System.Windows.Forms.ToolStripStatusLabel();
        this.lblHora = new System.Windows.Forms.ToolStripStatusLabel();
        this.timer1 = new System.Windows.Forms.Timer(this.components);
        this.menuStrip1.SuspendLayout();
        this.toolStrip1.SuspendLayout();
        this.statusStrip1.SuspendLayout();
        this.SuspendLayout();

        // ---- Menu Archivo ----
        this.menuSalir.Text = "Salir";
        this.menuSalir.Click += new System.EventHandler(this.menuSalir_Click);
        this.menuCambiarContrasena.Text = "Cambiar mi contraseña...";
        this.menuCambiarContrasena.Click += new System.EventHandler(this.menuCambiarContrasena_Click);
        this.menuArchivo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuCambiarContrasena, this.menuSalir });
        this.menuArchivo.Text = "Archivo";

        // ---- Menu Modulos ----
        this.menuMiembros.Text = "Miembros";
        this.menuMiembros.Click += new System.EventHandler(this.menuMiembros_Click);
        this.menuPlanes.Text = "Planes";
        this.menuPlanes.Click += new System.EventHandler(this.menuPlanes_Click);
        this.menuEntrenadores.Text = "Entrenadores";
        this.menuEntrenadores.Click += new System.EventHandler(this.menuEntrenadores_Click);
        this.menuInscripciones.Text = "Inscripciones";
        this.menuInscripciones.Click += new System.EventHandler(this.menuInscripciones_Click);
        this.menuUsuariosItem.Text = "Usuarios";
        this.menuUsuariosItem.Click += new System.EventHandler(this.menuUsuariosItem_Click);
        this.menuMembresias.Text = "Membresias";
        this.menuMembresias.Click += new System.EventHandler(this.menuMembresias_Click);
        this.menuPagos.Text = "Pagos";
        this.menuPagos.Click += new System.EventHandler(this.menuPagos_Click);
        this.menuPermisosItem.Text = "Permisos por perfil";
        this.menuPermisosItem.Click += new System.EventHandler(this.menuPermisosItem_Click);
        this.menuAuditoriaItem.Text = "Auditoria y errores";
        this.menuAuditoriaItem.Click += new System.EventHandler(this.menuAuditoriaItem_Click);
        this.menuUsuarios.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuUsuariosItem, this.menuPermisosItem, this.menuAuditoriaItem });
        this.menuUsuarios.Text = "Seguridad";
        this.menuModulos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMiembros, this.menuPlanes, this.menuEntrenadores, this.menuInscripciones,
            this.menuMembresias, this.menuPagos });
        this.menuModulos.Text = "Modulos";

        // ---- Menu Reportes ----
        this.menuReporteMiembrosPorPlan.Text = "Miembros por Plan (sencillo)";
        this.menuReporteMiembrosPorPlan.Click += new System.EventHandler(this.menuReporteMiembrosPorPlan_Click);
        this.menuReporteInscripcionesPorMes.Text = "Inscripciones por Mes (grafico)";
        this.menuReporteInscripcionesPorMes.Click += new System.EventHandler(this.menuReporteInscripcionesPorMes_Click);
        this.menuReporteIngresos.Text = "Ingresos por Rango de Fechas (calculado)";
        this.menuReporteIngresos.Click += new System.EventHandler(this.menuReporteIngresos_Click);
        this.menuReporteFinanciero.Text = "Reportes financieros (cobrado, cartera)";
        this.menuReporteFinanciero.Click += new System.EventHandler(this.menuReporteFinanciero_Click);
        this.menuReportes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuReporteMiembrosPorPlan, this.menuReporteInscripcionesPorMes, this.menuReporteIngresos,
            this.menuReporteFinanciero });
        this.menuReportes.Text = "Reportes";

        // ---- Menu Ventana ----
        this.menuCascada.Text = "Cascada";
        this.menuCascada.Click += new System.EventHandler(this.menuCascada_Click);
        this.menuMosaicoH.Text = "Mosaico horizontal";
        this.menuMosaicoH.Click += new System.EventHandler(this.menuMosaicoH_Click);
        this.menuMosaicoV.Text = "Mosaico vertical";
        this.menuMosaicoV.Click += new System.EventHandler(this.menuMosaicoV_Click);
        this.menuVentana.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCascada, this.menuMosaicoH, this.menuMosaicoV });
        this.menuVentana.Text = "Ventana";

        this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuArchivo, this.menuModulos, this.menuUsuarios, this.menuReportes, this.menuVentana });
        this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        this.menuStrip1.Name = "menuStrip1";
        this.menuStrip1.Size = new System.Drawing.Size(1100, 24);

        // ---- ToolStrip (accesos directos) ----
        this.tsbMiembros.Text = "Miembros";
        this.tsbMiembros.Click += new System.EventHandler(this.menuMiembros_Click);
        this.tsbPlanes.Text = "Planes";
        this.tsbPlanes.Click += new System.EventHandler(this.menuPlanes_Click);
        this.tsbEntrenadores.Text = "Entrenadores";
        this.tsbEntrenadores.Click += new System.EventHandler(this.menuEntrenadores_Click);
        this.tsbInscripciones.Text = "Inscripciones";
        this.tsbInscripciones.Click += new System.EventHandler(this.menuInscripciones_Click);
        this.tsbMembresias.Text = "Membresias";
        this.tsbMembresias.Click += new System.EventHandler(this.menuMembresias_Click);
        this.tsbPagos.Text = "Pagos";
        this.tsbPagos.Click += new System.EventHandler(this.menuPagos_Click);
        this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbMiembros, this.tsbPlanes, this.tsbEntrenadores, this.tsbInscripciones,
            this.tsbMembresias, this.tsbPagos });
        this.toolStrip1.Location = new System.Drawing.Point(0, 24);
        this.toolStrip1.Name = "toolStrip1";
        this.toolStrip1.Size = new System.Drawing.Size(1100, 25);

        // ---- StatusStrip ----
        this.lblUsuario.Text = "Usuario: -";
        this.lblPerfil.Text = "Perfil: -";
        this.lblHora.Text = "00:00:00";
        this.lblAlertas.Spring = true;
        this.lblAlertas.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblAlertas.Text = "";
        this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUsuario, this.lblPerfil, this.lblAlertas, this.lblHora });
        this.statusStrip1.Location = new System.Drawing.Point(0, 728);
        this.statusStrip1.Name = "statusStrip1";
        this.statusStrip1.Size = new System.Drawing.Size(1100, 22);

        // ---- Timer (reloj) ----
        this.timer1.Interval = 1000;
        this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

        // ---- Form ----
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1100, 750);
        this.Controls.Add(this.toolStrip1);
        this.Controls.Add(this.statusStrip1);
        this.Controls.Add(this.menuStrip1);
        this.IsMdiContainer = true;
        this.MainMenuStrip = this.menuStrip1;
        this.Name = "FormPrincipal";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Sistema de Gestion de Gimnasio";
        this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        this.menuStrip1.ResumeLayout(false);
        this.menuStrip1.PerformLayout();
        this.toolStrip1.ResumeLayout(false);
        this.toolStrip1.PerformLayout();
        this.statusStrip1.ResumeLayout(false);
        this.statusStrip1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    
        _usuario = usuario;

        lblUsuario.Text = $"Usuario: {_usuario.NombreUsuario}";
        lblPerfil.Text = $"Perfil: {_usuario.PerfilNombre}";
        timer1.Start();

        ConfigurarPermisosPorPerfil();
        MostrarAlertas();
    }

    private void timer1_Tick(object sender, EventArgs e) => lblHora.Text = DateTime.Now.ToString("HH:mm:ss");

    /// <summary>
    /// Habilita o esconde menus segun los PERMISOS del perfil (tabla PerfilPermisos), ya no segun el nombre del perfil.
    /// Asi el administrador puede cambiar lo que ve cada perfil desde Seguridad > Permisos por perfil.
    /// (Ademas, los servicios de la BLL vuelven a exigir cada permiso: ocultar un menu no es la unica barrera.)
    /// </summary>
    private void ConfigurarPermisosPorPerfil()
    {
        bool verMiembros = SesionActual.Tiene(Permisos.MiembrosVer);
        bool verPlanes = SesionActual.Tiene(Permisos.PlanesVer);
        bool verEntrenadores = SesionActual.Tiene(Permisos.EntrenadoresVer);
        bool verInscripciones = SesionActual.Tiene(Permisos.InscripcionesVer);
        bool verMembresias = SesionActual.Tiene(Permisos.MembresiasVer);
        bool verPagos = SesionActual.Tiene(Permisos.PagosVer);
        bool reportesBasicos = SesionActual.Tiene(Permisos.ReportesBasicos);
        bool reportesFinancieros = SesionActual.Tiene(Permisos.ReportesFinancieros);
        bool usuariosAdmin = SesionActual.Tiene(Permisos.UsuariosAdmin);
        bool permisosAdmin = SesionActual.Tiene(Permisos.PermisosAdmin);
        bool auditoria = SesionActual.Tiene(Permisos.AuditoriaVer);

        menuMiembros.Enabled = tsbMiembros.Enabled = verMiembros;
        menuPlanes.Enabled = tsbPlanes.Enabled = verPlanes;
        menuEntrenadores.Enabled = tsbEntrenadores.Enabled = verEntrenadores;
        menuInscripciones.Enabled = tsbInscripciones.Enabled = verInscripciones;
        menuMembresias.Enabled = tsbMembresias.Enabled = verMembresias;
        menuPagos.Enabled = tsbPagos.Enabled = verPagos;

        // El menu "Modulos" completo se deshabilita si no hay ningun modulo disponible.
        menuModulos.Enabled = verMiembros || verPlanes || verEntrenadores || verInscripciones || verMembresias || verPagos;

        menuReporteMiembrosPorPlan.Enabled = menuReporteInscripcionesPorMes.Enabled = menuReporteIngresos.Enabled = reportesBasicos;
        menuReporteFinanciero.Enabled = reportesFinancieros;
        menuReportes.Enabled = reportesBasicos || reportesFinancieros;

        menuUsuariosItem.Visible = usuariosAdmin;
        menuPermisosItem.Visible = permisosAdmin;
        menuAuditoriaItem.Visible = auditoria;
        menuUsuarios.Visible = usuariosAdmin || permisosAdmin || auditoria;
        menuUsuarios.Enabled = menuUsuarios.Visible;

        if (!menuModulos.Enabled && menuReportes.Enabled)
            lblPerfil.Text += "  (solo reportes)";
    }

    /// <summary>Marca las membresias vencidas y muestra en la barra de estado los avisos del dia.</summary>
    private void MostrarAlertas()
    {
        try
        {
            var membresias = new MembresiaService();
            membresias.ActualizarVencidas();

            if (!SesionActual.Tiene(Permisos.MembresiasVer)) return;

            var resumen = membresias.ObtenerResumenAlertas();
            lblAlertas.Text = $"Por vencer ({Configuracion.DiasAvisoVencimiento} dias): {resumen.PorVencer}   |   Vencidas: {resumen.Vencidas}   |   Con saldo pendiente: {resumen.ConSaldo}";
        }
        catch (Exception ex)
        {
            // Los avisos son un extra: si fallan (por ejemplo falta ejecutar mejoras_v9.sql), la app sigue funcionando.
            ManejadorErrores.Registrar(ex, "FormPrincipal.MostrarAlertas");
            lblAlertas.Text = "";
        }
    }

    /// <summary>Abre un formulario hijo dentro del MDI evitando duplicados (no abrir el mismo formulario dos veces).</summary>
    private void AbrirFormularioUnico<TForm>(Func<TForm> crear) where TForm : Form
    {
        foreach (Form hijo in MdiChildren)
        {
            if (hijo is TForm existente)
            {
                existente.Activate();
                if (existente.WindowState == FormWindowState.Minimized)
                    existente.WindowState = FormWindowState.Normal;
                return;
            }
        }

        var nuevo = crear();
        nuevo.MdiParent = this;
        nuevo.Show();
    }

    private void menuMiembros_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormMiembros(SoloLectura(Permisos.MiembrosEditar)));

    private void menuPlanes_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormPlanes(SoloLectura(Permisos.PlanesEditar)));

    private void menuEntrenadores_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormEntrenadores(SoloLectura(Permisos.EntrenadoresEditar)));

    private void menuInscripciones_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormInscripciones(SoloLectura(Permisos.InscripcionesEditar)));

    private void menuUsuariosItem_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormUsuarios());

    private void menuPermisosItem_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormPermisos());

    private void menuAuditoriaItem_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormAuditoria());

    private void menuMembresias_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormMembresias());

    private void menuPagos_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormPagos());

    private void menuReporteFinanciero_Click(object sender, EventArgs e) =>
        AbrirFormularioUnico(() => new FormReportesFinancieros());

    private void menuCambiarContrasena_Click(object sender, EventArgs e)
    {
        using var form = new FormCambiarContrasena();
        form.ShowDialog(this);
    }

    private void menuReporteMiembrosPorPlan_Click(object sender, EventArgs e) => AbrirReportes(0);
    private void menuReporteInscripcionesPorMes_Click(object sender, EventArgs e) => AbrirReportes(1);
    private void menuReporteIngresos_Click(object sender, EventArgs e) => AbrirReportes(2);

    private void AbrirReportes(int pestanaInicial)
    {
        foreach (Form hijo in MdiChildren)
        {
            if (hijo is FormReportes existente)
            {
                existente.SeleccionarPestana(pestanaInicial);
                existente.Activate();
                return;
            }
        }

        var nuevo = new FormReportes(pestanaInicial) { MdiParent = this };
        nuevo.Show();
    }

    private void menuCascada_Click(object sender, EventArgs e) => LayoutMdi(MdiLayout.Cascade);
    private void menuMosaicoH_Click(object sender, EventArgs e) => LayoutMdi(MdiLayout.TileHorizontal);
    private void menuMosaicoV_Click(object sender, EventArgs e) => LayoutMdi(MdiLayout.TileVertical);

    private void menuSalir_Click(object sender, EventArgs e) => Close();
}
