namespace rise.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Language" />
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the native
        /// </summary>
        public string native { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="Location" />
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the geoname_id
        /// </summary>
        public int? geoname_id { get; set; }

        /// <summary>
        /// Gets or sets the capital
        /// </summary>
        public string capital { get; set; }

        /// <summary>
        /// Gets or sets the languages
        /// </summary>
        public IList<Language> languages { get; set; }

        /// <summary>
        /// Gets or sets the country_flag
        /// </summary>
        public string country_flag { get; set; }

        /// <summary>
        /// Gets or sets the country_flag_emoji
        /// </summary>
        public string country_flag_emoji { get; set; }

        /// <summary>
        /// Gets or sets the country_flag_emoji_unicode
        /// </summary>
        public string country_flag_emoji_unicode { get; set; }

        /// <summary>
        /// Gets or sets the calling_code
        /// </summary>
        public string calling_code { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is_eu
        /// </summary>
        public bool is_eu { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="IPData" />
    /// </summary>
    public class IPData
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ip
        /// </summary>
        public string ip { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the continent_code
        /// </summary>
        public string continent_code { get; set; }

        /// <summary>
        /// Gets or sets the continent_name
        /// </summary>
        public string continent_name { get; set; }

        /// <summary>
        /// Gets or sets the country_code
        /// </summary>
        public string country_code { get; set; }

        /// <summary>
        /// Gets or sets the country_name
        /// </summary>
        public string country_name { get; set; }

        /// <summary>
        /// Gets or sets the region_code
        /// </summary>
        public string region_code { get; set; }

        /// <summary>
        /// Gets or sets the region_name
        /// </summary>
        public string region_name { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// Gets or sets the zip
        /// </summary>
        public string zip { get; set; }

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        public double latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        public double longitude { get; set; }

        /// <summary>
        /// Gets or sets the location
        /// </summary>
        public Location location { get; set; }
    }
}

/**
 * https://www.evertechie.com/get-geolocation-for-ip-address-c/ *
 */