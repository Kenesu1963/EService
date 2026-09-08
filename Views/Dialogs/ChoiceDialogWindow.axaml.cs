using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EService.Views.Dialogs;

/// <summary>
/// A "pick one of these options" popup - equivalent of Java's
/// JOptionPane.showOptionDialog(...) used e.g. when choosing "Empleado
/// Normal" vs "Gerente". Returns the chosen option's text, or null if
/// cancelled.
/// </summary>
public partial class ChoiceDialogWindow : Window
{
    public ChoiceDialogWindow()
    {
        InitializeComponent();
    }

    public ChoiceDialogWindow(string title, string message, string[] options, int selectedIndex = 0) : this()
    {
        TitleText.Text = title;
        MessageText.Text = message;
        Title = title;
        OptionsList.ItemsSource = options;
        OptionsList.SelectedIndex = selectedIndex;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e) => Close(OptionsList.SelectedItem as string);
    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close(null);

    public static Task<string?> ShowAsync(Window owner, string title, string message, string[] options, int selectedIndex = 0)
    {
        var dialog = new ChoiceDialogWindow(title, message, options, selectedIndex);
        return dialog.ShowDialog<string?>(owner);
    }
}
