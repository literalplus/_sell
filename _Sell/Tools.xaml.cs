using System.Windows;

namespace _Sell
{
    /// <summary>
    /// Interaktionslogik für Tools.xaml
    /// </summary>
    public partial class Tools
    {
        public Tools()
        {
            InitializeComponent();
        }

        private void btnRechnungsFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = SellInfo.RechnungenPath,
                UseShellExecute = true
            });
        }

        private void btnMainDir_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = SellInfo.MainPath,
                UseShellExecute = true
            });
        }
    }
}
