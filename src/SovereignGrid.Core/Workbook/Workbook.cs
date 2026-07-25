using System.Collections.Generic;

namespace SovereignGrid.Core.Workbook;

public sealed class Workbook
{
    private readonly List<Worksheet> _worksheets = [];

    public IReadOnlyList<Worksheet> Worksheets => _worksheets;

    public Worksheet AddWorksheet(string name)
    {
        var worksheet = new Worksheet(name);

        _worksheets.Add(worksheet);

        return worksheet;
    }
}
