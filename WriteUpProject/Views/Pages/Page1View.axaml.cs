using Avalonia.Controls;
using Avalonia.Media;
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

        private void OnDerivationPathChanged(object? sender, TextChangedEventArgs e)
        {
            string path = DerivationPathBox.Text?.Trim() ?? "";
            if (ValidatorService.ValidateDerivationPath(path) && path != string.Empty)
            {
                DerivationPathValidator.Text = "✅ Valid Path";
                DerivationPathValidator.Foreground = Brushes.Green;
            } 
            else
            {
                DerivationPathValidator.Text = "⚠️ Invalid Path";
                DerivationPathValidator.Foreground = Brushes.Red;
            }
        }

        private void OnFingerprintChanged(object? sender, TextChangedEventArgs e) 
        {
            string fingerprint = FingerPrintBox.Text?.Trim() ?? "";
            if (ValidatorService.ValidateFingerprint(fingerprint))
            {
                FingerprintValidator.Text = "✅ Valid Fingerprint";
                FingerprintValidator.Foreground = Brushes.Green;
            }
            else
            {
                FingerprintValidator.Text = "⚠️ Invalid Fingerprint";
                FingerprintValidator.Foreground = Brushes.Red;
            }
        }
    }
}
