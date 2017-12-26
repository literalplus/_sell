using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using xytools;
using _Hotkey;
using _Sell.Action;
using _Sell.Model;
using _Sell.Service;

namespace _Sell
{
    public delegate void EnterButtonHandler();

    public delegate void KeyEventHandler(KeyEventArgs args);

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //public event NumberReturnsFromOuterSpaceHandler OnNumberReturns;
        public bool isCashLoggingEnabled;

        public bool areDrinksVisible;
        public event EnterButtonHandler OnEnterButton = () => { };
        public bool isMultiplicationWaiting;
        public Button[] NmrBtns;
        public Dictionary<Button, int> dctNmrBtns = new Dictionary<Button, int>();
        public Dictionary<string, int> productAmounts = new Dictionary<string, int>();
        public Dictionary<string, int> productIndexes = new Dictionary<string, int>();
        public List<KeyValuePair<string, int>> productsInOrder = new List<KeyValuePair<string, int>>();
        private Dictionary<Key, KeyEventHandler> localKeyHandlers = new Dictionary<Key, KeyEventHandler>();
        public int intTempSaved;
        public Price[] lastPrices = new Price[0];
        private IProductRegistry _productRegistry = new DefaultProductRegistry();
        private ProductGrid _productGrid;

        public Price Cash
        {
            get { return _cash; }
            set
            {
                _cash = value;
                Display.setSecondLine(value.ToString());
                _SellInfo.cashStand = value.RawValue;
            }
        }

        private Price _cash;

        public Price total
        {
            get { return _total; }
            set
            {
                _total = value;
                tbloTotal.Text = value.ToString();
            }
        }

        private Price _total;

        public MainWindow()
        {
            try
            {
                _SellInfo.MainWindow = this;
                InitializeComponent();
                Display.initDisplay(tbloNmrDisplay, tbloLineOne, tbloLineTwo);
                D.W("Checking cash!");
                checkCashLogging();
                total = new Price(0);
                setStatus("Bereit!");
                NmrBtns = new[]
                    {btnZero, btnOne, btnTwo, btnThree, btnFour, btnFive, btnSix, btnSeven, btnEight, btnNine};
                initNmrBtns();
                _productGrid = new ProductGrid(ProductGrid.DefaultGridMeta, ProductButtonsGrid);
                initFirstPage();
                clearProducts();
            }
            catch (Exception ex)
            {
                D.W("Exception when initializing MainWindow..." + ex.ToString());
                Application.Current.Shutdown();
            }
        }

        private void initFirstPage()
        {
            _productGrid.Add(new GenericGridAction(
                () => btnSonstiges_Click(null, null),
                "Sonstiges", "€ xx,xx"
            ));
            _productGrid.Add(new GenericGridAction(
                () => btnSpende_Click(null, null),
                "Spende", "€ xx,xx"
            ));
            foreach (var product in _productRegistry.Products)
            {
                _productGrid.Add(new ProductGridAction(product, HandleProductButtonClick));
            }
        }

        private void HandleProductButtonClick(Product product)
        {
            AddItemToLists(product.Name, product.Price);
            btnEnter.Focus();
            areDrinksVisible = false;
        }

        public void checkCashLogging()
        {
            if (_SellInfo.cashStand != -1)
            {
                isCashLoggingEnabled = true;
                _cash = new Price(_SellInfo._cashStand);
                Display.setSecondLine(_cash.ToString());
                D.W(_cash.ToString(), "mainwindow cash set to");
                miSetCash.IsEnabled = true;
                miSubtractCash.IsEnabled = true;
            }
            else D.W("cashStand equals -1");
        }

        public void addPriceToList(Price prc)
        {
            try
            {
                Price[] tempPrices = {prc};
                lastPrices = lastPrices.Concat(tempPrices).ToArray();
                D.W(xy_str.listArrayToString(lastPrices));
                if (isCashLoggingEnabled)
                {
                    Cash = prc.Plus(_cash);
                }
            }
            catch (Exception ex)
            {
                D.W(ex.Message, "ex bei addPriceToList()");
            }
            total = _total.Plus(prc);
        }

        public void clearProducts()
        {
            total = new Price(0);
            lbxItems.Items.Clear();
            lbxPrices.Items.Clear();
            lbxItems.Items.Add(String.Format("{0,-40}| {1,-10}| {2,-10}", "Position", "EPreis", "Menge"));
            lbxItems.Items.Add(("".PadLeft(110, '-')));
            lbxPrices.Items.Add("Preis");
            lbxPrices.Items.Add("".PadLeft(30, '-'));
            productAmounts.Clear();
            productIndexes.Clear();
            productsInOrder.Clear();
        }

