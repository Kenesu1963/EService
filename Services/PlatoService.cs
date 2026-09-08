using System.Collections.Generic;
using System.Linq;
using EService.Models;

namespace EService.Services;

/// <summary>
/// Replaces PlatoController.java: the menu catalog (dishes, starters,
/// desserts, drinks) plus each item's recipe (which Insumos it consumes,
/// and how much). This is static reference data - unlike inventory or
/// employees, it isn't loaded from a .txt file, exactly like the original.
/// Every recipe below is copied 1:1 from the Java version.
/// </summary>
public class PlatoService
{
    private readonly List<Plato> _platos = new();
    public IReadOnlyList<Plato> Platos => _platos;

    public PlatoService()
    {
        void Add(string nombre, string tipo, int precio, string codigo, string[] insumos, double[] cantidades) =>
            _platos.Add(new Plato(nombre, tipo, precio, codigo, insumos, cantidades));

        // ---------- Platos fuertes ----------
        Add("Mojarra frita", "Plato fuerte", 25000, "P000",
            new[] { "I000", "I002", "I012", "I007", "I009", "I008", "I004", "I005", "I040", "I010" },
            new[] { 1, 170, 28, 0.10, 1, 0.25, 0.50, 0.40, 0.50, 20.0 });

        Add("Bagre frito", "Plato fuerte", 25000, "P001",
            new[] { "I000", "I002", "I013", "I007", "I009", "I008", "I004", "I005", "I040", "I010" },
            new[] { 1, 170, 28, 0.10, 1, 0.25, 0.50, 0.40, 0.50, 20.0 });

        Add("Sabalo frito", "Plato fuerte", 25000, "P002",
            new[] { "I000", "I002", "I014", "I007", "I009", "I008", "I004", "I005", "I040", "I010" },
            new[] { 1, 170, 28, 0.10, 1, 0.25, 0.50, 0.40, 0.50, 20.0 });

        Add("Corvina", "Plato fuerte", 25000, "P003",
            new[] { "I000", "I002", "I015", "I007", "I009", "I008", "I004", "I005", "I040", "I010" },
            new[] { 1, 170, 28, 0.10, 1, 0.25, 0.50, 0.40, 0.50, 20.0 });

        Add("Arroz de camaron", "Plato fuerte", 20000, "P004",
            new[] { "I000", "I002", "I006", "I009", "I003", "I017", "I018", "I019" },
            new[] { 170, 20, 0.10, 1, 15, 100, 2, 1.0 });

        Add("Sancocho de pescado", "Plato fuerte", 22000, "P005",
            new[] { "I012", "I026", "I020", "I023", "I004", "I024", "I025", "I008", "I028", "I002" },
            new[] { 2, 2, 1, 250, 1, 250, 250, 2, 10, 30.0 });

        Add("Sancocho de mondongo", "Plato fuerte", 22000, "P006",
            new[] { "I027", "I028", "I029", "I021", "I023", "I024", "I009", "I020", "I008", "I003", "I026", "I002" },
            new[] { 1.814, 15, 10, 160, 907, 1.360, 6, 2, 2, 20, 2, 30 });

        Add("Ejecutivo de carne", "Plato fuerte", 14000, "P007",
            new[] { "I033", "I000", "I002", "I008", "I009", "I005", "I006" },
            new[] { 1, 170, 20, 0.10, 1, 0.50, 0.40 });

        Add("Ejecutivo de pollo", "Plato fuerte", 14000, "P008",
            new[] { "I034", "I000", "I002", "I008", "I009", "I005", "I006" },
            new[] { 1, 170, 20, 0.10, 1, 0.50, 0.40 });

        Add("Ejecutivo de pescado", "Plato fuerte", 14000, "P009",
            new[] { "I016", "I000", "I002", "I008", "I009", "I040", "I005", "I006" },
            new[] { 1, 170, 20, 0.10, 1, 0.25, 0.50, 0.40 });

        // ---------- Entradas ----------
        Add("6 Patacones con jamon", "Entrada", 9000, "E000",
            new[] { "I004", "I030", "I002", "I010" }, new[] { 1, 3, 8, 25.0 });

        Add("Papas fritas", "Entrada", 8000, "E001",
            new[] { "I031", "I002", "I010" }, new[] { 350, 8, 25.0 });

        Add("5 Empanadas", "Entrada", 10000, "E002",
            new[] { "I036", "I030", "I035" }, new[] { 250, 3, 200.0 });

        Add("Sopa de frijoles", "Entrada", 9000, "E003",
            new[] { "I002", "I032", "I005", "I006" }, new[] { 30, 120, 0.5, 0.5 });

        Add("10 Anillos de cebolla", "Entrada", 10000, "E004",
            new[] { "I006", "I002", "I037", "I038", "I039" }, new[] { 1, 8, 8, 20, 1.0 });

        // ---------- Postres ----------
        Add("Enyucados", "Postre", 16000, "D000",
            new[] { "I023", "I035", "I001", "I041", "I042", "I043", "I044" },
            new[] { 320, 100, 113, 25, 80, 236, 25.0 });

        Add("Porcion de brazo de reina", "Postre", 10000, "D001",
            new[] { "I045" }, new[] { 0.5 });

        Add("Gelatina", "Postre", 9000, "D002",
            new[] { "I050", "I051", "I052", "I053" }, new[] { 110, 110, 110, 110.0 });

        Add("Dulce de coco", "Postre", 14000, "D003",
            new[] { "I042", "I001", "I046", "I048", "I049" }, new[] { 250, 113, 180, 40, 30.0 });

        Add("Cocadas", "Postre", 14000, "D004",
            new[] { "I001", "I042", "I039", "I073" }, new[] { 150, 450, 6, 945.0 });

        Add("Bola de helado de vainilla", "Postre", 10000, "D005",
            new[] { "I005" }, new[] { 120.0 });

        // ---------- Bebidas ----------
        Add("Agua", "Bebida", 2000, "B000", System.Array.Empty<string>(), System.Array.Empty<double>());
        Add("Postobon uva", "Bebida", 4000, "B001", new[] { "I055" }, new[] { 1.0 });
        Add("Postobon manzana", "Bebida", 4000, "B002", new[] { "I056" }, new[] { 1.0 });
        Add("Postobon kola", "Bebida", 4000, "B003", new[] { "I058" }, new[] { 1.0 });
        Add("Colombiana", "Bebida", 4000, "B004", new[] { "I057" }, new[] { 1.0 });
        Add("Coca cola", "Bebida", 4000, "B005", new[] { "I059" }, new[] { 1.0 });
        Add("Kola roman", "Bebida", 4000, "B006", new[] { "I060" }, new[] { 1.0 });
        Add("Limonada", "Bebida", 4000, "B007", new[] { "I001", "I067" }, new[] { 100, 60.0 });
        Add("Jugo de mora", "Bebida", 5000, "B008", new[] { "I001", "I069" }, new[] { 1, 60.0 });
        Add("Jugo de maracuya", "Bebida", 5000, "B009", new[] { "I001", "I070" }, new[] { 1, 60.0 });
        Add("Jugo de naranja", "Bebida", 5000, "B010", new[] { "I001", "I068" }, new[] { 120, 30.0 });
        Add("Jugo de mango", "Bebida", 5000, "B011", new[] { "I001", "I071" }, new[] { 1, 60.0 });
        Add("Jugo de fresa", "Bebida", 5000, "B012", new[] { "I001", "I072" }, new[] { 1, 60.0 });
        Add("Cerveza andina", "Bebida", 6000, "B013", new[] { "I061" }, new[] { 1.0 });
        Add("Cerveza costeñita", "Bebida", 4000, "B014", new[] { "I062" }, new[] { 1.0 });
        Add("Cerveza budweiser", "Bebida", 6000, "B015", new[] { "I063" }, new[] { 1.0 });
        Add("Cerveza aguila", "Bebida", 6000, "B016", new[] { "I064" }, new[] { 1.0 });
        Add("Cerveza club colombia roja", "Bebida", 6000, "B017", new[] { "I065" }, new[] { 1.0 });
        Add("Cerveza club colombia negra", "Bebida", 6000, "B018", new[] { "I066" }, new[] { 1.0 });
    }

    public Plato? BuscarPorCodigo(string codigo) =>
        _platos.FirstOrDefault(p => p.Codigo == codigo);

    /// <summary>All distinct categories, in menu order, for grouping the UI.</summary>
    public IEnumerable<string> Categorias => new[] { "Plato fuerte", "Entrada", "Postre", "Bebida" };

    public IEnumerable<Plato> PorCategoria(string categoria) =>
        _platos.Where(p => p.Tipo == categoria);
}
