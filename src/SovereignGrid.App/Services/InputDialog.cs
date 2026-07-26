using System.Windows;
using System.Windows.Controls;

namespace SovereignGrid.App.Services;

public static class InputDialog
{
    public static string? Ask(string title, string label, string initial = "")
    {
        var win = new Window
        {
            Title = title, Width = 340, Height = 160,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize
        };
        var panel = new StackPanel { Margin = new Thickness(14) };
        panel.Children.Add(new TextBlock { Text = label, Margin = new Thickness(0,0,0,8) });
        var tb = new TextBox { Text = initial, Padding = new Thickness(6), Margin = new Thickness(0,0,0,12) };
        panel.Children.Add(tb);
        var ok = new Button { Content = "OK", Width = 90, HorizontalAlignment = HorizontalAlignment.Right,
                              Padding = new Thickness(6,4,6,4), IsDefault = true };
        panel.Children.Add(ok);
        win.Content = panel;
        bool okp = false;
        ok.Click += (_, _) => { okp = true; win.Close(); };
        win.ShowDialog();
        return okp ? tb.Text : null;
    }
}
