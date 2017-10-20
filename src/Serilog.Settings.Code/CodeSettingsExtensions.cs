using System.Reflection;
using Serilog.Configuration;

namespace Serilog.Settings.Code
{
    public static class CodeSettingsExtensions
    {
        public static LoggerConfiguration Code(this LoggerSettingsConfiguration lsc, string csharpCode, Assembly[] referencedAssemblies = null)
        {
            return lsc.Settings(new CodeSettings(csharpCode, referencedAssemblies));
        }
    }
}