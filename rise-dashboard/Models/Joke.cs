using System.Collections.Generic;

namespace rise.Models
{
    public class Joke
    {
        public class Attachment
        {
            public string fallback { get; set; }
            public string footer { get; set; }
            public string text { get; set; }
        }

        public class JokeResult
        {
            public List<Attachment> attachments { get; set; }
            public string response_type { get; set; }
            public string username { get; set; }
        }
    }
}