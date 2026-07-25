namespace SovereignGrid.Core.Workbook;

public readonly record struct CellAddress(
    int Row,
    int Column
);
