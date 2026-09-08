namespace EService.Models;

/// <summary>
/// A menu item (dish, appetizer, dessert or drink). "InsumosUsados" and
/// "CantidadesConsumidas" are parallel arrays describing the recipe: which
/// raw ingredients (by Insumo code) this dish consumes, and how much of
/// each, so that confirming an order can deduct the right amounts from
/// inventory automatically - exactly like the original Java logic.
/// </summary>
public class Plato
{
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public int Precio { get; set; }
    public string Codigo { get; set; }
    public string[] InsumosUsados { get; set; }
    public double[] CantidadesConsumidas { get; set; }

    public Plato()
    {
        Nombre = "";
        Tipo = "";
        Precio = 0;
        Codigo = "";
        InsumosUsados = new string[20];
        CantidadesConsumidas = new double[20];
    }

    public Plato(string nombre, string tipo, int precio, string codigo,
                 string[] insumosUsados, double[] cantidadesConsumidas)
    {
        Nombre = nombre;
        Tipo = tipo;
        Precio = precio;
        Codigo = codigo;
        InsumosUsados = (string[])insumosUsados.Clone();
        CantidadesConsumidas = (double[])cantidadesConsumidas.Clone();
    }

    /// <summary>
    /// Maps a dish code like "B013" to its icon file name "b13.png".
    /// The original icons are named with the category letter lowercase
    /// plus a 2-digit number, while dish codes use 3 digits - this just
    /// bridges the two naming conventions.
    /// </summary>
    public string IconFileName
    {
        get
        {
            var letter = Codigo.Length > 0 ? char.ToLowerInvariant(Codigo[0]) : 'x';
            var number = Codigo.Length > 1 ? Codigo[1..] : "00";
            var twoDigit = int.TryParse(number, out var n) ? n.ToString("00") : "00";
            return $"{letter}{twoDigit}.png";
        }
    }
}
