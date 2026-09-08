using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using EService.Models;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Replaces manageragregarcompra.java. A manager picks an ingredient,
/// enters how much was bought and for how much, and builds up a list of
/// purchase lines. Confirming the whole purchase: adds every line's
/// quantity to inventory (topping up existing stock, same as
/// InsumoService/InsumoController's AgregarInsumo behavior), records a
/// single "COMPRA" transaction for the combined cost, and writes an
/// invoice .txt file - same three steps the Java version performed.
/// </summary>
public class AddPurchaseViewModel : ViewModelBase
{
    private readonly InsumoService _insumoService = InsumoService.Instance;

    public System.Collections.Generic.List<Insumo> InsumosDisponibles { get; }

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

    private string _costoTexto = "";
    public string CostoTexto
    {
        get => _costoTexto;
        set => SetField(ref _costoTexto, value);
    }

    public ObservableCollection<PurchaseLineViewModel> Lineas { get; } = new();

    public double CostoTotal => Lineas.Sum(l => l.Costo);
    public string CostoTotalTexto => $"${CostoTotal:N0}";

    public ICommand AgregarLineaCommand { get; }
    public ICommand ConfirmarCommand { get; }
    public ICommand CancelarCommand { get; }

    public AddPurchaseViewModel()
    {
        InsumosDisponibles = _insumoService.Insumos.OrderBy(i => i.Nombre).ToList();

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

        if (!double.TryParse(CostoTexto.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var costo) || costo < 0)
        {
            await DialogService.ShowMessageAsync("Error", "Ingrese un costo válido.");
            return;
        }

        Lineas.Add(new PurchaseLineViewModel(
            InsumoSeleccionado.Codigo, InsumoSeleccionado.Nombre, cantidad, InsumoSeleccionado.TipoUnidad, costo));

        OnPropertyChanged(nameof(CostoTotal));
        OnPropertyChanged(nameof(CostoTotalTexto));

        CantidadTexto = "";
        CostoTexto = "";
    }

    private async void Confirmar()
    {
        if (Lineas.Count == 0)
        {
            await DialogService.ShowMessageAsync("Compra Vacía", "Agregue al menos un insumo antes de confirmar.");
            return;
        }

        var confirmar = await DialogService.ShowConfirmAsync(
            "Confirmar Compra",
            $"¿Confirmar compra de {Lineas.Count} insumo(s) por un total de {CostoTotalTexto}?");

        if (!confirmar) return;

        foreach (var linea in Lineas)
            _insumoService.AgregarInsumo(new Insumo(linea.Nombre, linea.Unidad, linea.Cantidad, linea.Codigo));

        var currentUser = LoginService.Instance.CurrentUserCode ?? "SYSTEM";
        var descripcion = "COMPRA - " + string.Join(", ", Lineas.Select(l => l.Nombre));
        TransactionService.Instance.AddPurchase(CostoTotal, descripcion, currentUser);

        var invoiceNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
        GenerarFactura(invoiceNumber, currentUser);

        await DialogService.ShowMessageAsync("Éxito", "Compra registrada exitosamente.");
        NavigationService.NavigateTo(new ManagerDashboardViewModel());
    }

    private void GenerarFactura(string invoiceNumber, string currentUser)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== FACTURA DE COMPRA ===");
        sb.AppendLine($"Número de factura: {invoiceNumber}");
        sb.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        sb.AppendLine($"Registrado por: {currentUser}");
        sb.AppendLine();
        sb.AppendLine("INSUMOS COMPRADOS:");

        foreach (var linea in Lineas)
            sb.AppendLine($"- {linea.Nombre}: {linea.CantidadTexto} ({linea.CostoTexto})");

        sb.AppendLine();
        sb.AppendLine($"COSTO TOTAL: {CostoTotalTexto}");
        sb.AppendLine("==========================");

        FileDataStore.WritePurchaseInvoice(invoiceNumber, sb.ToString());
    }
}
