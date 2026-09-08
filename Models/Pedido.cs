namespace EService.Models;

/// <summary>
/// Represents a placed order's basic receipt info. Kept from the original
/// project for future use, same as in the Java version.
/// </summary>
public class Pedido
{
    public string NumeroDePedido { get; set; } = "";
    public string Fecha { get; set; } = "";
    public string Hora { get; set; } = "";
    public double Valor { get; set; }
}
