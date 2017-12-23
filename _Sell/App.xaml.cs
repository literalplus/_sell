using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using xytools;

namespace _Sell
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Activated_1(object sender, EventArgs e)
        {
            D.W("***************************************");
            D.W("_Sell v" + _SellInfo.VersionString);
            D.W("by xxyy - http://l1t.li/");
            D.W("***************************************");
            _SellInfo.MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @".xxyy\_sell");
            D.W(_SellInfo.MainPath, "Setting MainPath to");
            _SellInfo.RechnungenPath = Path.Combine(_SellInfo.MainPath, @"rechnungen\");
            _SellInfo.rechnungsNummerConfig=Path.Combine(_SellInfo.MainPath,"rechnungen.int.cfg.txt");
            _SellInfo.cashConfig = Path.Combine(_SellInfo.MainPath, "cash.int.cfg.txt");
            D.W(_SellInfo.RechnungenPath, "Setting RechnungsPath to");
            try
            {
                if (!Directory.Exists(_SellInfo.MainPath))
                {
                    D.W("MainPath doesn't exist! Creating...");
                    Directory.CreateDirectory(_SellInfo.MainPath);
                }
                if (!Directory.Exists(_SellInfo.RechnungenPath))
                {
                    D.W("RechnungsPath doesn't exist! Creating...");
                    Directory.CreateDirectory(_SellInfo.RechnungenPath);
                }
                if (!File.Exists(_SellInfo.rechnungsNummerConfig))
                {
                    D.W("Erstelle rechnungsNummerConfig!");
                    StreamWriter fs = File.CreateText(_SellInfo.rechnungsNummerConfig);
                    fs.Write("0001");
                    fs.Close();
                    _SellInfo._rechnungsNummer = 0;
                }
                else
                {
                    D.W("Lese rechnungsNummerConfig!");
                    StreamReader fr = File.OpenText(_SellInfo.rechnungsNummerConfig);
                    string strCurrentRIndex = fr.ReadLine();
                    int intCurrentRIndex;
                    try
                    {
                        intCurrentRIndex = Convert.ToInt32(strCurrentRIndex);
                        _SellInfo._rechnungsNummer = intCurrentRIndex;
                    }
                    catch (Exception)
                    {
                        D.W("Exception beim Lesen von rechnungsNummerCfg");
                    }
                }
                if (!File.Exists(_SellInfo.cashConfig))
                {
                    D.W("Erstelle cashConfig!");
                    StreamWriter fs = File.CreateText(_SellInfo.cashConfig);
                    fs.Write("-0000001");//max 99k €
                    fs.Close();
                    _SellInfo.cashStand = -1;
                }
                else
                {
                    D.W("Lese cashConfig!");
                    StreamReader fr = File.OpenText(_SellInfo.cashConfig);
                    string strCurrentRIndex = fr.ReadLine();
                    fr.Close();
                    D.W(strCurrentRIndex, "gelesen");
                    int intCurrentRIndex;
                    try
                    {
                        intCurrentRIndex = Convert.ToInt32(strCurrentRIndex);
                        _SellInfo.cashStand = intCurrentRIndex;
                    }
                    catch (Exception)
                    {
                        D.W("Exception beim Lesen von cashConfig");
                    }
                }
            }
            catch (Exception ex)
            {
                D.W("##EXCEPTION IN APPLICATION_ACTIVATED_1##");
                D.W(ex.Message);
            }
            if (_SellInfo.MainWindow != null)
            {
                _SellInfo.MainWindow.checkCashLogging();
            }
        }
    }
    public static class _SellInfo
    {
        public static MainWindow MainWindow;

        public const string VersionString="0.0.1a";
        public static string MainPath;
        public static string RechnungenPath;
        public static string rechnungsNummerConfig;
        public static string cashConfig;

        public static int _cashStand;
        public static int cashStand
        {
            get { return _SellInfo._cashStand; }
            set {
                try
                {
                    if (value == _SellInfo._cashStand) return;
                    //FileStream fs = File.Create(cashConfig);
                    FileStream fs = new FileStream(cashConfig, FileMode.Create, FileAccess.Write, FileShare.None);
                    byte[] strNummer = new UTF8Encoding(true).GetBytes(value.ToString("0000000"));//max 99k € (in cents)
                    fs.Write(strNummer, 0, strNummer.Length);
                    fs.Close();
                    _cashStand = value;
                    D.W(value, "Cash set to");
                }
                catch (Exception ex)
                {
                    D.W("##Fehler beim Setzen der Kasse! " + ex.Message);
                    MainWindow.setStatus("Kassenstand wurde nicht gespeichert!");
                }
            }
        }

        public static int rechnungsNummer
        {
            get
            {
                return _SellInfo._rechnungsNummer;
            }
            set
            {
                try
                {
                    if (value == _SellInfo._rechnungsNummer) return;
                    FileStream fs = File.Create(rechnungsNummerConfig);
                    byte[] strNummer = new UTF8Encoding(true).GetBytes(value.ToString("0000"));
                    fs.Write(strNummer, 0, strNummer.Length);
                    fs.Close();
                    _rechnungsNummer = value;
                }
                catch (Exception ex)
                {
                    D.W("##Fehler beim Setzen der Rechungsnummer! " + ex.Message);
                    MainWindow.setStatus("Fehler beim Speichern der Rechnung!");
                }
            }
        }
        internal static int _rechnungsNummer = 0;
    }
}
