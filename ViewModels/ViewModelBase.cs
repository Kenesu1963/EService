using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EService.ViewModels;

/// <summary>
/// Every ViewModel inherits from this. It gives them the ability to notify
/// the UI when a property changes (e.g. "Total" changed, please redraw the
/// label showing it). This is the C#/XAML equivalent of manually calling
/// label.setText(...) in the old Swing code - except here you just set the
/// property, and the UI updates itself automatically because of the binding.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Call this from a property's setter to tell the UI "this value changed,
    /// please refresh anything bound to it". [CallerMemberName] means we
    /// usually don't have to type the property name ourselves.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Helper used inside property setters: sets the backing field, raises
    /// PropertyChanged only if the value actually changed, and returns
    /// whether it changed (handy for chaining extra logic).
    ///
    /// Example usage inside a ViewModel:
    ///
    ///     private string _name = "";
    ///     public string Name
    ///     {
    ///         get => _name;
    ///         set => SetField(ref _name, value);
    ///     }
    /// </summary>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
