using System.Collections.ObjectModel;
using SovereignGrid.Core.Workbook;

namespace SovereignGrid.App.ViewModels;

public sealed class ShellViewModel
{
    public Workbook Workbook { get; }

    public Worksheet Worksheet { get; }

    public ObservableCollection<SpreadsheetRow> Rows { get; }

    public ShellViewModel()
    {
        Workbook = new Workbook();

        Worksheet =
            Workbook.AddWorksheet("Sheet1");

        Rows = [];

        for (int i = 0; i < 100; i++)
        {
            Rows.Add(
                new SpreadsheetRow(
                    Worksheet,
                    i + 1));
        }
    }
}
