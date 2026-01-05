using NBitcoin;
using ReactiveUI;
using System;
using System.Collections.Generic;
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
        private string _fundingTxHex;
        private string _vout;
        private string _xpub;
        private string _derivationPath;
        private string _fingerprint;

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

        public string FundingTxHex
        {
            get => _fundingTxHex;
            set => SetProperty(ref _fundingTxHex, value);
        }

        public string Vout 
        {
            get => _vout;
            set => SetProperty(ref _vout, value);
        }

        public string Xpub
        {
            get => _xpub;
            set => SetProperty(ref _xpub, value);
        }

        public string DerivationPath
        {
            get => _derivationPath;
            set => SetProperty(ref _derivationPath, value);
        }

        public string Fingerprint
        {
            get => _fingerprint;
            set => SetProperty(ref _fingerprint, value);
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

            _navigationService.NavigateTo(new Page2ViewModel(_navigationService, _dialogService, new FundingTxInfo(network, FundingTxHex, Vout, Xpub, DerivationPath, Fingerprint)));
        }

        private bool IsValid()
        {
            return ValidatorService.ValidateTxHex(FundingTxHex) && Vout is not null && Xpub is not null && DerivationPath is not null && Fingerprint is not null;
        }
    }
}
