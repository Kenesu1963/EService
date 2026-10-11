namespace EService.ViewModels;

/// <summary>
/// One pending line in a stock purchase, before it's confirmed. Notice
/// there's no price here - the cost is entered ONCE, for the whole
/// purchase, right before confirming (see CostoTotalTexto in
/// AddPurchaseViewModel.cs).
/// </summary>
public class PurchaseLineViewModel
{
    public string Codigo { get; }
    public string Nombre { get; }
    public double Cantidad { get; }
    public string Unidad { get; }

    public string CantidadTexto => $"{Cantidad:N2} {Unidad}";

    public PurchaseLineViewModel(string codigo, string nombre, double cantidad, string unidad)
    {
        Codigo = codigo;
        Nombre = nombre;
        Cantidad = cantidad;
        Unidad = unidad;
    }
}
