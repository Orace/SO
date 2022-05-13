using System.Windows.Input;
using Common.WPF.ViewModels;
using Microsoft.Xaml.Behaviors.Core;

namespace SO_72229375;

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

    private void PressedChanged()
    {
        Count++;
    }
}