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

    /// <summary>Used when creating a brand new transaction right now.</summary>
    public Transaction(double amount, string type, string description, string userCode)
    {
        Amount = amount;
        Type = type;
        Description = description;
        UserCode = userCode;
        Timestamp = DateTime.Now;
        _transactionCounter++;
        TransactionId = $"TRX{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{_transactionCounter:0000}";
    }

    /// <summary>Used when re-loading an existing transaction from the .txt file.</summary>
    public Transaction(double amount, string type, string description, string userCode,
                        DateTime timestamp, string transactionId)
    {
        Amount = amount;
        Type = type;
        Description = description;
        UserCode = userCode;
        Timestamp = timestamp;
        TransactionId = transactionId;
    }

    public string FormattedTimestamp => Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
}
