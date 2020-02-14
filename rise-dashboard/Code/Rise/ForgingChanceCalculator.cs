namespace rise.Code.Rise
{
    using Helpers;
    using Models;
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
            var rnd = new Random();
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
            workingDelegatesLst = delegateResult.Delegates.Where(x => x.Infos.rank <= 199).OrderBy(x => x.Infos.rank).ToList();

            var totalWeight = workingDelegatesLst.Sum(x => x.VotesWeight);

            double slotProb = 0;
            double chance = 0;

            for (int slot = 0; slot < 101; slot++)
            {
                slotProb = workingDelegatesLst.FirstOrDefault(x => x.Infos.rank == slot + 1).VotesWeight;

                foreach (var dele in workingDelegatesLst)
                {
                    if (dele.Infos.rank != slot + 1)
                    {
                        chance = (double)dele.VotesWeight / ((double)totalWeight - slotProb);
                    }
                    else
                    {
                        chance = (double)dele.VotesWeight / (double)totalWeight;
                    }

                    slotProb -= dele.VotesWeight;

                    var currdel = delegateResult.Delegates.FirstOrDefault(x => x.Address == dele.Address);

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
                workingDelegatesLst = delegateResult.Delegates.Where(x => x.Infos.rank <= 199).ToList();

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