using SovereignGrid.Core.Workbook;
using SovereignGrid.Core.Assets;

namespace SovereignGrid.App.ViewModels;

public sealed class ShellViewModel
{
    public Workbook Workbook { get; }

    public Worksheet Worksheet { get; }

    public Asset DemoAsset { get; }

    public string ProductName => "SovereignGrid";

    public string Status => "Asset Workspace Ready";

    public string Mode => "Local First";

    public string Version => "0.3.0-dev";

    public ShellViewModel()
    {
        Workbook = new Workbook();

        Worksheet = Workbook.AddWorksheet("Sheet1");

        Worksheet
            .GetCell(new CellAddress(1,1))
            .Value = "SovereignGrid";

        DemoAsset = new Asset
        {
            AssetTag = "DEMO-0001",
            Description = "Example Asset",
            Location = "Building A",
            Status = AssetStatus.Active
        };
    }
}
