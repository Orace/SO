using System.Windows;
using System.Windows.Input;
using Common.WPF.ViewModels;
using Microsoft.Xaml.Behaviors.Core;

namespace SO_72230510;

public class ViewModel : NotifyPropertyChangedBase
{
    public ViewModel()
    {
        PressedChangedCommand = new ActionCommand(PressedChanged);
    }

    private int _count;

    public int Count
    {
        get => _count;
        private set => Update(ref _count, value);
    }

    public ICommand PressedChangedCommand { get; }

    private void PressedChanged(object o)
    {
        if (o is not RoutedEventArgs routedEventArgs)
            return;

        if (routedEventArgs.OriginalSource is not DependencyPropertyChangedEventArgs eventArgs)
            return;

        if (eventArgs.NewValue is true)
            Count++;

        if (eventArgs.NewValue is false)
            Count--;

    }
}