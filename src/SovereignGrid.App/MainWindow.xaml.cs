using SovereignGrid.App.ViewModels;

namespace SovereignGrid.App;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext =
            new ShellViewModel();
    }
}
