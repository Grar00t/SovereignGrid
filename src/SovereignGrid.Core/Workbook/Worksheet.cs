using System.Collections.Generic;

namespace SovereignGrid.Core.Workbook;

public sealed class Worksheet
{
    public string Name { get; set; }

    private readonly Dictionary<CellAddress, Cell> _cells = [];

    public Worksheet(string name)
    {
        Name = name;
    }

    public Cell GetCell(CellAddress address)
    {
        if (!_cells.TryGetValue(address, out var cell))
        {
            cell = new Cell();
            _cells[address] = cell;
        }

        return cell;
    }
}
