namespace SovereignGrid.Core.Assets;

public sealed class Asset
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string AssetTag { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public AssetStatus Status { get; set; } = AssetStatus.Pending;
}
