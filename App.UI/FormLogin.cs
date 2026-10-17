using System.Configuration;
using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

public partial class FormLogin : Form
{
    private System.Windows.Forms.Label lblTitulo;
    private System.Windows.Forms.Label lblUsuario;
    private System.Windows.Forms.Label lblContrasena;
    private System.Windows.Forms.TextBox txtUsuario;
    private System.Windows.Forms.TextBox txtContrasena;
    private System.Windows.Forms.Button btnIngresar;
    private System.Windows.Forms.Button btnSalir;
    private System.Windows.Forms.Label lblIntentos;
    private System.Windows.Forms.Label lblEstadoBd;
    private System.Windows.Forms.LinkLabel lnkOlvidoContrasena;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;

    private readonly UsuarioService _usuarioService = new();
    private int _intentos;
    private readonly int _maxIntentos;

    public Usuario? UsuarioAutenticado { get; private set; }

    public FormLogin()
    {
        InitializeComponent();
        this.lblTitulo = new System.Windows.Forms.Label();
        this.lblUsuario = new System.Windows.Forms.Label();
        this.lblContrasena = new System.Windows.Forms.Label();
        this.txtUsuario = new System.Windows.Forms.TextBox();
        this.txtContrasena = new System.Windows.Forms.TextBox();
        this.btnIngresar = new System.Windows.Forms.Button();
        this.btnSalir = new System.Windows.Forms.Button();
        this.lblIntentos = new System.Windows.Forms.Label();
        this.lblEstadoBd = new System.Windows.Forms.Label();
        this.lnkOlvidoContrasena = new System.Windows.Forms.LinkLabel();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.SuspendLayout();

        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
        this.lblTitulo.Location = new System.Drawing.Point(130, 25);
        this.lblTitulo.ForeColor = Color.FromArgb(0, 40, 85);
        this.lblTitulo.Text = "Iniciar Sesión";
        this.BackColor = Color.FromArgb(245, 247, 250);

        this.lblUsuario.AutoSize = true;
        this.lblUsuario.Location = new System.Drawing.Point(40, 90);
        this.lblUsuario.Text = "Usuario";

        this.txtUsuario.Location = new System.Drawing.Point(140, 87);
        this.txtUsuario.Size = new System.Drawing.Size(220, 20);
        this.toolTip1.SetToolTip(this.txtUsuario, "Nombre de usuario registrado en el sistema");

        this.lblContrasena.AutoSize = true;
        this.lblContrasena.Location = new System.Drawing.Point(40, 130);
        this.lblContrasena.Text = "Contraseña";

        this.txtContrasena.Location = new System.Drawing.Point(140, 127);
        this.txtContrasena.Size = new System.Drawing.Size(220, 20);
        this.txtContrasena.PasswordChar = '*';
        this.txtContrasena.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtContrasena_KeyPress);
        this.toolTip1.SetToolTip(this.txtContrasena, "Presione Enter para ingresar");

        this.btnIngresar.Location = new System.Drawing.Point(80, 170);
        this.btnIngresar.Size = new System.Drawing.Size(100, 32);
        this.btnIngresar.Text = "Ingresar";
        this.btnIngresar.ForeColor = Color.FromArgb(255, 255, 205);
        this.btnIngresar.BackColor = Color.FromArgb(0, 40, 85);
        this.btnIngresar.Click += new System.EventHandler(this.btnIngresar_Click);

