using Avalonia.Controls;
using NBitcoin;
using WriteUpProject.ViewModels;

namespace WriteUpProject.Views;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
    /*
    private void GenerateTXHex(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int networkOptionIndex = NetworkCombobox.SelectedIndex;
        Network network = Helper.SupportedNetworks[networkOptionIndex];

        string message = MessageBox.Text ?? "";
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);

        string fundingTxID = TxIdBox.Text ?? "";
        uint vout = uint.Parse(VoutBox.Text ?? "");
        int amountSats = int.Parse(AmountBox.Text ?? "");
        string changeAddressStr = ChangeAddressBox.Text ?? "";
        BitcoinAddress changeAddress = Helper.GetAddressFromString(changeAddressStr, network);

        FeeRate feeRate = new FeeRate((Int64)2000);
        if (int.TryParse(FeeBox.Text, out int userFee))
        {
            feeRate = new FeeRate((long)userFee * 1000);
        }

        Transaction = Helper.BuildTx(network, messageBytes, fundingTxID, vout, amountSats, changeAddress, feeRate);
        ResultHexBox.Text = Transaction?.GetGlobalTransaction().ToHex() ?? string.Empty;
        if(!string.IsNullOrEmpty(ResultHexBox.Text))
        {
            ResultHexPanel.IsVisible = true;
        }
    }


    public async void SavePSBT(object? sender, RoutedEventArgs args)
    {
        if (Transaction is null)
        {
            return;
        }

        try
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null) 
            {
                return;
            }
            await FileSaveHelper.FileSaveHelper.ExportTransactionAsBinary(topLevel.StorageProvider, Transaction);
        }
        catch (Exception ex)
        {
            ResultHexBox.Text = ex.Message;
        }
        
    }
    */

    public PSBT? Transaction { get; set; }
}
