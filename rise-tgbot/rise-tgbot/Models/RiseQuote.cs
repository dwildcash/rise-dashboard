using System;

namespace rise_tgbot.Models
{
    public class RiseQuote
    {
        public int id { get; set; }
        public DateTime timeStamp { get; set; }
        public string exchange { get; set; }
        public double price { get; set; }
        public double volume { get; set; }
        public double usdPrice { get; set; }
    }
}