using EService.Models;

namespace EService.ViewModels;

/// <summary>One row in the Manager Dashboard's employee list.</summary>
public class UserRowViewModel
{
    public string Codigo { get; }
    public string TipoUsuario { get; }

    public UserRowViewModel(Empleado empleado)
    {
        Codigo = empleado.Codigo;
        TipoUsuario = empleado.EsGerente ? "Gerente" : "Mesero";
    }
}
