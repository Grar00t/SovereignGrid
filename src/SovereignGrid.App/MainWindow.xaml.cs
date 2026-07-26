using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SovereignGrid.App.Services;
using SovereignGrid.Connectors.Abstractions;
using unvell.ReoGrid;
using unvell.ReoGrid.Chart;
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
                                      "Consolas","Verdana","Georgia" };
        FontBox.SelectedIndex = 0;
        SizeBox.ItemsSource = new[] { "8","9","10","11","12","14","16","18","20","24","28","36","48","72" };
        SizeBox.SelectedIndex = 3;

        NewFile();
        WireEvents();
        InitZoomTimer();
        HookCellChanges();
        InitAutoSave();
        ThemeBox.ItemsSource = new[] { "Green", "Blue", "Dark" };
        ThemeBox.SelectedIndex = 0;
        ThemeBox.SelectionChanged += (_, _) => ApplyTheme(ThemeBox.SelectedItem as string);
        ThemeBox.ItemsSource = new[] { "Green", "Blue", "Dark" };
        ThemeBox.SelectedIndex = 0;
        ThemeBox.SelectionChanged += (_, _) => ApplyTheme(ThemeBox.SelectedItem as string);
    }

    private void WireEvents()
    {
        NewButton.Click    += (_, _) => NewFile();
        OpenFileBtn.Click  += (_, _) => OpenXlsx();
        OpenButton.Click   += async (_, _) => await OpenViaConnectorAsync();
        SaveButton.Click   += (_, _) => SaveXlsx();
        SaveAsBtn.Click    += (_, _) => SaveXlsx();

        SaveSgBtn.Click += (_, _) => SaveSgrid();
        OpenSgBtn.Click += (_, _) => OpenSgrid();

        ChartColBtn.Click  += (_, _) => InsertChart("column");
        ChartLineBtn.Click += (_, _) => InsertChart("line");


        UndoBtn.Click  += (_, _) => _grid.Undo();
        RedoBtn.Click  += (_, _) => _grid.Redo();
        CutBtn.Click   += (_, _) => { try { Sheet.Cut();   } catch { } };
        CopyBtn.Click  += (_, _) => { try { Sheet.Copy();  } catch { } };
        PasteBtn.Click += (_, _) => { try { Sheet.Paste(); } catch { } };

        FontBox.SelectionChanged += (_, _) => { if (FontBox.SelectedItem is string f)
            Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontName, FontName = f }); };
        SizeBox.SelectionChanged += (_, _) => { if (SizeBox.SelectedItem is string s && float.TryParse(s, out var v))
            Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontSize, FontSize = v }); };

        BoldBtn.Click      += (_, _) => Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontStyleBold, Bold = BoldBtn.IsChecked == true });
        ItalicBtn.Click    += (_, _) => Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontStyleItalic, Italic = ItalicBtn.IsChecked == true });
        UnderlineBtn.Click += (_, _) => Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.FontStyleUnderline, Underline = UnderlineBtn.IsChecked == true });

        TextColorBtn.Click += (_, _) => { var c = ColorPickerDialog.Pick(); if (c.HasValue)
            Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.TextColor, TextColor = c.Value }); };
        FillColorBtn.Click += (_, _) => { var c = ColorPickerDialog.Pick(); if (c.HasValue)
            Apply(new WorksheetRangeStyle { Flag = PlainStyleFlag.BackColor, BackColor = c.Value }); };

        AlignLeftBtn.Click   += (_, _) => Apply(HAlign(ReoGridHorAlign.Left));
        AlignCenterBtn.Click += (_, _) => Apply(HAlign(ReoGridHorAlign.Center));
        AlignRightBtn.Click  += (_, _) => Apply(HAlign(ReoGridHorAlign.Right));

        MergeBtn.Click   += (_, _) => { try { Sheet.MergeRange(Sel); } catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); } };
        UnmergeBtn.Click += (_, _) => { try { Sheet.UnmergeRange(Sel); } catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); } };

        FmtNumberBtn.Click   += (_, _) => Sheet.SetRangeDataFormat(Sel, CellDataFormatFlag.Number,
            new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 2, UseSeparator = true });
        FmtCurrencyBtn.Click += (_, _) => ApplyCurrency();
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

        TableStyleBtn.Click += (_, _) =>
        {
            var style = TableStylePicker.Pick();
            if (style != null) TableStyles.Apply(Sheet, Sel, style);
        };

        FilterBtn.Click   += (_, _) => { try { Sheet.CreateColumnFilter(Sel.Col, Sel.EndCol, Sel.Row); } catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); } };
        FreezeBtn.Click   += (_, _) => { try { Sheet.FreezeToCell(Sel.StartPos); } catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); } };
        UnfreezeBtn.Click += (_, _) => { try { Sheet.Unfreeze(); } catch { } };
        ExportPngBtn.Click += (_, _) => ExportChartPng();

        AddSheetBtn.Click += (_, _) =>
        {
            try
            {
                var ws = _grid.CreateWorksheet("Sheet" + (_grid.Worksheets.Count + 1));
                _grid.AddWorksheet(ws);
                _grid.CurrentWorksheet = ws;
            }
            catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
        };
        DelSheetBtn.Click += (_, _) =>
        {
            try
            {
                if (_grid.Worksheets.Count <= 1) { System.Windows.MessageBox.Show("Cannot delete the only sheet."); return; }
                _grid.RemoveWorksheet(Sheet);
            }
            catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
        };
        RenameSheetBtn.Click += (_, _) =>
        {
            var name = InputDialog.Ask("Rename Sheet", "New name:", Sheet.Name);
            if (!string.IsNullOrWhiteSpace(name))
                try { Sheet.Name = name; } catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
        };

        FindBtn.Click += (_, _) => ShowFindReplace();

        FormulaBox.KeyDown += (_, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                try { Sheet[Sel.Row, Sel.Col] = FormulaBox.Text; } catch { }
            }
        };

        ZoomSlider.ValueChanged += (_, _) =>
        {
            ZoomText.Text = ((int)ZoomSlider.Value) + "%";
            _zoomTimer.Stop();
            _zoomTimer.Start();
        };

        FullScreenBtn.Click += (_, _) => ToggleFullScreen();

        FormulaBox.TextChanged += (_, _) =>
        {
            if (FormulaBox.IsKeyboardFocused)
                try { Sheet[Sel.Row, Sel.Col] = FormulaBox.Text; } catch { }
        };

        _grid.PreviewMouseWheel += (_, e) =>
        {
            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                double step = e.Delta > 0 ? 5 : -5;
                ZoomSlider.Value = System.Math.Clamp(ZoomSlider.Value + step, ZoomSlider.Minimum, ZoomSlider.Maximum);
                e.Handled = true;
            }
        };

        this.KeyDown += (_, e) => { if (e.Key == System.Windows.Input.Key.F11) ToggleFullScreen(); };

        ShapeBtn.Click += (_, _) => InsertShape();

        RtlButton.Click += (_, _) => ToggleRtl();

        Sheet.SelectionRangeChanged += (_, _) => UpdateStatus();

        this.KeyDown += (_, e) =>
        {
            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                if (e.Key == System.Windows.Input.Key.S) SaveXlsx();
                else if (e.Key == System.Windows.Input.Key.O) OpenXlsx();
                else if (e.Key == System.Windows.Input.Key.Z) _grid.Undo();
                else if (e.Key == System.Windows.Input.Key.Y) _grid.Redo();
            }
        };
    }

    private void Apply(WorksheetRangeStyle style) => Sheet.SetRangeStyles(Sel, style);

    private static WorksheetRangeStyle HAlign(ReoGridHorAlign a) =>
        new() { Flag = PlainStyleFlag.HorizontalAlign, HAlign = a };

    private void ApplyCurrency()
    {
        var symbol = CurrencyPickerDialog.Pick();
        if (symbol is null) return;

        try
        {
            Sheet.SetRangeDataFormat(Sel, CellDataFormatFlag.Currency,
                new CurrencyDataFormatter.CurrencyFormatArgs
                {
                    DecimalPlaces = 2,
                    PrefixSymbol = symbol,
                    CultureEnglishName = "en-US"
                });
        }
        catch
        {
            Sheet.SetRangeDataFormat(Sel, CellDataFormatFlag.Number,
                new NumberDataFormatter.NumberFormatArgs { DecimalPlaces = 2, UseSeparator = true });
        }
    }

    private void UpdateStatus()
    {
        try
        {
            CellRefText.Text = Sel.StartPos.ToAddress();
            NameBox.Text = Sel.StartPos.ToAddress();
            var active = Sheet.GetCell(Sel.Row, Sel.Col);
            FormulaBox.Text = active?.Formula is string f && f.Length > 0 ? "=" + f
                              : active?.Data?.ToString() ?? "";

            double sum = 0; int count = 0;
            for (int r = Sel.Row; r <= Sel.EndRow; r++)
                for (int c = Sel.Col; c <= Sel.EndCol; c++)
                {
                    var cell = Sheet.GetCell(r, c);
                    if (cell?.Data == null) continue;
                    count++;
                    if (double.TryParse(cell.Data.ToString(), out var v)) sum += v;
                }

            StatsText.Text = count > 1
                ? $"Count: {count}    Sum: {sum:N2}    Average: {sum / count:N2}"
                : "";
        }
        catch { }
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
            catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
    }

    private void SaveXlsx()
    {
        var dlg = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = "workbook.xlsx" };
        if (dlg.ShowDialog() == true)
            try { _grid.Save(dlg.FileName, FileFormat.Excel2007); }
            catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
    }

    // Native SovereignGrid format (.sgrid) - self-contained, offline, keeps cells+formulas+styles+charts
    private void SaveSgrid()
    {
        var dlg = new SaveFileDialog { Filter = "SovereignGrid (*.sgrid)|*.sgrid", FileName = "workbook.sgrid" };
        if (dlg.ShowDialog() == true)
            try { _grid.Save(dlg.FileName, FileFormat.ReoGridFormat); }
            catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
    }

    private void OpenSgrid()
    {
        var dlg = new OpenFileDialog { Filter = "SovereignGrid (*.sgrid)|*.sgrid" };
        if (dlg.ShowDialog() == true)
            try { _grid.Load(dlg.FileName, FileFormat.ReoGridFormat); }
            catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
    }

    private void InsertChart(string kind)
    {
        try
        {
            if (Sel.Rows < 1 || Sel.Cols < 1)
            {
                System.Windows.MessageBox.Show("Select a data range first.");
                return;
            }

            var dataRange = Sel;
            var categoryRange = new RangePosition(dataRange.Row, dataRange.Col, dataRange.Rows, 1);

            var ds = new WorksheetChartDataSource(Sheet, categoryRange, dataRange);

            Chart chart = kind switch
            {
                "line" => new LineChart(),
                "pie"  => new PieChart(),
                _      => new ColumnChart()
            };

            chart.Location = new unvell.ReoGrid.Graphics.Point(320, 20);
            chart.Size = new unvell.ReoGrid.Graphics.Size(420, 280);
            chart.Title = "Chart";
            chart.DataSource = ds;

            Sheet.FloatingObjects.Add(chart);
        }
        catch (System.Exception ex)
        {
            System.Windows.MessageBox.Show("Chart error: " + ex.Message);
        }
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
            if (!result.Success) { System.Windows.MessageBox.Show(result.Message); return; }
            RenderCoreToGrid(coreSheet);
            System.Windows.MessageBox.Show($"{result.Message}  ({result.RowsAffected} rows)");
        }
        catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
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

    private void ExportChartPng()
    {
        try
        {
            unvell.ReoGrid.Drawing.IDrawingObject? target = null;
            foreach (var obj in Sheet.FloatingObjects)
            {
                if (obj is unvell.ReoGrid.Chart.Chart) { target = obj; break; }
            }
            if (target is null)
            {
                System.Windows.MessageBox.Show("Insert a chart first, then export.");
                return;
            }

            var dlg = new SaveFileDialog { Filter = "PNG Image (*.png)|*.png", FileName = "chart.png" };
            if (dlg.ShowDialog() != true) return;

            int w = (int)target.Size.Width;
            int h = (int)target.Size.Height;
            if (w < 1 || h < 1) { w = 420; h = 280; }

            var rtb = new System.Windows.Media.Imaging.RenderTargetBitmap(
                w, h, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);

            var visual = new System.Windows.Media.DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(System.Windows.Media.Brushes.White, null,
                    new System.Windows.Rect(0, 0, w, h));
            }
            rtb.Render(visual);

            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));
            using var fs = System.IO.File.Create(dlg.FileName);
            encoder.Save(fs);

            System.Windows.MessageBox.Show("Exported: " + dlg.FileName);
        }
        catch (System.Exception ex)
        {
            System.Windows.MessageBox.Show("PNG export error: " + ex.Message);
        }
    }

    private void ShowFindReplace()
    {
        var find = InputDialog.Ask("Find & Replace", "Find what:");
        if (string.IsNullOrEmpty(find)) return;
        var replace = InputDialog.Ask("Find & Replace", "Replace with (leave empty to just find):", "");

        int hits = 0;
        var s = Sheet;
        int rows = s.Rows, cols = s.Columns;
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                var cell = s.GetCell(r, c);
                if (cell?.Data == null) continue;
                var text = cell.Data.ToString();
                if (text != null && text.Contains(find))
                {
                    hits++;
                    if (!string.IsNullOrEmpty(replace))
                        s[r, c] = text.Replace(find, replace);
                }
            }

        System.Windows.MessageBox.Show(
            string.IsNullOrEmpty(replace) ? $"Found {hits} match(es)."
                                          : $"Replaced in {hits} cell(s).");
    }

    private readonly System.Windows.Threading.DispatcherTimer _zoomTimer = new()
        { Interval = System.TimeSpan.FromMilliseconds(120) };

    private void InitZoomTimer()
    {
        _zoomTimer.Tick += (_, _) =>
        {
            _zoomTimer.Stop();
            try { Sheet.SetScale((float)(ZoomSlider.Value / 100.0)); } catch { }
        };
    }

    private void HookCellChanges()
    {
        try
        {
            Sheet.CellDataChanged += (_, ev) =>
            {
                if (!FormulaBox.IsKeyboardFocused)
                {
                    var cell = Sheet.GetCell(Sel.Row, Sel.Col);
                    FormulaBox.Text = cell?.Data?.ToString() ?? "";
                }
            };
        }
        catch { }
    }

    private readonly System.Windows.Threading.DispatcherTimer _autoSaveTimer = new()
        { Interval = System.TimeSpan.FromMinutes(2) };


    private void InitAutoSave()
    {
        _autoSaveTimer.Tick += (_, _) =>
        {
            try
            {
                var dir = System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                    "SovereignGrid", "AutoSave");
                System.IO.Directory.CreateDirectory(dir);
                var path = System.IO.Path.Combine(dir, "autosave.sgrid");
                _grid.Save(path, unvell.ReoGrid.IO.FileFormat.ReoGridFormat);
                CellRefText.Text = "Auto-saved " + System.DateTime.Now.ToString("HH:mm");
            }
            catch { }
        };
        _autoSaveTimer.Start();
    }

    private void ApplyTheme(string? name)
    {
        string bg = name switch { "Blue" => "#1E3A5F", "Dark" => "#2B2B2B", _ => "#217346" };
        var brush = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom(bg)!;
        TitleBar.Background = brush;
        StatusBar.Background = brush;

        // Deep: grid background + selection color via ReoGrid ControlStyle
        try
        {
            var cs = _grid.ControlStyle;
            if (name == "Dark")
            {
                cs.SetColor(unvell.ReoGrid.ControlAppearanceColors.GridBackground,
                    unvell.ReoGrid.Graphics.SolidColor.FromArgb(255, 45, 45, 45));
                cs.SetColor(unvell.ReoGrid.ControlAppearanceColors.GridText,
                    unvell.ReoGrid.Graphics.SolidColor.White);
            }
            else
            {
                cs.SetColor(unvell.ReoGrid.ControlAppearanceColors.GridBackground,
                    unvell.ReoGrid.Graphics.SolidColor.White);
                cs.SetColor(unvell.ReoGrid.ControlAppearanceColors.GridText,
                    unvell.ReoGrid.Graphics.SolidColor.Black);
            }
            _grid.ControlStyle = cs;
        }
        catch { }
    }

    private void InsertShape()
    {
        var kind = InputDialog.Ask("Insert Shape", "Type: rectangle / ellipse / line", "rectangle");
        if (string.IsNullOrWhiteSpace(kind)) return;

        try
        {
            var loc = new unvell.ReoGrid.Graphics.Point(60, 60);
            var size = new unvell.ReoGrid.Graphics.Size(160, 90);
            unvell.ReoGrid.Drawing.IDrawingObject shape = kind.Trim().ToLower() switch
            {
                "ellipse" => new unvell.ReoGrid.Drawing.Shapes.EllipseShape { Location = loc, Size = size },
                "line"    => new unvell.ReoGrid.Drawing.Shapes.Line
                             { StartPoint = loc, EndPoint = new unvell.ReoGrid.Graphics.Point(220, 150) },
                _         => new unvell.ReoGrid.Drawing.Shapes.RectangleShape { Location = loc, Size = size }
            };
            Sheet.FloatingObjects.Add(shape);
        }
        catch (System.Exception ex) { System.Windows.MessageBox.Show("Shape error: " + ex.Message); }
    }


    private bool _isFull = false;
    private WindowStyle _prevStyle;
    private WindowState _prevState;

    private void ToggleFullScreen()
    {
        if (!_isFull)
        {
            _prevStyle = this.WindowStyle;
            _prevState = this.WindowState;
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Normal;   // reset so Maximized re-applies fullscreen
            this.WindowState = WindowState.Maximized;
            _isFull = true;
        }
        else
        {
            this.WindowStyle = _prevStyle;
            this.WindowState = _prevState;
            _isFull = false;
        }
    }

    private void ToggleRtl()
    {
        this.FlowDirection = this.FlowDirection == System.Windows.FlowDirection.LeftToRight
            ? System.Windows.FlowDirection.RightToLeft
            : System.Windows.FlowDirection.LeftToRight;
    }
}



