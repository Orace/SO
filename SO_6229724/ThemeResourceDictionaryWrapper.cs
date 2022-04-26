using System.Windows;

namespace SO_6229724;

public class ThemeResourceDictionaryWrapper : ResourceDictionary
{
    private ThemesController? _themesController;

    public ThemesController? ThemesController
    {
        get => _themesController;
        set
        {
            if (Equals(_themesController, value))
            {
                return;
            }

            if (_themesController is not null)
            {
                _themesController.ThemeChanged -= UpdateSource;
            }

            _themesController = value;

            if (_themesController is not null)
            {
                _themesController.ThemeChanged += UpdateSource;
            }

            UpdateSource();
        }
    }

    private void UpdateSource()
    {
        MergedDictionaries.Clear();

        var currentTheme = _themesController?.CurrentTheme;
        if (currentTheme is not null)
        {
            MergedDictionaries.Add(currentTheme);
        }
    }
}