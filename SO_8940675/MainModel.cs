using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SO_8940675;

public class MainModel : INotifyPropertyChanged
{
    private int _value1;
    private int _value2;

    public MainModel()
    {
        var values = new[] { 1, 2, 3 };
        Values1 = values.ToImmutableArray();
        Values2 = values.ToImmutableArray();
    }

    public IReadOnlyList<int> Values1 { get; }

    public int Value1
    {
        get => _value1;
        set => SetField(ref _value1, value);
    }

    public ImmutableArray<int> Values2 { get; }

    public int Value2
    {
        get => _value2;
        set => SetField(ref _value2, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}