namespace rise_lib.Models
{
    public class Account
    {
        public string address { get; set; }
        public string publicKey { get; set; }
        public string secret { get; set; }
    }

    public class AccountResult
    {
        public bool success { get; set; }
        public Account account { get; set; }
    }
}