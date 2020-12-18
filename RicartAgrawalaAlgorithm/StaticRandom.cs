using System;
using System.Threading;

namespace RicartAgrawalaAlgorithm
{
    public static class StaticRandom
    {
        private static int _seed = Environment.TickCount;

        public static readonly ThreadLocal<Random> Random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        public static int Rand(int min, int max)
        {
            return Random.Value.Next(min, max);
        }
    }
}