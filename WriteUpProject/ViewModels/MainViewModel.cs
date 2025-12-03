using WriteUpProject.Navigation;
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

    public MainViewModel()
    {
        _navigationService = new NavigationService();
        _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;

        // Start with Page1
        _navigationService.NavigateTo(new Page1ViewModel(_navigationService));
    }

    private void OnCurrentViewModelChanged(ViewModelBase viewModel)
    {
        CurrentViewModel = viewModel;
    }
}
