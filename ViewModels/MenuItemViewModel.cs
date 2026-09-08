using Avalonia.Media.Imaging;
using EService.Models;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Wraps a single Plato (dish) for display as one "card" in the Order Menu
/// grid - exposing a ready-to-bind price string and icon image, so the
/// XAML stays simple.
/// </summary>
public class MenuItemViewModel
{
    public Plato Plato { get; }

    public string Nombre => Plato.Nombre;
    public string PrecioTexto => $"${Plato.Precio:N0}";
    public Bitmap? Icono { get; }

    public MenuItemViewModel(Plato plato)
    {
        Plato = plato;
        Icono = AssetImageLoader.TryLoad($"Assets/Dishes/{plato.IconFileName}");
    }
}
