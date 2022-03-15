using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;

namespace Common.WPF.ViewModels;

public class ItemsViewModel
{
    public ItemsViewModel()
    {
        Items = new ObservableCollection<ItemViewModel>
        {
            BuildItem(),
            BuildItem(),
            BuildItem(),
            BuildItem(),
            BuildItem(),
            BuildItem()
        };

        AddCommand = new ActionCommand(AddItem);
        RemoveCommand = new ActionCommand(RemoveItem);
    }

    public ICommand AddCommand { get; }

    public ICommand RemoveCommand { get; }

    public ObservableCollection<ItemViewModel> Items { get; }

    private void AddItem()
    {
        Items.Insert(Random.Shared.Next(Items.Count + 1), BuildItem());
    }

    private void RemoveItem()
    {
        if (Items.Count is 0)
            return;

        Items.RemoveAt(Random.Shared.Next(Items.Count));
    }

    private static ItemViewModel BuildItem()
    {
        var v = Random.Shared.Next(69);
        return new ItemViewModel($"Item #{v}", v);
    }
}