using Avalonia.Controls;
using Avalonia.Media;
using NBitcoin;


namespace WriteUpProject.Views.Pages
{

    public partial class Page1View : UserControl
    {
        public Page1View()
        {
            InitializeComponent();
        }

        private void OnTxIdChanged(object? sender, TextChangedEventArgs e)
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
    }

}
