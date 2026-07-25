using System.Text.Json;
using SovereignGrid.Core.Persistence;
using SovereignGrid.Core.Workbook;

namespace SovereignGrid.Storage.LocalStorage;

public sealed class WorkbookStorageService
{
    public async Task SaveAsync(
        Workbook workbook,
        string filePath)
    {
        WorkbookFile file = new();

        foreach (var sheet in workbook.Worksheets)
        {
            WorksheetFile ws = new()
            {
                Name = sheet.Name
            };

            foreach (var cell in sheet.Cells)
            {
                ws.Cells.Add(
                    new CellFile
                    {
                        Row = cell.Key.Row,
                        Column = cell.Key.Column,
                        Value = cell.Value.Value
                    });
            }

            file.Worksheets.Add(ws);
        }

        await File.WriteAllTextAsync(
            filePath,
            JsonSerializer.Serialize(
                file,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
    }

    public async Task<Workbook> LoadAsync(
        string filePath)
    {
        var json =
            await File.ReadAllTextAsync(filePath);

        var file =
            JsonSerializer.Deserialize<WorkbookFile>(json)!;

        Workbook workbook = new();

        foreach (var sheetFile in file.Worksheets)
        {
            var sheet =
                workbook.AddWorksheet(
                    sheetFile.Name);

            foreach (var cell in sheetFile.Cells)
            {
                sheet.SetValue(
                    cell.Row,
                    cell.Column,
                    cell.Value);
            }
        }

        return workbook;
    }
}
