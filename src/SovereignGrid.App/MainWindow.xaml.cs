using System.Windows;
using Microsoft.Win32;
using unvell.ReoGrid;
using unvell.ReoGrid.IO;

namespace SovereignGrid.App;

public partial class MainWindow : Window
{
    private readonly ReoGridControl _grid;

    public MainWindow()
    {
        InitializeComponent();

        _grid = new ReoGridControl();
        RootGrid.Children.Add(_grid);

        var sheet = _grid.CurrentWorksheet;
        sheet.Name = "Sheet1";
        sheet["A1"] = "SovereignGrid";
        sheet["A2"] = "Local First / Offline / Zero Telemetry";
        sheet["A4"] = "Sum demo:";
        sheet["B4"] = 10;
        sheet["B5"] = 20;
        sheet["B6"] = "=SUM(B4:B5)";   // محرك الصيغة الحقيقي

        OpenButton.Click += (_, _) => OpenXlsx();
        SaveButton.Click += (_, _) => SaveXlsx();
        RtlButton.Click  += (_, _) => ToggleRtl();
    }

    private void OpenXlsx()
    {
        var dlg = new OpenFileDialog { Filter = "Excel (*.xlsx)|*.xlsx" };
        if (dlg.ShowDialog() == true)
        {
            try { _grid.Load(dlg.FileName, FileFormat.Excel2007); }
            catch (System.Exception ex) { MessageBox.Show(ex.Message); }
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
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;
    }
}
