using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Replaces Order_menu.java. Instead of 40 individually hand-placed Swing
/// buttons (one per dish, each wired up the same way), this ViewModel just
/// exposes the menu catalog as four bindable lists - one per category - and
/// the View lays them out as a grid of dish cards. Clicking a card runs the
/// exact same flow the Java version had: ask quantity, confirm, add to
/// cart, update the badge count.
/// </summary>
public class OrderMenuViewModel : ViewModelBase
{
    private readonly PlatoService _platoService = new();
    private readonly CartService _cart = CartService.Instance;

    public List<MenuItemViewModel> PlatosFuertes { get; }
    public List<MenuItemViewModel> Entradas { get; }
    public List<MenuItemViewModel> Postres { get; }
    public List<MenuItemViewModel> Bebidas { get; }

    private int _cantidadEnCarrito;
    public int CantidadEnCarrito
    {
        get => _cantidadEnCarrito;
        set => SetField(ref _cantidadEnCarrito, value);
    }

    public ICommand AgregarAlCarritoCommand { get; }
    public ICommand TerminarPedidoCommand { get; }
    public ICommand HomeCommand { get; }

    public OrderMenuViewModel()
    {
        PlatosFuertes = _platoService.PorCategoria("Plato fuerte").Select(p => new MenuItemViewModel(p)).ToList();
        Entradas = _platoService.PorCategoria("Entrada").Select(p => new MenuItemViewModel(p)).ToList();
        Postres = _platoService.PorCategoria("Postre").Select(p => new MenuItemViewModel(p)).ToList();
        Bebidas = _platoService.PorCategoria("Bebida").Select(p => new MenuItemViewModel(p)).ToList();

        CantidadEnCarrito = _cart.CantidadItems;

        AgregarAlCarritoCommand = new RelayCommand<MenuItemViewModel>(AgregarAlCarrito);
        TerminarPedidoCommand = new RelayCommand(TerminarPedido);
        HomeCommand = new RelayCommand(() => NavigationService.NavigateTo(new LobbyViewModel()));
    }

    private async void AgregarAlCarrito(MenuItemViewModel? item)
    {
        if (item is null) return;
        var plato = item.Plato;

        var cantidadStr = await DialogService.ShowInputAsync(
            "Agregar al carrito",
            $"Producto: {plato.Nombre}\nCódigo: {plato.Codigo}\nPrecio unitario: ${plato.Precio:N0}\n\nIngrese la cantidad que desea:");

        // Cancelled
        if (cantidadStr is null) return;

        var cantidad = 1;
        if (!string.IsNullOrWhiteSpace(cantidadStr) && int.TryParse(cantidadStr.Trim(), out var parsed) && parsed > 0)
            cantidad = parsed;

        var confirmar = await DialogService.ShowConfirmAsync(
            "Confirmar",
            $"¿Agregar {cantidad} unidad(es) de {plato.Nombre} (Total: ${plato.Precio * cantidad:N0}) al carrito?");

        if (!confirmar) return;

        for (var i = 0; i < cantidad; i++)
            _cart.AgregarPlato(plato.Codigo);

        CantidadEnCarrito = _cart.CantidadItems;

        await DialogService.ShowMessageAsync("Éxito", $"Se agregaron {cantidad} unidad(es) de {plato.Nombre}");
    }

    private async void TerminarPedido()
    {
        if (_cart.CantidadItems == 0)
        {
            await DialogService.ShowMessageAsync("Carrito Vacío", "El carrito está vacío");
            return;
        }

        NavigationService.NavigateTo(new CheckoutViewModel());
    }
}
