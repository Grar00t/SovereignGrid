namespace SovereignGrid.App.Services;

public sealed class LocalizationService
{
    public string CurrentLanguage { get; private set; } = "en";

    public void SetLanguage(string language)
    {
        CurrentLanguage = language;
    }
}
