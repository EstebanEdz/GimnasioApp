using App.BLL;

namespace App.UI;

public partial class FormRecuperarContrasena : Form
{
    private System.Windows.Forms.Label lblTitulo;
    private System.Windows.Forms.Label lblUsuario;
    private System.Windows.Forms.TextBox txtUsuario;
    private System.Windows.Forms.Button btnContinuar;
    private System.Windows.Forms.Label lblPregunta;
    private System.Windows.Forms.TextBox txtRespuesta;
    private System.Windows.Forms.Button btnVerificar;
    private System.Windows.Forms.Label lblNuevaContrasena;
    private System.Windows.Forms.TextBox txtNuevaContrasena;
    private System.Windows.Forms.Label lblConfirmarContrasena;
    private System.Windows.Forms.TextBox txtConfirmarContrasena;
    private System.Windows.Forms.Button btnCambiar;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;

    private readonly UsuarioService _service = new();
    private string _usuarioValidado = string.Empty;

    public FormRecuperarContrasena()
    {
        InitializeComponent();
        this.lblTitulo = new System.Windows.Forms.Label();
        this.lblUsuario = new System.Windows.Forms.Label();
        this.txtUsuario = new System.Windows.Forms.TextBox();
        this.btnContinuar = new System.Windows.Forms.Button();
        this.lblPregunta = new System.Windows.Forms.Label();
        this.txtRespuesta = new System.Windows.Forms.TextBox();
        this.btnVerificar = new System.Windows.Forms.Button();
        this.lblNuevaContrasena = new System.Windows.Forms.Label();
        this.txtNuevaContrasena = new System.Windows.Forms.TextBox();
        this.lblConfirmarContrasena = new System.Windows.Forms.Label();
        this.txtConfirmarContrasena = new System.Windows.Forms.TextBox();
        this.btnCambiar = new System.Windows.Forms.Button();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.SuspendLayout();

        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
        this.lblTitulo.Location = new System.Drawing.Point(20, 15);
        this.lblTitulo.Text = "Recuperar contraseña";

        // Paso 1
        this.lblUsuario.AutoSize = true;
        this.lblUsuario.Location = new System.Drawing.Point(20, 60);
        this.lblUsuario.Text = "Ingresa tu nombre de usuario:";
        this.txtUsuario.Location = new System.Drawing.Point(20, 82);
        this.txtUsuario.Size = new System.Drawing.Size(300, 20);
        this.toolTip1.SetToolTip(this.txtUsuario, "El mismo usuario que usas para iniciar sesion");

        this.btnContinuar.Location = new System.Drawing.Point(20, 115);
        this.btnContinuar.Size = new System.Drawing.Size(110, 28);
        this.btnContinuar.ForeColor = Color.FromArgb(255, 255, 255);
        this.btnContinuar.BackColor = Color.FromArgb(0, 40, 85);
        this.btnContinuar.Text = "Continuar";
        this.btnContinuar.Click += new System.EventHandler(this.btnContinuar_Click);

        // Paso 2
        this.lblPregunta.AutoSize = false;
        this.lblPregunta.Location = new System.Drawing.Point(20, 60);
        this.lblPregunta.Size = new System.Drawing.Size(300, 40);
        this.lblPregunta.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
        this.lblPregunta.Text = "(pregunta de seguridad)";

        this.txtRespuesta.Location = new System.Drawing.Point(20, 100);
        this.txtRespuesta.Size = new System.Drawing.Size(300, 20);
        this.toolTip1.SetToolTip(this.txtRespuesta, "No distingue mayusculas ni espacios extra");

        this.btnVerificar.Location = new System.Drawing.Point(20, 133);
        this.btnVerificar.Size = new System.Drawing.Size(110, 28);
        this.btnVerificar.Text = "Verificar";
        this.btnVerificar.ForeColor = Color.FromArgb(255, 255, 255);
        this.btnVerificar.BackColor = Color.FromArgb(0, 40, 85);
        this.btnVerificar.Click += new System.EventHandler(this.btnVerificar_Click);

        // Paso 3
        this.lblNuevaContrasena.AutoSize = true;
        this.lblNuevaContrasena.Location = new System.Drawing.Point(20, 60);
        this.lblNuevaContrasena.Text = "Nueva contraseña (min. 8, letras y numeros):";
        this.txtNuevaContrasena.Location = new System.Drawing.Point(20, 82);
        this.txtNuevaContrasena.Size = new System.Drawing.Size(300, 20);
        this.txtNuevaContrasena.PasswordChar = '*';

        this.lblConfirmarContrasena.AutoSize = true;
        this.lblConfirmarContrasena.Location = new System.Drawing.Point(20, 112);
        this.lblConfirmarContrasena.Text = "Confirmar contraseña:";
        this.txtConfirmarContrasena.Location = new System.Drawing.Point(20, 134);
        this.txtConfirmarContrasena.Size = new System.Drawing.Size(300, 20);
        this.txtConfirmarContrasena.PasswordChar = '*';

        this.btnCambiar.Location = new System.Drawing.Point(20, 167);
        this.btnCambiar.Size = new System.Drawing.Size(150, 28);
        this.btnCambiar.Text = "Cambiar contraseña";
        this.btnCambiar.ForeColor = Color.FromArgb(255, 255, 255);
        this.btnCambiar.BackColor = Color.FromArgb(0, 40, 85);
        this.btnCambiar.Click += new System.EventHandler(this.btnCambiar_Click);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(344, 220);
        this.Controls.Add(this.lblTitulo);
        this.Controls.Add(this.lblUsuario);
        this.Controls.Add(this.txtUsuario);
        this.Controls.Add(this.btnContinuar);
        this.Controls.Add(this.lblPregunta);
        this.Controls.Add(this.txtRespuesta);
        this.Controls.Add(this.btnVerificar);
        this.Controls.Add(this.lblNuevaContrasena);
        this.Controls.Add(this.txtNuevaContrasena);
        this.Controls.Add(this.lblConfirmarContrasena);
        this.Controls.Add(this.txtConfirmarContrasena);
        this.Controls.Add(this.btnCambiar);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "FormRecuperarContrasena";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Recuperar contraseña";
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    
        MostrarPaso(1);
    }

