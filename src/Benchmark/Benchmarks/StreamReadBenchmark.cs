using System.IO;
using BenchmarkDotNet.Attributes;
using FreeImageAPI;
using FreeImageAPI.IO;

namespace Benchmarking.Benchmarks
{
    [CoreJob]
    [ClrJob]
    [MemoryDiagnoser]
    public class StreamReadBenchmark
    {
        private Stream stream;

        [GlobalSetup]
        public void Setup()
        {
            FIBITMAP bmp = FreeImage.Allocate(1000, 1000, 24);
            stream = new MemoryStream(3000054);
            FreeImage.SaveToStream(bmp, stream, FREE_IMAGE_FORMAT.FIF_BMP);

            FreeImage.Unload(bmp);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            stream.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void Legacy()
        {
            FreeImage.IO = FreeImageStreamIO.IO;
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

            FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
            FIBITMAP dib = FreeImage.LoadFromStream(stream, ref format);
            FreeImage.Unload(dib);
        }
    }
}
