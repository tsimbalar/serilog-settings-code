using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Settings.Code.Tests.Support;

namespace Serilog
{
    public static class ExtensionsInNameSpaceSerilog
    {
        public static LoggerConfiguration FakeSink(this LoggerSinkConfiguration loggerSinkConfiguration)
        {
            return loggerSinkConfiguration.Sink(new DelegatingSink(e => { }));
        }

        public static LoggerConfiguration FakeSink(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            ITextFormatter formatter,
            string pathFormat,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            return loggerSinkConfiguration.Sink(new DelegatingSink(evt => { }), restrictedToMinimumLevel);
        }
    }
}
