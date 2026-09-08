using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EService.Views.Dialogs;

/// <summary>
/// A simple "OK" popup - the equivalent of
/// JOptionPane.showMessageDialog(...) in the original Java code.
/// Call MessageDialogWindow.ShowAsync(owner, title, message) to use it.
/// </summary>
public partial class MessageDialogWindow : Window
{
    public MessageDialogWindow()
    {
        InitializeComponent();
    }

    public MessageDialogWindow(string title, string message) : this()
    {
        TitleText.Text = title;
        MessageText.Text = message;
        Title = title;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e) => Close();

    public static System.Threading.Tasks.Task ShowAsync(Window owner, string title, string message)
    {
        var dialog = new MessageDialogWindow(title, message);
        return dialog.ShowDialog(owner);
    }
}
