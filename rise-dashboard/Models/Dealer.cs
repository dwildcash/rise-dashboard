
using rise.Helpers;

namespace rise.Models
{
    public class Dealer
    {

        public Dealer()
        {
            var arr1 = new[] { 0, 1, 2, 3, 5, 0, 1, 2, 3, 3, 0, 1, 2, 3, 4, 0, 1, 2, 3, 0, 0, 1, 3, 2, 3, 0, 4, 0, 1, 2, 3, 5, 0, 6, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3, 7, 0, 1, 2, 3, 0, 3, 5, 0, 4, 2, 1, 0, 4, 3, 2, 5, 1, 4, 0, 3, 2, 1, 5, 0, 6, 2, 1, 3, 3, 0, 2, 3, 1, 0, 6, 3, 2, 1, 0, 1, 2, 0, 1, 3, 3, 0, 2, 4, 2, 1 };

            WinNumber = int.Parse(RandomGenerator.NextLong(0, 99).ToString());
            AmountToPay = CalculteAmountToPay();
        }

        /// <summary>
        /// Get the Winning Number
        /// </summary>
        /// <returns></returns>
        public int WinNumber { get; }

        /// <summary>
        /// Return the amount to pay
        /// </summary>
        public int AmountToPay { get; }

        /// <summary>
        /// Generate the Amount to pat
        /// </summary>
        /// <returns></returns>
        private int CalculteAmountToPay()
        {
            switch (WinNumber)
            {
                // 1%
                // 3%
                case 7:
                    return 50;
                // 5%
                case 6:
                    return 6;
                // 10%
                case 5:
                    return 4;
                case 4:
                    return 2;
                default:
                    // User lost -1.
                    return -1;
            }
        }
    }
}