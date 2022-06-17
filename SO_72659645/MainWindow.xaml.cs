using System.Windows.Controls;
using System.Windows.Data;

namespace SO_72659645;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        // ItemsSource="{Binding Source={local:MyMarkup}}"
        var myMarkupExtension1 = new MyMarkupExtension();
        var value1 = myMarkupExtension1.ProvideValue(null);
        var binding = new Binding { Source = value1 };
        ListViewWithBinding.SetBinding(ItemsControl.ItemsSourceProperty, binding);

        // ItemsSource = "{local:MyMarkup}"
        var myMarkupExtension2 = new MyMarkupExtension();
        var value2 = myMarkupExtension2.ProvideValue(null);
        ListViewDirect.SetValue(ItemsControl.ItemsSourceProperty, value2);
    }
}