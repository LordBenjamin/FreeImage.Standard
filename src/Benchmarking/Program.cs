using BenchmarkDotNet.Running;
using Benchmarking.Benchmarks;

namespace Benchmarking
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
#if !DEBUG
            BenchmarkRunner.Run<StreamWriteBenchmark>();
#else
            StreamWriteBenchmark b = new StreamWriteBenchmark();
            b.Setup();
            for (int i = 0; i < 10000; i++)
                b.WriteSpan();
            b.Cleanup();
#endif
        }
    }
}
