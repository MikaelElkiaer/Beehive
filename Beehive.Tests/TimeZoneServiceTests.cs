using Beehive.Services;
using FakeItEasy;
using FluentAssertions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Beehive.Tests
{
    public class TimeZoneServiceTests
    {
        private readonly ILogger fakeLogger;

        public TimeZoneServiceTests()
        {
            fakeLogger = A.Fake<ILogger>();
            A.CallTo(fakeLogger).WithVoidReturnType().DoesNothing();
            A.CallTo(fakeLogger).WithReturnType<ILogger>().Returns(fakeLogger);
        }

        [Theory]
        [InlineData("Europe/Copenhagen", "Romance Standard Time")]
        [InlineData("UTC", "UTC")]
        public void GetTimeZoneInfo(string tz, string timeZoneId)
        {
            var timeZoneInfo = new TimeZoneService(fakeLogger).GetTimeZoneInfo(tz);

            timeZoneInfo.Should().NotBeNull();
            timeZoneInfo.Id.Should().Be(timeZoneId);
        }
    }
}
