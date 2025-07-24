using Avalonia.Controls;
using NBitcoin;
using WriteUpProject.Crypto;

namespace WriteUpProject.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int networkOptionIndex = NetworkCombobox.SelectedIndex;
        Network network = Helper.SupportedNetworks[networkOptionIndex];

        string message = MessageBox.Text ?? "";
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);

        if (message is null || !Helper.ValidateMessageBytesLength(messageBytes)) 
        { 
            // show error
        }

        string fundingTxID = TxIdBox.Text ?? "";
        uint vout = uint.Parse(VoutBox.Text ?? "");
        int amountSats = int.Parse(AmountBox.Text ?? "");
        string fundAddressStr = FundAddressBox.Text ?? "";
        BitcoinAddress fundAddress = Helper.GetAddressFromString(fundAddressStr, network);
        string changeAddressStr = ChangeAddressBox.Text ?? "";
        BitcoinAddress changeAddress = Helper.GetAddressFromString(changeAddressStr, network);

        int fee = 100;
        if (int.TryParse(FeeBox.Text, out var userFee)) 
        {
            fee = userFee;
        }

        ResultHexBox.Text = Helper.BuildTx(network, messageBytes, fundingTxID, vout, amountSats, fundAddress, changeAddress, fee);
    }
}
