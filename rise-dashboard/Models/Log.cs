namespace rise.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class Log
    {
        private int Id { get; set; }

        private DateTime Date { get; set; }

        private string Message { get; set; }
    }

    // Log a Message
    public partial class Log
    {
        public void LogMessage(string Message)
        {
            this.Date = DateTime.Now;
            this.Message = Message;
        }
    }
}
