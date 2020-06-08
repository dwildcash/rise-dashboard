namespace rise.Models
{
    using System;

    public partial class Log
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Message { get; set; }
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