using Avalonia.Controls;
using WriteUpProject.Navigation;
using WriteUpProject.Services;
using WriteUpProject.ViewModels.Pages;

namespace WriteUpProject.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private ViewModelBase _currentViewModel;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public MainViewModel(DialogService dialogService)
    {
        _navigationService = new NavigationService();
        _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
        // Start with Page1
        _navigationService.NavigateTo(new Page1ViewModel(_navigationService, dialogService));
    }

    private void OnCurrentViewModelChanged(ViewModelBase viewModel)
    {
        CurrentViewModel = viewModel;
    }
}
