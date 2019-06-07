using System;

namespace rise.Models
{
    public class TgPinnedMsg
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public ApplicationUser AppUser { get; set; }

        public string Message { get; set; }
    }
}