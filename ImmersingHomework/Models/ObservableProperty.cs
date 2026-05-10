using System;

namespace ImmersingHomework.Models;

public class ObservableProperty<T>
{
    private T? _value;

    public T? Value
    {
        get => _value;
        set
        {
            _value = value;
            ValueChanged?.Invoke(value);
        }
    }
    
    public event Action<T?> ValueChanged;
}