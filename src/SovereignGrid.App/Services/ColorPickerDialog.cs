using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using unvell.ReoGrid.Graphics;

namespace SovereignGrid.App.Services;

public static class ColorPickerDialog
{
    private static readonly string[] Palette =
    {
        "#000000","#404040","#808080","#C0C0C0","#FFFFFF",
        "#1E3A5F","#2E75B6","#00B0F0","#00B050","#92D050",
        "#FFC000","#FF6600","#C00000","#FF0000","#7030A0",
        "#F2F2F2","#DDEBF7","#E2EFDA","#FFF2CC","#FCE4D6"
    };

    public static SolidColor? Pick()
    {
        var win = new Window
        {
            Title = "Color", Width = 260, Height = 240,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize
        };

        var wrap = new WrapPanel { Margin = new Thickness(10) };
        SolidColor? chosen = null;

        foreach (var hex in Palette)
        {
            var mc = (Color)ColorConverter.ConvertFromString(hex);
            var btn = new Button
            {
                Width = 38, Height = 38, Margin = new Thickness(3),
                Background = new SolidColorBrush(mc),
                BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1)
            };
            btn.Click += (_, _) =>
            {
                chosen = SolidColor.FromArgb(mc.A, mc.R, mc.G, mc.B);
                win.Close();
            };
            wrap.Children.Add(btn);
        }

        win.Content = wrap;
        win.ShowDialog();
        return chosen;
    }
}
