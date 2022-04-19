using System.Windows.Input;
using Common.WPF.ViewModels;
using Microsoft.Xaml.Behaviors.Core;

namespace SO_10878500.ViewModels;

public class ItemViewModel : NotifyPropertyChangedBase
{
    private Group _group;
    private string _label;

    public ItemViewModel(Group group, string label)
    {
        _group = group;
        _label = label;

        ChangeGroupAction = new ActionCommand(ChangeGroup);
    }

    private void ChangeGroup()
    {
        Group = Group.IsRed ? new Group("Group of P", false) : new Group("Group of G", true);
    }

    public ICommand ChangeGroupAction { get; }

    public Group Group
    {
        get => _group;
        private set => Update(ref _group, value);
    }

    public string Label
    {
        get => _label;
        private set => Update(ref _label, value);
    }
}