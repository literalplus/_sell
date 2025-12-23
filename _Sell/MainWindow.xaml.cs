using _Sell.Action;
using _Sell.Model;
using _Sell.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using xytools;

namespace _Sell
{
    public delegate void EnterButtonHandler();

    public delegate void LocalKeyEventHandler(KeyEventArgs args);

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public event EnterButtonHandler OnEnterButton = () => { };
        
        private bool _isCashLoggingEnabled;
        private bool _isMultiplicationWaiting;
        private readonly Dictionary<string, int> _productAmounts = new();
        private readonly Dictionary<string, int> _productIndexes = new();
        private readonly List<KeyValuePair<string, int>> _productsInOrder = [];
        private readonly Dictionary<Key, LocalKeyEventHandler> _localKeyHandlers = new();
        private int _intTempSaved;
        private Price[] _lastPrices = [];
        private readonly DefaultProductRegistry _productRegistry = new();
        private readonly ProductGrid _productGrid;
        private readonly NumberKeyManager _numberKeyManager = new();

        public Price Cash
        {
            set
            {
                _cash = value;
                Display.SetSecondLine(value.ToString());
                SellInfo.CashInRegister = value.RawValue;
            }
        }

        private Price _cash;

        private Price Total
        {
            get => _total;
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
                SellInfo.MainWindow = this;
                InitializeComponent();
                Display.InitDisplay(tbloNmrDisplay, tbloLineOne, tbloLineTwo);
                D.W("Checking cash!");
                CheckCashLogging();
                Total = new Price(0);
                SetStatus("Bereit!");
                _numberKeyManager.RegisterAll(btnZero, btnOne, btnTwo, btnThree, btnFour, btnFive, btnSix, btnSeven, btnEight, btnNine);
                _productGrid = new ProductGrid(ProductGrid.DefaultGridMeta, ProductButtonsGrid);
                InitFirstPage();
                ClearProducts();
            }
            catch (Exception ex)
            {
                D.W("Exception when initializing MainWindow..." + ex);
                Application.Current.Shutdown();
            }
        }

        private void InitFirstPage()
        {
            _productGrid.Add(new GenericGridAction(
                () => btnSonstiges_Click(),
                "Sonstiges", "€ xx,xx"
            ));
            _productGrid.Add(new GenericGridAction(
                () => btnSpende_Click(),
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
        }

        public void CheckCashLogging()
        {
            if (SellInfo.CashInRegister != -1)
            {
                _isCashLoggingEnabled = true;
                _cash = new Price(SellInfo.CashInRegister);
                Display.SetSecondLine(_cash.ToString());
                D.W(_cash.ToString(), "mainwindow cash set to");
                miSetCash.IsEnabled = true;
                miSubtractCash.IsEnabled = true;
            }
            else D.W("cashStand equals -1");
        }

        public void AddPriceToList(Price prc)
        {
            try
            {
                Price[] tempPrices = { prc };
                _lastPrices = _lastPrices.Concat(tempPrices).ToArray();
                D.W(XyStr.ListArrayToString(_lastPrices));
                if (_isCashLoggingEnabled)
                {
                    Cash = prc.Plus(_cash);
                }
            }
            catch (Exception ex)
            {
                D.W(ex.Message, "ex bei addPriceToList()");
            }
            Total = _total.Plus(prc);
        }

        public void ClearProducts()
        {
            Total = new Price(0);
            lbxItems.Items.Clear();
            lbxPrices.Items.Clear();
            lbxItems.Items.Add($"{"Position",-40}| {"EPreis",-10}| {"Menge",-10}");
            lbxItems.Items.Add(("".PadLeft(110, '-')));
            lbxPrices.Items.Add("Preis");
            lbxPrices.Items.Add("".PadLeft(30, '-'));
            _productAmounts.Clear();
            _productIndexes.Clear();
            _productsInOrder.Clear();
        }

        public void AddItemToLists(string name, Price price, bool addToTotal = true)
        {
            var totalAmount = 1;
            if (_isMultiplicationWaiting)
            {
                _isMultiplicationWaiting = false;
                OnEnterButton -= GetItForMultiplication;
                totalAmount = _intTempSaved;
            }
            _productsInOrder.Add(new KeyValuePair<string, int>(name, totalAmount));
            ModifyProduct(name, price, totalAmount);
            if (addToTotal) AddPriceToList(price);
            else AddPriceToList(new Price(0));
        }

        private void ModifyProduct(string name, Price singlePrice, int modifier)
        {
            var newProduct = !_productAmounts.ContainsKey(name);
            if (newProduct && modifier < 0)
            {
                throw new ArgumentOutOfRangeException("" + modifier);
            }
            modifier += newProduct ? 0 : _productAmounts[name];
            var index = newProduct ? -1 : _productIndexes[name];
            if (modifier > 0)
            {
                UpdateProduct(index, name, singlePrice, modifier);
                _productAmounts[name] = modifier;
            }
            else
            {
                D.W(lbxItems.Items.Count);
                lbxItems.Items.RemoveAt(index);
                lbxPrices.Items.RemoveAt(index);
                _productAmounts.Remove(name);
            }
        }

        private void UpdateProduct(int index, string name, Price singlePrice, int amount)
        {
            var itemString =
                $"{name,-40}| {singlePrice,-10}| {"x" + amount.ToString("00"),-10}";
            var priceString = singlePrice.Times(amount).ToString();
            if (index == -1)
            {
                _productIndexes[name] = lbxItems.Items.Add(itemString);
                lbxPrices.Items.Add(priceString);
            }
            else
            {
                lbxItems.Items[index] = itemString;
                lbxPrices.Items[index] = priceString;
            }
        }

        private void GetItForSonstiges()
        {
            try
            {
                var intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                AddItemToLists("Sonstiger Artikel", new Price(intPrice));
            }
            catch (Exception)
            {
                SetStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            Display.SetFirstLine("0");
            OnEnterButton -= GetItForSonstiges;
        }

        private void GetItForCash()
        {
            try
            {
                var intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                Cash = new Price(intPrice);
            }
            catch (Exception)
            {
                SetStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            Display.SetFirstLine("0");
            OnEnterButton -= GetItForCash;
        }

        private void GetItForCashSub()
        {
            try
            {
                var intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                Cash = _cash.Minus(new Price(intPrice));
            }
            catch (Exception)
            {
                SetStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            Display.SetFirstLine("0");
            OnEnterButton -= GetItForCashSub;
        }

        private void GetItForAddition()
        {
            try
            {
                var intNew = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.SetFirstLine((intNew + _intTempSaved).ToString());
            }
            catch (Exception)
            {
                SetStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            OnEnterButton -= GetItForAddition;
        }

        private void GetItForSubtraction()
        {
            try
            {
                var intNew = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.SetFirstLine((_intTempSaved - intNew).ToString());
            }
            catch (Exception)
            {
                SetStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            OnEnterButton -= GetItForSubtraction;
        }

        private void GetItForMultiplication()
        {
            try
            {
                var intNew = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.SetFirstLine((intNew * _intTempSaved).ToString());
            }
            catch (Exception)
            {
                SetStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            _isMultiplicationWaiting = false;
            OnEnterButton -= GetItForMultiplication;
        }

        private void GetItForDivision()
        {
            try
            {
                var intNew = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.SetFirstLine((_intTempSaved / intNew).ToString());
            }
            catch (Exception)
            {
                SetStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            OnEnterButton -= GetItForDivision;
        }

        private void btnSonstiges_Click()
        {
            OnEnterButton += GetItForSonstiges;
            OnEnterButton -= GetItForMultiplication;
        }

        private void GetItForSpenden()
        {
            try
            {
                var intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                AddItemToLists("Spende", new Price(intPrice), false);
            }
            catch (Exception)
            {
                SetStatus("Keine Zahl oder anderer Fehler!");
                return;
            }
            Display.SetFirstLine("0");
            OnEnterButton -= GetItForSpenden;
        }

        private void btnSpende_Click()
        {
            OnEnterButton += GetItForSpenden;
        }

        private void btnClear_Click()
        {
            ClearProducts();
            SetStatus("Alle Produkte entfernt!");
        }

        private void btnClearLast_Click(object sender, RoutedEventArgs e)
        {
            if (lbxItems.Items.Count <= 2 || lbxPrices.Items.Count <= 2)
            {
                ClearProducts();
                return;
            }
            var lbICount = lbxItems.Items.Count;
            var lb2ICount = lbxPrices.Items.Count;
            if (lbICount != lb2ICount)
            {
                MessageBox.Show("Fehler! Es gibt nicht gleich viele Preise wie Produkte! Leere Liste..");
                btnClear_Click();
                return;
            }
            if (lbICount <= 0) return;
            var entry = _productsInOrder[^1];
            _productsInOrder.RemoveAt(_productsInOrder.Count - 1);
            ModifyProduct(entry.Key, new Price(_lastPrices[^1].RawValue / entry.Value),
                -1 * entry.Value);
            if (_lastPrices.Length >= 1)
            {
                Total = _total.Minus(_lastPrices[^1]);
                Cash = _cash.Minus(_lastPrices[^1]);
                if (_lastPrices.Length == 1) _lastPrices = Array.Empty<Price>();
                else _lastPrices = _lastPrices.SubArray(0, _lastPrices.Length - 1);
            }
            SetStatus("Letztes Produkt entfernt!");
        }

        public void SetStatus(string txt)
        {
            tbloStatus.Text = txt;
        }

        private void btnSaveRechnung_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetStatus("Speichern...");
                D.W("Speichere Rechnung...");
                var nextRechungName = string.Format("rechnung_{0:0000}_{1:yyyy-MM-dd_HHmm}.txt",
                    SellInfo.RechnungsNummer, DateTime.Now);
                var path = Path.Combine(SellInfo.RechnungenPath, nextRechungName);
                D.W(path, "Speichere Rechung unter");
                var fs = File.CreateText(path);
                fs.WriteLine("".PadLeft(40, '#'));
                fs.WriteLine("_Sell v" + SellInfo.VersionString);
                fs.WriteLine("{0:dd.MM.yyyy HH:mm}", DateTime.Now);
                fs.WriteLine("".PadLeft(40, '#'));
                fs.WriteLine("|{0,-19}{1,19}|", "Position", "Preis");
                var lbICount = lbxItems.Items.Count;
                if (lbICount != lbxPrices.Items.Count) return;
                var i = 0;
                while (i < lbICount)
                {
                    fs.WriteLine("|{0,-19}{1,19}|", lbxItems.Items.GetItemAt(i), lbxPrices.Items.GetItemAt(i));
                    i++;
                }
                fs.WriteLine("".PadLeft(40, '-'));
                fs.WriteLine("|{0,-19}{1,19}|", "Gesamt:", Total);
                fs.WriteLine("".PadLeft(40, '-'));
                fs.WriteLine("Vielen Dank. END");
                fs.Close();
                SetStatus($"saved to {nextRechungName}! [PROTIP: Strg+Shift+T]");
                SellInfo.RechnungsNummer++;
            }
            catch (Exception ex)
            {
                D.W(ex.Message + "Exception in btnSaveRechnung_Click()!");
                SetStatus("Fehler: Datei konnte nicht gespeichert werden.");
            }
            btnClear_Click();
        }

        private void OnToolButtonClick(object sender, RoutedEventArgs e)
        {
            var tlz = new Tools();
            tlz.Show();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            _localKeyHandlers[Key.Return] = _ => btnEnter_Click(null, null);
            _localKeyHandlers[Key.Back] = _ => btnBackspace_Click(null, null);
            _localKeyHandlers[Key.Delete] = _ => btnClearLast_Click(null, null);
            _localKeyHandlers[Key.OemPeriod] = _ => btnSaveRechnung_Click(null, null);
            _localKeyHandlers[Key.PrintScreen] = _ => btnSaveRechnung_Click(null, null);
            _localKeyHandlers[Key.Multiply] = _ => btnTimes_Click();
            _localKeyHandlers[Key.Add] = _ => btnAdd_Click();
            _localKeyHandlers[Key.Subtract] = _ => btnSubtract_Click();
            _localKeyHandlers[Key.Divide] = _ => btnDivide_Click();
            _localKeyHandlers[Key.Left] = _ => PrevPageButton_OnClick(null, null);
            _localKeyHandlers[Key.Right] = _ => NextPageButton_OnClick(null, null);
        }

        private void removeFirstZeroOnDisplay()
        {
            if (Display.FirstLineItem.Text.StartsWith('0'))
            {
                Display.SetFirstLine(Display.FirstLineItem.Text.TrimStart('0'));
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var number = sender switch
                {
                    Button button => _numberKeyManager.GetValueOrNegativeOne(button),
                    int i => i,
                    _ => -1
                };
                HandleNumberEntryIfNonNegative(number);
            }
            catch (Exception)
            {
                SetStatus("Ein Fehler ist aufgetreten!");
            }
        }

        private void HandleNumberEntryIfNonNegative(int number)
        {
            if (number < 0)
            {
                return;
            }
            removeFirstZeroOnDisplay();
            Display.AddToFirstLine(number);
        }

        private void btnBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (Display.FirstLineItem.Text.Length <= 0) return;
            var i = 0;
            var newString = "";
            while (i < Display.FirstLineItem.Text.Length - 1)
            {
                newString += Display.FirstLineItem.Text[i];
                i++;
            }
            Display.SetFirstLine(newString);
            if (Display.FirstLineItem.Text == "") Display.SetFirstLine("0");
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            OnEnterButton();
        }

        private void btnAdd_Click()
        {
            try
            {
                _intTempSaved = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.SetFirstLine("0");
            }
            catch (Exception)
            {
                SetStatus("Fehler!");
                return;
            }
            OnEnterButton += GetItForAddition;
        }

        private void btnSubtract_Click()
        {
            try
            {
                _intTempSaved = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.SetFirstLine("0");
            }
            catch (Exception)
            {
                SetStatus("Fehler!");
                return;
            }
            OnEnterButton += GetItForSubtraction;
        }

        private void btnTimes_Click()
        {
            if (_isMultiplicationWaiting) return;
            try
            {
                _intTempSaved = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.SetFirstLine("0");
            }
            catch (Exception)
            {
                SetStatus("Fehler!");
                return;
            }
            _isMultiplicationWaiting = true;
            OnEnterButton += GetItForMultiplication;
        }

        private void btnDivide_Click()
        {
            try
            {
                _intTempSaved = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.SetFirstLine("0");
            }
            catch (Exception)
            {
                SetStatus("Fehler!");
                return;
            }
            OnEnterButton += GetItForDivision;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Display.SetFirstLine("0");
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
                var intPrice = Convert.ToInt32(Display.FirstLineItem.Text);
                Display.SetFirstLine(new Price(intPrice).Minus(_total).RawValue.ToString());
            }
            catch (Exception)
            {
                SetStatus("Keine Zahl oder anderer Fehler!");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_productGrid.HandleKeyPress(e.Key))
            {
                e.Handled = true;
            }
            else if (_localKeyHandlers.ContainsKey(e.Key))
            {
                _localKeyHandlers[e.Key](e);
                e.Handled = true;
            }
            var numberKeyValue = _numberKeyManager.GetValueOrNegativeOne(e.Key);
            HandleNumberEntryIfNonNegative(numberKeyValue);
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
        private static TextBlock _item;
        public static TextBlock FirstLineItem;
        private static TextBlock _secondLineItem;

        public static void InitDisplay(TextBlock tblo, TextBlock firstLine, TextBlock secondLine)
        {
            _item = tblo;
            FirstLineItem = firstLine;
            _secondLineItem = secondLine;
        }

        public static void SetFirstLine(string text)
        {
            FirstLineItem.Text = text;
            D.W(text, "text for 1st Line");
            D.W(FirstLineItem.Text, "Actual text");
            D.W(_item, "Item Text");
        }

        public static void SetSecondLine(string text)
        {
            _secondLineItem.Text = text;
        }

        public static void AddToFirstLine(object obj)
        {
            FirstLineItem.Text += obj.ToString();
        }
    }
}