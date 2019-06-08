using BenchmarkDotNet.Running;
using Benchmarking.Benchmarks;

namespace Benchmarking
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<StreamBenchmark>();
        }
    }
}