        public void AddItemToLists(string Name, Price price, bool addToTotal = true)
        {
            int totalAmount = 1;
            if (isMultiplicationWaiting)
            {
                isMultiplicationWaiting = false;
                OnEnterButton -= GetItForMultiplication;
                totalAmount = intTempSaved;
            }
            productsInOrder.Add(new KeyValuePair<string, int>(Name, totalAmount));
            modifyProduct(Name, price, totalAmount);
            if (addToTotal) addPriceToList(price);
            else addPriceToList(new Price(0));
        }

        private void modifyProduct(string Name, Price singlePrice, int modifier)
        {
            bool newProduct = !productAmounts.ContainsKey(Name);
            if (newProduct && modifier < 0)
            {
                throw new ArgumentOutOfRangeException("" + modifier);
            }
            modifier += newProduct ? 0 : productAmounts[Name];
            int index = newProduct ? -1 : productIndexes[Name];
            if (modifier > 0)
            {
                updateProduct(index, Name, singlePrice, modifier);
                productAmounts[Name] = modifier;
            }
            else
            {
                D.W(lbxItems.Items.Count);
                lbxItems.Items.RemoveAt(index);
                lbxPrices.Items.RemoveAt(index);
                productAmounts.Remove(Name);
            }
        }

        private void updateProduct(int index, string Name, Price singlePrice, int amount)
        {
            String itemString =
                String.Format("{0,-40}| {1,-10}| {2,-10}", Name, singlePrice, "x" + amount.ToString("00"));
            String priceString = singlePrice.Times(amount).ToString();
            if (index == -1)
            {
                productIndexes[Name] = lbxItems.Items.Add(itemString);
                lbxPrices.Items.Add(priceString);
            }
            else
            {
                lbxItems.Items[index] = itemString;
                lbxPrices.Items[index] = priceString;
            }
        }

        private void initNmrBtns()
        {
            int i = 0;
            foreach (Button item in NmrBtns)
            {
                dctNmrBtns.Add(item, i);
                i++;
            }
        }

