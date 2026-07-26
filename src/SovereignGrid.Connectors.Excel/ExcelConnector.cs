using ClosedXML.Excel;
using SovereignGrid.Connectors.Abstractions;
using SovereignGrid.Core.Workbook;

namespace SovereignGrid.Connectors.Excel;

public sealed class ExcelConnector : IConnector
{
    public ConnectorCapabilities Capabilities => new(
        Id: "excel.xlsx",
        DisplayName: "Excel (.xlsx)",
        Kind: ConnectorKind.FileImportExport,
        RequiresNetwork: false,
        SupportsRead: true,
        SupportsWrite: true);

    public Task<ConnectorResult> ImportAsync(Worksheet target,
        IDictionary<string,string> options, CancellationToken ct = default)
    {
        var path = options["path"];
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheets.First();

        int n = 0;
        foreach (var cell in ws.CellsUsed())
        {
            target.SetValue(cell.Address.RowNumber, cell.Address.ColumnNumber, cell.GetString());
            n++;
        }
        return Task.FromResult(new ConnectorResult(true, "Imported .xlsx", n));
    }

    public Task<ConnectorResult> ExportAsync(Worksheet source,
        IDictionary<string,string> options, CancellationToken ct = default)
    {
        var path = options["path"];
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet(string.IsNullOrWhiteSpace(source.Name) ? "Sheet1" : source.Name);
        foreach (var kv in source.Cells)
            ws.Cell(kv.Key.Row, kv.Key.Column).Value = kv.Value.Value;
        wb.SaveAs(path);
        return Task.FromResult(new ConnectorResult(true, "Exported .xlsx", source.Cells.Count));
    }
}
