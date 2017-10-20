using System;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Settings.Code.Tests.Support
{
    public class DelegatingSink : ILogEventSink
    {
        readonly Action<LogEvent> _onEvent;

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
