using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace rise.Models
{
    public class RollHundredRecord
    {
        public int RollHundredRecordId { get; set; }

        /// <summary>
        /// Date of Transaction
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Set under or Over
        /// </summary>
        public RollOptions Options { get; set; }

        /// <summary>
        /// Bet Amount
        /// </summary>
        [Required]
        public double Amount { get; set; }


        /// <summary>
        /// The number user picked
        /// </summary>
        [Required]
        public int PickedNumber { get; set; }

        /// <summary>
        /// The Lucky Number
        /// </summary>
        [Required]
        public int LuckyNumber { get; set; }


        /// <summary>
        /// The multiplier
        /// </summary>
        [Required]
        public double Multiplier { get; set; }


        /// <summary>
        /// Paid Amount
        /// </summary>
        [Required]
        public double AmountPaid { get; set; }

        /// <summary>
        /// Winner or Not
        /// </summary>
        [Required]
        public bool Winner { get; set; }

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

    public enum RollOptions
    {
        Under,
        Over
    }
}
