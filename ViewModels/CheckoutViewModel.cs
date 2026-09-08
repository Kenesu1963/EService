using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Replaces TerminarCompra.java. Shows what's in the cart, and on
/// confirmation: deducts every ingredient used from inventory, records a
/// "VENTA" transaction for the manager dashboard, writes a plain-text order
/// receipt to disk (same idea as the original's generarArchivoPedido), then
/// empties the cart - identical sequence of steps to the Java version.
/// </summary>
public class CheckoutViewModel : ViewModelBase
{
    private readonly CartService _cart = CartService.Instance;
    private readonly PlatoService _platoService = new();
    private readonly InsumoService _insumoService = InsumoService.Instance;

    public List<CartLineViewModel> Lineas { get; }
    public int Total { get; }

    public ICommand ConfirmarCommand { get; }
    public ICommand CancelarCommand { get; }

    public CheckoutViewModel()
    {
        var codigos = _cart.GetPlatosSeleccionados();

        Lineas = codigos
            .GroupBy(c => c)
            .Select(g =>
            {
                var plato = _platoService.BuscarPorCodigo(g.Key);
                var nombre = plato?.Nombre ?? g.Key;
                var precio = plato?.Precio ?? 0;
                return new CartLineViewModel(nombre, g.Count(), precio);
            })
            .ToList();

        Total = Lineas.Sum(l => l.Subtotal);

        ConfirmarCommand = new RelayCommand(Confirmar);
        CancelarCommand = new RelayCommand(Cancelar);
    }

    private async void Confirmar()
    {
        try
        {
            DescontarInsumos();

            var currentUser = LoginService.Instance.CurrentUserCode ?? "SYSTEM";
            var codigos = _cart.GetPlatosSeleccionados();

            string saleDescription = codigos.Count <= 5
                ? "VENTA - " + string.Join(", ", codigos.Select(c => _platoService.BuscarPorCodigo(c)?.Nombre ?? c))
                : $"VENTA ({codigos.Count} items)";

            TransactionService.Instance.AddSale(Total, saleDescription, currentUser);

            GenerarArchivoPedido(currentUser);

            _cart.LimpiarCarrito();

            await DialogService.ShowMessageAsync("Éxito", "Pedido confirmado exitosamente!");
            NavigationService.NavigateTo(new OrderMenuViewModel());
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageAsync("Error", $"Error al confirmar pedido: {e.Message}");
        }
    }

    private void Cancelar()
    {
        // Same as the Java version: cancelling checkout does NOT clear the
        // cart, it just goes back to the menu so the server can keep adding
        // or fix the order.
        NavigationService.NavigateTo(new OrderMenuViewModel());
    }

    private void DescontarInsumos()
    {
        foreach (var codigo in _cart.GetPlatosSeleccionados())
        {
            var plato = _platoService.BuscarPorCodigo(codigo);
            if (plato?.InsumosUsados is null) continue;

            for (var i = 0; i < plato.InsumosUsados.Length; i++)
            {
                var codigoInsumo = plato.InsumosUsados[i];
                if (codigoInsumo is null || i >= plato.CantidadesConsumidas.Length) continue;

                _insumoService.DescontarInsumo(codigoInsumo, plato.CantidadesConsumidas[i]);
            }
        }
    }

    private void GenerarArchivoPedido(string currentUser)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== PEDIDO DEL RESTAURANTE ===");
        sb.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy}");
        sb.AppendLine($"Hora: {DateTime.Now:HH:mm:ss}");
        sb.AppendLine($"Usuario: {currentUser}");
        sb.AppendLine();
        sb.AppendLine("PLATOS ORDENADOS:");

        foreach (var linea in Lineas)
            sb.AppendLine($"- {linea.Nombre} x{linea.Cantidad} (${linea.Subtotal:N0})");

        sb.AppendLine();
        sb.AppendLine($"TOTAL: ${Total:N0}");
        sb.AppendLine("==============================");

        FileDataStore.WriteOrderReceipt(sb.ToString());
    }
}
