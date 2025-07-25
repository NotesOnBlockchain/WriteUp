using Avalonia.Controls;
using Avalonia.Media;
using NBitcoin;
using System.Text;
using WriteUpProject.Crypto;

namespace WriteUpProject.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void GenerateTXHex(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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

    private void ResetForm(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MessageBox.Text = "";
        TxIdBox.Text = "";
        VoutBox.Text = "";
        AmountBox.Text = "";
        FundAddressBox.Text = "";
        ChangeAddressBox.Text = "";
        FeeBox.Text = "";
    }

    public void OnMessageChanged(object? sender, TextChangedEventArgs e)
    {
        string msg = MessageBox.Text ?? "";
        int byteLength = Encoding.UTF8.GetBytes(msg).Length;

        if (byteLength > 80)
        {
            MessageByteCounter.Text = $"⚠️ Too long: {byteLength}/80 bytes";
            MessageByteCounter.Foreground = Brushes.Red;
        }
        else
        {
            MessageByteCounter.Text = $"🧮 {byteLength}/80 bytes";
            MessageByteCounter.Foreground = Brushes.Gray;
        }
    }
    public void OnTxIdChanged(object? sender, TextChangedEventArgs e) 
    {
        string txid = TxIdBox.Text?.Trim() ?? "";
        if (uint256.TryParse(txid, out _))
        {
            TxIdValidator.Text = "✅ Valid TXID";
            TxIdValidator.Foreground = Brushes.Green;
        }
        else
        {
            TxIdValidator.Text = "⚠️ Invalid TXID";
            TxIdValidator.Foreground = Brushes.Red;
        }
    }

    public void FundAddressChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        int networkOptionIndex = NetworkCombobox.SelectedIndex;
        Network network = Helper.SupportedNetworks[networkOptionIndex];

        if (Helper.TryParseAddress(FundAddressBox.Text ?? "", network))
        {
            FundAddressValidator.Text = "✅ Valid Address";
            FundAddressValidator.Foreground = Brushes.Green;
        }
        else
        {
            FundAddressValidator.Text = "⚠️ Invalid Address";
            FundAddressValidator.Foreground = Brushes.Red;
        }
    }

    public void ChangeAddressChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        int networkOptionIndex = NetworkCombobox.SelectedIndex;
        Network network = Helper.SupportedNetworks[networkOptionIndex];

        if (Helper.TryParseAddress(ChangeAddressBox.Text ?? "", network))
        {
            ChangeAddressValidator.Text = "✅ Valid Address";
            ChangeAddressValidator.Foreground = Brushes.Green;
        }
        else
        {
            ChangeAddressValidator.Text = "⚠️ Invalid Address";
            ChangeAddressValidator.Foreground = Brushes.Red;
        }
    }
}
