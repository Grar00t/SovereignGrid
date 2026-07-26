using System.Windows;
using System.Windows.Controls;

namespace SovereignGrid.App.Services;

public static class ConnectionPrompt
{
    // ترجع (connectionString, query) أو null لو أُلغيت. لا تُخزّن أي قيمة.
    public static (string cs, string sql)? Show(string title)
    {
        var win = new Window
        {
            Title = title, Width = 620, Height = 260,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize, FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
        };

        var grid = new Grid { Margin = new Thickness(16) };
        for (int i = 0; i < 4; i++) grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        grid.ColumnDefinitions.Add(new ColumnDefinition());

        var lblCs  = new TextBlock { Text = "Connection", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0,0,8,8) };
        var txtCs  = new TextBox { Margin = new Thickness(0,0,0,8), Padding = new Thickness(6) };
        var lblSql = new TextBlock { Text = "SQL Query", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0,0,8,8) };
        var txtSql = new TextBox { Margin = new Thickness(0,0,0,8), Padding = new Thickness(6),
                                   AcceptsReturn = true, Height = 70, TextWrapping = TextWrapping.Wrap,
                                   Text = "SELECT * FROM " };

        Grid.SetRow(lblCs,0);  Grid.SetColumn(lblCs,0);
        Grid.SetRow(txtCs,0);  Grid.SetColumn(txtCs,1);
        Grid.SetRow(lblSql,1); Grid.SetColumn(lblSql,0);
        Grid.SetRow(txtSql,1); Grid.SetColumn(txtSql,1);

        var panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        var ok = new Button { Content = "Connect", Width = 100, Margin = new Thickness(0,0,8,0), Padding = new Thickness(6,4,6,4), IsDefault = true };
        var cancel = new Button { Content = "Cancel", Width = 100, Padding = new Thickness(6,4,6,4), IsCancel = true };
        panel.Children.Add(ok); panel.Children.Add(cancel);
        Grid.SetRow(panel,2); Grid.SetColumn(panel,1);

        grid.Children.Add(lblCs);  grid.Children.Add(txtCs);
        grid.Children.Add(lblSql); grid.Children.Add(txtSql);
        grid.Children.Add(panel);
        win.Content = grid;

        bool okPressed = false;
        ok.Click += (_, _) => { okPressed = true; win.Close(); };
        win.ShowDialog();

        if (!okPressed || string.IsNullOrWhiteSpace(txtCs.Text) || string.IsNullOrWhiteSpace(txtSql.Text))
            return null;
        return (txtCs.Text.Trim(), txtSql.Text.Trim());
    }
}
