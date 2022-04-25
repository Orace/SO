using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SO_39748090;

public abstract class StepsFieldConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is IEnumerable<StepViewModel> steps
                   ? $"[{string.Join(" ; ", steps.Select(GetField))}]"
                   : DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    protected abstract string GetField(StepViewModel stepViewModel);
}

public class StepsDurationConverter : StepsFieldConverter
{
    protected override string GetField(StepViewModel stepViewModel)
    {
        return stepViewModel.Duration.ToString(@"d\.hh\:mm\:ss");
    }
}

public class StepsStartConverter : StepsFieldConverter
{
    protected override string GetField(StepViewModel stepViewModel)
    {
        return stepViewModel.Start.ToString();
    }
}

public class StepsStopConverter : StepsFieldConverter
{
    protected override string GetField(StepViewModel stepViewModel)
    {
        return stepViewModel.Stop.ToString();
    }
}