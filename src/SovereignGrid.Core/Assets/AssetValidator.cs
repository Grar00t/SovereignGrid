namespace SovereignGrid.Core.Assets;

public static class AssetValidator
{
    public static bool Validate(
        Asset asset,
        out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(asset.AssetTag))
        {
            error = "Asset tag is required.";
            return false;
        }

        if (asset.AssetTag.Length > 100)
        {
            error = "Asset tag is too long.";
            return false;
        }

        return true;
    }
}
