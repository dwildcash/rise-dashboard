namespace rise.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Success" />
    /// </summary>
    public class Success
    {
        /// <summary>
        /// Gets or sets the total
        /// </summary>
        public int total { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Quote" />
    /// </summary>
    public class Quote
    {
        /// <summary>
        /// Gets or sets the quote
        /// </summary>
        public string quote { get; set; }

        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public string length { get; set; }

        /// <summary>
        /// Gets or sets the author
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// Gets or sets the tags
        /// </summary>
        public List<string> tags { get; set; }

        /// <summary>
        /// Gets or sets the category
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// Gets or sets the date
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// Gets or sets the permalink
        /// </summary>
        public string permalink { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Gets or sets the background
        /// </summary>
        public string background { get; set; }

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public string id { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Contents" />
    /// </summary>
    public class Contents
    {
        /// <summary>
        /// Gets or sets the quotes
        /// </summary>
        public List<Quote> quotes { get; set; }

        /// <summary>
        /// Gets or sets the copyright
        /// </summary>
        public string copyright { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="QuoteOfTheDayResult" />
    /// </summary>
    public class QuoteOfTheDayResult
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static QuoteOfTheDayResult Current { get; set; }

        /// <summary>
        /// Gets or sets the success
        /// </summary>
        public Success success { get; set; }

        /// <summary>
        /// Gets or sets the contents
        /// </summary>
        public Contents contents { get; set; }
    }
}