using System.Linq;
using Serilog.Events;
using Serilog.Settings.Code.Tests.Support;
using TestDummies;
using Xunit;

namespace Serilog.Settings.Code.Tests
{
    public class ReadFromCodeSettingsTest
    {
        [Fact]
        public void ProcessesConfigurationWithCoreComponents()
        {
            var minimalistCSharp = "LoggerConfiguration.MinimumLevel.Warning();";

            LogEvent evt = null;

            var logger = new LoggerConfiguration()
                .ReadFrom.CodeString(minimalistCSharp)
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
            var cSharpWithExternalReference = @"
using TestDummies;

LoggerConfiguration
    .WriteTo.DummyRollingFile(""C:/temp/log.txt"");
";

            var extraReferencedAssemblies = new[] { typeof(DummyRollingFileSink).Assembly };
            var logger = new LoggerConfiguration()
                .ReadFrom.CodeString(cSharpWithExternalReference,
                    referencedAssemblies: extraReferencedAssemblies)
                .CreateLogger();

            logger.Information("This should show up");
            var emitted = DummyRollingFileSink.Emitted.FirstOrDefault();
            Assert.NotNull(emitted);
        }
    }
}
