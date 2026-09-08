using System;
using System.Collections.Generic;
using System.IO;

namespace EService.Services;

/// <summary>
/// This is the direct replacement for the original FileManager.java. It
/// keeps doing exactly the same job - reading and writing plain ".txt"
/// files with "|" separated fields, no database involved - because that's
/// what the project is meant to keep doing.
///
/// The one real change: the old version hardcoded Windows paths like
/// "C:\EserviceData\users.txt", which only worked on Windows. This version
/// stores everything under the current user's standard "application data"
/// folder, which .NET resolves correctly on Windows, Linux and macOS:
///   Linux:   ~/.local/share/EService/
///   Windows: C:\Users\you\AppData\Roaming\EService\
/// The file names and the "|"-delimited format inside them are unchanged,
/// so this is a drop-in modernization, not a data-format change.
/// </summary>
public static class FileDataStore
{
    private static readonly string BaseDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EService");

    private static readonly string DataDirectory = Path.Combine(BaseDirectory, "Data");
    private static readonly string OrdersDirectory = Path.Combine(BaseDirectory, "Pedidos");
    private static readonly string InvoicesDirectory = Path.Combine(BaseDirectory, "Facturas");

    private static readonly string UsersFile = Path.Combine(DataDirectory, "users.txt");
    private static readonly string InsumosFile = Path.Combine(DataDirectory, "insumos.txt");
    private static readonly string TransactionsFile = Path.Combine(DataDirectory, "transactions.txt");

    public static void EnsureDirectoriesExist()
    {
        Directory.CreateDirectory(DataDirectory);
        Directory.CreateDirectory(OrdersDirectory);
        Directory.CreateDirectory(InvoicesDirectory);
    }

    // ========== Users ==========
    public static void SaveUsers(List<string[]> users)
    {
        EnsureDirectoriesExist();
        try
        {
            using var writer = new StreamWriter(UsersFile, append: false);
            foreach (var user in users)
                writer.WriteLine($"{user[0]}|{user[1]}");
        }
        catch (IOException e)
        {
            Console.Error.WriteLine($"Error guardando usuarios: {e.Message}");
        }
    }

    public static List<string[]> LoadUsers()
    {
        var users = new List<string[]>();
        if (!File.Exists(UsersFile)) return users;

        try
        {
            foreach (var line in File.ReadLines(UsersFile))
            {
                var parts = line.Split('|');
                if (parts.Length == 2) users.Add(parts);
            }
        }
        catch (IOException e)
        {
            Console.Error.WriteLine($"Error cargando usuarios: {e.Message}");
        }
        return users;
    }

    // ========== Insumos (inventory) ==========
    public static void SaveInsumos(List<string[]> insumos)
    {
        EnsureDirectoriesExist();
        try
        {
            using var writer = new StreamWriter(InsumosFile, append: false);
            foreach (var insumo in insumos)
                writer.WriteLine($"{insumo[0]}|{insumo[1]}|{insumo[2]}|{insumo[3]}");
        }
        catch (IOException e)
        {
            Console.Error.WriteLine($"Error guardando insumos: {e.Message}");
        }
    }

    public static List<string[]> LoadInsumos()
    {
        var insumos = new List<string[]>();
        if (!File.Exists(InsumosFile)) return insumos;

        try
        {
            foreach (var line in File.ReadLines(InsumosFile))
            {
                var parts = line.Split('|');
                if (parts.Length == 4) insumos.Add(parts);
            }
        }
        catch (IOException e)
        {
            Console.Error.WriteLine($"Error cargando insumos: {e.Message}");
        }
        return insumos;
    }

    // ========== Transactions (sales ledger) ==========
    public static void SaveTransactions(List<string[]> transactions)
    {
        EnsureDirectoriesExist();
        try
        {
            using var writer = new StreamWriter(TransactionsFile, append: false);
            foreach (var t in transactions)
                // id|fecha|tipo|descripcion|monto|usuario
                writer.WriteLine($"{t[0]}|{t[1]}|{t[2]}|{t[3]}|{t[4]}|{t[5]}");
        }
        catch (IOException e)
        {
            Console.Error.WriteLine($"Error guardando transacciones: {e.Message}");
        }
    }

    public static List<string[]> LoadTransactions()
    {
        var transactions = new List<string[]>();
        if (!File.Exists(TransactionsFile)) return transactions;

        try
        {
            foreach (var line in File.ReadLines(TransactionsFile))
            {
                var parts = line.Split('|');
                if (parts.Length == 6) transactions.Add(parts);
            }
        }
        catch (IOException e)
        {
            Console.Error.WriteLine($"Error cargando transacciones: {e.Message}");
        }
        return transactions;
    }

    // ========== Order receipts (one .txt file per confirmed order) ==========
    public static string WriteOrderReceipt(string contents)
    {
        EnsureDirectoriesExist();
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var path = Path.Combine(OrdersDirectory, $"pedido_{timestamp}.txt");
        File.WriteAllText(path, contents);
        return path;
    }

    // ========== Purchase invoices (one .txt file per stock purchase) ==========
    public static string WritePurchaseInvoice(string invoiceNumber, string contents)
    {
        EnsureDirectoriesExist();
        var fileName = $"factura_{invoiceNumber}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.txt";
        var path = Path.Combine(InvoicesDirectory, fileName);
        File.WriteAllText(path, contents);
        return path;
    }
}
