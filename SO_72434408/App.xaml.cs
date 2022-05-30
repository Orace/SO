using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace SO_72434408;

public partial class App
{
    static App()
    {
        var ietfLanguageTag = CultureInfo.CurrentCulture.IetfLanguageTag;
        var xmlLanguage = XmlLanguage.GetLanguage(ietfLanguageTag);
        var propertyMetadata = new FrameworkPropertyMetadata(xmlLanguage);
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), propertyMetadata);
    }
}