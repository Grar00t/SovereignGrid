using System.Collections.Generic;

namespace SovereignGrid.Core.Workbook;

public sealed class Worksheet
{
    public string Name { get; set; }

    private readonly Dictionary<CellAddress, Cell> _cells = [];

    public IReadOnlyDictionary<CellAddress, Cell>
        Cells => _cells;

    public Worksheet(string name)
    {
        Name = name;
    }

    public Cell GetCell(CellAddress address)
    {
        if (!_cells.TryGetValue(address, out var cell))
        {
            cell = new Cell
            {
                Row = address.Row,
                Column = address.Column
            };

            _cells[address] = cell;
        }

        return cell;
    }

    public void SetValue(
        int row,
        int column,
        string value)
    {
        GetCell(
            new CellAddress(row, column))
            .Value = value;
    }
}
