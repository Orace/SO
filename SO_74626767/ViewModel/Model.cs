using System;
using Common.WPF.ViewModels;

namespace SO_74626767.ViewModel;

public class Model : NotifyPropertyChangedBase
{
    private int _count;

    public Model()
    {
        RunLogicAsync();
    }

    public int Count
    {
        get => _count;
        private set => Update(ref _count, value);
    }

    public AwaitableCommand OnNextCommand { get; } = new();
    
    /// <summary>
    /// We should avoid async void I know, I know
    /// </summary>
    private async void RunLogicAsync()
    {
        try
        {
            for (;;)
            {
                await OnNextCommand.WaitAsync();
                Count++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}