using NBitcoin;
using ReactiveUI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WriteUpProject.Models;
using WriteUpProject.Navigation;
using WriteUpProject.Services;

namespace WriteUpProject.ViewModels.Pages
{
    public class TxPreviewPageViewModel : ViewModelBase
    {
        private NavigationService _navigationService;
        private DialogService _dialogService;
        private PSBT _psbt;
        private Transaction _transaction;
        private Network _network;
        private BitcoinAddress _changeAddress;
        private string _message;
        private int _fee;

        // To calculate fee
        private FundingTxInfo _fundingTxInfo;

        public TxPreviewPageViewModel(NavigationService navigationService, DialogService dialogService, PSBT psbt, FundingTxInfo fundingTxInfo)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _psbt = psbt;
            _transaction = psbt.GetGlobalTransaction();
            _network = fundingTxInfo.Network;
            _fundingTxInfo = fundingTxInfo;
            _changeAddress = GetChangeAddress(_transaction);
            _message = GetMessageFromTx(_transaction);
            _fee = GetFeePaid();
            SavePSBT = ReactiveCommand.CreateFromTask(SavePSBTtoDrive);
            NavigateBackCommand = ReactiveCommand.Create(_navigationService.NavigateBack);
        }

        public PSBT Psbt 
        {
            get => _psbt;
        }

        public Transaction Transaction 
        {
            get => _transaction;
        }
        public int InputCount => Transaction.Inputs.Count;
        public int OutputCount => Transaction.Outputs.Count;
        public BitcoinAddress ChangeAddress 
        {
            get => _changeAddress;
        }
        public string Message { get => _message; }
        public int Fee { get => _fee; }

        public ICommand NavigateBackCommand { get; }

        public ICommand SavePSBT { get; }

        private async Task SavePSBTtoDrive()
        {
            await _dialogService.ExportTransactionAsBinary(Psbt);
        }
        private string GetMessageFromTx(Transaction transaction)
        {
            return Crypto.Helper.ExtractMessageFromOutput(transaction.Outputs.FirstOrDefault(output => output.Value == Money.Zero) ?? throw new Exception("Couldn't find OP RETURN message output with zero value."));
        }
        private int GetFeePaid()
        {
            return Crypto.Helper.ExtractFee(Transaction, _fundingTxInfo);
        }

        private BitcoinAddress GetChangeAddress(Transaction transaction)
        {
            Script script = Crypto.Helper.GetChangeAddress(transaction) ?? throw new System.Exception("Couldn't get change address from Transaction.");
            return script.GetDestinationAddress(_network) ?? throw new System.Exception("Couldn't convert script to Destination address");
        }
    }
}
