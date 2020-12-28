namespace rise.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// rise Quote Object from Xt.com
    /// </summary>

    public class XtcomQuote
    {
        public string message { get; set; }
        public object method { get; set; }
        public string code { get; set; }
    }

    public class XtcomQuoteResult
    {
        public static XtcomQuoteResult Current { get; set; }
        public IList<string> datas { get; set; }
        public XtcomQuote resMsg { get; set; }
    }
}