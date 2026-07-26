using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SovereignGrid.App.Services;

public static class TableStylePicker
{
    public static TableStyle? Pick()
    {
        var win = new Window
        {
            Title = "Format as Table", Width = 300, Height = 260,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize
        };

        var panel = new StackPanel { Margin = new Thickness(14) };
        panel.Children.Add(new TextBlock { Text = "Choose a table style:", Margin = new Thickness(0,0,0,10) });

        TableStyle? chosen = null;

        foreach (var t in TableStyles.All)
        {
            var mc = Color.FromRgb((byte)t.Header.R, (byte)t.Header.G, (byte)t.Header.B);
            var btn = new Button
            {
                Content = t.Name,
                Height = 34, Margin = new Thickness(0,0,0,6),
                Background = new SolidColorBrush(mc),
                Foreground = Brushes.White, FontWeight = FontWeights.SemiBold,
                HorizontalContentAlignment = HorizontalAlignment.Left, Padding = new Thickness(10,0,0,0)
            };
            var captured = t;
            btn.Click += (_, _) => { chosen = captured; win.Close(); };
            panel.Children.Add(btn);
        }

        win.Content = panel;
        win.ShowDialog();
        return chosen;
    }
}
