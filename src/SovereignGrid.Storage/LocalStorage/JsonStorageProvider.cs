using System.Text.Json;

namespace SovereignGrid.Storage.LocalStorage;

public sealed class JsonStorageProvider
{
    private readonly JsonSerializerOptions _options =
        new()
        {
            WriteIndented = true
        };

    public async Task SaveAsync<T>(
        string filePath,
        T model)
    {
        var json =
            JsonSerializer.Serialize(
                model,
                _options);

        await File.WriteAllTextAsync(
            filePath,
            json);
    }
}
