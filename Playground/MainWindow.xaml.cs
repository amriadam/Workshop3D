using Playground.Ogl;
using System.Windows;

namespace Playground;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new ViewportViewModel(
            Dispatcher, 
            new OglGeometryBuilder());
    }

    protected override void OnClosed(EventArgs e)
    {
        DataContext = null;

        base.OnClosed(e);
    }
}
