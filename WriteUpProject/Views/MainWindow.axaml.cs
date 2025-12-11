using Avalonia.Controls;
using WriteUpProject.Services;
using WriteUpProject.ViewModels;

namespace WriteUpProject.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var fileSaver = new DialogService(this.StorageProvider);
        DataContext = new MainViewModel(fileSaver);
    }
}
