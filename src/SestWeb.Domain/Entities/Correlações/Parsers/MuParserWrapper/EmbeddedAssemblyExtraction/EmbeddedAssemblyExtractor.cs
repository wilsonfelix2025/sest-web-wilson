using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper.EmbeddedAssemblyExtraction
{
    public static class EmbeddedAssemblyExtractor
    {
        private const string TestProjectName = "SestWeb.Domain.Tests";
        private const string DomainProjectName = "SestWeb.Domain";

        public static ExtractionResult ExtractEmbeddedAssemblyToFile(string filename)
        {
            try
            {
                var assemblyDirectory = GetAssemblyDirectory();
                var resourceName = GetResourceName(assemblyDirectory, filename);
                var assemblyName = GetAssemblyName(assemblyDirectory, filename);
                var assemblyPath = GetAssemblyPathPath(assemblyDirectory, assemblyName);
                Assembly assemblyToLoadFrom = LoadAssemblyFromPath(assemblyPath);

                LoadAssembly(filename, assemblyToLoadFrom, resourceName);
                return new ExtractionResult(ExtractionResultStatus.Sucesso);
            }
            catch (Exception e)
            {
                return new ExtractionResult(ExtractionResultStatus.Falha,e.Message);
            }
        }

        private static void LoadAssembly(string filename, Assembly assemblyToLoadFrom, string resourceName)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (Stream s = assemblyToLoadFrom.GetManifestResourceStream(resourceName))
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                byte[] b = new byte[s.Length];
                s.Read(b, 0, b.Length);
                fs.Write(b, 0, b.Length);
            }
        }

        private static Assembly LoadAssemblyFromPath(string assemblyPath)
        {
            return Assembly.LoadFrom(assemblyPath);
        }

        private static string GetAssemblyPathPath(string assemblyDirectory, string assemblyName)
        {
            return Path.GetFullPath(Path.Combine(assemblyDirectory, assemblyName));
        }

        private static string GetResourceName(string assemblyDirectory, string filename)
        {
            string resourceName = string.Empty;

            if (IsTestProject(assemblyDirectory))
            {
                resourceName = $"{TestProjectName}.{filename}";
            }
            else if (IsDomainProject(assemblyDirectory, filename))
            {
                resourceName = $"{DomainProjectName}.{filename}";
            }

            return resourceName;
        }

        private static string GetAssemblyName(string assemblyDirectory, string filename)
        {
            string assemblyName = string.Empty;

            if (IsTestProject(assemblyDirectory))
            {
                assemblyName = $"{TestProjectName}.dll";
            }
            else if (IsDomainProject(assemblyDirectory, filename))
            {
                assemblyName = $"{DomainProjectName}.dll";
            }

            return assemblyName;
        }

        private static bool IsDomainProject(string assemblyDirectory, string filename)
        {
            return assemblyDirectory.Contains($"{DomainProjectName}.{filename}");
        }

        private static bool IsTestProject(string assemblyDirectory)
        {
            return assemblyDirectory.Contains(TestProjectName);
        }


        public static Stream GetEmbeddedResourceStream
            (this Assembly assembly, string relativeResourcePath)
        {
            if (string.IsNullOrEmpty(relativeResourcePath))
                throw new ArgumentNullException("relativeResourcePath");

            var resourcePath = String.Format("{0}.{1}",
                Regex.Replace(assembly.ManifestModule.Name, @"\.(exe|dll)$",
                    string.Empty, RegexOptions.IgnoreCase), relativeResourcePath);

            var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                throw new ExtractionException(String.Format("The specified embedded resource\"{0}\" is not found.", relativeResourcePath));
            }

            return stream;
        }

        public static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
