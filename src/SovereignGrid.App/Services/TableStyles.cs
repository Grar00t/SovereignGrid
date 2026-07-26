using unvell.ReoGrid;
using unvell.ReoGrid.Graphics;

namespace SovereignGrid.App.Services;

public sealed class TableStyle
{
    public string Name = "";
    public SolidColor Header;
    public SolidColor HeaderText;
    public SolidColor Band;
    public SolidColor Border;
}

public static class TableStyles
{
    public static readonly TableStyle[] All =
    {
        new() { Name="Blue",   Header=RGB(30,58,95),   HeaderText=SolidColor.White, Band=RGB(221,235,247), Border=RGB(158,182,206) },
        new() { Name="Green",  Header=RGB(33,97,64),    HeaderText=SolidColor.White, Band=RGB(226,239,218), Border=RGB(169,208,142) },
        new() { Name="Orange", Header=RGB(191,89,0),    HeaderText=SolidColor.White, Band=RGB(252,228,214), Border=RGB(244,177,131) },
        new() { Name="Gray",   Header=RGB(64,64,64),    HeaderText=SolidColor.White, Band=RGB(242,242,242), Border=RGB(191,191,191) },
        new() { Name="Red",    Header=RGB(156,0,6),     HeaderText=SolidColor.White, Band=RGB(255,224,224), Border=RGB(230,150,150) },
    };

    private static SolidColor RGB(int r, int g, int b) => SolidColor.FromArgb(255, r, g, b);

    // Apply a full table style to a range: styled header row + banded rows + borders
    public static void Apply(Worksheet sheet, RangePosition range, TableStyle t)
    {
        if (range.Rows < 1 || range.Cols < 1) return;

        // Header row
        var header = new RangePosition(range.Row, range.Col, 1, range.Cols);
        sheet.SetRangeStyles(header, new WorksheetRangeStyle
        {
            Flag = PlainStyleFlag.BackColor | PlainStyleFlag.TextColor
                 | PlainStyleFlag.FontStyleBold | PlainStyleFlag.HorizontalAlign
                 | PlainStyleFlag.VerticalAlign,
            BackColor = t.Header, TextColor = t.HeaderText, Bold = true,
            HAlign = ReoGridHorAlign.Center, VAlign = ReoGridVerAlign.Middle
        });

        // Banded body rows
        for (int i = 1; i < range.Rows; i++)
        {
            if (i % 2 == 1)
            {
                var band = new RangePosition(range.Row + i, range.Col, 1, range.Cols);
                sheet.SetRangeStyles(band, new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor, BackColor = t.Band
                });
            }
        }

        // Borders around whole table
        sheet.SetRangeBorders(range, BorderPositions.All,
            new RangeBorderStyle { Color = t.Border, Style = BorderLineStyle.Solid });
        sheet.SetRangeBorders(range, BorderPositions.Outside,
            new RangeBorderStyle { Color = t.Header, Style = BorderLineStyle.BoldSolid });
    }
}
