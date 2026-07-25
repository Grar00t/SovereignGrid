using System.Collections.Generic;

namespace SovereignGrid.Core.Persistence;

public sealed class WorksheetFile
{
    public string Name { get; set; }
        = "";

    public List<CellFile> Cells { get; set; }
        = [];
}
