using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using NotesSearcher.ViewModels;

namespace NotesSearcher.Views;
public partial class AboutWindow : Window
{
    public AboutWindow(AboutViewModel viewModel)
    {
        var windowInteropHelper = new WindowInteropHelper(this)
        {
            Owner = Process.GetCurrentProcess().MainWindowHandle
        };
        InitializeComponent();
        DataContext = viewModel;
    }
}
