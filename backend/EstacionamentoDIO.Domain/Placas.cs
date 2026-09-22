using System.Text.RegularExpressions;

namespace EstacionamentoDIO.Domain;

/// <summary>
/// Aceita o formato antigo (ABC1234) e o padrão Mercosul (ABC1D23).
/// </summary>
public static partial class Placas
{
    public static string Normalizar(string placa) => placa.Trim().ToUpperInvariant();

    public static bool EhValida(string? placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
        {
            return false;
        }

        return FormatoRegex().IsMatch(Normalizar(placa));
    }

    [GeneratedRegex("^[A-Z]{3}\\d[A-Z0-9]\\d{2}$")]
    private static partial Regex FormatoRegex();
}
