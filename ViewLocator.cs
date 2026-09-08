using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using EService.ViewModels;

namespace EService;

/// <summary>
/// This is the piece of "glue" that makes ViewModel-first navigation work.
/// Whenever something on screen needs to display a ViewModel (e.g. the
/// MainWindow's ContentControl bound to CurrentPage), Avalonia asks this
/// class "what control should represent this object?" and this class
/// answers with a simple naming convention:
///
///     EService.ViewModels.OrderMenuViewModel  -->  EService.Views.OrderMenuView
///
/// i.e. take the ViewModel's full name, replace "ViewModels" with "Views"
/// and drop the "Model" suffix. As long as every screen follows that
/// naming pattern (which they all do in this project), you never have to
/// manually register each screen anywhere - adding a new screen is just
/// adding a new "XyzViewModel" + "XyzView" pair.
/// </summary>
public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is null) return new TextBlock { Text = "(null view model)" };

        var name = data.GetType().FullName!
            .Replace("ViewModels", "Views")
            .Replace("ViewModel", "View");

        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "View not found: " + name };
    }

    public bool Match(object? data) => data is ViewModelBase;
}
