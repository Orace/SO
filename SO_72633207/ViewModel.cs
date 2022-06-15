using System;
using System.Windows.Input;
using Common.WPF.ViewModels;
using Microsoft.Xaml.Behaviors.Core;

namespace SO_72633207;

public class ViewModel : NotifyPropertyChangedBase
{
    private string _firstColumnWidth = string.Empty;
    private string _firstRowHeight = string.Empty;
    private string _secondColumnWidth = string.Empty;
    private string _secondRowHeight = string.Empty;

    public ViewModel()
    {
        ChangeWidthAndHeightCommand = new ActionCommand(ChangeWidthAndHeight);
    }

    public ICommand ChangeWidthAndHeightCommand { get; }

    public string FirstColumnWidth
    {
        get => _firstColumnWidth;
        private set => Update(ref _firstColumnWidth, value);
    }

    public string FirstRowHeight
    {
        get => _firstRowHeight;
        private set => Update(ref _firstRowHeight, value);
    }

    public string SecondColumnWidth
    {
        get => _secondColumnWidth;
        private set => Update(ref _secondColumnWidth, value);
    }

    public string SecondRowHeight
    {
        get => _secondRowHeight;
        private set => Update(ref _secondRowHeight, value);
    }

    private void ChangeWidthAndHeight()
    {
        var random = Random.Shared;
        var firstColumnWidth = random.NextDouble();
        FirstColumnWidth = $"{firstColumnWidth}*";
        SecondColumnWidth = $"{1 - firstColumnWidth}*";

        var firstRowHeight = random.NextDouble();
        FirstRowHeight = $"{firstRowHeight}*";
        SecondRowHeight = $"{1 - firstRowHeight}*";
    }
}