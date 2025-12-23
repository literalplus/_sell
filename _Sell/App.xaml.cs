using System;
using System.IO;
using System.Text;
using xytools;

namespace _Sell
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {
        private void Application_Activated_1(object sender, EventArgs e)
        {
            D.W("***************************************");
            D.W("_Sell v" + SellInfo.VersionString);
            D.W("https://github.com/literalplus/_Sell");
            D.W("***************************************");
            SellInfo.MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @".xxyy\_sell");
            D.W(SellInfo.MainPath, "Setting MainPath to");
            SellInfo.RechnungenPath = Path.Combine(SellInfo.MainPath, @"rechnungen\");
            SellInfo.RechnungsNummerConfig = Path.Combine(SellInfo.MainPath, "rechnungen.int.cfg.txt");
            SellInfo.CashConfig = Path.Combine(SellInfo.MainPath, "cash.int.cfg.txt");
            D.W(SellInfo.RechnungenPath, "Setting RechnungsPath to");
            try
            {
                if (!Directory.Exists(SellInfo.MainPath))
                {
                    D.W("MainPath doesn't exist! Creating...");
                    Directory.CreateDirectory(SellInfo.MainPath);
                }

                if (!Directory.Exists(SellInfo.RechnungenPath))
                {
                    D.W("RechnungsPath doesn't exist! Creating...");
                    Directory.CreateDirectory(SellInfo.RechnungenPath);
                }

                if (!File.Exists(SellInfo.RechnungsNummerConfig))
                {
                    D.W("Erstelle rechnungsNummerConfig!");
                    var fs = File.CreateText(SellInfo.RechnungsNummerConfig);
                    fs.Write("0001");
                    fs.Close();
                    SellInfo.RechnungsNummerRaw = 0;
                }
                else
                {
                    D.W("Lese rechnungsNummerConfig!");
                    var fr = File.OpenText(SellInfo.RechnungsNummerConfig);
                    var strCurrentRIndex = fr.ReadLine();
                    int intCurrentRIndex;
                    try
                    {
                        intCurrentRIndex = Convert.ToInt32(strCurrentRIndex);
                        SellInfo.RechnungsNummerRaw = intCurrentRIndex;
                    }
                    catch (Exception)
                    {
                        D.W("Exception beim Lesen von rechnungsNummerCfg");
                    }
                }

                if (!File.Exists(SellInfo.CashConfig))
                {
                    D.W("Erstelle cashConfig!");
                    var fs = File.CreateText(SellInfo.CashConfig);
                    fs.Write("-0000001"); //max 99k €
                    fs.Close();
                    SellInfo.CashInRegisterRaw = -1;
                }
                else
                {
                    D.W("Lese cashConfig!");
                    var fr = File.OpenText(SellInfo.CashConfig);
                    var strCurrentRIndex = fr.ReadLine();
                    fr.Close();
                    D.W(strCurrentRIndex, "gelesen");
                    int intCurrentRIndex;
                    try
                    {
                        intCurrentRIndex = Convert.ToInt32(strCurrentRIndex);
                        SellInfo.CashInRegisterRaw = intCurrentRIndex;
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

            if (SellInfo.MainWindow != null)
            {
                SellInfo.MainWindow.CheckCashLogging();
            }
        }
    }

    public static class SellInfo
    {
        public static MainWindow MainWindow;

        public const string VersionString = "0.0.1a";
        public static string MainPath;
        public static string RechnungenPath;
        public static string RechnungsNummerConfig;
        public static string CashConfig;

        internal static int CashInRegisterRaw;

        public static int CashInRegister
        {
            get => CashInRegisterRaw;
            set
            {
                try
                {
                    if (value == CashInRegisterRaw) return;
                    //FileStream fs = File.Create(cashConfig);
                    var fs = new FileStream(CashConfig, FileMode.Create, FileAccess.Write, FileShare.None);
                    var strNummer = new UTF8Encoding(true).GetBytes(value.ToString("0000000")); //max 99k € (in cents)
                    fs.Write(strNummer, 0, strNummer.Length);
                    fs.Close();
                    CashInRegisterRaw = value;
                    D.W(value, "Cash set to");
                }
                catch (Exception ex)
                {
                    D.W("##Fehler beim Setzen der Kasse! " + ex.Message);
                    MainWindow.SetStatus("Kassenstand wurde nicht gespeichert!");
                }
            }
        }

        public static int RechnungsNummer
        {
            get => RechnungsNummerRaw;
            set
            {
                try
                {
                    if (value == RechnungsNummerRaw) return;
                    var fs = File.Create(RechnungsNummerConfig);
                    var strNummer = new UTF8Encoding(true).GetBytes(value.ToString("0000"));
                    fs.Write(strNummer, 0, strNummer.Length);
                    fs.Close();
                    RechnungsNummerRaw = value;
                }
                catch (Exception ex)
                {
                    D.W("##Fehler beim Setzen der Rechungsnummer! " + ex.Message);
                    MainWindow.SetStatus("Fehler beim Speichern der Rechnung!");
                }
            }
        }

        internal static int RechnungsNummerRaw;
    }
}