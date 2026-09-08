using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EService.Models;

namespace EService.Services;

/// <summary>
/// Replaces TransactionController.java. Keeps the running ledger of sales
/// ("VENTA") and purchases ("COMPRA"), persisted to transactions.txt, and
/// the running totals (total sales, total purchases, net profit) the
/// Manager Dashboard displays. The original also built a Swing
/// DefaultTableModel here - that's UI-specific, so instead this class just
/// exposes the plain list of transactions, and the Manager Dashboard
/// ViewModel turns that into whatever the UI needs to display.
/// </summary>
public class TransactionService
{
    private static TransactionService? _instance;
    public static TransactionService Instance => _instance ??= new TransactionService();

    private readonly List<Transaction> _transactions = new();

    public double TotalSales { get; private set; }
    public double TotalPurchases { get; private set; }
    public double NetProfit => TotalSales - TotalPurchases;

    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    private TransactionService()
    {
        CargarTransaccionesDesdeArchivo();
    }

    private void CargarTransaccionesDesdeArchivo()
    {
        foreach (var data in FileDataStore.LoadTransactions())
        {
            try
            {
                var id = data[0];
                var fechaTexto = data[1];
                var tipo = data[2];
                var descripcion = data[3];
                var monto = double.Parse(data[4], CultureInfo.InvariantCulture);
                var usuario = data[5];

                var fecha = DateTime.ParseExact(fechaTexto, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var t = new Transaction(monto, tipo, descripcion, usuario, fecha, id);
                _transactions.Add(t);

                if (tipo == "VENTA") TotalSales += monto;
                else if (tipo == "COMPRA") TotalPurchases += monto;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error cargando transacción: {e.Message}");
            }
        }
    }

    private void GuardarTransacciones()
    {
        var data = _transactions.Select(t => new[]
        {
            t.TransactionId, t.FormattedTimestamp, t.Type, t.Description,
            t.Amount.ToString(CultureInfo.InvariantCulture), t.UserCode
        }).ToList();
        FileDataStore.SaveTransactions(data);
    }

    public void AddTransaction(double amount, string type, string description, string userCode)
    {
        var transaction = new Transaction(amount, type, description, userCode);
        _transactions.Add(transaction);

        if (type == "VENTA") TotalSales += amount;
        else if (type == "COMPRA") TotalPurchases += amount;

        GuardarTransacciones();
    }

    public void AddPurchase(double amount, string description, string userCode) =>
        AddTransaction(amount, "COMPRA", description, userCode);

    public void AddSale(double amount, string description, string userCode) =>
        AddTransaction(amount, "VENTA", description, userCode);
}
