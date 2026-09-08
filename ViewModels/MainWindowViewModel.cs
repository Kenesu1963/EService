using EService.Services;

namespace EService.ViewModels;

/// <summary>
/// The "shell" ViewModel for the one and only window in the app.
/// CurrentPage holds whichever screen's ViewModel is active right now
/// (LoginViewModel, OrderMenuViewModel, ManagerDashboardViewModel, etc).
/// MainWindow.axaml just has a single ContentControl bound to CurrentPage,
/// and a ViewLocator (see ViewLocator.cs) figures out which View (XAML) to
/// render for whatever ViewModel is currently set here.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentPage;

    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        set => SetField(ref _currentPage, value);
    }

    public MainWindowViewModel()
    {
        NavigationService.Main = this;
        _currentPage = new LoginViewModel();
    }
}
