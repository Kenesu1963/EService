using System.Collections.Generic;
using EService.Collections;
using EService.Models;

namespace EService.Services;

/// <summary>
/// Replaces TransactionController.java. Keeps the running ledger of sales
/// ("VENTA") and purchases ("COMPRA"), persisted to transactions.dat, and
/// the running totals (total sales, total purchases, net profit) the
/// Manager Dashboard displays. The original also built a Swing
/// DefaultTableModel here - that's UI-specific, so instead this class just
/// exposes the plain list of transactions, and the Manager Dashboard
/// ViewModel turns that into whatever the UI needs to display.
///
/// El libro de transacciones es una DoublyLinkedList: cada venta o compra
/// nueva se agrega al final (O(1)), así que la lista siempre queda en orden
/// cronológico, y se puede recorrer de la más reciente a la más antigua
/// con Backwards() sin ordenar nada.
/// </summary>
public class TransactionService
{
    private static TransactionService? _instance;
    public static TransactionService Instance => _instance ??= new TransactionService();

    private readonly DoublyLinkedList<Transaction> _transactions = new();

    public double TotalSales { get; private set; }
    public double TotalPurchases { get; private set; }
    public double NetProfit => TotalSales - TotalPurchases;

    // Solo lectura hacia afuera: se puede recorrer, no modificar.
    public IEnumerable<Transaction> Transactions => _transactions;

    private TransactionService()
    {
        CargarTransaccionesDesdeArchivo();
    }

    private void CargarTransaccionesDesdeArchivo()
    {
        foreach (var t in FileDataStore.LoadTransactions())
        {
            _transactions.Add(t);

            if (t.Type == "VENTA") TotalSales += t.Amount;
            else if (t.Type == "COMPRA") TotalPurchases += t.Amount;
        }
    }

    private void GuardarTransacciones() => FileDataStore.SaveTransactions(_transactions);

    public void AddTransaction(double amount, string type, string description, string userCode,
                                string providerName = "", string receiptNumber = "")
    {
        var transaction = new Transaction(amount, type, description, userCode, providerName, receiptNumber);
        _transactions.Add(transaction);

        if (type == "VENTA") TotalSales += amount;
        else if (type == "COMPRA") TotalPurchases += amount;

        GuardarTransacciones();
    }

    public void AddPurchase(double amount, string description, string userCode,
                             string providerName = "", string receiptNumber = "") =>
        AddTransaction(amount, "COMPRA", description, userCode, providerName, receiptNumber);

    public void AddSale(double amount, string description, string userCode) =>
        AddTransaction(amount, "VENTA", description, userCode);
}
