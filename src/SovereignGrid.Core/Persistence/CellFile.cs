namespace SovereignGrid.Core.Persistence;

public sealed class CellFile
{
    public int Row { get; set; }

    public int Column { get; set; }

    public string Value { get; set; }
        = "";
}
