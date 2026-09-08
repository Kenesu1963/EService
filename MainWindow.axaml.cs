using Avalonia.Controls;
using EService.Services;

namespace EService;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Every dialog (message boxes, input prompts, etc.) needs an
        // "owner" window to center itself on top of - this is that window.
        DialogService.OwnerWindow = this;
    }
}
