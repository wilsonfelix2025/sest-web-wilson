using System;

namespace SestWeb.Domain.Entities.PontosEntity.InternalCollections
{
    internal class HashUtilities
    {
        private static readonly int[] PrimeNumbers = new int[55]
        {
            17,
            37,
            67,
            131,
            257,
            521,
            1103,
            1327,
            1597,
            1931,
            2333,
            2801,
            3371,
            4049,
            4861,
            5839,
            7013,
            8419,
            10103,
            12143,
            14591,
            17519,
            21023,
            25229,
            30293,
            36353,
            43627,
            52361,
            62851,
            75431,
            90523,
            108631,
            130363,
            156437,
            187751,
            225307,
            270371,
            324449,
            389357,
            467237,
            560689,
            672827,
            807403,
            968897,
            1162687,
            1395263,
            1674319,
            2009191,
            2411033,
            2893249,
            3471899,
            4166287,
            4999559,
            5999471,
            7199369
        };

        internal static int SelectPrimeNumber(int hashSize)
        {
            int val1 = Array.BinarySearch<int>(HashUtilities.PrimeNumbers, hashSize);
            if (val1 < 0)
                val1 = ~val1;
            return HashUtilities.PrimeNumbers[Math.Min(val1, HashUtilities.PrimeNumbers.Length - 1)];
        }
    }
}
