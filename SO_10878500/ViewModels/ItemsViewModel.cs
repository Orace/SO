using System.Collections.ObjectModel;

namespace SO_10878500.ViewModels;

public class ItemsViewModel
{
    public ItemsViewModel()
    {
        Items = new ObservableCollection<ItemViewModel>
        {
            new(Group.GroupOfP, "Pipo"),
            new(Group.GroupOfP, "Poil"),
            new(Group.GroupOfG, "Gigo"),
            new(Group.GroupOfG, "Goil"),
        };
    }

    public ObservableCollection<ItemViewModel> Items { get; }
}