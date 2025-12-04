using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WriteUpProject.Models;
using WriteUpProject.Navigation;

namespace WriteUpProject.ViewModels.Pages
{
    public class Page2ViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private FundingTxInfo _receivedData;
        private string _page2Data = "Data from Page 2";

        public FundingTxInfo ReceivedData
        {
            get => _receivedData;
            set => SetProperty(ref _receivedData, value);
        }

        public string Page2Data
        {
            get => _page2Data;
            set => SetProperty(ref _page2Data, value);
        }

        public ICommand NavigateToPage3Command { get; }
        public ICommand NavigateBackCommand { get; }

        public Page2ViewModel(NavigationService navigationService, FundingTxInfo dataFromPage1)
        {
            _navigationService = navigationService;
            ReceivedData = dataFromPage1;

            NavigateToPage3Command = ReactiveCommand.Create(NavigateToPage3);
            NavigateBackCommand = ReactiveCommand.Create(_navigationService.NavigateBack);
        }

        private void NavigateToPage3()
        {
        }
    }
}
