using Avalonia.Controls;
using Avalonia.Media;
using NBitcoin;
using System.Linq;
using System.Text;
using WriteUpProject.Crypto;
using WriteUpProject.Services;

namespace WriteUpProject.Views.Pages
{
    public partial class Page2View : UserControl
    {
        public Page2View()
        {
            InitializeComponent();
        }

        private void ChangeAddressChanged(object? sender, TextChangedEventArgs e)
        {
            var networks = new[] { Network.Main, Network.TestNet, Network.TestNet4, Network.RegTest };
            var input = ChangeAddressBox.Text ?? string.Empty;

            bool isValid = networks.Any(n => ValidatorService.ValidateChangeAddress(input, n));

            if (isValid)
            {
                AddressValidator.Text = "✅ Valid Address";
                AddressValidator.Foreground = Brushes.Green;
            }
            else
            {
                AddressValidator.Text = "⚠️ Invalid Address";
                AddressValidator.Foreground = Brushes.Red;
            }
        }

        private void OnMessageChanged(object? sender, TextChangedEventArgs e)
        {
            string msg = MessageBox.Text ?? "";
            (bool isValid, int byteLength) result = ValidatorService.ValidateMessage(msg);
            if (result.isValid)
            {
                MessageByteCounter.Text = $"🧮 {result.byteLength}/80 bytes";
                MessageByteCounter.Foreground = Brushes.Gray;
            }
            else
            {
                MessageByteCounter.Text = $"⚠️ Too long: {result.byteLength}/80 bytes";
                MessageByteCounter.Foreground = Brushes.Red;
            }
        }

        private void OnFeeRateChanged(object? sender, TextChangedEventArgs e)
        {
            if(decimal.TryParse(FeeRateBox.Text, out decimal feeRate))
            {
                if (feeRate > 10)
                {
                    FeeRateValidator.Text = "⚠️ Warning: High FeeRate. Please check the average transaction fee, before wasting too much money.";
                    FeeRateValidator.Foreground = Brushes.Gray;
                }
                else
                {
                    FeeRateValidator.Text = string.Empty;
                }
            }
            else
            {
                FeeRateValidator.Text = "⚠️ Invalid FeeRate";
                FeeRateValidator.Foreground = Brushes.Red;
            }
        }
    }
}
