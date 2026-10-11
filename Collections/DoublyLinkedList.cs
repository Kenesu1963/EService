using System;
using System.Collections;
using System.Collections.Generic;

namespace EService.Collections;

/// <summary>
/// Lista doblemente enlazada genérica, implementada a mano.
///
/// Cada elemento vive en un "nodo" que guarda tres cosas: su valor, una
/// referencia al nodo anterior (Prev) y una referencia al nodo siguiente
/// (Next). La lista solo conoce dos nodos directamente: el primero (_head)
/// y el último (_tail).
///
///      null <- [Prev|A|Next] <-> [Prev|B|Next] <-> [Prev|C|Next] -> null
///                  ^ _head                              ^ _tail
///
/// Reemplaza a List&lt;T&gt; (el "ArrayList" de C#), que guarda los elementos
/// en un arreglo contiguo. Diferencias importantes:
///
///   - Agregar al inicio o al final es O(1) (solo se reconectan 2 nodos);
///     en un arreglo, insertar al inicio obliga a mover todos los elementos.
///   - Quitar un nodo ya localizado es O(1): solo se "puentean" sus vecinos.
///   - Acceder por posición (lista[i]) es O(n): hay que recorrer nodos.
///     Para ahorrar camino, el indexador parte del extremo más cercano
///     (head o tail) - esto solo es posible porque la lista es DOBLE.
///   - Se puede recorrer en ambos sentidos (ver Backwards()).
///
/// Esta clase NO sabe nada de pantallas ni de Avalonia: es una estructura de
/// datos pura. Implementa IEnumerable&lt;T&gt;, por lo que también funciona con
/// foreach y con LINQ (Where, Select, OrderBy, Sum...).
/// </summary>
public class DoublyLinkedList<T> : IEnumerable<T>
{
    private sealed class Node
    {
        public T Value;
        public Node? Prev;
        public Node? Next;

        public Node(T value) => Value = value;
    }

    private Node? _head;
    private Node? _tail;

    // Se incrementa en cada modificación. Los enumeradores lo comparan para
    // detectar si alguien modificó la lista mientras se recorría con foreach
    // (igual que hace List<T>), en vez de recorrer datos inconsistentes.
    private int _version;

    public int Count { get; private set; }

    // ---------------------------------------------------------------
    // Agregar
    // ---------------------------------------------------------------

    /// <summary>Agrega al final de la lista. O(1).</summary>
    public void Add(T item)
    {
        var node = new Node(item);

        if (_tail is null)          // lista vacía: el nodo es head y tail a la vez
        {
            _head = _tail = node;
        }
        else
        {
            node.Prev = _tail;      // el nuevo apunta hacia atrás al antiguo último
            _tail.Next = node;      // el antiguo último apunta hacia adelante al nuevo
            _tail = node;
        }

        Count++;
        _version++;
    }

    /// <summary>Agrega al inicio de la lista. O(1).</summary>
    public void AddFirst(T item)
    {
        var node = new Node(item);

        if (_head is null)
        {
            _head = _tail = node;
        }
        else
        {
            node.Next = _head;
            _head.Prev = node;
            _head = node;
        }

        Count++;
        _version++;
    }

    // ---------------------------------------------------------------
    // Quitar
    // ---------------------------------------------------------------

    /// <summary>
    /// Quita la primera aparición del elemento. Devuelve false si no estaba.
    /// Buscar es O(n); una vez encontrado el nodo, quitarlo es O(1).
    /// </summary>
    public bool Remove(T item)
    {
        var comparer = EqualityComparer<T>.Default;

        for (var node = _head; node is not null; node = node.Next)
        {
            if (comparer.Equals(node.Value, item))
            {
                Unlink(node);
                return true;
            }
        }

        return false;
    }

    /// <summary>Quita el elemento en la posición indicada.</summary>
    public void RemoveAt(int index) => Unlink(NodeAt(index));

