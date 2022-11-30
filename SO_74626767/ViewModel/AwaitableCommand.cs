using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SO_74626767.ViewModel;

public class AwaitableCommand : ICommand
{
    private readonly object _lock = new();
    
    private TaskCompletionSource? _taskCompletionSource;

    /// <summary>
    /// null-event since it's never raised
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    /// <summary>
    /// Always executable
    /// </summary>
    public bool CanExecute(object? parameter) => true;
    

    public void Execute(object? parameter)
    {
        lock (_lock)
        {
            if (_taskCompletionSource is null)
                return;

            _taskCompletionSource.SetResult();

            // start a new cycle
            _taskCompletionSource = null;
        }
    }

    public Task WaitAsync()
    {
        lock (_lock)
        {
            _taskCompletionSource ??= new TaskCompletionSource();
            return _taskCompletionSource.Task;
        }
    }
}