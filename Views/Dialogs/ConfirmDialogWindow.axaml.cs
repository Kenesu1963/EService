using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EService.Views.Dialogs;

/// <summary>
/// A Yes/No popup - equivalent of Java's
/// JOptionPane.showConfirmDialog(..., YES_NO_OPTION).
/// Returns true if the user picked "Confirmar", false otherwise.
/// </summary>
public partial class ConfirmDialogWindow : Window
{
    public ConfirmDialogWindow()
    {
        InitializeComponent();
    }

    public ConfirmDialogWindow(string title, string message) : this()
    {
        TitleText.Text = title;
        MessageText.Text = message;
        Title = title;
    }

    private void OnYesClick(object? sender, RoutedEventArgs e) => Close(true);
    private void OnNoClick(object? sender, RoutedEventArgs e) => Close(false);

    public static Task<bool> ShowAsync(Window owner, string title, string message)
    {
        var dialog = new ConfirmDialogWindow(title, message);
        return dialog.ShowDialog<bool>(owner);
    }
}
