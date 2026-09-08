namespace EService.Models;

/// <summary>
/// A raw ingredient/supply item in inventory (e.g. "Arroz", 0.0 Lb).
/// </summary>
public class Insumo
{
    public string Nombre { get; set; }
    public string TipoUnidad { get; set; }
    public double Cantidad { get; set; }
    public string Codigo { get; set; }

    public Insumo()
    {
        Nombre = "";
        TipoUnidad = "";
        Cantidad = 0;
        Codigo = "";
    }

    public Insumo(string nombre, string tipoUnidad, double cantidad, string codigo)
    {
        Nombre = nombre;
        TipoUnidad = tipoUnidad;
        Cantidad = cantidad;
        Codigo = codigo;
    }
}
