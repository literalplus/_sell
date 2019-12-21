using System.Windows;

namespace _Sell
{
    /// <summary>
    /// Interaktionslogik für Tools.xaml
    /// </summary>
    public partial class Tools : Window
    {
        public Tools()
        {
            InitializeComponent();
        }

        private void btnRechnungsFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(_SellInfo.RechnungenPath);

        }

        private void btnMainDir_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(_SellInfo.MainPath);
        }
    }
}
