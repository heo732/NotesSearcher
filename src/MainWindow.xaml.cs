using System.Windows;
using NotesSearcher.ViewModels;

namespace NotesSearcher;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
