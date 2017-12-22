using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
