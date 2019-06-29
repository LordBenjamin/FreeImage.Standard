using BenchmarkDotNet.Running;
using Benchmarking.Benchmarks;

namespace Benchmarking
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
#if !DEBUG
            BenchmarkRunner.Run<StreamReadBenchmark>();
#else
            StreamReadBenchmark b = new StreamReadBenchmark();
            b.Setup();
            for (int i = 0; i < 10000; i++)
                b.ReadSpan();
            b.Cleanup();
#endif
        }
    }
}
