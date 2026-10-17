using App.BLL;

namespace App.UI;

/// <summary>El usuario con sesion abierta cambia su propia contraseña (pide la actual y aplica la politica).</summary>
public partial class FormCambiarContrasena : Form
{
    private System.Windows.Forms.Label lblActual;
    private System.Windows.Forms.TextBox txtActual;
    private System.Windows.Forms.Label lblNueva;
    private System.Windows.Forms.TextBox txtNueva;
    private System.Windows.Forms.Label lblConfirmar;
    private System.Windows.Forms.TextBox txtConfirmar;
    private System.Windows.Forms.Label lblPolitica;
    private System.Windows.Forms.Button btnGuardar;
    private System.Windows.Forms.Button btnCancelar;
    private System.Windows.Forms.ErrorProvider errorProvider1;

    private readonly UsuarioService _service = new();

    public FormCambiarContrasena()
    {
        InitializeComponent();
        this.lblActual = new System.Windows.Forms.Label();
        this.txtActual = new System.Windows.Forms.TextBox();
        this.lblNueva = new System.Windows.Forms.Label();
        this.txtNueva = new System.Windows.Forms.TextBox();
        this.lblConfirmar = new System.Windows.Forms.Label();
        this.txtConfirmar = new System.Windows.Forms.TextBox();
        this.lblPolitica = new System.Windows.Forms.Label();
        this.btnGuardar = new System.Windows.Forms.Button();
        this.btnCancelar = new System.Windows.Forms.Button();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.SuspendLayout();

        this.lblActual.AutoSize = true;
        this.lblActual.Location = new System.Drawing.Point(15, 20);
        this.lblActual.Text = "Contraseña actual";
        this.txtActual.Location = new System.Drawing.Point(140, 17);
        this.txtActual.Size = new System.Drawing.Size(200, 20);
        this.txtActual.PasswordChar = '*';

        this.lblNueva.AutoSize = true;
        this.lblNueva.Location = new System.Drawing.Point(15, 54);
        this.lblNueva.Text = "Contraseña nueva";
        this.txtNueva.Location = new System.Drawing.Point(140, 51);
        this.txtNueva.Size = new System.Drawing.Size(200, 20);
        this.txtNueva.PasswordChar = '*';

        this.lblConfirmar.AutoSize = true;
        this.lblConfirmar.Location = new System.Drawing.Point(15, 88);
        this.lblConfirmar.Text = "Confirmar nueva";
        this.txtConfirmar.Location = new System.Drawing.Point(140, 85);
        this.txtConfirmar.Size = new System.Drawing.Size(200, 20);
        this.txtConfirmar.PasswordChar = '*';

        this.lblPolitica.AutoSize = true;
        this.lblPolitica.ForeColor = System.Drawing.Color.DimGray;
        this.lblPolitica.Location = new System.Drawing.Point(15, 118);
        this.lblPolitica.Text = "Minimo 8 caracteres, con letras y numeros.";

        this.btnGuardar.Location = new System.Drawing.Point(140, 148);
        this.btnGuardar.Size = new System.Drawing.Size(95, 28);
        this.btnGuardar.Text = "Guardar";
        this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

        this.btnCancelar.Location = new System.Drawing.Point(245, 148);
        this.btnCancelar.Size = new System.Drawing.Size(95, 28);
        this.btnCancelar.Text = "Cancelar";
        this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

        this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(365, 195);
        this.Controls.Add(this.lblActual);
        this.Controls.Add(this.txtActual);
        this.Controls.Add(this.lblNueva);
        this.Controls.Add(this.txtNueva);
        this.Controls.Add(this.lblConfirmar);
        this.Controls.Add(this.txtConfirmar);
        this.Controls.Add(this.lblPolitica);
        this.Controls.Add(this.btnGuardar);
        this.Controls.Add(this.btnCancelar);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "FormCambiarContrasena";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Cambiar mi contraseña";
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void btnCancelar_Click(object? sender, EventArgs e) => Close();

    private void btnGuardar_Click(object? sender, EventArgs e)
    {
        errorProvider1.Clear();

        if (string.IsNullOrEmpty(txtActual.Text))
        {
            errorProvider1.SetError(txtActual, "Ingrese su contraseña actual.");
            return;
        }
        if (txtNueva.Text != txtConfirmar.Text)
        {
            errorProvider1.SetError(txtConfirmar, "La confirmacion no coincide con la contraseña nueva.");
            return;
        }

        try
        {
            var errores = _service.CambiarContrasenaPropia(SesionActual.NombreUsuario, txtActual.Text, txtNueva.Text);
            if (errores.Count > 0)
            {
                Mensajes.Advertencia(this, string.Join("\n", errores));
                return;
            }

            Mensajes.Info(this, "Contraseña actualizada correctamente.");
            Close();
        }
        catch (Exception ex)
        {
            Mensajes.Error(this, ex, "CambiarContrasena");
        }
    }
}
