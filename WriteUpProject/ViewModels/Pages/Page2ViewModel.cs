using ReactiveUI;
using System.Windows.Input;
using WriteUpProject.Models;
using WriteUpProject.Navigation;

namespace WriteUpProject.ViewModels.Pages
{
    public class Page2ViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private FundingTxInfo _receivedData;
        private string _changeAddress;
        private string _feeRate;
        private string _customMessage;

        public string ChangeAddress
        {
            get => _changeAddress;
            set => SetProperty(ref _changeAddress, value);
        }

        public string FeeRate
        {
            get => _feeRate;
            set => SetProperty(ref _feeRate, value);
        }

        public string CustomMessage
        {
            get => _customMessage;
            set => SetProperty(ref _customMessage, value);
        }

        public ICommand SavePSBT { get; }
        public ICommand NavigateBackCommand { get; }

        public Page2ViewModel(NavigationService navigationService, FundingTxInfo dataFromPage1)
        {
            _navigationService = navigationService;
            _receivedData = dataFromPage1;

            SavePSBT = ReactiveCommand.Create(SavePSBTtoDrive);
            NavigateBackCommand = ReactiveCommand.Create(_navigationService.NavigateBack);
        }

        private void SavePSBTtoDrive()
        {
        }
    }
}
