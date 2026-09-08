using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Replaces manager01page.java: the manager's overview screen showing
/// total sales/purchases/profit, the full transaction ledger, the employee
/// list, and quick links to every other manager-only screen.
/// </summary>
public class ManagerDashboardViewModel : ViewModelBase
{
    private readonly TransactionService _transactionService = TransactionService.Instance;
    private readonly LoginService _loginService = LoginService.Instance;

    public string TotalVentasTexto => $"${_transactionService.TotalSales:N0}";
    public string TotalComprasTexto => $"${_transactionService.TotalPurchases:N0}";
    public string GananciaNetaTexto => $"${_transactionService.NetProfit:N0}";

    public List<TransactionRowViewModel> Transacciones { get; }
    public List<UserRowViewModel> Usuarios { get; }

    public ICommand IrAInventarioCommand { get; }
    public ICommand IrAAgregarCompraCommand { get; }
    public ICommand HacerPedidoCommand { get; }
    public ICommand IrAUsuariosCommand { get; }
    public ICommand HomeCommand { get; }

    public ManagerDashboardViewModel()
    {
        // Most recent transactions first, same information the original
        // JTable showed, just ordered so today's activity is on top.
        Transacciones = _transactionService.Transactions
            .OrderByDescending(t => t.Timestamp)
            .Select(t => new TransactionRowViewModel(t))
            .ToList();

        Usuarios = _loginService.ObtenerTodosLosEmpleados()
            .Select(e => new UserRowViewModel(e))
            .ToList();

        IrAInventarioCommand = new RelayCommand(() => NavigationService.NavigateTo(new InventoryViewModel()));
        IrAAgregarCompraCommand = new RelayCommand(() => NavigationService.NavigateTo(new AddPurchaseViewModel()));
        HacerPedidoCommand = new RelayCommand(() => NavigationService.NavigateTo(new OrderMenuViewModel()));
        IrAUsuariosCommand = new RelayCommand(() => NavigationService.NavigateTo(new UserManagementViewModel()));
        HomeCommand = new RelayCommand(() => NavigationService.NavigateTo(new LobbyViewModel()));
    }
}
