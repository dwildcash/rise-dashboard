namespace rise.Code.Rise
{
    using rise.Helpers;
    using rise.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="ForgingChanceCalculator" />
    /// </summary>
    public static class ForgingChanceCalculator
    {
        /// <summary>
        /// Defines the workingDelegatesLst
        /// </summary>
        private static List<Models.Delegate> workingDelegatesLst;

        /// <summary>
        /// Get a Random number from 1 to sum of weight
        /// </summary>
        /// <returns></returns>
        private static long GenerateRandomWeight()
        {
            Random rnd = new Random();
            var e = RandomGenerator.NextLong(1, SumofWeight());
            return RandomGenerator.NextLong(1, SumofWeight());
        }

        /// <summary>
        /// Return the Sum of Weight
        /// </summary>
        /// <returns></returns>
        private static long SumofWeight()
        {
            return workingDelegatesLst.Sum(x => x.VotesWeight);
        }

        public static void generateForgingStat2(DelegateResult delegateResult)
        {
            workingDelegatesLst = delegateResult.Delegates.Where(x => x.Rank <= 199).OrderBy(x => x.Rank).ToList();

            var totalWeight = workingDelegatesLst.Sum(x => x.VotesWeight);

            double slotProb = 0;
            double chance = 0;

            for (int slot = 0; slot < 101; slot++)
            {
                slotProb = workingDelegatesLst.Where(x => x.Rank == slot + 1).FirstOrDefault().VotesWeight;

                foreach (var dele in workingDelegatesLst)
                {
                    if (dele.Rank != slot + 1)
                    {
                        chance = (double)dele.VotesWeight / ((double)totalWeight - slotProb);
                    }
                    else
                    {
                        chance = (double)dele.VotesWeight / (double)totalWeight;
                    }

                    slotProb -= dele.VotesWeight;

                    var currdel = delegateResult.Delegates.Where(x => x.Address == dele.Address).FirstOrDefault();
                    if (currdel.Username == "dwildcash")
                    {
                        System.Diagnostics.Debug.WriteLine("Slot " + slot + " Delegate " + currdel.Username + " chance " + chance);
                    }
                    currdel.Chance2 += chance;
                }
            }

            var e = delegateResult;
        }

        /// <summary>
        /// The SimulateForgingRounds
        /// </summary>
        /// <param name="delegateResult">The delegateResult<see cref="DelegateResult"/></param>
        /// <returns>The <see cref="DelegateResult"/></returns>
        public static DelegateResult SimulateForgingRounds(DelegateResult delegateResult)
        {
            // Simulate X forging rounds
            for (int j = 0; j < AppSettingsProvider.SimulateRoundCount; j++)
            {
                workingDelegatesLst = delegateResult.Delegates.Where(x => x.Rank <= 199).ToList();

                // Generate the possibilities for 101 slot
                for (int slot = 0; slot < 101; slot++)
                {
                    var randomWeight = GenerateRandomWeight();

                    foreach (var del in workingDelegatesLst)
                    {
                        randomWeight -= del.VotesWeight;
                        if (randomWeight <= 0)
                        {
                            // Got it!
                            delegateResult.Delegates.Where(x => x.Address == del.Address).FirstOrDefault().AddChance += 1;
                            workingDelegatesLst.Remove(del);
                            break;
                        }
                    }
                }
            }

            return delegateResult;
        }
    }
}