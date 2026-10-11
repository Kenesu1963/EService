using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using EService.Collections;
using EService.Models;

namespace EService.Services;

/// <summary>
/// Reemplaza a FileManager.java. Guarda y carga los datos del sistema
/// (empleados, inventario y transacciones) en archivos BINARIOS ".dat",
/// sin base de datos.
///
/// ---------------------------------------------------------------------
/// Ubicación de los archivos (la carpeta de datos del usuario):
///   Linux:   ~/.config/EService/Data/
///   Windows: C:\Users\usuario\AppData\Roaming\EService\Data\
///     users.dat  insumos.dat  transactions.dat
///
/// ---------------------------------------------------------------------
/// Formato de cada .dat (se escribe/lee con BinaryWriter/BinaryReader):
///
///   [cabecera]   string "ESRV"   marca para reconocer que es un archivo nuestro
///                int32  1        versión del formato (por si cambia en el futuro)
///   [cuerpo]     int32  N        cuántos registros hay
///                N registros, uno tras otro, campo por campo:
///
///   users.dat         codigo:string, esGerente:bool
///   insumos.dat       codigo:string, nombre:string, cantidad:double, unidad:string
///   transactions.dat  id:string, fecha:int64 (ticks), tipo:string,
///                     descripcion:string, monto:double, usuario:string,
///                     proveedor:string, numeroRecibo:string
///
/// A diferencia del .txt, un .dat no se puede leer ni editar con un editor
/// de texto: solo este programa sabe interpretarlo.
///
/// ---------------------------------------------------------------------
/// Dos protecciones para no perder datos:
///   1. Escritura segura: se escribe primero a "archivo.dat.tmp" y solo si
///      terminó bien se reemplaza al .dat real. Si el programa se cierra
///      a mitad de un guardado, el .dat anterior queda intacto.
///   2. Archivo dañado: si un .dat no se puede leer, NO se sobreescribe;
///      se renombra a "archivo.dat.corrupto-FECHA" y se avisa por consola.
///
/// ---------------------------------------------------------------------
/// Migración automática: si todavía no existe el .dat pero sí existe el
/// .txt de la versión anterior, se importa UNA vez y se crea el .dat. El
/// .txt original no se borra (queda como respaldo).
///
/// Los recibos de pedidos y las facturas de compra siguen siendo .txt
/// (son documentos para leer o imprimir, no datos del sistema).
/// </summary>
public static class FileDataStore
{
    private const string Magic = "ESRV";
    private const int FormatVersion = 1;