        private void GetItForSonstiges()
        {
            try
            {
                int intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                AddItemToLists("Sonstiger Artikel", new Price(intPrice));
            }
            catch (Exception)
            {
                setStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            Display.setFirstLine("0");
            OnEnterButton -= GetItForSonstiges;
        }

        private void GetItForCash()
        {
            try
            {
                int intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                Cash = new Price(intPrice);
            }
            catch (Exception)
            {
                setStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            Display.setFirstLine("0");
            OnEnterButton -= GetItForCash;
        }

        private void GetItForCashSub()
        {
            try
            {
                int intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                Cash = _cash.Minus(new Price(intPrice));
            }
            catch (Exception)
            {
                setStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            Display.setFirstLine("0");
            OnEnterButton -= GetItForCashSub;
        }

        private void GetItForAddition()
        {
            try
            {
                int intNew = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.setFirstLine((intNew + intTempSaved).ToString());
            }
            catch (Exception)
            {
                setStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            OnEnterButton -= GetItForAddition;
        }

        private void GetItForSubtraction()
        {
            try
            {
                int intNew = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.setFirstLine((intTempSaved - intNew).ToString());
            }
            catch (Exception)
            {
                setStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            OnEnterButton -= GetItForSubtraction;
        }

        private void GetItForMultiplication()
        {
            try
            {
                int intNew = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.setFirstLine((intNew * intTempSaved).ToString());
            }
            catch (Exception)
            {
                setStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            isMultiplicationWaiting = false;
            OnEnterButton -= GetItForMultiplication;
        }

        private void GetItForDivision()
        {
            try
            {
                int intNew = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.setFirstLine((intTempSaved / intNew).ToString());
            }
            catch (Exception)
            {
                setStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            OnEnterButton -= GetItForDivision;
        }

        private void btnSonstiges_Click(object sender, RoutedEventArgs e)
        {
            OnEnterButton += GetItForSonstiges;
            OnEnterButton -= GetItForMultiplication;
        }

        private void GetItForSpenden()
        {
            try
            {
                int intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                AddItemToLists("Spende", new Price(intPrice), false);
            }
            catch (Exception)
            {
                setStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            Display.setFirstLine("0");
            OnEnterButton -= GetItForSpenden;
        }

        private void btnSpende_Click(object sender, RoutedEventArgs e)
        {
            OnEnterButton += GetItForSpenden;
            areDrinksVisible = false;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clearProducts();
            setStatus("Alle Produkte entfernt!");
            areDrinksVisible = false;
        }

        private void btnClearLast_Click(object sender, RoutedEventArgs e)
        {
            if (lbxItems.Items.Count <= 2 || lbxPrices.Items.Count <= 2)
            {
                clearProducts();
                return;
            }
            int lbICount = lbxItems.Items.Count;
            int lb2ICount = lbxPrices.Items.Count;
            if (lbICount != lb2ICount)
            {
                MessageBox.Show("Fehler! Es gibt nicht gleich viele Preise wie Produkte! Leere Liste..");
                btnClear_Click(this, new RoutedEventArgs());
                return;
            }
            if (lbICount <= 0) return;
            KeyValuePair<string, int> entry = productsInOrder[productsInOrder.Count - 1];
            productsInOrder.RemoveAt(productsInOrder.Count - 1);
            modifyProduct(entry.Key, new Price(lastPrices[lastPrices.Length - 1].RawValue / entry.Value),
                -1 * entry.Value);
            if (lastPrices.Length >= 1)
            {
                total = _total.Minus(lastPrices[lastPrices.Length - 1]);
                Cash = _cash.Minus(lastPrices[lastPrices.Length - 1]);
                if (lastPrices.Length == 1) lastPrices = new Price[] { };
                else lastPrices = lastPrices.SubArray(0, lastPrices.Length - 1);
            }
            setStatus("Letztes Produkt entfernt!");
        }

        public void setStatus(string txt)
        {
            tbloStatus.Text = txt;
        }

        private void btnSaveRechnung_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                setStatus("Speichern...");
                D.W("Speichere Rechnung...");
                string nextRechungName = string.Format("rechnung_{0:0000}_{1:yyyyMMdd_HHmm}.txt",
                    _SellInfo.rechnungsNummer, DateTime.Now);
                string path = Path.Combine(_SellInfo.RechnungenPath, nextRechungName);
                D.W(path, "Speichere Rechung unter");
                StreamWriter fs = File.CreateText(path);
                fs.WriteLine("".PadLeft(40, '#'));
                fs.WriteLine("_Sell v" + _SellInfo.VersionString);
                fs.WriteLine("{0:dd.MM.yyyy HH:mm}", DateTime.Now);
                fs.WriteLine("".PadLeft(40, '#'));
                fs.WriteLine("|{0,-19}{1,19}|", "Position", "Preis");
                int lbICount = lbxItems.Items.Count;
                if (lbICount != lbxPrices.Items.Count) return;
                int i = 0;
                while (i < lbICount)
                {
                    fs.WriteLine("|{0,-19}{1,19}|", lbxItems.Items.GetItemAt(i), lbxPrices.Items.GetItemAt(i));
                    i++;
                }
                fs.WriteLine("".PadLeft(40, '-'));
                fs.WriteLine("|{0,-19}{1,19}|", "Gesamt:", total);
                fs.WriteLine("".PadLeft(40, '-'));
                fs.WriteLine("Vielen Dank. END");
                fs.Close();
                setStatus(string.Format("saved to {0}! [PROTIP: Strg+Shift+T]", nextRechungName));
                _SellInfo.rechnungsNummer++;
            }
            catch (Exception ex)
            {
                D.W(ex.Message + "Exception in btnSaveRechnung_Click()!");
                setStatus("Fehler: Datei konnte nicht gespeichert werden.");
            }
            btnClear_Click(null, null);
        }

        private void onToolButtonClick(object sender, RoutedEventArgs e)
        {
            Tools tlz = new Tools();
            tlz.Show();
        }

        private void onNumKeyPress(RegisteredHotkey hotkey)
        {
            NmrBtns_Click(Win32.GetNumpadKeyId(hotkey.KeyCode), new RoutedEventArgs());
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            HotkeyManager hotkeyManager = new HotkeyManager(this, "_Sell");
            for (int i = 0; i <= 9; i++)
            {
                hotkeyManager.registerHotkey(Win32.GetNumpadKeyCode(i), Win32.MOD_NOREPEAT).onPress += onNumKeyPress;
            }
            hotkeyManager.registerHotkey(Convert.ToUInt32('M'), Win32.MOD_NOREPEAT).onPress +=
                h => btnSaveRechnung_Click(null, null); //Print Screen
            localKeyHandlers[Key.Return] = x => btnEnter_Click(null, null);
            localKeyHandlers[Key.Back] = x => btnBackspace_Click(null, null);
            localKeyHandlers[Key.Delete] = x => btnClearLast_Click(null, null);
            localKeyHandlers[Key.OemPeriod] = x => btnSaveRechnung_Click(null, null);
            localKeyHandlers[Key.Multiply] = h => btnTimes_Click(null, null);
            localKeyHandlers[Key.Add] = h => btnAdd_Click(null, null);
            localKeyHandlers[Key.Subtract] = h => btnSubtract_Click(null, null);
            localKeyHandlers[Key.Divide] = h => btnDivide_Click(null, null);
            localKeyHandlers[Key.Left] = _ => PrevPageButton_OnClick(null, null);
            localKeyHandlers[Key.Right] = _ => NextPageButton_OnClick(null, null);
        }

        private void removeFirstZeroOnDisplay()
        {
            if (Display.FirstLineItem.Text.StartsWith("0"))
            {
                Display.setFirstLine(Display.FirstLineItem.Text.TrimStart('0'));
            }
        }

        private void NmrBtns_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int number;
                if (sender is Button)
                {
                    dctNmrBtns.TryGetValue((Button) sender, out number);
                }
                else if (sender is int)
                {
                    number = (int) sender;
                }
                else
                {
                    return;
                }
                removeFirstZeroOnDisplay();
                Display.AddToFirstLine(number);
            }
            catch (Exception)
            {
                setStatus("Ein Fehler ist aufgetreten!");
            }
        }

        private void btnBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (Display.FirstLineItem.Text.Length <= 0) return;
            int i = 0;
            string newString = "";
            while (i < Display.FirstLineItem.Text.Length - 1)
            {
                newString += Display.FirstLineItem.Text[i];
                i++;
            }
            Display.setFirstLine(newString);
            if (Display.FirstLineItem.Text == "") Display.setFirstLine("0");
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            OnEnterButton();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                intTempSaved = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.setFirstLine("0");
            }
            catch (Exception)
            {
                setStatus("Fehler!");
                return;
            }
            OnEnterButton += GetItForAddition;
        }

        private void btnSubtract_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                intTempSaved = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.setFirstLine("0");
            }
            catch (Exception)
            {
                setStatus("Fehler!");
                return;
            }
            OnEnterButton += GetItForSubtraction;
        }

