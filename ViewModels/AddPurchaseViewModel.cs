using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using EService.Collections;
using EService.Models;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Replaces manageragregarcompra.java. A manager picks an ingredient,
/// enters how much was bought, and builds up a list of purchase lines with
/// no price attached yet. Confirming the whole purchase: adds every line's
/// quantity to inventory (topping up existing stock), records a single
/// "COMPRA" transaction for the total cost entered at the end, and writes
/// an invoice .txt file with the provider/receipt information.
/// </summary>
public class AddPurchaseViewModel : ViewModelBase
{
    private readonly InsumoService _insumoService = InsumoService.Instance;

    public DoublyLinkedList<Insumo> InsumosDisponibles { get; }

    private Insumo? _insumoSeleccionado;
    public Insumo? InsumoSeleccionado
    {
        get => _insumoSeleccionado;
        set => SetField(ref _insumoSeleccionado, value);
    }

    private string _cantidadTexto = "";
    public string CantidadTexto
    {
        get => _cantidadTexto;
        set => SetField(ref _cantidadTexto, value);
    }

    // Información legal de la compra completa (no de una línea individual),
    // se pide una sola vez, al final, antes de confirmar.
    private string _nombreProveedor = "";
    public string NombreProveedor
    {
        get => _nombreProveedor;
        set => SetField(ref _nombreProveedor, value);
    }

    private string _numeroRecibo = "";
    public string NumeroRecibo
    {
        get => _numeroRecibo;
        set => SetField(ref _numeroRecibo, value);
    }

    // El costo se pide UNA sola vez, al final, como el total de toda la compra.
    private string _costoTotalTexto = "";
    public string CostoTotalTexto
    {
        get => _costoTotalTexto;
        set => SetField(ref _costoTotalTexto, value);
    }

    // La fecha es automática: el usuario no la escribe.
    public string FechaActualTexto => DateTime.Now.ToString("dd/MM/yyyy");

    // Lista de líneas pendientes. Para que la pantalla se refresque, cada
    // cambio crea una copia nueva con el cambio aplicado y la reemplaza
    // (SetField avisa a Avalonia de que ItemsSource ahora es otra lista).
    private DoublyLinkedList<PurchaseLineViewModel> _lineas = new();
    public DoublyLinkedList<PurchaseLineViewModel> Lineas
    {
        get => _lineas;
        private set => SetField(ref _lineas, value);
    }

    public ICommand AgregarLineaCommand { get; }
    public ICommand ConfirmarCommand { get; }
    public ICommand CancelarCommand { get; }

    public AddPurchaseViewModel()
    {
        InsumosDisponibles = _insumoService.Insumos.OrderBy(i => i.Nombre).ToDoublyLinkedList();

        AgregarLineaCommand = new RelayCommand(AgregarLinea);
        ConfirmarCommand = new RelayCommand(Confirmar);
        CancelarCommand = new RelayCommand(() => NavigationService.NavigateTo(new ManagerDashboardViewModel()));
    }

    private async void AgregarLinea()
    {
        if (InsumoSeleccionado is null)
        {
            await DialogService.ShowMessageAsync("Error", "Seleccione un insumo.");
            return;
        }

        if (!double.TryParse(CantidadTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var cantidad) || cantidad <= 0)
        {
            await DialogService.ShowMessageAsync("Error", "Ingrese una cantidad válida (mayor a 0).");
            return;
        }

        var nuevas = Lineas.Copy();
        nuevas.Add(new PurchaseLineViewModel(
            InsumoSeleccionado.Codigo, InsumoSeleccionado.Nombre, cantidad, InsumoSeleccionado.TipoUnidad));
        Lineas = nuevas;

        CantidadTexto = "";
    }

    private async void Confirmar()
    {
        if (Lineas.Count == 0)
        {
            await DialogService.ShowMessageAsync("Compra Vacía", "Agregue al menos un insumo antes de confirmar.");
            return;
        }

        if (string.IsNullOrWhiteSpace(NombreProveedor))
        {
            await DialogService.ShowMessageAsync("Error", "Ingrese el nombre del proveedor.");
            return;
        }

        if (string.IsNullOrWhiteSpace(NumeroRecibo))
        {
            await DialogService.ShowMessageAsync("Error", "Ingrese el número de recibo o factura del proveedor.");
            return;
        }

        if (!double.TryParse(CostoTotalTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var costoTotal) || costoTotal <= 0)
        {
            await DialogService.ShowMessageAsync("Error", "Ingrese el costo total de la compra (mayor a 0).");
            return;
        }

        var confirmar = await DialogService.ShowConfirmAsync(
            "Confirmar Compra",
            $"¿Confirmar compra de {Lineas.Count} insumo(s) por un total de ${costoTotal:N0}?\n" +
            $"Proveedor: {NombreProveedor}\nRecibo N°: {NumeroRecibo}");

        if (!confirmar) return;

        foreach (var linea in Lineas)
            _insumoService.AgregarInsumo(new Insumo(linea.Nombre, linea.Unidad, linea.Cantidad, linea.Codigo));

        var currentUser = LoginService.Instance.CurrentUserCode ?? "SYSTEM";
        var descripcion = "COMPRA - " + string.Join(", ", Lineas.Select(l => l.Nombre));
        TransactionService.Instance.AddPurchase(costoTotal, descripcion, currentUser, NombreProveedor, NumeroRecibo);

        var invoiceNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
        GenerarFactura(invoiceNumber, currentUser, costoTotal);

        await DialogService.ShowMessageAsync("Éxito", "Compra registrada exitosamente.");
        NavigationService.NavigateTo(new ManagerDashboardViewModel());
    }

    private void GenerarFactura(string invoiceNumber, string currentUser, double costoTotal)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== FACTURA DE COMPRA ===");
        sb.AppendLine($"Número de factura interno: {invoiceNumber}");
        sb.AppendLine($"Proveedor: {NombreProveedor}");
        sb.AppendLine($"Número de recibo del proveedor: {NumeroRecibo}");
        sb.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        sb.AppendLine($"Registrado por: {currentUser}");
        sb.AppendLine();
        sb.AppendLine("INSUMOS COMPRADOS:");

        foreach (var linea in Lineas)
            sb.AppendLine($"- {linea.Nombre}: {linea.CantidadTexto}");

        sb.AppendLine();
        sb.AppendLine($"COSTO TOTAL: ${costoTotal:N0}");
        sb.AppendLine("==========================");

        FileDataStore.WritePurchaseInvoice(invoiceNumber, sb.ToString());
    }
}
