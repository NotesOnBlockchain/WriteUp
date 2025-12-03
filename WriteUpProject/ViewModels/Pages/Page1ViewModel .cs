using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WriteUpProject.Navigation;

namespace WriteUpProject.ViewModels.Pages
{
    public class Page1ViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private string _selectedNetwork = "Main";

        public List<string> Networks { get; } = new()
        {
            "Main",
            "Testnet",
            "Testnet4",
            "RegTest"
        };

        public string SelectedNetwork
        {
            get => _selectedNetwork;
            set => SetProperty(ref _selectedNetwork, value);
        }


        public ICommand NavigateToPage2Command { get; }

        public Page1ViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateToPage2Command = ReactiveCommand.Create(NavigateToPage2);
        }

        private void NavigateToPage2()
        {
            _navigationService.NavigateTo(new Page2ViewModel(_navigationService, "SharedData"));
        }
    }
}
