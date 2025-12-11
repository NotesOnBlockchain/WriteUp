using NBitcoin;
using ReactiveUI;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using WriteUpProject.Crypto;
using WriteUpProject.Models;
using WriteUpProject.Navigation;
using WriteUpProject.Services;

namespace WriteUpProject.ViewModels.Pages
{
    public class Page2ViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private FundingTxInfo _receivedData;
        private readonly DialogService _dialogService;
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

        public Page2ViewModel(NavigationService navigationService, DialogService dialogService, FundingTxInfo dataFromPage1)
        {
            _navigationService = navigationService;
            _receivedData = dataFromPage1;
            _dialogService = dialogService;

            SavePSBT = ReactiveCommand.CreateFromTask(SavePSBTtoDrive);
            NavigateBackCommand = ReactiveCommand.Create(_navigationService.NavigateBack);
        }

        private async Task SavePSBTtoDrive()
        {
            OutputSideTxInfo outputSideTxInfo = new(ChangeAddress, FeeRate, CustomMessage);
            PSBT psbt = Helper.BuildTx(_receivedData, outputSideTxInfo);

            await _dialogService.ExportTransactionAsBinary(psbt);
        }
    }
}
