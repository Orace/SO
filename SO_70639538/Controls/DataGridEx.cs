using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SO_70639538.Controls;

public class DataGridEx : DataGrid
{
    private static readonly DependencyPropertyKey IsAnExtremityPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly(
                                            "IsAnExtremity",
                                            typeof(bool),
                                            typeof(DataGridEx),
                                            new FrameworkPropertyMetadata(defaultValue: false,
                                                                          flags: FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty IsAnExtremityProperty = IsAnExtremityPropertyKey.DependencyProperty;
    
    public static bool GetIsAnExtremity(DataGridRow dataGridRow)
    {
        return (bool)dataGridRow.GetValue(IsAnExtremityProperty);
    }

    private static void SetIsAnExtremity(DataGridRow dataGridRow, bool value)
    {
        dataGridRow.SetValue(IsAnExtremityPropertyKey, value);
    }

    private IReadOnlyList<DataGridRow> _extremities = Array.Empty<DataGridRow>();

    protected override void OnLoadingRow(DataGridRowEventArgs e)
    {
        base.OnLoadingRow(e);
        UpdateExtremities();
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);
        UpdateExtremities();
    }

    private void UpdateExtremities()
    {
        // Current extremities
        var extremities = new[]
            {
                ItemContainerGenerator.ContainerFromIndex(0),
                ItemContainerGenerator.ContainerFromIndex(Items.Count - 1)
            }.OfType<DataGridRow>()
             .Distinct()
             .ToArray();

        // Remove the flag from old extremities (if any).
        foreach (var oldExtremityContainer in _extremities.Except(extremities))
        {
            SetIsAnExtremity(oldExtremityContainer, false);
        }

        // Ensure the flag for new extremities.
        foreach (var extremityContainer in extremities)
        {
            SetIsAnExtremity(extremityContainer, true);
        }

        _extremities = extremities;
    }
}