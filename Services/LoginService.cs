using System;
using System.Collections.Generic;
using EService.Collections;
using EService.Models;

namespace EService.Services;

/// <summary>
/// Replaces LoginController.java. It keeps all the same employee-management
/// logic (validate code, add/remove/modify/verify employee, list them all),
/// loaded from and saved to users.dat (binary, see FileDataStore).
///
/// Internamente los empleados se guardan en un Dictionary (búsqueda por
/// código en O(1), que es lo que más se usa al iniciar sesión); las listas
/// que se entregan hacia afuera son DoublyLinkedList.
///
/// One deliberate change: the original Java class popped up JOptionPane
/// dialogs directly from inside the controller, mixing business logic with
/// UI code. Here, this class only does the *data* work and throws a plain
/// exception (or returns a plain result) when something goes wrong. The
/// ViewModel is what decides how to ask the user for input or how to show
/// an error - that separation is what makes the MVVM pattern easy to test
/// and easy to reskin later without touching this logic at all.
/// </summary>
public class LoginService
{
    // Singleton, same as the Java version's getInstance() pattern.
    private static LoginService? _instance;
    public static LoginService Instance => _instance ??= new LoginService();

    private readonly Dictionary<string, Empleado> _empleados = new();

    public string? CurrentUserCode { get; set; }
    public bool CurrentUserIsManager { get; set; }

    private LoginService()
    {
        CargarEmpleadosDesdeArchivo();
        if (_empleados.Count == 0)
            CargarEmpleadosPorDefecto();
    }

    private void CargarEmpleadosPorDefecto()
    {
        // Same seed accounts as the original project, so existing paper
        // notes / muscle memory for demo codes still work.
        Agregar("DW1963", false);
        Agregar("WR2005", false);
        Agregar("RD6941", false);
        Agregar("FF2014", false);
        Agregar("MS0110", false);
        Agregar("TH2007", true);
        Agregar("MK1975", true);

        void Agregar(string codigo, bool esGerente) => _empleados[codigo] = new Empleado(codigo, esGerente);

        GuardarEmpleados();
    }

    private void CargarEmpleadosDesdeArchivo()
    {
        foreach (var empleado in FileDataStore.LoadUsers())
            _empleados[empleado.Codigo] = empleado;
    }

    private void GuardarEmpleados() => FileDataStore.SaveUsers(_empleados.Values);

    public Empleado? ValidarCodigo(string codigo) =>
        _empleados.TryGetValue(codigo, out var empleado) ? empleado : null;

    /// <summary>Copia de la lista actual de empleados (no la interna).</summary>
    public DoublyLinkedList<Empleado> ObtenerTodosLosEmpleados() => _empleados.Values.ToDoublyLinkedList();

    /// <summary>Throws InvalidOperationException if the code already exists.</summary>
    public void AgregarEmpleado(string codigo, bool esGerente)
    {
        codigo = codigo.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(codigo))
            throw new ArgumentException("El código no puede estar vacío.");
        if (_empleados.ContainsKey(codigo))
            throw new InvalidOperationException($"El código {codigo} ya existe en el sistema.");

        _empleados[codigo] = new Empleado(codigo, esGerente);
        GuardarEmpleados();
    }

    /// <summary>Returns false if the code doesn't exist.</summary>
    public bool EliminarEmpleado(string codigo)
    {
        codigo = codigo.Trim().ToUpperInvariant();
        if (!_empleados.Remove(codigo)) return false;
        GuardarEmpleados();
        return true;
    }

    /// <summary>Throws KeyNotFoundException if the code doesn't exist.</summary>
    public void ModificarEmpleado(string codigo, bool nuevoEsGerente)
    {
        codigo = codigo.Trim().ToUpperInvariant();
        if (!_empleados.TryGetValue(codigo, out var empleado))
            throw new KeyNotFoundException($"El código {codigo} no existe en el sistema.");

        empleado.EsGerente = nuevoEsGerente;
        GuardarEmpleados();
    }
}