        this.btnSalir.Location = new System.Drawing.Point(220, 170);
        this.btnSalir.Size = new System.Drawing.Size(100, 32);
        this.btnSalir.Text = "Salir";
        this.btnSalir.BackColor = Color.FromArgb(255, 255, 255);
        this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);

        this.lblIntentos.AutoSize = true;
        this.lblIntentos.ForeColor = System.Drawing.Color.DimGray;
        this.lblIntentos.Location = new System.Drawing.Point(140, 236);
        this.lblIntentos.Text = "Intentos restantes: 3";

        this.lblEstadoBd.AutoSize = true;
        this.lblEstadoBd.Location = new System.Drawing.Point(40, 262);
        this.lblEstadoBd.MaximumSize = new System.Drawing.Size(340, 0);
        this.lblEstadoBd.Text = "Verificando conexion...";

        this.lnkOlvidoContrasena.AutoSize = true;
        this.lnkOlvidoContrasena.Location = new System.Drawing.Point(140, 210);
        this.lnkOlvidoContrasena.Text = "¿Olvido su contraseña?";
        this.lnkOlvidoContrasena.Click += new System.EventHandler(this.lnkOlvidoContrasena_Click);

        this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(400, 320);
        this.Controls.Add(this.lblTitulo);
        this.Controls.Add(this.lblUsuario);
        this.Controls.Add(this.txtUsuario);
        this.Controls.Add(this.lblContrasena);
        this.Controls.Add(this.txtContrasena);
        this.Controls.Add(this.btnIngresar);
        this.Controls.Add(this.btnSalir);
        this.Controls.Add(this.lnkOlvidoContrasena);
        this.Controls.Add(this.lblIntentos);
        this.Controls.Add(this.lblEstadoBd);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "FormLogin";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Sistema de Gestion de Gimnasio";
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    
        _maxIntentos = int.TryParse(ConfigurationManager.AppSettings["MaxIntentosLogin"], out int m) ? m : 3;
        lblIntentos.Text = $"Intentos restantes: {_maxIntentos}";

        if (!_usuarioService.ProbarConexion(out string? errorConexion))
        {
            lblEstadoBd.Text = "Aviso: no se pudo conectar a SQL Server.\n" + (errorConexion ?? "Verifique App.config y que la BD 'GymDB' este creada (Database/database.sql).");
            lblEstadoBd.ForeColor = Color.Firebrick;
        }
        else
        {
            lblEstadoBd.Text = "Conexion a SQL Server correcta.";
            lblEstadoBd.ForeColor = Color.DarkGreen;
        }
    }

    private void txtContrasena_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            e.Handled = true;
            IntentarLogin();
        }
    }

    private void btnIngresar_Click(object sender, EventArgs e) => IntentarLogin();

    private void lnkOlvidoContrasena_Click(object sender, EventArgs e)
    {
        using var formRecuperar = new FormRecuperarContrasena();
        formRecuperar.ShowDialog(this);
    }

    private void IntentarLogin()
    {
        errorProvider1.Clear();

        if (string.IsNullOrWhiteSpace(txtUsuario.Text))
        {
            errorProvider1.SetError(txtUsuario, "Ingrese el usuario.");
            return;
        }
        if (string.IsNullOrWhiteSpace(txtContrasena.Text))
        {
            errorProvider1.SetError(txtContrasena, "Ingrese la contraseña.");
            return;
        }

        ResultadoAutenticacion resultado;
        try
        {
            resultado = _usuarioService.AutenticarDetallado(txtUsuario.Text.Trim(), txtContrasena.Text);
        }
        catch (Exception ex)
        {
            // Mensaje entendible (sin detalles tecnicos); el detalle queda en el log con un codigo de soporte.
            Mensajes.Error(this, ex, "Login");
            return;
        }

        Usuario? usuario = resultado.Usuario;

        if (usuario == null)
        {
            // Cuenta inactiva o bloqueada: no cuenta como "intento" de esta ventana, solo se informa.
            if (resultado.Resultado is ResultadoLogin.CuentaBloqueada or ResultadoLogin.UsuarioInactivo)
            {
                txtContrasena.Clear();
                MessageBox.Show(resultado.Mensaje, "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            _intentos++;
            int restantes = _maxIntentos - _intentos;
            lblIntentos.Text = $"Intentos restantes: {Math.Max(restantes, 0)}";
            txtContrasena.Clear();
            txtContrasena.Focus();

            if (restantes <= 0)
            {
                MessageBox.Show("Ha superado el numero maximo de intentos. La aplicacion se cerrara.",
                    "Acceso bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
                return;
            }

            MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso denegado",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UsuarioAutenticado = usuario;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnSalir_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }
}
