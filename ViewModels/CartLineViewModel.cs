namespace EService.ViewModels;

/// <summary>
/// One row in the checkout summary: a dish, how many of it, and the
/// subtotal. The original Java version listed every single unit as its
/// own line (e.g. "Coca cola" appearing 3 separate times); grouping equal
/// items with a quantity is the one small, natural improvement made here -
/// the underlying total and inventory deduction logic is unchanged.
/// </summary>
public class CartLineViewModel
{
    public string Nombre { get; }
    public int Cantidad { get; }
    public int PrecioUnitario { get; }
    public int Subtotal => Cantidad * PrecioUnitario;

    public CartLineViewModel(string nombre, int cantidad, int precioUnitario)
    {
        Nombre = nombre;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }
}
