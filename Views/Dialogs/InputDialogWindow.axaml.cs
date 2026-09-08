using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace EService.Views.Dialogs;

/// <summary>
/// A text-entry popup - equivalent of Java's
/// JOptionPane.showInputDialog(...). Returns the typed text, or null if the
/// user cancelled (mirrors the Java version's "codigo == null" cancel check).
/// </summary>
public partial class InputDialogWindow : Window
{
    public InputDialogWindow()
    {
        InitializeComponent();
        InputBox.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter) OnOkClick(this, new RoutedEventArgs());
        };
    }

    public InputDialogWindow(string title, string message) : this()
    {
        TitleText.Text = title;
        MessageText.Text = message;
        Title = title;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e) => Close(InputBox.Text);
    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close(null);

    public static Task<string?> ShowAsync(Window owner, string title, string message)
    {
        var dialog = new InputDialogWindow(title, message);
        return dialog.ShowDialog<string?>(owner);
    }
}
