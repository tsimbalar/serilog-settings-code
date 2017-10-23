using System.IO;
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
        public void SerilogNameSpaceIsImportedByDefault()
        {
            // FakeSink() is an extension method in namespace Serilog
            var csharpWithBuiltInTypes = @"
LoggerConfiguration
    .WriteTo.FakeSink();";

            LogEvent evt = null;
            var extraReferencedAssemblies = new[] { typeof(ReadFromCodeSettingsTest).Assembly };
            var logger = new LoggerConfiguration()
                .ReadFrom.CodeString(csharpWithBuiltInTypes, extraReferencedAssemblies)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            logger.Information("This should not show up");
            Assert.NotNull(evt);
        }

        [Fact]
        public void ProcessesConfigurationWithBuiltInTypesWithFullyQualifiedName()
        {
            var csharpWithBuiltInTypes = @"
LoggerConfiguration
    .WriteTo.FakeSink(
        new Serilog.Formatting.Json.JsonFormatter(),
        @""C:\temp\log.log"",
        Serilog.Events.LogEventLevel.Debug);
";

            LogEvent evt = null;

            var extraReferencedAssemblies = new[] { typeof(ReadFromCodeSettingsTest).Assembly };

            var logger = new LoggerConfiguration()
                .ReadFrom.CodeString(csharpWithBuiltInTypes, extraReferencedAssemblies)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            logger.Information("This should not show up");
            Assert.NotNull(evt);
        }

        [Fact]
        public void ProcessesConfigurationWithBuiltInTypesWithUsings()
        {
            var csharpWithBuiltInTypes = @"
using Serilog.Formatting.Json;
using Serilog.Events;

LoggerConfiguration
    .WriteTo.FakeSink(
        new JsonFormatter(),
        @""C:\temp\log.log"",
        LogEventLevel.Debug);
";

            LogEvent evt = null;

            var extraReferencedAssemblies = new[] { typeof(ReadFromCodeSettingsTest).Assembly };

            var logger = new LoggerConfiguration()
                .ReadFrom.CodeString(csharpWithBuiltInTypes, extraReferencedAssemblies)
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            logger.Information("This should not show up");
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

        [Fact]
        public void LoadsExternalDependenciesWithHashtagR()
        {
            var dllPath = Path.Combine(System.Environment.CurrentDirectory, "TestDummies.dll");
            var cSharpWithExternalReference = $@"
#r ""{dllPath}""
using TestDummies;

LoggerConfiguration
    .WriteTo.DummyRollingFile(""C:/temp/log.txt"");
";
            var logger = new LoggerConfiguration()
                .ReadFrom.CodeString(cSharpWithExternalReference)
                .CreateLogger();

            logger.Information("This should show up");
            var emitted = DummyRollingFileSink.Emitted.FirstOrDefault();
            Assert.NotNull(emitted);

        }
    }
}
