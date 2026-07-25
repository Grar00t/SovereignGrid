namespace SovereignGrid.App.Services;

public sealed class ThemeService
{
    public string CurrentTheme { get; private set; } = "Dark";

    public void SetTheme(string theme)
    {
        CurrentTheme = theme;
    }
}
