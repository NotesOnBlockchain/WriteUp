using NBitcoin;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WriteUpProject.Models;
using WriteUpProject.Navigation;
using WriteUpProject.Services;

namespace WriteUpProject.ViewModels.Pages
{
    public class Page1ViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly DialogService _dialogService;
        private string _selectedNetwork = "Main";
        private string _fundingTxID;
        private string _vout;
        private string _amountValue;

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

        public string FundingTxId
        {
            get => _fundingTxID;
            set => SetProperty(ref _fundingTxID, value);
        }

        public string Vout 
        {
            get => _vout;
            set => SetProperty(ref _vout, value);
        }

        public string AmountBox 
        {
            get => _amountValue;
            set => SetProperty(ref _amountValue, value);
        }



        public ICommand NavigateToPage2Command { get; }

        public Page1ViewModel(NavigationService navigationService, DialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            NavigateToPage2Command = ReactiveCommand.Create(NavigateToPage2);
        }

        private void NavigateToPage2()
        {
            /* check for missing info */
            if(!IsValid())
            {
                return;
            }

            Network network = Network.GetNetwork(SelectedNetwork) ?? throw new Exception("Invalid Network.");

            _navigationService.NavigateTo(new Page2ViewModel(_navigationService, _dialogService, new FundingTxInfo(network, FundingTxId, Vout, AmountBox)));
        }

        private bool IsValid()
        {
            return ValidatorService.ValidateTxID(FundingTxId) && Vout is not null && AmountBox is not null;

        }
    }
}
