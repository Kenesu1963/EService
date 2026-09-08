namespace EService.ViewModels;

/// <summary>One pending line in a stock purchase, before it's confirmed.</summary>
public class PurchaseLineViewModel
{
    public string Codigo { get; }
    public string Nombre { get; }
    public double Cantidad { get; }
    public string Unidad { get; }
    public double Costo { get; }

    public string CantidadTexto => $"{Cantidad:N2} {Unidad}";
    public string CostoTexto => $"${Costo:N0}";

    public PurchaseLineViewModel(string codigo, string nombre, double cantidad, string unidad, double costo)
    {
        Codigo = codigo;
        Nombre = nombre;
        Cantidad = cantidad;
        Unidad = unidad;
        Costo = costo;
    }
}
