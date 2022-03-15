using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SO_71475662;

public class DataGridEx
{
    public static readonly DependencyProperty AutoEditColumnProperty = DependencyProperty.RegisterAttached("AutoEditColumn", typeof(int), typeof(DataGridEx), new PropertyMetadata(default(int), AutoEditColumnChangedCallback));

    public static void SetAutoEditColumn(DependencyObject element, int value)
    {
        element.SetValue(AutoEditColumnProperty, value);
    }

    public static int GetAutoEditColumn(DependencyObject element)
    {
        return (int)element.GetValue(AutoEditColumnProperty);
    }

    private static void AutoEditColumnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid dataGrid)
            return;

        GetAutoEditColumnHelper(dataGrid)?.Dispose();

        if (e.NewValue is int columnIndex)
        {
            SetAutoEditColumnHelper(d, new AutoEditColumnHelper(dataGrid, columnIndex));
        }
        else
        {
            d.ClearValue(AutoEditColumnHelperProperty);
        }
    }


    private static readonly DependencyProperty AutoEditColumnHelperProperty = DependencyProperty.RegisterAttached("AutoEditColumnHelper", typeof(AutoEditColumnHelper), typeof(DataGridEx), new PropertyMetadata(default(AutoEditColumnHelper)));

    private static void SetAutoEditColumnHelper(DependencyObject element, AutoEditColumnHelper value)
    {
        element.SetValue(AutoEditColumnHelperProperty, value);
    }

    private static AutoEditColumnHelper? GetAutoEditColumnHelper(DependencyObject element)
    {
        return element.GetValue(AutoEditColumnHelperProperty) as AutoEditColumnHelper;
    }


    private class AutoEditColumnHelper : IDisposable
    {
        private readonly DataGrid _dataGrid;
        private readonly int _columnIndex;

        private object? _lastItem;

        public AutoEditColumnHelper(DataGrid dataGrid, int columnIndex)
        {
            _dataGrid = dataGrid;
            _columnIndex = columnIndex;

            _dataGrid.CurrentCellChanged += OnCurrentCellChanged;
        }

        public void Dispose()
        {
            _dataGrid.CurrentCellChanged -= OnCurrentCellChanged;
        }

        private void OnCurrentCellChanged(object? sender, EventArgs e)
        {
            var currentCell = _dataGrid.CurrentCell;

            if (!currentCell.IsValid || Equals(currentCell.Item, _lastItem))
                return;

            var autoEditColumn = GetAutoEditColumn();
            if (autoEditColumn is null)
                return;

            _dataGrid.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, () =>
            {
                _lastItem = _dataGrid.SelectedItem;
                _dataGrid.CurrentCell = new DataGridCellInfo(_lastItem, autoEditColumn);
                _dataGrid.BeginEdit();
            });
        }

        private DataGridColumn? GetAutoEditColumn()
        {
            return _columnIndex < 0 || _columnIndex > _dataGrid.Columns.Count ? null : _dataGrid.Columns[_columnIndex];
        }
    }
}