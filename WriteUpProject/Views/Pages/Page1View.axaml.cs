using Avalonia.Controls;
using Avalonia.Media;
using NBitcoin;
using WriteUpProject.Services;


namespace WriteUpProject.Views.Pages
{

    public partial class Page1View : UserControl
    {
        public Page1View()
        {
            InitializeComponent();
        }

        private void OnTxHexChanged(object? sender, TextChangedEventArgs e)
        {
            string txHex = TxHexBox.Text?.Trim() ?? "";
            if (ValidatorService.ValidateTxHex(txHex))
            {
                TxHexValidator.Text = "✅ Valid Hex";
                TxHexValidator.Foreground = Brushes.Green;
            }
            else
            {
                TxHexValidator.Text = "⚠️ Invalid Hex";
                TxHexValidator.Foreground = Brushes.Red;
            }
        }
    }

}
