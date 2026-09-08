namespace EService.Models;

/// <summary>
/// An employee/user of the system. "EsGerente" (Spanish for "is manager")
/// decides whether they land on the Manager Dashboard or the Order Menu
/// after logging in - same role-switch idea as the original Java version.
/// </summary>
public class Empleado
{
    public string Codigo { get; set; }
    public bool EsGerente { get; set; }

    public Empleado(string codigo, bool esGerente)
    {
        Codigo = codigo;
        EsGerente = esGerente;
    }
}
