using Avalonia.Media;
using EService.Models;

namespace EService.ViewModels;

/// <summary>
/// One row in the Manager Dashboard's transaction table. Exposes a
/// ready-made background color so sales show green and purchases show red,
/// exactly like the original Java version's custom TransactionCellRenderer.
/// </summary>
public class TransactionRowViewModel
{
    public string Id { get; }
    public string FechaHora { get; }
    public string Tipo { get; }
    public string Descripcion { get; }
    public string MontoTexto { get; }
    public string Usuario { get; }
    public IBrush RowBackground { get; }

    public TransactionRowViewModel(Transaction t)
    {
        Id = t.TransactionId;
        FechaHora = t.FormattedTimestamp;
        Tipo = t.Type;
        Descripcion = t.Description;
        MontoTexto = $"${t.Amount:N2}";
        Usuario = t.UserCode;

        RowBackground = t.Type switch
        {
            "COMPRA" => new SolidColorBrush(Color.Parse("#FFE0E0")),
            "VENTA" => new SolidColorBrush(Color.Parse("#E1F5E1")),
            _ => Brushes.Transparent
        };
    }
}
