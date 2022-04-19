using System.Collections.ObjectModel;

namespace SO_10878500.ViewModels;

public class ItemsViewModel
{
    public ItemsViewModel()
    {
        Items = new ObservableCollection<ItemViewModel>
        {
            new(new Group("Group of P", false), "Pipo"),
            new(new Group("Group of P", false), "Poil"),
            new(new Group("Group of G", true), "Gigo"),
            new(new Group("Group of G", true), "Goil")
        };
    }

    public ObservableCollection<ItemViewModel> Items { get; }
}