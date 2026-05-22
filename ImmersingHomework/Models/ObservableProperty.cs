using System;
using Serilog;

namespace ImmersingHomework.Models;

public class ObservableProperty<T>
{
    private readonly ILogger _logger = Log.ForContext<ObservableProperty<T>>();
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

    public ObservableProperty(T? value)
    {
        _value = value;
    }
}