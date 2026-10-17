using App.BLL;
using App.BLL.Entidades;

namespace App.UI;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration_EnableVisualStyles();

        // Red de seguridad: cualquier error que ningun formulario haya atrapado termina aqui,
        // se registra con un codigo de soporte y el usuario ve un mensaje claro en vez de que la app se cierre.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) => Mensajes.Error(null, e.Exception, "Error no controlado (interfaz)");
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            if (e.ExceptionObject is Exception ex) ManejadorErrores.Registrar(ex, "Error no controlado (dominio)");
        };

        using var login = new FormLogin();
        if (login.ShowDialog() == DialogResult.OK && login.UsuarioAutenticado != null)
        {
            Application.Run(new FormPrincipal(login.UsuarioAutenticado));
            new AuditoriaService().Registrar("LOGOUT", "Usuario", login.UsuarioAutenticado.Id, "Cierre de la aplicacion");
            SesionActual.Cerrar();
        }
    }

    private static void ApplicationConfiguration_EnableVisualStyles()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
    }
}
