using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SO_72230510;

public static class ButtonEx
{
    public static readonly DependencyProperty PressedChangedCommandProperty =
        DependencyProperty.RegisterAttached(name: "PressedChangedCommand",
                                            propertyType: typeof(ICommand),
                                            ownerType: typeof(ButtonEx),
                                            defaultMetadata: new PropertyMetadata(default(ICommand), PropertyChangedCallback));

    public static void SetPressedChangedCommand(DependencyObject element, ICommand value)
    {
        element.SetValue(PressedChangedCommandProperty, value);
    }

    public static ICommand GetPressedChangedCommand(DependencyObject element)
    {
        return (ICommand)element.GetValue(PressedChangedCommandProperty);
    }

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ButtonBase)
            return;

        if (e.OldValue is null && e.NewValue is not null)
        {
            DependencyPropertyDescriptor.FromProperty(ButtonBase.IsPressedProperty, d.GetType())
                                        .AddValueChanged(d, Handler);
        }

        if (e.OldValue is not null && e.NewValue is null)
        {
            DependencyPropertyDescriptor.FromProperty(ButtonBase.IsPressedProperty, d.GetType())
                                        .RemoveValueChanged(d, Handler);
        }
    }

    private static void Handler(object? sender, EventArgs e)
    {
        if (sender is not ButtonBase button)
            return;

        var command = GetPressedChangedCommand(button);

        var isPressed = button.IsPressed;
        if (command.CanExecute(isPressed))
            command.Execute(isPressed);
    }
}