namespace SovereignGrid.Core.Workbook;

public readonly record struct CellRange(
    CellAddress Start,
    CellAddress End
);