    private static readonly string BaseDirectory =
        // SpecialFolderOption.Create: si la carpeta base del usuario (por ejemplo
        // ~/.config) no existe, la crea. Sin esta opción, .NET devolvería texto
        // vacío y los datos terminarían en una ruta relativa, dentro de la
        // carpeta desde donde se ejecutó el programa.
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData,
                                               Environment.SpecialFolderOption.Create), "EService");

    private static readonly string DataDirectory = Path.Combine(BaseDirectory, "Data");
    private static readonly string OrdersDirectory = Path.Combine(BaseDirectory, "Pedidos");
    private static readonly string InvoicesDirectory = Path.Combine(BaseDirectory, "Facturas");

    private static readonly string UsersFile = Path.Combine(DataDirectory, "users.dat");
    private static readonly string InsumosFile = Path.Combine(DataDirectory, "insumos.dat");
    private static readonly string TransactionsFile = Path.Combine(DataDirectory, "transactions.dat");

    // Archivos de la versión anterior (solo se leen para migrar).
    private static readonly string LegacyUsersFile = Path.Combine(DataDirectory, "users.txt");
    private static readonly string LegacyInsumosFile = Path.Combine(DataDirectory, "insumos.txt");
    private static readonly string LegacyTransactionsFile = Path.Combine(DataDirectory, "transactions.txt");

    public static void EnsureDirectoriesExist()
    {
        Directory.CreateDirectory(DataDirectory);
        Directory.CreateDirectory(OrdersDirectory);
        Directory.CreateDirectory(InvoicesDirectory);
    }

    // =====================================================================
    //  Empleados
    // =====================================================================
    public static void SaveUsers(IEnumerable<Empleado> users)
    {
        var list = users.ToDoublyLinkedList();

        WriteDat(UsersFile, writer =>
        {
            writer.Write(list.Count);
            foreach (var u in list)
            {
                writer.Write(u.Codigo);
                writer.Write(u.EsGerente);
            }
        });
    }

    public static DoublyLinkedList<Empleado> LoadUsers()
    {
        if (TryReadDat(UsersFile, ReadUsers, out var users)) return users;

        var migrated = LegacyLoadUsers();
        if (migrated.Count > 0) SaveUsers(migrated);
        return migrated;
    }

    private static DoublyLinkedList<Empleado> ReadUsers(BinaryReader reader)
    {
        var count = ReadCount(reader);
        var list = new DoublyLinkedList<Empleado>();

        for (var i = 0; i < count; i++)
        {
            var codigo = reader.ReadString();
            var esGerente = reader.ReadBoolean();
            list.Add(new Empleado(codigo, esGerente));
        }
        return list;
    }

    private static DoublyLinkedList<Empleado> LegacyLoadUsers()
    {
        var list = new DoublyLinkedList<Empleado>();

        foreach (var parts in LegacyRows(LegacyUsersFile))
            if (parts.Length == 2 && bool.TryParse(parts[1], out var esGerente))
                list.Add(new Empleado(parts[0], esGerente));

        return list;
    }

    // =====================================================================
    //  Insumos (inventario)
    // =====================================================================
    public static void SaveInsumos(IEnumerable<Insumo> insumos)
    {
        var list = insumos.ToDoublyLinkedList();

        WriteDat(InsumosFile, writer =>
        {
            writer.Write(list.Count);
            foreach (var i in list)
            {
                writer.Write(i.Codigo);
                writer.Write(i.Nombre);
                writer.Write(i.Cantidad);
                writer.Write(i.TipoUnidad);
            }
        });
    }

    public static DoublyLinkedList<Insumo> LoadInsumos()
    {
        if (TryReadDat(InsumosFile, ReadInsumos, out var insumos)) return insumos;

        var migrated = LegacyLoadInsumos();
        if (migrated.Count > 0) SaveInsumos(migrated);
        return migrated;
    }

    private static DoublyLinkedList<Insumo> ReadInsumos(BinaryReader reader)
    {
        var count = ReadCount(reader);
        var list = new DoublyLinkedList<Insumo>();

        for (var i = 0; i < count; i++)
        {
            var codigo = reader.ReadString();
            var nombre = reader.ReadString();
            var cantidad = reader.ReadDouble();
            var unidad = reader.ReadString();
            list.Add(new Insumo(nombre, unidad, cantidad, codigo));
        }
        return list;
    }

    private static DoublyLinkedList<Insumo> LegacyLoadInsumos()
    {
        var list = new DoublyLinkedList<Insumo>();

        // formato viejo: codigo|nombre|cantidad|unidad
        foreach (var p in LegacyRows(LegacyInsumosFile))
            if (p.Length == 4 && double.TryParse(p[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var cantidad))
                list.Add(new Insumo(p[1], p[3], cantidad, p[0]));

        return list;
    }

    // =====================================================================
    //  Transacciones (libro de ventas y compras)
    // =====================================================================
    public static void SaveTransactions(IEnumerable<Transaction> transactions)
    {
        var list = transactions.ToDoublyLinkedList();

        WriteDat(TransactionsFile, writer =>
        {
            writer.Write(list.Count);
            foreach (var t in list)
            {
                writer.Write(t.TransactionId);
                writer.Write(t.Timestamp.Ticks);
                writer.Write(t.Type);
                writer.Write(t.Description);
                writer.Write(t.Amount);
                writer.Write(t.UserCode);
                writer.Write(t.ProviderName);
                writer.Write(t.ReceiptNumber);
            }
        });
    }

    public static DoublyLinkedList<Transaction> LoadTransactions()
    {
        if (TryReadDat(TransactionsFile, ReadTransactions, out var transactions)) return transactions;

        var migrated = LegacyLoadTransactions();
        if (migrated.Count > 0) SaveTransactions(migrated);
        return migrated;
    }

    private static DoublyLinkedList<Transaction> ReadTransactions(BinaryReader reader)
    {
        var count = ReadCount(reader);
        var list = new DoublyLinkedList<Transaction>();

        for (var i = 0; i < count; i++)
        {
            var id = reader.ReadString();
            var ticks = reader.ReadInt64();
            var tipo = reader.ReadString();
            var descripcion = reader.ReadString();
            var monto = reader.ReadDouble();
            var usuario = reader.ReadString();
            var proveedor = reader.ReadString();
            var recibo = reader.ReadString();

            list.Add(new Transaction(monto, tipo, descripcion, usuario,
                                     new DateTime(ticks, DateTimeKind.Local), id, proveedor, recibo));
        }
        return list;
    }

    private static DoublyLinkedList<Transaction> LegacyLoadTransactions()
    {
        var list = new DoublyLinkedList<Transaction>();

        // formato viejo: id|fecha|tipo|descripcion|monto|usuario  (6 campos)
        // o, en la versión intermedia, con "|proveedor|numeroRecibo" al final
        // (8 campos). Se aceptan ambos y cualquier cantidad >= 6, y las
        // líneas inválidas se saltan sin descartar las demás.
        foreach (var p in LegacyRows(LegacyTransactionsFile))
        {
            if (p.Length < 6) continue;

            if (!double.TryParse(p[4], NumberStyles.Float, CultureInfo.InvariantCulture, out var monto)) continue;
            if (!DateTime.TryParseExact(p[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                                         DateTimeStyles.None, out var fecha)) continue;

            var proveedor = p.Length > 6 ? p[6] : "";
            var recibo = p.Length > 7 ? p[7] : "";

            list.Add(new Transaction(monto, p[2], p[3], p[5], fecha, p[0], proveedor, recibo));
        }
        return list;
    }

    // =====================================================================
    //  Infraestructura común de lectura/escritura .dat
    // =====================================================================

    /// <summary>
    /// Escribe un .dat de forma segura: primero a un archivo temporal y
    /// solo si todo salió bien lo mueve sobre el definitivo.
    /// </summary>
    private static void WriteDat(string path, Action<BinaryWriter> writeBody)
    {
        EnsureDirectoriesExist();
        var tempPath = path + ".tmp";

        try
        {
            using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(Magic);
                writer.Write(FormatVersion);
                writeBody(writer);
            }   // aquí se cierra y se vacía el archivo temporal

            File.Move(tempPath, path, overwrite: true);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Error guardando {Path.GetFileName(path)}: {e.Message}");
        }
    }

    /// <summary>
    /// Intenta leer un .dat. Devuelve false si el archivo no existe o si
    /// estaba dañado (en ese caso lo aparta, no lo borra ni lo pisa).
    /// </summary>
    private static bool TryReadDat<T>(string path, Func<BinaryReader, T> readBody, out T value)
    {
        value = default!;
        if (!File.Exists(path)) return false;

        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            if (reader.ReadString() != Magic)
                throw new InvalidDataException("la cabecera no corresponde a un archivo de E-Service");

            var version = reader.ReadInt32();
            if (version != FormatVersion)
                throw new InvalidDataException($"versión de formato {version} no soportada");

            value = readBody(reader);
            return true;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"El archivo {Path.GetFileName(path)} está dañado ({e.Message}).");
            QuarantineCorruptFile(path);
            value = default!;
            return false;
        }
    }

    private static void QuarantineCorruptFile(string path)
    {
        try
        {
            var backup = $"{path}.corrupto-{DateTime.Now:yyyyMMdd_HHmmss}";
            File.Move(path, backup);
            Console.Error.WriteLine($"Se conservó una copia en: {backup}");
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"No se pudo apartar el archivo dañado: {e.Message}");
        }
    }

    private static int ReadCount(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        if (count < 0) throw new InvalidDataException("cantidad de registros negativa");
        return count;
    }

    // ---- lectura del formato .txt anterior (solo para migrar) ----

    private static IEnumerable<string[]> LegacyRows(string path)
    {
        foreach (var line in ReadLinesOrEmpty(path))
            if (!string.IsNullOrWhiteSpace(line))
                yield return line.Split('|');
    }

    private static string[] ReadLinesOrEmpty(string path)
    {
        if (!File.Exists(path)) return Array.Empty<string>();

        try { return File.ReadAllLines(path); }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"No se pudo leer {Path.GetFileName(path)}: {e.Message}");
            return Array.Empty<string>();
        }
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
