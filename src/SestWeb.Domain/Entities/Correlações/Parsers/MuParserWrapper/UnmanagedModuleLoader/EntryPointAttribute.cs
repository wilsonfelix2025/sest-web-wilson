using System;

namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper.UnmanagedModuleLoader
{
    /// <summary>
    /// Allows the specification of the unmanaged entry point.
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate)]
    public class EntryPointAttribute : Attribute
    {
        public string EntryPoint { get; set; }
    }
}
