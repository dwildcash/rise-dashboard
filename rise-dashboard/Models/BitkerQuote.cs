using System.Collections.Generic;

namespace rise.Models
{
    /// <summary>
    /// rise Quote Object from bitker
    /// </summary>
    public class BitkerQuote
    {

        public static BitkerQuote Current { get; set; }
        public int amount { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public long id { get; set; }
        public double close { get; set; }
        public double open { get; set; }
    }

    public class BitkerQuoteResult
    {
        public List<BitkerQuote> tick { get; set; }
        public string status { get; set; }
        public long ts { get; set; }
    }
}