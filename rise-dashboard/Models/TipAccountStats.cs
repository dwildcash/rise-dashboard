using System;
using System.Collections.Generic;

namespace rise.Models
{
    public static class TipAccountStats
    {
        public static double TotalBalance { get; set; }

        public static int TotalTransactions { get; set; }
        public static double TotalAmountTransactions { get; set; }
        public static DateTime LastGenerated { get; set; }
        public static List<string> AddressLst {get;set;}
        public static int UsersCount { get; set; }
    }
}