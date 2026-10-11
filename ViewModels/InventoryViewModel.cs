using System.Linq;
using System.Windows.Input;
using EService.Collections;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Replaces Inventario.java: a read-only, sortable-by-name view of current
/// stock levels for every raw ingredient.
/// </summary>
public class InventoryViewModel : ViewModelBase
{
    public DoublyLinkedList<InsumoRowViewModel> Insumos { get; }

    public ICommand HomeCommand { get; }
    public ICommand VolverCommand { get; }

    public InventoryViewModel()
    {
        Insumos = InsumoService.Instance.Insumos
            .OrderBy(i => i.Nombre)
            .Select(i => new InsumoRowViewModel(i))
            .ToDoublyLinkedList();

        HomeCommand = new RelayCommand(() => NavigationService.NavigateTo(new LobbyViewModel()));
        VolverCommand = new RelayCommand(() => NavigationService.NavigateTo(new ManagerDashboardViewModel()));
    }
}
