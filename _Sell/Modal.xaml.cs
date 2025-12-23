using System.Windows;

namespace _Sell
{
    /// <summary>
    /// Interaktionslogik für Modal.xaml
    /// </summary>
    public partial class Modal : Window
    {
        private bool _isOk = false;
        public Modal(string text)
        {
            InitializeComponent();
            tbloCaption.Text = text;
        }
        public Modal(string text, string title)
        {
            InitializeComponent();
            tbloCaption.Text = text;
            this.Title = title;
        }

        private ModalReturn ShowAndGet()
        {
            this.ShowDialog();
            return new ModalReturn(txtInput.Text, this._isOk);
        }

        public static ModalReturn StaticShow(string text, string title = "Frage")
        {
            var tempMdl = new Modal(text, title);
            return tempMdl.ShowAndGet();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this._isOk = true;
            this.Close();
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            this._isOk = false;
            this.Close();
        }
    }

    public class ModalReturn
    {
        public string Input;
        //public MessageBoxResult Status;
        public bool IsOk;
        public ModalReturn(string input, bool isOk)
        {
            this.Input = input;
            this.IsOk = isOk;
        }
    }
}
