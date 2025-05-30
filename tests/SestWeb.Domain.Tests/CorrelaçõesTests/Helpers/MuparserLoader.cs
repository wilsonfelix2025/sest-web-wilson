using System;
using System.IO;
using System.Reflection;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.Helpers
{
    public static class MuparserLoader
    {
        static MuparserLoader()
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

        private static void ExtractResourceToFile(string resourceName, string filename)
        {
            Assembly assemblyToLoadFrom = Assembly.GetAssembly(typeof(Parser));
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
    }
}
