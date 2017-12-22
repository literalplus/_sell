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
    /// Interaktionslogik für Modal.xaml
    /// </summary>
    public partial class Modal : Window
    {
        private bool isOK = false;
        public Modal(string Text)
        {
            InitializeComponent();
            tbloCaption.Text = Text;
        }
        public Modal(string Text, string Title)
        {
            InitializeComponent();
            tbloCaption.Text = Text;
            this.Title = Title;
        }
        public ModalReturn ShowAndGet()
        {
            this.ShowDialog();
            return new ModalReturn(txtInput.Text,this.isOK);
        }

        public static ModalReturn staticShow(string text, string Title = "Frage")
        {
            Modal tempMdl = new Modal(text, Title);
            return tempMdl.ShowAndGet();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.isOK = true;
            this.Close();
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            this.isOK = false;
            this.Close();
        }
    }

    public class ModalReturn
    {
        public string Input;
        //public MessageBoxResult Status;
        public bool isOK;
        public ModalReturn(string Input, bool isOK)
        {
            this.Input = Input;
            this.isOK = isOK;
        }
    }
}
