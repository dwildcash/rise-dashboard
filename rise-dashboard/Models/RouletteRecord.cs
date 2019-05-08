using System;
using System.ComponentModel.DataAnnotations;

namespace rise.Models
{
    public class RouletteRecord
    {
        public int RouletteRecordId { get; set; }

        /// <summary>
        /// Date of Transaction
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Bet Amount
        /// </summary>
        [Required]
        public double Amount { get; set; }

        /// <summary>
        /// The Lucky Number
        /// </summary>
        [Required]
        public int LuckySymbol { get; set; }

        /// <summary>
        /// Paid Amount
        /// </summary>
        [Required]
        public double AmountPaid { get; set; }

        /// <summary>
        /// Result of Transaction
        /// </summary>
        [Required]
        public bool TransactionResult { get; set; }

        /// <summary>
        /// User Associated
        /// </summary>
        [Required]
        public virtual ApplicationUser User { get; set; }
    }
}