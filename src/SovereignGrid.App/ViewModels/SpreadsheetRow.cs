using System.ComponentModel;
using SovereignGrid.Core.Workbook;

namespace SovereignGrid.App.ViewModels;

public sealed class SpreadsheetRow :
    INotifyPropertyChanged
{
    private readonly Worksheet _worksheet;

    private readonly int _row;

    public SpreadsheetRow(
        Worksheet worksheet,
        int row)
    {
        _worksheet = worksheet;
        _row = row;
    }

    private string GetValue(
        int column)
    {
        return _worksheet
            .GetCell(
                new CellAddress(
                    _row,
                    column))
            .Value;
    }

    private void SetValue(
        int column,
        string value)
    {
        _worksheet
            .SetValue(
                _row,
                column,
                value);

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(null));
    }

    public string A { get => GetValue(1); set => SetValue(1,value); }
    public string B { get => GetValue(2); set => SetValue(2,value); }
    public string C { get => GetValue(3); set => SetValue(3,value); }
    public string D { get => GetValue(4); set => SetValue(4,value); }
    public string E { get => GetValue(5); set => SetValue(5,value); }
    public string F { get => GetValue(6); set => SetValue(6,value); }
    public string G { get => GetValue(7); set => SetValue(7,value); }
    public string H { get => GetValue(8); set => SetValue(8,value); }
    public string I { get => GetValue(9); set => SetValue(9,value); }
    public string J { get => GetValue(10); set => SetValue(10,value); }

    public event PropertyChangedEventHandler?
        PropertyChanged;
}
