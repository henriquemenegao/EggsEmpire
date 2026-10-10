
using System.Collections.Generic;

namespace Creatures
{
    public static class CreatureMergeSimulator
    {
        public const int MaxLevel = 18;

        public static bool TrySimulate(
            IReadOnlyList<int> currentLevels,
            int purchaseLevel,
            int purchaseAmount,
            int slotCapacity,
            out List<int> result)
        {
            result = new List<int>();

            if (currentLevels == null ||
                purchaseLevel < 1 ||
                purchaseLevel > MaxLevel ||
                purchaseAmount <= 0 ||
                slotCapacity <= 0)
            {
                return false;
            }

            long[] counts = new long[MaxLevel + 1];

            foreach (int level in currentLevels)
            {
                if (level < 1 || level > MaxLevel)
                    return false;

                counts[level]++;
            }

            counts[purchaseLevel] += purchaseAmount;

            long total = currentLevels.Count + (long)purchaseAmount;

            // Ao atingir 100 slots, realiza todos os
            // merges possíveis, incluindo cascatas.
            if (total >= slotCapacity)
            {
                for (int level = 1; level < MaxLevel; level++)
                {
                    long merges = counts[level] / 3;

                    if (merges == 0)
                        continue;

                    counts[level] -= merges * 3;
                    counts[level + 1] += merges;

                    total -= merges * 2;
                }
            }

            // Bloqueia somente se continuar acima
            // da capacidade após os merges.
            if (total > slotCapacity)
                return false;

            // Organiza do maior nível para o menor.
            for (int level = MaxLevel; level >= 1; level--)
            {
                for (long i = 0; i < counts[level]; i++)
                    result.Add(level);
            }

            return true;
        }
        
        
        public static bool HasAvailableMerge(
            IReadOnlyList<int> currentLevels)
        {
            if (currentLevels == null)
                return false;

            int[] counts = new int[MaxLevel + 1];

            foreach (int level in currentLevels)
            {
                if (level < 1 || level > MaxLevel)
                    return false;

                if (level == MaxLevel)
                    continue;

                counts[level]++;

                if (counts[level] >= 3)
                    return true;
            }

            return false;
        }

        public static List<int> SimulateManualMerge(
            IReadOnlyList<int> currentLevels)
        {
            List<int> result = new();

            if (currentLevels == null)
                return result;

            long[] counts = new long[MaxLevel + 1];

            foreach (int level in currentLevels)
            {
                if (level < 1 || level > MaxLevel)
                    return result;

                counts[level]++;
            }

            // Executa todos os merges, inclusive cascatas.
            for (int level = 1; level < MaxLevel; level++)
            {
                long merges = counts[level] / 3;

                counts[level] -= merges * 3;
                counts[level + 1] += merges;
            }

            // Mantém a ordenação da fábrica.
            for (int level = MaxLevel; level >= 1; level--)
            {
                for (long i = 0; i < counts[level]; i++)
                    result.Add(level);
            }

            return result;
        }

    }
}
