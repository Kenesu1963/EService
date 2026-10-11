using System;

namespace EService.Models;

/// <summary>
/// A single financial movement: a sale ("VENTA") or a purchase ("COMPRA").
/// The manager dashboard reads a list of these to compute total sales,
/// total purchases and net profit - identical logic to the Java version.
/// </summary>
public class Transaction
{
    private static int _transactionCounter = 0;

    public double Amount { get; }
    public string Type { get; }
    public string Description { get; }
    public DateTime Timestamp { get; }
    public string UserCode { get; }
    public string TransactionId { get; }

    // Información legal del proveedor. Solo aplica a compras ("COMPRA");
    // en una venta simplemente quedan como texto vacío.
    public string ProviderName { get; }
    public string ReceiptNumber { get; }

    /// <summary>Used when creating a brand new transaction right now.</summary>
    public Transaction(double amount, string type, string description, string userCode,
                        string providerName = "", string receiptNumber = "")
    {
        Amount = amount;
        Type = type;
        Description = description;
        UserCode = userCode;
        Timestamp = DateTime.Now;
        ProviderName = providerName;
        ReceiptNumber = receiptNumber;
        _transactionCounter++;
        TransactionId = $"TRX{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{_transactionCounter:0000}";
    }

    /// <summary>Used when re-loading an existing transaction from disk.</summary>
    public Transaction(double amount, string type, string description, string userCode,
                        DateTime timestamp, string transactionId,
                        string providerName = "", string receiptNumber = "")
    {
        Amount = amount;
        Type = type;
        Description = description;
        UserCode = userCode;
        Timestamp = timestamp;
        TransactionId = transactionId;
        ProviderName = providerName;
        ReceiptNumber = receiptNumber;
    }

    public string FormattedTimestamp => Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
}
