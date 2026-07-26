using System.Windows;
using System.Windows.Controls;

namespace SovereignGrid.App.Services;

public static class CurrencyPickerDialog
{
    // symbol shown as prefix. Default = system region currency.
    public static readonly (string Code, string Symbol)[] Currencies =
    {
        ("USD", "$"), ("EUR", "€"), ("GBP", "£"), ("JPY", "¥"),
        ("SAR", "SAR "), ("AED", "AED "), ("KWD", "KWD "), ("QAR", "QAR "),
        ("BHD", "BHD "), ("OMR", "OMR "), ("EGP", "EGP "), ("CHF", "CHF "),
        ("CAD", "C$"), ("AUD", "A$"), ("CNY", "¥"), ("INR", "₹")
    };

    public static string SystemDefaultCode()
    {
        try { return new System.Globalization.RegionInfo(
            System.Globalization.CultureInfo.CurrentCulture.Name).ISOCurrencySymbol; }
        catch { return "USD"; }
    }

    // returns chosen prefix symbol, or null if cancelled
    public static string? Pick()
    {
        var win = new Window
        {
            Title = "Currency", Width = 300, Height = 160,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize
        };

        var panel = new StackPanel { Margin = new Thickness(16) };
        panel.Children.Add(new TextBlock { Text = "Select currency:", Margin = new Thickness(0,0,0,8) });

        var combo = new ComboBox { Margin = new Thickness(0,0,0,12) };
        foreach (var (code, sym) in Currencies) combo.Items.Add($"{code}  ({sym.Trim()})");
        var def = SystemDefaultCode();
        int idx = System.Array.FindIndex(Currencies, x => x.Code == def);
        combo.SelectedIndex = idx >= 0 ? idx : 0;
        panel.Children.Add(combo);

        var ok = new Button { Content = "Apply", Width = 90, HorizontalAlignment = HorizontalAlignment.Right,
                              Padding = new Thickness(6,4,6,4), IsDefault = true };
        panel.Children.Add(ok);
        win.Content = panel;

        string? result = null;
        ok.Click += (_, _) => { result = Currencies[combo.SelectedIndex].Symbol; win.Close(); };
        win.ShowDialog();
        return result;
    }
}
