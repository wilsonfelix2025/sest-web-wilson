using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace SestWeb.Domain.Tests.CorrelaçõesTests
{
    [SetUpFixture]
    public class SetUpMuparserTests
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            try
            {
                //ExtractResourceToFile(@"SestWeb.Domain.muparser64.dll", "muparser64.dll");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            // ...
        }

        private static void ExtractResourceToFile(string resourceName, string filename)
        {
            Assembly assemblyToLoadFrom = Assembly.GetExecutingAssembly();
            if (!File.Exists(filename))
            {
                using (Stream s = assemblyToLoadFrom.GetManifestResourceStream(resourceName))
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, b.Length);
                    fs.Write(b, 0, b.Length);
                }
            }
        }

        private static string GetFileContents(string sampleFile)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format("SestWeb.Domain.Tests.{0}", sampleFile);
            using (var stream = asm.GetManifestResourceStream(resource))
            {
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
            }
            return string.Empty;
        }
    }
}