namespace rise.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// rise Quote Object from Xt.com
    /// </summary>

    public class XtcomQuote
    {
        public double moneyVol { get; set; }
        public double monthRate { get; set; }
        public int coinVol { get; set; }
        public double weekRate { get; set; }
        public double high { get; set; }
        public double rate { get; set; }
        public double low { get; set; }
        public double price { get; set; }
        public double ask { get; set; }
        public double bid { get; set; }
    }

    public class XtcomQuoteResult
    {
        public static XtcomQuoteResult Current { get; set; }
        public XtcomQuote quote { get; set; }
    }
}