using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace rise.Models
{

    using System;

    public class RiseForceWinner
    {
        public int id { get; set; }
        public DateTime timestamp { get; set; }
        public int season { get; set; }
        public string name { get; set; }
        public string riseAddress { get; set; }
        public string transId { get; set; }
        public int score { get; set; }
        public int potSize { get; set; }

    }
    public class RiseForceTop
    {
        public DateTime timestamp { get; set; }
        public string name { get; set; }
        public int score { get; set; }

    }
    public class RiseForceResult
    {
        public string season { get; set; }
        public int recordCount { get; set; }
        public int sumScore { get; set; }
        public int avgScore { get; set; }
        public int distinctPlayers { get; set; }
        public RiseForceWinner winner { get; set; }
        public IList<RiseForceTop> top10 { get; set; }

    }
    public class Application
    {
        public bool success { get; set; }
        public RiseForceResult result { get; set; }

    }
}