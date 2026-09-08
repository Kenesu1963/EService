using System.Threading.Tasks;
using Avalonia.Controls;
using EService.Views.Dialogs;

namespace EService.Services;

/// <summary>
/// A thin wrapper so ViewModels can show a popup dialog with one simple
/// call (e.g. "await DialogService.ShowMessageAsync(...)") without needing
/// to know anything about Avalonia Windows themselves. This plays the same
/// role that static JOptionPane.showXxx(...) calls played in the original
/// Java code, just routed through proper Avalonia dialog windows.
///
/// "OwnerWindow" is set once, in MainWindow's code-behind, right after the
/// window is created - every dialog then centers itself on top of it.
/// </summary>
public static class DialogService
{
    public static Window? OwnerWindow { get; set; }

    public static Task ShowMessageAsync(string title, string message) =>
        MessageDialogWindow.ShowAsync(OwnerWindow!, title, message);

    public static Task<bool> ShowConfirmAsync(string title, string message) =>
        ConfirmDialogWindow.ShowAsync(OwnerWindow!, title, message);

    public static Task<string?> ShowInputAsync(string title, string message) =>
        InputDialogWindow.ShowAsync(OwnerWindow!, title, message);

    public static Task<string?> ShowChoiceAsync(string title, string message, string[] options, int selectedIndex = 0) =>
        ChoiceDialogWindow.ShowAsync(OwnerWindow!, title, message, options, selectedIndex);
}
