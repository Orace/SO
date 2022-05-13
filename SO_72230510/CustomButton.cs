using System.Windows;
using System.Windows.Controls;

namespace SO_72230510;

public class CustomButton : Button
{
    public static readonly RoutedEvent CustomIsPressedChangedEvent = EventManager.RegisterRoutedEvent(
     name: nameof(CustomIsPressedChanged),
     routingStrategy: RoutingStrategy.Bubble,
     handlerType: typeof(RoutedEventHandler),
     ownerType: typeof(CustomButton));

    public event RoutedEventHandler CustomIsPressedChanged
    {
        add => AddHandler(CustomIsPressedChangedEvent, value);
        remove => RemoveHandler(CustomIsPressedChangedEvent, value);
    }

    protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnIsPressedChanged(e);
        RaiseEvent(new RoutedEventArgs(CustomIsPressedChangedEvent, e));
    }
}