using Beehive.Config;
using Beehive.Services;
using Beehive.Utils;
using FakeItEasy;
using FluentAssertions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        [MemberData(nameof(GetTimeZoneInfo_Theory_Data))]
        public void GetTimeZoneInfoWithUtcFallback_Success_Theory(string tz, TimeSpan baseUtcOffset, bool supportsDaylightSavingTime)
        {
            var timeZoneInfo = new TimeZoneService(fakeLogger).GetTimeZoneInfoWithUtcFallback(tz);

            timeZoneInfo.Should().NotBeNull();
            timeZoneInfo.BaseUtcOffset.Should().Be(baseUtcOffset);
            timeZoneInfo.SupportsDaylightSavingTime.Should().Be(supportsDaylightSavingTime);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("nonsense")]
        public void GetTimeZoneInfoWithUtcFallback_Fail_Theory(string tz)
        {
            var timeZoneInfo = new TimeZoneService(fakeLogger).GetTimeZoneInfoWithUtcFallback(tz);

            timeZoneInfo.Should().NotBeNull();
            timeZoneInfo.Id.Should().Be("UTC");
        }

        public static IEnumerable<object[]> GetTimeZoneInfo_Theory_Data()
        {
            yield return new object[] {
                "Europe/Copenhagen", TimeSpan.FromHours(1), true
            };
            yield return new object[] {
                "UTC", TimeSpan.Zero, false
            };
        }
    }
}