    /// <summary>Vacía la lista.</summary>
    public void Clear()
    {
        _head = _tail = null;
        Count = 0;
        _version++;
    }

    /// <summary>
    /// "Puentea" un nodo: conecta su anterior con su siguiente, dejando al
    /// nodo fuera de la cadena. Los cuatro casos (único, primero, último,
    /// intermedio) son los clásicos de las listas doblemente enlazadas.
    /// </summary>
    private void Unlink(Node node)
    {
        if (node.Prev is null) _head = node.Next;        // era el primero
        else node.Prev.Next = node.Next;

        if (node.Next is null) _tail = node.Prev;        // era el último
        else node.Next.Prev = node.Prev;

        // Se limpian sus referencias para que no mantenga vivos a sus vecinos.
        node.Prev = node.Next = null;

        Count--;
        _version++;
    }

    // ---------------------------------------------------------------
    // Consultar
    // ---------------------------------------------------------------

    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>Posición de la primera aparición, o -1 si no está.</summary>
    public int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        var index = 0;

        for (var node = _head; node is not null; node = node.Next, index++)
            if (comparer.Equals(node.Value, item)) return index;

        return -1;
    }

    /// <summary>Primer elemento que cumple la condición, o default si ninguno.</summary>
    public T? Find(Predicate<T> match)
    {
        for (var node = _head; node is not null; node = node.Next)
            if (match(node.Value)) return node.Value;

        return default;
    }

    /// <summary>
    /// Acceso por posición. O(n), pero parte del extremo más cercano:
    /// si el índice está en la primera mitad se recorre desde _head hacia
    /// adelante; si está en la segunda, desde _tail hacia atrás.
    /// </summary>
    public T this[int index]
    {
        get => NodeAt(index).Value;
        set => NodeAt(index).Value = value;
    }

    private Node NodeAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Índice fuera de rango (Count = {Count}).");

        Node node;
        if (index < Count / 2)
        {
            node = _head!;
            for (var i = 0; i < index; i++) node = node.Next!;
        }
        else
        {
            node = _tail!;
            for (var i = Count - 1; i > index; i--) node = node.Prev!;
        }

        return node;
    }

    // ---------------------------------------------------------------
    // Recorrer
    // ---------------------------------------------------------------

    /// <summary>Recorre de primero a último (el orden normal de foreach).</summary>
    public IEnumerator<T> GetEnumerator()
    {
        var version = _version;

        for (var node = _head; node is not null; node = node.Next)
        {
            yield return node.Value;

            if (version != _version)
                throw new InvalidOperationException("La lista fue modificada durante el recorrido.");
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Recorre de último a primero siguiendo los enlaces Prev. Es lo que una
    /// lista simple NO puede hacer sin recorrerse completa antes. Ejemplo:
    /// mostrar las transacciones de la más reciente a la más antigua.
    /// </summary>
    public IEnumerable<T> Backwards()
    {
        var version = _version;

        for (var node = _tail; node is not null; node = node.Prev)
        {
            yield return node.Value;

            if (version != _version)
                throw new InvalidOperationException("La lista fue modificada durante el recorrido.");
        }
    }

    /// <summary>Copia independiente: los elementos son los mismos, los nodos no.</summary>
    public DoublyLinkedList<T> Copy()
    {
        var copy = new DoublyLinkedList<T>();
        foreach (var item in this) copy.Add(item);
        return copy;
    }
}

/// <summary>
/// Permite escribir "consulta.ToDoublyLinkedList()" al final de una cadena
/// LINQ, igual que se escribía ".ToList()" antes.
/// </summary>
public static class DoublyLinkedListExtensions
{
    public static DoublyLinkedList<T> ToDoublyLinkedList<T>(this IEnumerable<T> source)
    {
        var list = new DoublyLinkedList<T>();
        foreach (var item in source) list.Add(item);
        return list;
    }
}
