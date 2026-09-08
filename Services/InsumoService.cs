using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EService.Models;

namespace EService.Services;

/// <summary>
/// Replaces InsumoController.java (inventory management). The original used
/// a fixed-size array (Insumo[100]) because that's how it was first written;
/// here it's a normal growable List, which is simpler and removes the old
/// "no se pueden agregar más insumos" (100-item ceiling) limitation for free.
/// Behavior is otherwise identical: same default catalog of ~74 raw
/// ingredients, same load/save to insumos.txt, same add/discount logic.
/// </summary>
public class InsumoService
{
    private static InsumoService? _instance;
    public static InsumoService Instance => _instance ??= new InsumoService();

    private readonly List<Insumo> _insumos = new();
    public IReadOnlyList<Insumo> Insumos => _insumos;

    private InsumoService()
    {
        CargarInsumosDesdeArchivo();
        if (_insumos.Count == 0)
            CargarInsumosPorDefecto();
    }

    private void CargarInsumosDesdeArchivo()
    {
        foreach (var data in FileDataStore.LoadInsumos())
        {
            var codigo = data[0];
            var nombre = data[1];
            var cantidad = double.Parse(data[2], CultureInfo.InvariantCulture);
            var unidad = data[3];
            _insumos.Add(new Insumo(nombre, unidad, cantidad, codigo));
        }
    }

    private void CargarInsumosPorDefecto()
    {
        // Same default catalog as InsumoController.java's cargarInsumosPorDefecto().
        (string nombre, string unidad, string codigo)[] catalogo =
        {
            ("Arroz","Lb","I000"), ("Azucar","Gr","I001"), ("Sal","Gr","I002"),
            ("Pimienta","Gr","I003"), ("Platano","Unidad","I004"), ("Tomate","Unidad","I005"),
            ("Cebolla blanca","Unidad","I006"), ("Cebolla roja","Unidad","I007"), ("Cebolla larga","Unidad","I008"),
            ("Cabezas de ajo","Unidad","I009"), ("Aceite de girasol","ml","I010"), ("Aceite de oliva","ml","I011"),
            ("Mojarra","Unidad","I012"), ("Bagre","Unidad","I013"), ("Sabalo","Unidad","I014"),
            ("Corvina","Unidad","I015"), ("Tilapia","Unidad","I016"), ("Camaron","Gr","I017"),
            ("Pimenton rojo","Unidad","I018"), ("Pimenton verde","Unidad","I019"), ("Cubo de Maggi","Unidad","I020"),
            ("Zanahoria","Gr","I021"), ("Arveja","Gr","I022"), ("Yuca","Gr","I023"),
            ("Papa","Gr","I024"), ("Papa criolla","Gr","I025"), ("Maiz","Unidad","I026"),
            ("Panza de vaca/mondongo","Gr","I027"), ("Cilantro","Gr","I028"), ("Perejil","Gr","I029"),
            ("Jamon","Unidad","I030"), ("Papas fritas en bolsa","Gr","I031"), ("Frijoles","Gr","I032"),
            ("Lomo de vaca","Unidad","I033"), ("Pollo","Unidad","I034"), ("Queso mozarella","Gr","I035"),
            ("Masa de maiz","Gr","I036"), ("Paprica","Gr","I037"), ("Harina","Gr","I038"),
            ("Huevo","Unidad","I039"), ("Lima limon","Unidad","I040"), ("Mantequilla","Gr","I041"),
            ("Coco rallado","Gr","I042"), ("Leche de coco","ml","I043"), ("Anis molido","Gr","I044"),
            ("Brazo de reina","Unidad","I045"), ("Leche entera","ml","I046"), ("Leche deslactosada","ml","I047"),
            ("Leche condensada","ml","I048"), ("Canela","Gr","I049"), ("Gelatina de limon","Gr","I050"),
            ("Gelatina de fresa","Gr","I051"), ("Gelatina de piña","Gr","I052"), ("Gelatina de mora","Gr","I053"),
            ("Helado vainilla","ml","I054"), ("Postobon uva","Unidad","I055"), ("Postobon manzana","Unidad","I056"),
            ("Postobon kola","Unidad","I057"), ("Colombiana","Unidad","I058"), ("Coca cola","Unidad","I059"),
            ("Kola roman","Unidad","I060"), ("Cerveza andina","Unidad","I061"), ("Cerveza costeñita","Unidad","I062"),
            ("Cerveza budweiser","Unidad","I063"), ("Cerveza aguila","Unidad","I064"), ("Cerveza club colombia roja","Unidad","I065"),
            ("Cerveza club colombia negra","Unidad","I066"), ("Zumo de limon","ml","I067"), ("Zumo de naranja","ml","I068"),
            ("Pulpa de mora","Unidad","I069"), ("Pulpa de maracuya","Unidad","I070"), ("Pulpa de mango","Unidad","I071"),
            ("Pulpa de fresa","Unidad","I072"), ("Leche evaporada","Gr","I073"),
        };

        foreach (var (nombre, unidad, codigo) in catalogo)
            AgregarInsumo(new Insumo(nombre, unidad, 0.0, codigo));

        GuardarInsumos();
    }

    public void GuardarInsumos()
    {
        var data = _insumos.Select(i => new[]
        {
            i.Codigo, i.Nombre, i.Cantidad.ToString(CultureInfo.InvariantCulture), i.TipoUnidad
        }).ToList();
        FileDataStore.SaveInsumos(data);
    }

    /// <summary>
    /// If the code already exists, adds the quantity to the existing stock
    /// (this is how a new purchase "tops up" inventory). Otherwise inserts
    /// it as a brand-new item.
    /// </summary>
    public void AgregarInsumo(Insumo insumo)
    {
        var existente = BuscarInsumoPorCodigo(insumo.Codigo);
        if (existente != null)
        {
            existente.Cantidad += insumo.Cantidad;
        }
        else
        {
            _insumos.Add(insumo);
        }
        GuardarInsumos();
    }

    public void DescontarInsumo(string codigo, double cantidad)
    {
        var insumo = BuscarInsumoPorCodigo(codigo);
        if (insumo != null)
        {
            insumo.Cantidad -= cantidad;
            GuardarInsumos();
        }
    }

    public Insumo? BuscarInsumoPorCodigo(string codigo)
    {
        var normalizado = codigo.Trim().ToUpperInvariant();
        return _insumos.FirstOrDefault(i => string.Equals(i.Codigo, normalizado, StringComparison.OrdinalIgnoreCase));
    }

    public void EliminarInsumo(Insumo insumo)
    {
        _insumos.Remove(insumo);
        GuardarInsumos();
    }
}
