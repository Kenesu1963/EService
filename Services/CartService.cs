using EService.Collections;

namespace EService.Services;

/// <summary>
/// Replaces Carrito.java (Spanish for "cart"). Holds the list of dish codes
/// the server has added for the current order, exactly like before -
/// a simple singleton list of codes, cleared once the order is confirmed.
/// Ahora es una DoublyLinkedList en vez de un List.
/// </summary>
public class CartService
{
    private static CartService? _instance;
    public static CartService Instance => _instance ??= new CartService();

    private readonly DoublyLinkedList<string> _platosSeleccionados = new();

    private CartService() { }

    public void AgregarPlato(string codigoPlato) => _platosSeleccionados.Add(codigoPlato);

    public void LimpiarCarrito() => _platosSeleccionados.Clear();

    /// <summary>Devuelve una copia, para que quien la use no altere el carrito real.</summary>
    public DoublyLinkedList<string> GetPlatosSeleccionados() => _platosSeleccionados.Copy();

    public int CantidadItems => _platosSeleccionados.Count;
}
