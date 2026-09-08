namespace EService.Models;

/// <summary>
/// Represents a customer/table. Kept from the original project for future
/// use (e.g. attaching an order to a table with N guests) - it isn't wired
/// into the current screens yet, same as in the original Java code.
/// </summary>
public class Cliente
{
    public string Nombre { get; set; } = "";
    public int CantidadIntegrantes { get; set; }
}
