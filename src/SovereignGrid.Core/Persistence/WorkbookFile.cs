using System.Collections.Generic;

namespace SovereignGrid.Core.Persistence;

public sealed class WorkbookFile
{
    public List<WorksheetFile> Worksheets { get; set; }
        = [];
}
