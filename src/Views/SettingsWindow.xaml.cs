using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

using QAHelper.ViewModels;

namespace QAHelper.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel viewModel)
        {
            var windowInteropHelper = new WindowInteropHelper(this)
            {
                Owner = Process.GetCurrentProcess().MainWindowHandle
            };
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}