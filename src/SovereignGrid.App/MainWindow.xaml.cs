using System.Data;

namespace SovereignGrid.App;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var table = new DataTable();

        for (int c = 0; c < 10; c++)
        {
            table.Columns.Add(
                ((char)('A' + c)).ToString());
        }

        for (int r = 0; r < 20; r++)
        {
            table.Rows.Add(table.NewRow());
        }

        SpreadsheetGrid.ItemsSource =
            table.DefaultView;
    }
}
