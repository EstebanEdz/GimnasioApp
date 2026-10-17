using App.BLL;

namespace App.UI;

/// <summary>
/// Cuadros de dialogo comunes. Error() traduce la excepcion a un mensaje entendible y, si fue un
/// fallo inesperado, lo registra con un codigo de soporte (ver App.BLL.ManejadorErrores).
/// </summary>
internal static class Mensajes
{
    public static void Info(IWin32Window? duenio, string texto, string titulo = "Informacion") =>
        MessageBox.Show(duenio, texto, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static void Advertencia(IWin32Window? duenio, string texto, string titulo = "Validacion") =>
        MessageBox.Show(duenio, texto, titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static bool Confirmar(IWin32Window? duenio, string texto, string titulo = "Confirmacion") =>
        MessageBox.Show(duenio, texto, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    public static void Error(IWin32Window? duenio, Exception ex, string contexto)
    {
        string texto = ManejadorErrores.MensajeParaUsuario(ex, contexto);
        bool esperado = ErrorTranslator.EsEsperado(ex);
        MessageBox.Show(duenio, texto, esperado ? "Aviso" : "Error",
            MessageBoxButtons.OK, esperado ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
    }

    /// <summary>Pide un texto corto (por ejemplo el motivo de una anulacion). Devuelve null si el usuario cancela.</summary>
    public static string? PedirTexto(IWin32Window? duenio, string titulo, string indicacion)
    {
        using var form = new Form
        {
            Text = titulo,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MinimizeBox = false,
            MaximizeBox = false,
            ClientSize = new Size(420, 150)
        };
        var lbl = new Label { Text = indicacion, Left = 12, Top = 12, Width = 396, Height = 36 };
        var txt = new TextBox { Left = 12, Top = 52, Width = 396, MaxLength = 200 };
        var ok = new Button { Text = "Aceptar", Left = 232, Top = 100, Width = 84, DialogResult = DialogResult.OK };
        var cancelar = new Button { Text = "Cancelar", Left = 324, Top = 100, Width = 84, DialogResult = DialogResult.Cancel };
        form.Controls.Add(lbl);
        form.Controls.Add(txt);
        form.Controls.Add(ok);
        form.Controls.Add(cancelar);
        form.AcceptButton = ok;
        form.CancelButton = cancelar;

        var resultado = duenio == null ? form.ShowDialog() : form.ShowDialog(duenio);
        return resultado == DialogResult.OK ? txt.Text.Trim() : null;
    }
}
