using unvell.ReoGrid;
using unvell.ReoGrid.Graphics;

namespace SovereignGrid.App.Services;

public static class GridTheme
{
    private static readonly SolidColor HeaderBg = SolidColor.FromArgb(255, 30, 58, 95);
    private static readonly SolidColor HeaderFg = SolidColor.White;
    private static readonly SolidColor GridLine = SolidColor.FromArgb(255, 200, 208, 218);
    private static readonly SolidColor ZebraBg  = SolidColor.FromArgb(255, 245, 248, 252);

    public static void ApplyHeader(Worksheet sheet, string range)
    {
        sheet.SetRangeStyles(range, new WorksheetRangeStyle
        {
            Flag = PlainStyleFlag.BackColor | PlainStyleFlag.TextColor
                 | PlainStyleFlag.FontStyleBold | PlainStyleFlag.FontSize
                 | PlainStyleFlag.FontName | PlainStyleFlag.HorizontalAlign
                 | PlainStyleFlag.VerticalAlign,
            BackColor = HeaderBg,
            TextColor = HeaderFg,
            Bold = true,
            FontSize = 12,
            FontName = "Segoe UI",
            HAlign = ReoGridHorAlign.Center,
            VAlign = ReoGridVerAlign.Middle
        });
        if (sheet.RowCount > 0) sheet.SetRowsHeight(0, 1, 30);
    }

    public static void ApplyBody(Worksheet sheet, int rows, int cols)
    {
        if (rows < 2 || cols < 1) return;

        string full = $"A1:{new CellPosition(rows - 1, cols - 1).ToAddress()}";

        sheet.SetRangeStyles($"A2:{new CellPosition(rows - 1, cols - 1).ToAddress()}",
            new WorksheetRangeStyle
            {
                Flag = PlainStyleFlag.FontName | PlainStyleFlag.FontSize,
                FontName = "Segoe UI",
                FontSize = 11
            });

        sheet.SetRangeBorders(new RangePosition(full), BorderPositions.All,
            new RangeBorderStyle { Color = GridLine, Style = BorderLineStyle.Solid });

        for (int r = 2; r < rows; r += 2)
        {
            sheet.SetRangeStyles($"A{r + 1}:{new CellPosition(r, cols - 1).ToAddress()}",
                new WorksheetRangeStyle { Flag = PlainStyleFlag.BackColor, BackColor = ZebraBg });
        }
    }

    public static void ApplyRtlHint(Worksheet sheet, int rows, int cols)
    {
        if (rows < 1 || cols < 1) return;
        sheet.SetRangeStyles($"A1:{new CellPosition(rows - 1, cols - 1).ToAddress()}",
            new WorksheetRangeStyle
            {
                Flag = PlainStyleFlag.HorizontalAlign,
                HAlign = ReoGridHorAlign.Right
            });
    }
}
