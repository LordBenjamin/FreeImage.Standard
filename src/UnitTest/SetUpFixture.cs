using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace FreeImageNETUnitTest
{
    [SetUpFixture]
    public class SetUpFixture
    {

        [OneTimeSetUp]
        public void Init()
        {
            string dir = Path.GetDirectoryName(typeof(SetUpFixture).GetTypeInfo().Assembly.Location);
            Directory.SetCurrentDirectory(dir);

            NativeLibraryLoader.CopyFreeImageNativeDll();
        }

        [OneTimeTearDown]
        public void DeInit()
        {
        }
    }
}
