using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SovereignGrid.App.Services;
using SovereignGrid.Connectors.Abstractions;
using unvell.ReoGrid;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.IO;
using CoreWorkbook = SovereignGrid.Core.Workbook.Workbook;

namespace SovereignGrid.App;

public partial class MainWindow : Window
{
    private readonly ReoGridControl _grid;

    private Worksheet Sheet => _grid.CurrentWorksheet;
    private RangePosition Sel => Sheet.SelectionRange;

    public MainWindow()
    {
        InitializeComponent();

        _grid = new ReoGridControl();
        RootGrid.Children.Add(_grid);

        ConnectorBox.ItemsSource = ConnectorRegistry.All;
        ConnectorBox.DisplayMemberPath = "Capabilities.DisplayName";
        ConnectorBox.SelectedIndex = 0;

        FontBox.ItemsSource = new[] { "Segoe UI","Calibri","Arial","Tahoma","Times New Roman",
                                      "Traditional Arabic","Simplified Arabic","Sakkal Majalla","Amiri" };
        FontBox.SelectedIndex = 0;
        SizeBox.ItemsSource = new[] { "8","9","10","11","12","14","16","18","20","24","28","36","48","72" };
        SizeBox.SelectedIndex = 3;

        NewFile();
        WireEvents();
    }

    private void WireEvents()
    {
        NewButton.Click    += (_, _) => NewFile();
        OpenFileBtn.Click  += (_, _) => OpenXlsx();
        OpenButton.Click   += async (_, _) => await OpenViaConnectorAsync();
        SaveButton.Click   += (_, _) => SaveXlsx();
        SaveAsBtn.Click    += (_, _) => SaveXlsx();

        UndoBtn.Click  += (_, _) => _grid.Undo();
        RedoBtn.Click  += (_, _) => _grid.Redo();
        CutBtn.Click   += (_, _) => _grid.Cut();
        CopyBtn.Click  += (_, _) => _grid.Copy();
        PasteBtn.Click += (_, _) => _grid.Paste();

        FontBox.SelectionChanged += (_, _) => { if (FontBox.SelectedItem is string f)
            Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontName, FontName = f }); };
        SizeBox.SelectionChanged += (_, _) => { if (SizeBox.SelectedItem is string s && float.TryParse(s, out var v))
            Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontSize, FontSize = v }); };

        BoldBtn.Click      += (_, _) => Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontStyleBold, Bold = BoldBtn.IsChecked == true });
        ItalicBtn.Click    += (_, _) => Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontStyleItalic, Italic = ItalicBtn.IsChecked == true });
        UnderlineBtn.Click += (_, _) => Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontStyleUnderline, Underline = UnderlineBtn.IsChecked == true });

        TextColorBtn.Click += (_, _) => { var c = PickColor(); if (c.HasValue)
            Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.TextColor, TextColor = c.Value }); };
        FillColorBtn.Click += (_, _) => { var c = PickColor(); if (c.HasValue)
            Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.BackColor, BackColor = c.Value }); };

        AlignLeftBtn.Click   += (_, _) => Apply(HAlign(ReoGridHorAlign.Left));
        AlignCenterBtn.Click += (_, _) => Apply(HAlign(ReoGridHorAlign.Center));
        AlignRightBtn.Click  += (_, _) => Apply(HAlign(ReoGridHorAlign.Right));

        MergeBtn.Click   += (_, _) => { try { Sheet.MergeRange(Sel); } catch (System.Exception ex) { MessageBox.Show(ex.Message); } };
        UnmergeBtn.Click += (_, _) => { try { Sheet.UnmergeRange(Sel); } catch (System.Exception ex) { MessageBox.Show(ex.Message); } };

        FmtNumberBtn.Click   += (_, _) => Sheet.SetRangeDataFormat(Sel, CellDataFormatFlag.Number,
            new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 2, UseSeparator = true });
        FmtCurrencyBtn.Click += (_, _) => Sheet.SetRangeDataFormat(Sel, CellDataFormatFlag.Currency,
            new CurrencyDataFormatter.CurrencyFormatArgs { DecimalPlaces = 2, PrefixSymbol = "﷼ ", CultureEnglishName = "en-US" });
        FmtPercentBtn.Click  += (_, _) => Sheet.SetRangeDataFormat(Sel, CellDataFormatFlag.Percent,
            new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 0 });
        FmtDateBtn.Click     += (_, _) => Sheet.SetRangeDataFormat(Sel, CellDataFormatFlag.DateTime,
            new DateTimeDataFormatter.DateTimeFormatArgs { Format = "yyyy-MM-dd", CultureName = "en-US" });

        BorderAllBtn.Click     += (_, _) => Sheet.SetRangeBorders(Sel, BorderPositions.All,
            new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.Solid });
        BorderOutlineBtn.Click += (_, _) => Sheet.SetRangeBorders(Sel, BorderPositions.Outside,
            new RangeBorderStyle { Color = SolidColor.Black, Style = BorderLineStyle.Solid });
        BorderNoneBtn.Click    += (_, _) => Sheet.RemoveRangeBorders(Sel, BorderPositions.All);

        InsRowBtn.Click += (_, _) => Sheet.InsertRows(Sel.Row, 1);
        InsColBtn.Click += (_, _) => Sheet.InsertColumns(Sel.Col, 1);
        DelRowBtn.Click += (_, _) => Sheet.DeleteRows(Sel.Row, 1);
        DelColBtn.Click += (_, _) => Sheet.DeleteColumns(Sel.Col, 1);

        RtlButton.Click += (_, _) => ToggleRtl();
    }

    private void Apply(WorksheetRangeStyle style) => Sheet.SetRangeStyles(Sel, style);

    private static WorksheetRangeStyle HAlign(ReoGridHorAlign a) =>
        new() { Flag = PlainStyleFlag.HorizontalAlign, HAlign = a };

    private static SolidColor? PickColor()
    {
        using var dlg = new System.Windows.Forms.ColorDialog { FullOpen = true };
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            var c = dlg.Color;
            return SolidColor.FromArgb(c.A, c.R, c.G, c.B);
        }
        return null;
    }

    private void NewFile()
    {
        _grid.Reset();
        Sheet.Name = "Sheet1";
    }

    private void OpenXlsx()
    {
        var dlg = new OpenFileDialog { Filter = "Excel (*.xlsx)|*.xlsx" };
        if (dlg.ShowDialog() == true)
            try { _grid.Load(dlg.FileName, FileFormat.Excel2007); }
            catch (System.Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void SaveXlsx()
    {
        var dlg = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = "workbook.xlsx" };
        if (dlg.ShowDialog() == true)
            try { _grid.Save(dlg.FileName, FileFormat.Excel2007); }
            catch (System.Exception ex) { MessageBox.Show(ex.Message); }
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
        _grid.Reset();
        var s = Sheet;
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
        }
    }

    private void ToggleRtl()
    {
        FlowDirection = FlowDirection == FlowDirection.LeftToRight
            ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    }
}
