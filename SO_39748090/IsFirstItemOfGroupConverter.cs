using System;
using System.Globalization;
using System.Windows.Data;

namespace SO_39748090;

public class IsFirstItemOfGroupConverter : IMultiValueConverter
{
    public object Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length != 2)
        {
            return true;
        }

        if (values[0] is not CollectionView collectionView || values[1] is not FlatItemViewModel viewModel)
        {
            return true;
        }

        var previousIndex = collectionView.IndexOf(viewModel) - 1;
        if (previousIndex < 0)
        {
            return true;
        }

        if (collectionView.GetItemAt(previousIndex) is not FlatItemViewModel otherViewModel)
        {
            return true;
        }

        return !Equals(viewModel.GroupKey, otherViewModel.GroupKey);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return Array.Empty<object>();
    }
}