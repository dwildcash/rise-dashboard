namespace rise.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// rise Transactions ex: https://wallet.rise.vision/api/transactions?recipientId=5953135380169360325R
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Gets or sets the signatures
        /// </summary>
        public List<object> signatures { get; set; }

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the rowId
        /// </summary>
        public int rowId { get; set; }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public int height { get; set; }

        /// <summary>
        /// Gets or sets the blockId
        /// </summary>
        public string blockId { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// Gets or sets the timestamp
        /// </summary>
        public int timestamp { get; set; }

        /// <summary>
        /// Gets or sets the senderPublicKey
        /// </summary>
        public string senderPublicKey { get; set; }

        /// <summary>
        /// Gets or sets the senderId
        /// </summary>
        public string senderId { get; set; }

        /// <summary>
        /// Gets or sets the recipientId
        /// </summary>
        public string recipientId { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public long amount { get; set; }

        /// <summary>
        /// Gets or sets the fee
        /// </summary>
        public long fee { get; set; }

        /// <summary>
        /// Gets or sets the signature
        /// </summary>
        public string signature { get; set; }

        /// <summary>
        /// Gets or sets the signSignature
        /// </summary>
        public string signSignature { get; set; }

        /// <summary>
        /// Gets or sets the requesterPublicKey
        /// </summary>
        public object requesterPublicKey { get; set; }

        /// <summary>
        /// Gets or sets the asset
        /// </summary>
        public Asset asset { get; set; }

        /// <summary>
        /// Gets or sets the confirmations
        /// </summary>
        public int confirmations { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="TransactionsResult" />
    /// </summary>
    public class TransactionsResult
    {
        /// <summary>
        /// Gets or sets the Current
        /// </summary>
        public static TransactionsResult Current { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether success
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// Gets or sets the transactions
        /// </summary>
        public List<Transaction> transactions { get; set; }

        /// <summary>
        /// Gets or sets the count
        /// </summary>
        public string count { get; set; }
    }
}