using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Common.WPF.ViewModels;

public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
{
    private readonly Dispatcher _dispatcher;

    protected NotifyPropertyChangedBase()
    {
        _dispatcher = Dispatcher.CurrentDispatcher;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool Update<T>(ref T target,
                             T newValue,
                             IEqualityComparer<T>? equalityComparer = null,
                             [CallerMemberName] string? propertyName = null)
    {
        equalityComparer ??= EqualityComparer<T>.Default;

        if (equalityComparer.Equals(target, newValue))
            return false;

        target = newValue;
        if (_dispatcher.CheckAccess())
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        else
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged is null)
                return true;

            _dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                    () => propertyChanged(this, new PropertyChangedEventArgs(propertyName)));
        }

        return true;
    }
}