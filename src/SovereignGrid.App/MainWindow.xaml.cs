using System.Windows;
using Microsoft.Win32;
using SovereignGrid.App.Services;
using SovereignGrid.Connectors.Abstractions;
using unvell.ReoGrid;
using unvell.ReoGrid.IO;
using CoreWorkbook = SovereignGrid.Core.Workbook.Workbook;

namespace SovereignGrid.App;

public partial class MainWindow : Window
{
    private readonly ReoGridControl _grid;

    public MainWindow()
    {
        InitializeComponent();

        _grid = new ReoGridControl();
        RootGrid.Children.Add(_grid);

        ConnectorBox.ItemsSource = ConnectorRegistry.All;
        ConnectorBox.DisplayMemberPath = "Capabilities.DisplayName";
        ConnectorBox.SelectedIndex = 0;

        SeedDemo();

        OpenButton.Click += async (_, _) => await OpenViaConnectorAsync();
        SaveButton.Click += (_, _) => SaveXlsx();
        RtlButton.Click  += (_, _) => ToggleRtl();
    }

    private void SeedDemo()
    {
        var s = _grid.CurrentWorksheet;
        s.Reset();
        s["A1"]="Item"; s["B1"]="Location"; s["C1"]="Qty"; s["D1"]="Unit"; s["E1"]="Total";
        s["A2"]="Monitor"; s["B2"]="Ward A"; s["C2"]=12; s["D2"]=850;  s["E2"]="=C2*D2";
        s["A3"]="Pump";    s["B3"]="Ward B"; s["C3"]=8;  s["D3"]=1200; s["E3"]="=C3*D3";
        s["A4"]="Total";                                   s["E4"]="=SUM(E2:E3)";
        GridTheme.ApplyHeader(s, "A1:E1");
        GridTheme.ApplyBody(s, 5, 5);
    }

    private async System.Threading.Tasks.Task OpenViaConnectorAsync()
    {
        if (ConnectorBox.SelectedItem is not IConnector connector) return;

        var options = new Dictionary<string, string>();

        if (connector.Capabilities.Kind == ConnectorKind.FileImportExport)
        {
            var dlg = new OpenFileDialog { Filter = "Excel (*.xlsx)|*.xlsx" };
            if (dlg.ShowDialog() != true) return;
            options["path"] = dlg.FileName;
        }
        else
        {
            var input = ConnectionPrompt.Show(connector.Capabilities.DisplayName);
            if (input is null) return;
            options["connectionString"] = input.Value.cs;
            options["query"] = input.Value.sql;
        }

        var core = new CoreWorkbook();
        var coreSheet = core.AddWorksheet("Imported");

        try
        {
            var result = await connector.ImportAsync(coreSheet, options);
            if (!result.Success) { MessageBox.Show(result.Message); return; }
            RenderCoreToGrid(coreSheet);
            MessageBox.Show($"{result.Message}  ({result.RowsAffected} rows)");
        }
        catch (System.Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void RenderCoreToGrid(SovereignGrid.Core.Workbook.Worksheet coreSheet)
    {
        var s = _grid.CurrentWorksheet;
        s.Reset();

        int maxRow = 0, maxCol = 0;
        foreach (var kv in coreSheet.Cells)
        {
            int r = kv.Key.Row - 1, c = kv.Key.Column - 1;
            if (r >= 0 && c >= 0)
            {
                s[r, c] = kv.Value.Value;
                if (kv.Key.Row > maxRow) maxRow = kv.Key.Row;
                if (kv.Key.Column > maxCol) maxCol = kv.Key.Column;
            }
        }
        if (maxRow > 0 && maxCol > 0)
        {
            GridTheme.ApplyHeader(s, $"A1:{new CellPosition(0, maxCol - 1).ToAddress()}");
            GridTheme.ApplyBody(s, maxRow, maxCol);
            if (FlowDirection == FlowDirection.RightToLeft)
                GridTheme.ApplyRtlHint(s, maxRow, maxCol);
        }
    }

    private void SaveXlsx()
    {
        var dlg = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = "workbook.xlsx" };
        if (dlg.ShowDialog() == true)
        {
            try { _grid.Save(dlg.FileName, FileFormat.Excel2007); }
            catch (System.Exception ex) { MessageBox.Show(ex.Message); }
        }
    }

    private void ToggleRtl()
    {
        FlowDirection = FlowDirection == FlowDirection.LeftToRight
            ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        var s = _grid.CurrentWorksheet;
        GridTheme.ApplyRtlHint(s, s.Rows, s.Columns);
    }
}
