using System.Configuration;

namespace App.BLL;

/// <summary>Lectura tipada de valores de &lt;appSettings&gt; en App.config, con valor por defecto.</summary>
public static class Configuracion
{
    public static int Entero(string clave, int valorPorDefecto) =>
        int.TryParse(ConfigurationManager.AppSettings[clave], out int valor) && valor > 0 ? valor : valorPorDefecto;

    /// <summary>Intentos fallidos acumulados (de un usuario, entre sesiones) que bloquean la cuenta.</summary>
    public static int IntentosParaBloqueo => Entero("IntentosParaBloqueo", 5);

    /// <summary>Minutos que dura el bloqueo de cuenta.</summary>
    public static int MinutosBloqueo => Entero("MinutosBloqueo", 5);

    /// <summary>Dias de anticipacion para avisar que una membresia esta por vencer.</summary>
    public static int DiasAvisoVencimiento => Entero("DiasAvisoVencimiento", 7);
}