        private void btnTimes_Click(object sender, RoutedEventArgs e)
        {
            if (isMultiplicationWaiting) return;
            try
            {
                intTempSaved = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.setFirstLine("0");
            }
            catch (Exception)
            {
                setStatus("Fehler!");
                return;
            }
            isMultiplicationWaiting = true;
            OnEnterButton += GetItForMultiplication;
        }

        private void btnDivide_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                intTempSaved = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.setFirstLine("0");
            }
            catch (Exception)
            {
                setStatus("Fehler!");
                return;
            }
            OnEnterButton += GetItForDivision;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Display.setFirstLine("0");
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            OnEnterButton += GetItForCash;
        }

        private void miSubtractCash_Click(object sender, RoutedEventArgs e)
        {
            OnEnterButton += GetItForCashSub;
        }

        private void btnGgb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.setFirstLine(new Price(intPrice).Minus(_total).RawValue.ToString());
            }
            catch (Exception)
            {
                setStatus("Keine Zahl oder anderer Fehler!");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_productGrid.HandleKeyPress(e.Key))
            {
                e.Handled = true;
            }
            else if (localKeyHandlers.ContainsKey(e.Key))
            {
                localKeyHandlers[e.Key](e);
                e.Handled = true;
            }
        }

        private void FirstPageButton_OnClick(object sender, RoutedEventArgs e)
        {
            _productGrid.ToFirstPage();
        }

        private void PrevPageButton_OnClick(object sender, RoutedEventArgs e)
        {
            _productGrid.ToPreviousPage();
        }

        private void NextPageButton_OnClick(object sender, RoutedEventArgs e)
        {
            _productGrid.ToNextPage();
        }

        private void LastPageButton_OnClick(object sender, RoutedEventArgs e)
        {
            _productGrid.ToLastPage();
        }
    }

    public static class Display
    {
        public static TextBlock Item;
        public static TextBlock FirstLineItem;
        public static TextBlock SecondLineItem;

        public static void initDisplay(TextBlock tblo, TextBlock firstLine, TextBlock secondLine)
        {
            Item = tblo;
            FirstLineItem = firstLine;
            SecondLineItem = secondLine;
        }

        public static void setFirstLine(string Text)
        {
            FirstLineItem.Text = Text;
            D.W(Text, "text for 1st Line");
            D.W(FirstLineItem.Text, "Actual text");
            D.W(Item, "Item Text");
        }

        public static void setSecondLine(string Text)
        {
            SecondLineItem.Text = Text;
        }

        public static void AddToFirstLine(object obj)
        {
            FirstLineItem.Text += obj.ToString();
        }
    }
}