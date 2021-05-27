/*  CTRADER GURU --> Indicator Template 1.0.6

    Homepage    : https://ctrader.guru/
    Telegram    : https://t.me/ctraderguru
    Twitter     : https://twitter.com/cTraderGURU/
    Facebook    : https://www.facebook.com/ctrader.guru/
    YouTube     : https://www.youtube.com/channel/UCKkgbw09Fifj65W5t5lHeCQ
    GitHub      : https://github.com/ctrader-guru

*/

using System;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo
{

    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    [Levels(0)]
    public class CurrencyWatch : Indicator
    {

        #region Enums and Class

        // --> Eventuali enumeratori li mettiamo qui

        public class CurrencyInformation {

            public string Currency = "";
            public double LastValue = 0;

        }

        #endregion

        #region Identity

        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "Currency Watch";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.2";

        #endregion

        #region Params

        /// <summary>
        /// Identità del prodotto nel contesto di ctrader.guru
        /// </summary>
        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://ctrader.guru/product/currency-watch/")]
        public string ProductInfo { get; set; }

        [Parameter("End of Day", Group = "Params", DefaultValue = 23)]
        public int EndOfDay { get; set; }

        [Output("EUR", LineColor = "DodgerBlue")]
        public IndicatorDataSeries EUR { get; set; }

        [Output("USD", LineColor = "Red")]
        public IndicatorDataSeries USD { get; set; }

        [Output("GBP", LineColor = "DarkGray")]
        public IndicatorDataSeries GBP { get; set; }

        [Output("JPY", LineColor = "DarkViolet")]
        public IndicatorDataSeries JPY { get; set; }

        [Output("CAD", LineColor = "Orange")]
        public IndicatorDataSeries CAD { get; set; }

        [Output("CHF", LineColor = "Gray")]
        public IndicatorDataSeries CHF { get; set; }

        [Output("AUD", LineColor = "Lime")]
        public IndicatorDataSeries AUD { get; set; }

        [Output("NZD", LineColor = "Green")]
        public IndicatorDataSeries NZD { get; set; }

        #endregion

        #region Property

        int havecurr = 0;

        CurrencyInformation bestCurrency = new CurrencyInformation();
        CurrencyInformation worstCurrency = new CurrencyInformation();

        double EURUSDopenday = -1;
        double EURGBPopenday = -1;
        double EURJPYopenday = -1;
        double EURAUDopenday = -1;
        double EURCHFopenday = -1;
        double EURCADopenday = -1;
        double EURNZDopenday = -1;

        double GBPUSDopenday = -1;
        double USDJPYopenday = -1;
        double AUDUSDopenday = -1;
        double USDCHFopenday = -1;
        double USDCADopenday = -1;
        double NZDUSDopenday = -1;

        double GBPJPYopenday = -1;
        double GBPAUDopenday = -1;
        double GBPCHFopenday = -1;
        double GBPCADopenday = -1;
        double GBPNZDopenday = -1;

        double AUDJPYopenday = -1;
        double CHFJPYopenday = -1;
        double CADJPYopenday = -1;
        double NZDJPYopenday = -1;

        double CADCHFopenday = -1;
        double AUDCADopenday = -1;
        double NZDCADopenday = -1;

        double AUDCHFopenday = -1;
        double NZDCHFopenday = -1;

        double AUDNZDopenday = -1;

        private readonly string[] EURcross =
        {
            "EURUSD",
            "EURGBP",
            "EURJPY",
            "EURAUD",
            "EURCHF",
            "EURCAD",
            "EURNZD"
        };

        private readonly string[] USDcross =
        {
            "EURUSD",
            "GBPUSD",
            "USDJPY",
            "AUDUSD",
            "USDCHF",
            "USDCAD",
            "NZDUSD"
        };

        private readonly string[] GBPcross =
        {
            "EURGBP",
            "GBPUSD",
            "GBPJPY",
            "GBPAUD",
            "GBPCHF",
            "GBPCAD",
            "GBPNZD"
        };

        private readonly string[] JPYcross =
        {
            "EURJPY",
            "USDJPY",
            "GBPJPY",
            "AUDJPY",
            "CHFJPY",
            "CADJPY",
            "NZDJPY"
        };

        private readonly string[] CADcross =
        {
            "EURCAD",
            "USDCAD",
            "GBPCAD",
            "CADJPY",
            "CADCHF",
            "AUDCAD",
            "NZDCAD"
        };

        private readonly string[] CHFcross =
        {
            "EURCHF",
            "USDCHF",
            "GBPCHF",
            "CHFJPY",
            "CADCHF",
            "AUDCHF",
            "NZDCHF"
        };

        private readonly string[] AUDcross =
        {
            "EURAUD",
            "AUDUSD",
            "GBPAUD",
            "AUDJPY",
            "AUDCHF",
            "AUDCAD",
            "AUDNZD"
        };

        private readonly string[] NZDcross =
        {
            "EURNZD",
            "NZDUSD",
            "GBPNZD",
            "NZDJPY",
            "NZDCAD",
            "NZDCHF",
            "AUDNZD"
        };

        #endregion

        #region Indicator Events

        /// <summary>
        /// Viene generato all'avvio dell'indicatore, si inizializza l'indicatore
        /// </summary>
        protected override void Initialize()
        {

            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);            

        }

        /// <summary>
        /// Generato ad ogni tick, vengono effettuati i calcoli dell'indicatore
        /// </summary>
        /// <param name="index">L'indice della candela in elaborazione</param>
        public override void Calculate(int index)
        {

            if (Bars.TimeFrame > TimeFrame.Hour)
            {

                if (_canDraw()) Chart.DrawStaticText("MyError", "PLEASE, USE THIS INDICATOR WITH TIMEFRAME UP TO 1H", VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);

                return;

            }
            else if (havecurr == -1)
            {

                if (_canDraw())
                    Chart.DrawStaticText("MyError", "NOT SUPPORT THIS CROSS, ONLY MAJOR CURRENCY : EUR;USD;GBP;JPY;CAD;AUD;CHF;NZD", VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);

                return;

            }

            havecurr = -1;
            bestCurrency = new CurrencyInformation();
            worstCurrency = new CurrencyInformation();

            SetValue(EUR, index, "EUR", EURcross);
            SetValue(USD, index, "USD", USDcross);
            SetValue(GBP, index, "GBP", GBPcross);
            SetValue(JPY, index, "JPY", JPYcross);
            SetValue(CAD, index, "CAD", CADcross);
            SetValue(CHF, index, "CHF", CHFcross);
            SetValue(AUD, index, "AUD", AUDcross);
            SetValue(NZD, index, "NZD", NZDcross);
                        
            DrawBestandWorst();

        }

        #endregion

        #region Private Methods
        
        private T GetAttributeFrom<T>(string propertyName)
        {
            var attrType = typeof(T);
            var property = this.GetType().GetProperty(propertyName);
            return (T)property.GetCustomAttributes(attrType, false).GetValue(0);
        }

        private bool _canDraw()
        {

            return (RunningMode == RunningMode.RealTime || RunningMode == RunningMode.VisualBacktesting);

        }

        private double GetOpenDayPrice(string onecross)
        {

            if (onecross == "EURUSD")
                return EURUSDopenday;

            if (onecross == "EURGBP")
                return EURGBPopenday;

            if (onecross == "EURJPY")
                return EURJPYopenday;

            if (onecross == "EURAUD")
                return EURAUDopenday;

            if (onecross == "EURCHF")
                return EURCHFopenday;

            if (onecross == "EURCAD")
                return EURCADopenday;

            if (onecross == "EURNZD")
                return EURNZDopenday;

            if (onecross == "GBPUSD")
                return GBPUSDopenday;

            if (onecross == "USDJPY")
                return USDJPYopenday;

            if (onecross == "AUDUSD")
                return AUDUSDopenday;

            if (onecross == "USDCHF")
                return USDCHFopenday;

            if (onecross == "USDCAD")
                return USDCADopenday;

            if (onecross == "NZDUSD")
                return NZDUSDopenday;

            if (onecross == "GBPJPY")
                return GBPJPYopenday;

            if (onecross == "GBPAUD")
                return GBPAUDopenday;

            if (onecross == "GBPCHF")
                return GBPCHFopenday;

            if (onecross == "GBPCAD")
                return GBPCADopenday;

            if (onecross == "GBPNZD")
                return GBPNZDopenday;

            if (onecross == "AUDJPY")
                return AUDJPYopenday;

            if (onecross == "CHFJPY")
                return CHFJPYopenday;

            if (onecross == "CADJPY")
                return CADJPYopenday;

            if (onecross == "NZDJPY")
                return NZDJPYopenday;

            if (onecross == "CADCHF")
                return CADCHFopenday;

            if (onecross == "AUDCAD")
                return AUDCADopenday;

            if (onecross == "NZDCAD")
                return NZDCADopenday;

            if (onecross == "AUDCHF")
                return AUDCHFopenday;

            if (onecross == "NZDCHF")
                return NZDCHFopenday;

            if (onecross == "AUDNZD")
                return AUDNZDopenday;

            return -1;

        }

        private void SetOpenDayPrice(string onecross, double openprice)
        {

            if (onecross == "EURUSD")
            {
                EURUSDopenday = openprice;
                return;
            }


            if (onecross == "EURGBP")
            {
                EURGBPopenday = openprice;
                return;
            }

            if (onecross == "EURJPY")
            {
                EURJPYopenday = openprice;
                return;
            }

            if (onecross == "EURAUD")
            {
                EURAUDopenday = openprice;
                return;
            }

            if (onecross == "EURCHF")
            {
                EURCHFopenday = openprice;
                return;
            }

            if (onecross == "EURCAD")
            {
                EURCADopenday = openprice;
                return;
            }

            if (onecross == "EURNZD")
            {
                EURNZDopenday = openprice;
                return;
            }

            if (onecross == "GBPUSD")
            {
                GBPUSDopenday = openprice;
                return;
            }

            if (onecross == "USDJPY")
            {
                USDJPYopenday = openprice;
                return;
            }

            if (onecross == "AUDUSD")
            {
                AUDUSDopenday = openprice;
                return;
            }

            if (onecross == "USDCHF")
            {
                USDCHFopenday = openprice;
                return;
            }

            if (onecross == "USDCAD")
            {
                USDCADopenday = openprice;
                return;
            }

            if (onecross == "NZDUSD")
            {
                NZDUSDopenday = openprice;
                return;
            }

            if (onecross == "GBPJPY")
            {
                GBPJPYopenday = openprice;
                return;
            }

            if (onecross == "GBPAUD")
            {
                GBPAUDopenday = openprice;
                return;
            }

            if (onecross == "GBPCHF")
            {
                GBPCHFopenday = openprice;
                return;
            }

            if (onecross == "GBPCAD")
            {
                GBPCADopenday = openprice;
                return;
            }

            if (onecross == "GBPNZD")
            {
                GBPNZDopenday = openprice;
                return;
            }

            if (onecross == "AUDJPY")
            {
                AUDJPYopenday = openprice;
                return;
            }

            if (onecross == "CHFJPY")
            {
                CHFJPYopenday = openprice;
                return;
            }

            if (onecross == "CADJPY")
            {
                CADJPYopenday = openprice;
                return;
            }

            if (onecross == "NZDJPY")
            {
                NZDJPYopenday = openprice;
                return;
            }

            if (onecross == "CADCHF")
            {
                CADCHFopenday = openprice;
                return;
            }

            if (onecross == "AUDCAD")
            {
                AUDCADopenday = openprice;
                return;
            }

            if (onecross == "NZDCAD")
            {
                NZDCADopenday = openprice;
                return;
            }

            if (onecross == "AUDCHF")
            {
                AUDCHFopenday = openprice;
                return;
            }

            if (onecross == "NZDCHF")
            {
                NZDCHFopenday = openprice;
                return;
            }

            if (onecross == "AUDNZD")
            {
                AUDNZDopenday = openprice;
                return;
            }


        }

        private void SetValue(IndicatorDataSeries View, int index, string CROSSSymbol, string[] cross)
        {

            double crosspips = 0.0;

            // --> Devo fare un ciclio per valutare i cross

            foreach (string onecross in cross)
            {

                try
                {

                    double opendayprice = GetOpenDayPrice(onecross);

                    Symbol tmpcross = Symbols.GetSymbol(onecross);

                    Bars tmpcrosssr = MarketData.GetBars(TimeFrame, tmpcross.Name);

                    int index2 = tmpcrosssr.OpenTimes.GetIndexByExactTime(Bars.OpenTimes[index]);

                    if (tmpcrosssr.OpenTimes[index2].Hour == EndOfDay && tmpcrosssr.OpenTimes[index2].Minute >= 0 && tmpcrosssr.OpenTimes[index2].Minute <= 1)
                    {
                        SetOpenDayPrice(onecross, tmpcrosssr.OpenPrices[index2]);
                        opendayprice = tmpcrosssr.OpenPrices[index2];
                    }

                    if (opendayprice == -1)
                    {

                        havecurr = 1;
                        return;

                    }

                    double tmpcrosspips = 0.0;

                    if (onecross.IndexOf(CROSSSymbol) == 0)
                    {

                        if (tmpcrosssr.ClosePrices[index2] > opendayprice)
                        {

                            tmpcrosspips = (tmpcrosssr.ClosePrices[index2] - opendayprice) / tmpcross.PipSize;
                            crosspips += tmpcrosspips;

                        }
                        else if (opendayprice > tmpcrosssr.ClosePrices[index2])
                        {

                            tmpcrosspips = (opendayprice - tmpcrosssr.ClosePrices[index2]) / tmpcross.PipSize;
                            crosspips -= tmpcrosspips;

                        }

                    }
                    else if (onecross.IndexOf(CROSSSymbol) > 0)
                    {

                        if (tmpcrosssr.ClosePrices[index2] > opendayprice)
                        {

                            tmpcrosspips = (tmpcrosssr.ClosePrices[index2] - opendayprice) / tmpcross.PipSize;
                            crosspips -= tmpcrosspips;

                        }
                        else if (opendayprice > tmpcrosssr.ClosePrices[index2])
                        {

                            tmpcrosspips = (opendayprice - tmpcrosssr.ClosePrices[index2]) / tmpcross.PipSize;
                            crosspips += tmpcrosspips;

                        }

                    }
                    else
                    {

                        Print(string.Format("Error : {0} not exist", CROSSSymbol));

                    }


                }
                catch (Exception)
                {

                    //Print(string.Format("Errore : {0}", exc.Message));

                }

            }

            View[index] = crosspips;

            if (bestCurrency.LastValue == 0 || View[index] > bestCurrency.LastValue)
            {

                bestCurrency.LastValue = View[index];
                bestCurrency.Currency = CROSSSymbol;
                
            }
            else if (worstCurrency.LastValue == 0 || View[index] < worstCurrency.LastValue)
            {

                worstCurrency.LastValue = View[index];
                worstCurrency.Currency = CROSSSymbol;

            }

            havecurr = 1;

        }

        private void DrawBestandWorst()
        {


            if (_canDraw())
            {

                if (bestCurrency.LastValue != 0)
                {
                    
                    var myOutputBest = this.GetAttributeFrom<OutputAttribute>(bestCurrency.Currency);
                    ChartText myTextBest = IndicatorArea.DrawText("BestCurrency", "BEST » " + bestCurrency.Currency + " » " + bestCurrency.LastValue.ToString("N2"), Bars.OpenTimes.LastValue, bestCurrency.LastValue, Color.FromName(myOutputBest.LineColor));
                    myTextBest.IsInteractive = false;
                    myTextBest.FontSize = 12;
                    myTextBest.VerticalAlignment = VerticalAlignment.Center;
                    
                }

                if (worstCurrency.LastValue != 0)
                {

                    var myOutputWorst = this.GetAttributeFrom<OutputAttribute>(worstCurrency.Currency);
                    ChartText myTextWorst = IndicatorArea.DrawText("WorstCurrency", "WORST » " + worstCurrency.Currency + " » " + worstCurrency.LastValue.ToString("N2"), Bars.OpenTimes.LastValue, worstCurrency.LastValue, Color.FromName(myOutputWorst.LineColor));
                    myTextWorst.IsInteractive = false;
                    myTextWorst.FontSize = 12;
                    myTextWorst.VerticalAlignment = VerticalAlignment.Center;

                }
            }

        }

        #endregion

    }

}
