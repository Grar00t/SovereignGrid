using System.Collections.Generic;

namespace SovereignGrid.Core.Assets;

public sealed class AssetRepository
{
    private readonly List<Asset> _assets = [];

    public IReadOnlyList<Asset> Assets => _assets;

    public void Add(Asset asset)
    {
        _assets.Add(asset);
    }
}
