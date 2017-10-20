using System;
using System.Linq;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using TestDummies;
using Xunit;

namespace Serilog.Settings.Code.Tests
{
    public class ReadFromCodeSettingsTest
    {
        [Fact]
        public void ProcessesConfigurationWithCoreComponents()
        {
            string minimalistCSharp = @"LoggerConfiguration.MinimumLevel.Warning();";

            LogEvent evt = null;

            var logger = new LoggerConfiguration()
                .ReadFrom.Code(minimalistCSharp)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            logger.Information("This should not show up");
            Assert.Null(evt);
            logger.Warning("This should show up");
            Assert.NotNull(evt);
        }

        [Fact]
        public void ProcessesSinksFromExternalDependencies()
        {
            const string cSharpWithExternalReference = @"
using TestDummies;

LoggerConfiguration
    .WriteTo.DummyRollingFile(""C:/temp/log.txt"");
";

            var extraReferencedAssemblies = new[] { typeof(DummyRollingFileSink).Assembly };
            var logger = new LoggerConfiguration()
                .ReadFrom.Code(cSharpWithExternalReference, 
                    referencedAssemblies: extraReferencedAssemblies)
                .CreateLogger();

            logger.Information("This should show up");
            var emitted = DummyRollingFileSink.Emitted.FirstOrDefault();
            Assert.NotNull(emitted);
        }
    }

    public static class LoggerConfigurationTestExtensions
    {
        public static LoggerConfiguration TestSink(this LoggerSinkConfiguration lsc)
        {
            return lsc.Sink(new DelegatingSink(e => { }));
        }
    }

    public class DelegatingSink : ILogEventSink
    {
        private readonly Action<LogEvent> _onEvent;

        public DelegatingSink(Action<LogEvent> onEvent)
        {
            _onEvent = onEvent ?? throw new ArgumentNullException(nameof(onEvent));
        }

        public void Emit(LogEvent logEvent)
        {
            _onEvent(logEvent);
        }
    }
}
