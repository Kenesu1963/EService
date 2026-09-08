using EService.ViewModels;

namespace EService.Services;

/// <summary>
/// Switches which screen is currently shown in the single MainWindow.
///
/// In the original Java project, moving to a new screen meant literally
/// opening a new JFrame: e.g. "new Order_menu().setVisible(true);". Here,
/// there is only ever one window (MainWindow); "navigating" just means
/// telling it which ViewModel - and therefore which View - to display right
/// now. NavigateTo(new OrderMenuViewModel()) is the direct equivalent of
/// that old "new Order_menu().setVisible(true)" line.
/// </summary>
public static class NavigationService
{
    public static MainWindowViewModel? Main { get; set; }

    public static void NavigateTo(ViewModelBase viewModel)
    {
        if (Main is null) return;
        Main.CurrentPage = viewModel;
    }
}
