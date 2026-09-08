using System.Windows.Input;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Replaces Beginningpage.java. A single text field for the employee code;
/// pressing Enter or clicking the button validates it against LoginService
/// and routes to the Manager Dashboard or the Order Menu depending on role -
/// exactly the same branching the Java version did in
/// botondeingresoActionPerformed(...).
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private string _codigo = "";
    public string Codigo
    {
        get => _codigo;
        set => SetField(ref _codigo, value);
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(Login);
    }

    private async void Login()
    {
        var codigo = Codigo.Trim();
        var empleado = LoginService.Instance.ValidarCodigo(codigo);

        if (empleado is null)
        {
            await DialogService.ShowMessageAsync("Error", "Código inválido");
            return;
        }

        LoginService.Instance.CurrentUserCode = codigo;
        LoginService.Instance.CurrentUserIsManager = empleado.EsGerente;

        if (empleado.EsGerente)
            NavigationService.NavigateTo(new ManagerDashboardViewModel());
        else
            NavigationService.NavigateTo(new OrderMenuViewModel());
    }
}
