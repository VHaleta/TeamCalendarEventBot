using Microsoft.Extensions.Logging;
using NSubstitute;

namespace TeamCalendarEventBot.Tests
{
    public class LoggerFixture<T>
    {
        public ILogger<T> Logger { get; private set; }

        public LoggerFixture()
        {
            Logger = Substitute.For<ILogger<T>>();
        }
    }
}
