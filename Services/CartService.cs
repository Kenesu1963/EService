using System.Collections.Generic;

namespace EService.Services;

/// <summary>
/// Replaces Carrito.java (Spanish for "cart"). Holds the list of dish codes
/// the server has added for the current order, exactly like before -
/// a simple singleton list of codes, cleared once the order is confirmed.
/// </summary>
public class CartService
{
    private static CartService? _instance;
    public static CartService Instance => _instance ??= new CartService();

    private readonly List<string> _platosSeleccionados = new();

    private CartService() { }

    public void AgregarPlato(string codigoPlato) => _platosSeleccionados.Add(codigoPlato);

    public void LimpiarCarrito() => _platosSeleccionados.Clear();

    public List<string> GetPlatosSeleccionados() => new(_platosSeleccionados);

    public int CantidadItems => _platosSeleccionados.Count;
}