    private void MostrarPaso(int paso)
    {
        // Paso 1: pedir el usuario. Paso 2: responder la pregunta de seguridad.
        // Paso 3: definir la nueva contraseña.
        lblUsuario.Visible = txtUsuario.Visible = btnContinuar.Visible = paso == 1;
        lblPregunta.Visible = txtRespuesta.Visible = btnVerificar.Visible = paso == 2;
        lblNuevaContrasena.Visible = txtNuevaContrasena.Visible = paso == 3;
        lblConfirmarContrasena.Visible = txtConfirmarContrasena.Visible = paso == 3;
        btnCambiar.Visible = paso == 3;
    }

    private void btnContinuar_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();
        string usuario = txtUsuario.Text.Trim();

        if (string.IsNullOrWhiteSpace(usuario))
        {
            errorProvider1.SetError(txtUsuario, "Ingrese su nombre de usuario.");
            return;
        }

        var pregunta = _service.ObtenerPreguntaSeguridad(usuario);
        if (pregunta == null)
        {
            MessageBox.Show("No se encontro un usuario con pregunta de seguridad configurada.\n\n" +
                "Si el problema persiste, pide a un Administrador que restablezca tu contraseña " +
                "desde el modulo de Usuarios.", "Usuario no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _usuarioValidado = usuario;
        lblPregunta.Text = pregunta;
        txtRespuesta.Clear();
        MostrarPaso(2);
    }

    private void btnVerificar_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();

        if (string.IsNullOrWhiteSpace(txtRespuesta.Text))
        {
            errorProvider1.SetError(txtRespuesta, "Ingrese su respuesta.");
            return;
        }

        if (!_service.VerificarRespuestaSeguridad(_usuarioValidado, txtRespuesta.Text))
        {
            MessageBox.Show("La respuesta no es correcta.", "Verificacion fallida",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtNuevaContrasena.Clear();
        txtConfirmarContrasena.Clear();
        MostrarPaso(3);
    }

    private void btnCambiar_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();

        if (txtNuevaContrasena.Text != txtConfirmarContrasena.Text)
        {
            errorProvider1.SetError(txtConfirmarContrasena, "Las contraseñas no coinciden.");
            MessageBox.Show("Las contraseñas no coinciden.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var errores = _service.CambiarContrasena(_usuarioValidado, txtNuevaContrasena.Text);
        if (errores.Count > 0)
        {
            MessageBox.Show(string.Join("\n", errores), "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show("Contraseña actualizada correctamente. Ya puedes iniciar sesion con la nueva contraseña.",
            "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}
