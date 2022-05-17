using System;
using System.Collections.Generic;
using System.Windows.Input;
using Common.WPF.ViewModels;
using Microsoft.Xaml.Behaviors.Core;

namespace SO_72267688;

public class ViewModel
{
    public ViewModel()
    {
        Drives = new[]
        {
            new DriveViewModel { Name = "C:", Type = "NTFS", FreeSpace = 1024, TotalSize = 2048 },
            new DriveViewModel { Name = "D:", Type = "FAT32", FreeSpace = 1, TotalSize = 2 }
        };

        RefreshCommand = new ActionCommand(Refresh);
    }

    public IReadOnlyList<DriveViewModel> Drives { get; }

    public ICommand RefreshCommand { get; }

    private void Refresh()
    {
        foreach (var drive in Drives)
        {
            drive.FreeSpace = Random.Shared.NextDouble() * drive.TotalSize;
        }
    }
}

public class DriveViewModel : NotifyPropertyChangedBase
{
    private double _freeSpace;

    public string Name { get; init; } = "";
    public string Type { get; init; } = "";
    public double TotalSize { get; init; }
    
    public double FreeSpace
    {
        get => _freeSpace;
        set => Update(ref _freeSpace, value);
    }
}