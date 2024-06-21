using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SO_78651229;

public class MyGridSplitter : GridSplitter
{
    private readonly ITransformProvider _automationPeer;

    public MyGridSplitter()
    {
        _automationPeer = new GridSplitterAutomationPeer(this);
    }

    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
        base.OnMouseDoubleClick(e);

        var prtp = GetPositionRelativeToParent();
        if (prtp.X is 0)
        {
            _automationPeer.Move(double.MaxValue, double.MaxValue);
        }
        else
        {
            _automationPeer.Move(-double.MaxValue, -double.MaxValue);
        }
    }

    private Point GetPositionRelativeToParent()
    {
        return TranslatePoint(new Point(0, 0), VisualTreeHelper.GetParent(this) as UIElement);
    }
}