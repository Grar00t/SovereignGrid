using SovereignGrid.Core.Workbook;

namespace SovereignGrid.App.ViewModels;

public sealed class ShellViewModel
{
    public Workbook Workbook { get; }

    public Worksheet Worksheet { get; }

    public string ProductName => "SovereignGrid";

    public string Status => "Workbook Ready";

    public string Mode => "Local First";

    public string Version => "0.2.0-dev";

    public ShellViewModel()
    {
        Workbook = new Workbook();

        Worksheet = Workbook.AddWorksheet("Sheet1");

        Worksheet
            .GetCell(new CellAddress(1,1))
            .Value = "SovereignGrid";
    }
}
