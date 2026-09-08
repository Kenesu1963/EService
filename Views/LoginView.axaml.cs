using Avalonia.Controls;
using Avalonia.Input;
using EService.ViewModels;

namespace EService.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();

        // Same convenience the Java version had: pressing Enter in the
        // code field submits the login, instead of forcing a mouse click.
        CodigoBox.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter && DataContext is LoginViewModel vm)
                vm.LoginCommand.Execute(null);
        };
    }
}
