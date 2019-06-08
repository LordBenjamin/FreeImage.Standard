using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FreeImageAPI;
using FreeImageAPI.IO;
using NUnit.Framework;

namespace UnitTest.TestFixtures
{
    [TestFixture]
    public class IOTest
    {
        [Test]
        public void WriteStream()
        {
            FIBITMAP dib = FreeImage.Allocate(1000, 800, 24, 0xFF0000, 0xFF00, 0xFF);
            Assert.IsFalse(dib.IsNull);

            using (MemoryStream stream1 = new MemoryStream())
            using (MemoryStream stream2 = new MemoryStream())
            {
                FreeImage.IO = FreeImageStreamIO.IO;
                FreeImage.SaveToStream(dib, stream1, FREE_IMAGE_FORMAT.FIF_BMP);
                Assert.Greater(stream1.Position, 0);

                FreeImage.IO = SpanStreamIO.IO;
                FreeImage.SaveToStream(dib, stream2, FREE_IMAGE_FORMAT.FIF_BMP);
                Assert.Greater(stream2.Position, 0);

                Assert.IsTrue(Enumerable.SequenceEqual(stream1.GetBuffer(), stream2.GetBuffer()));
            }

            FreeImage.UnloadEx(ref dib);
        }
        [Test]
        public void ReadStream()
        {
            FIBITMAP dib = FreeImage.Allocate(1000, 800, 24, 0xFF0000, 0xFF00, 0xFF);
            Assert.IsFalse(dib.IsNull);

            using (MemoryStream stream = new MemoryStream())
            {
                FreeImage.IO = FreeImageStreamIO.IO;
                FreeImage.SaveToStream(dib, stream, FREE_IMAGE_FORMAT.FIF_BMP);
                Assert.Greater(stream.Position, 0);

                stream.Seek(0, SeekOrigin.Begin);

                FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
                FIBITMAP dib2 = FreeImage.LoadFromStream(stream, ref format);
                Assert.IsFalse(dib2.IsNull);
                Assert.IsTrue(FreeImage.Compare(dib, dib2, FREE_IMAGE_COMPARE_FLAGS.COMPLETE));
                FreeImage.UnloadEx(ref dib2);
                Assert.IsTrue(dib2.IsNull);

                FreeImage.IO = FreeImageStreamIO.IO;
                stream.Seek(0, SeekOrigin.Begin);

                dib2 = FreeImage.LoadFromStream(stream, ref format);
                Assert.IsFalse(dib2.IsNull);
                Assert.IsTrue(FreeImage.Compare(dib, dib2, FREE_IMAGE_COMPARE_FLAGS.COMPLETE));
                FreeImage.UnloadEx(ref dib2);
                Assert.IsTrue(dib2.IsNull);
            }

            FreeImage.UnloadEx(ref dib);
        }
    }
}
