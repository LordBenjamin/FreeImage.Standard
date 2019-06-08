using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using FreeImageAPI;
using FreeImageAPI.IO;

namespace Benchmarking.Benchmarks
{
    [CoreJob]
    [ClrJob]
    [MemoryDiagnoser]
    public class StreamBenchmark
    {
        private FIBITMAP bmp;
        private Stream stream;

        [GlobalSetup]
        public void Setup()
        {
            bmp = FreeImage.Allocate(1000, 1000, 24);
            stream = new MemoryStream(3000054);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            FreeImage.Unload(bmp);
            stream.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void Legacy()
        {
            FreeImage.IO = LegacyFreeImageStreamIO.IO;
            Exec();
        }

        [Benchmark]
        public void Span()
        {
            FreeImage.IO = SpanStreamIO.IO;
            Exec();
        }

        private void Exec()
        {
            stream.Seek(0, SeekOrigin.Begin);
            FreeImage.SaveToStream(bmp, stream, FREE_IMAGE_FORMAT.FIF_BMP);
        }
    }
}
