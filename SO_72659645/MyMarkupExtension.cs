using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace SO_72659645;

public class MyMarkupExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new List<string>
        {
            "Item 1",
            "Item 2",
            "Item 3",
            "Item 4"
        };
    }
}