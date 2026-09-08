using EService.Models;

namespace EService.ViewModels;

/// <summary>One row in the Inventory table.</summary>
public class InsumoRowViewModel
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string CantidadTexto { get; }
    public string Unidad { get; }

    public InsumoRowViewModel(Insumo insumo)
    {
        Codigo = insumo.Codigo;
        Nombre = insumo.Nombre;
        CantidadTexto = insumo.Cantidad.ToString("N2");
        Unidad = insumo.TipoUnidad;
    }
}
