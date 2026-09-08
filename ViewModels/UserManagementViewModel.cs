using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Replaces usuariosManagerPage.java. Lets a manager add, remove, or change
/// the role of an employee. Each action asks for input through the same
/// kind of dialog the Java version used (JOptionPane), just routed through
/// DialogService instead - the underlying add/remove/modify logic in
/// LoginService is identical.
/// </summary>
public class UserManagementViewModel : ViewModelBase
{
    private readonly LoginService _loginService = LoginService.Instance;

    public ObservableCollection<UserRowViewModel> Usuarios { get; } = new();

    public ICommand AgregarUsuarioCommand { get; }
    public ICommand EliminarUsuarioCommand { get; }
    public ICommand ModificarUsuarioCommand { get; }
    public ICommand VolverCommand { get; }
    public ICommand HomeCommand { get; }

    public UserManagementViewModel()
    {
        RecargarUsuarios();

        AgregarUsuarioCommand = new RelayCommand(AgregarUsuario);
        EliminarUsuarioCommand = new RelayCommand(EliminarUsuario);
        ModificarUsuarioCommand = new RelayCommand(ModificarUsuario);
        VolverCommand = new RelayCommand(() => NavigationService.NavigateTo(new ManagerDashboardViewModel()));
        HomeCommand = new RelayCommand(() => NavigationService.NavigateTo(new LobbyViewModel()));
    }

    private void RecargarUsuarios()
    {
        Usuarios.Clear();
        foreach (var e in _loginService.ObtenerTodosLosEmpleados())
            Usuarios.Add(new UserRowViewModel(e));
    }

    private async void AgregarUsuario()
    {
        var codigo = await DialogService.ShowInputAsync("Nuevo Empleado", "Ingrese el código del nuevo empleado:");
        if (string.IsNullOrWhiteSpace(codigo)) return;

        var rol = await DialogService.ShowChoiceAsync(
            "Rol del Empleado", "Seleccione el rol:", new[] { "Mesero", "Gerente" });
        if (rol is null) return;

        try
        {
            _loginService.AgregarEmpleado(codigo, esGerente: rol == "Gerente");
            RecargarUsuarios();
            await DialogService.ShowMessageAsync("Éxito", $"Empleado {codigo.ToUpperInvariant()} agregado.");
        }
        catch (System.Exception e)
        {
            await DialogService.ShowMessageAsync("Error", e.Message);
        }
    }

    private async void EliminarUsuario()
    {
        var codigo = await DialogService.ShowInputAsync("Eliminar Empleado", "Ingrese el código del empleado a eliminar:");
        if (string.IsNullOrWhiteSpace(codigo)) return;

        var confirmar = await DialogService.ShowConfirmAsync("Confirmar", $"¿Eliminar al empleado {codigo}?");
        if (!confirmar) return;

        if (_loginService.EliminarEmpleado(codigo))
        {
            RecargarUsuarios();
            await DialogService.ShowMessageAsync("Éxito", "Empleado eliminado.");
        }
        else
        {
            await DialogService.ShowMessageAsync("Error", $"El código {codigo} no existe.");
        }
    }

    private async void ModificarUsuario()
    {
        var codigo = await DialogService.ShowInputAsync("Modificar Empleado", "Ingrese el código del empleado a modificar:");
        if (string.IsNullOrWhiteSpace(codigo)) return;

        var rol = await DialogService.ShowChoiceAsync(
            "Nuevo Rol", "Seleccione el nuevo rol:", new[] { "Mesero", "Gerente" });
        if (rol is null) return;

        try
        {
            _loginService.ModificarEmpleado(codigo, nuevoEsGerente: rol == "Gerente");
            RecargarUsuarios();
            await DialogService.ShowMessageAsync("Éxito", "Empleado actualizado.");
        }
        catch (System.Exception e)
        {
            await DialogService.ShowMessageAsync("Error", e.Message);
        }
    }
}
