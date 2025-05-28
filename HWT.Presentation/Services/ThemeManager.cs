using System.Windows;
using HWT.Application.Interfaces;

namespace HWT.Presentation.Services
{
    public class ThemeManager : IThemeManager
    {
        private const string ThemeFolder = "Themes/";
        private readonly System.Windows.Application _app;

        public ThemeManager(System.Windows.Application application)
        {
            _app = application ?? throw new ArgumentNullException(nameof(application));
            AvailableThemes = new[] { "Dark", "Locops", "Tacops", "Specops" };
            CurrentTheme = AvailableThemes[0];
            ApplyTheme(CurrentTheme);
        }

        public string[] AvailableThemes { get; }
        public string CurrentTheme { get; private set; }

        public void ApplyTheme(string themeName)
        {
            if (!AvailableThemes.Contains(themeName, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"Unknown theme: {themeName}", nameof(themeName));
            
            var toRemove = _app.Resources.MergedDictionaries
                .Where(d =>
                    d.Source != null &&
                    d.Source.OriginalString
                        .IndexOf($"/{ThemeFolder}", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            foreach (var dic in toRemove)
                _app.Resources.MergedDictionaries.Remove(dic);
            
            var uri = new Uri($"/HWT.Presentation;component/{ThemeFolder}{themeName}.xaml", UriKind.Relative);
            var themeDict = new ResourceDictionary { Source = uri };
            
            _app.Resources.MergedDictionaries.Add(themeDict);

            CurrentTheme = themeName;
        }
    }
}