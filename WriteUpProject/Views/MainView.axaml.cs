using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using NBitcoin;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WriteUpProject.Crypto;
using WriteUpProject.FileSaveHelper;

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
        string changeAddressStr = ChangeAddressBox.Text ?? "";
        BitcoinAddress changeAddress = Helper.GetAddressFromString(changeAddressStr, network);

        int fee = 100;
        if (int.TryParse(FeeBox.Text, out var userFee))
        {
            fee = userFee;
        }

        Transaction = Helper.BuildTx(network, messageBytes, fundingTxID, vout, amountSats, changeAddress, fee);
        ResultHexBox.Text = Transaction?.GetGlobalTransaction().ToHex() ?? string.Empty;
        if(!string.IsNullOrEmpty(ResultHexBox.Text))
        {
            ResultHexPanel.IsVisible = true;
        }
    }

    private void ResetForm(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MessageBox.Text = "";
        TxIdBox.Text = "";
        VoutBox.Text = "";
        AmountBox.Text = "";
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

    public PSBT? Transaction { get; set; }
}
