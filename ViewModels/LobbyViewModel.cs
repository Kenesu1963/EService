using System.Windows.Input;
using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// Replaces lobbyPage.java. A small "home base" screen reachable from the
/// Home button on every other screen, from which you can jump to the
/// Manager Dashboard, the Order Menu, or fully log out back to the Login
/// screen - same three options the original had.
/// </summary>
public class LobbyViewModel : ViewModelBase
{
    public string CurrentUserCode => LoginService.Instance.CurrentUserCode ?? "";

    public ICommand GoToManagerCommand { get; }
    public ICommand GoToServerCommand { get; }
    public ICommand LogoutCommand { get; }

    public LobbyViewModel()
    {
        GoToManagerCommand = new RelayCommand(() => NavigationService.NavigateTo(new ManagerDashboardViewModel()));
        GoToServerCommand = new RelayCommand(() => NavigationService.NavigateTo(new OrderMenuViewModel()));
        LogoutCommand = new RelayCommand(() =>
        {
            LoginService.Instance.CurrentUserCode = null;
            NavigationService.NavigateTo(new LoginViewModel());
        });
    }
}
