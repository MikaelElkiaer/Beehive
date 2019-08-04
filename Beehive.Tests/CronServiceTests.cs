using Beehive.Model;
using Beehive.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Beehive.Tests
{
    public class CronServiceTests
    {
        [Theory]
        [MemberData(nameof(GetNextOccurence_Theory_Data))]
        public void GetNextOccurence_Theory(string cronText, DateTime nowUtc, TimeZoneInfo timeZoneInfo, DateTime? expected)
        {
            CronService.GetNextOccurence(cronText, nowUtc, timeZoneInfo)
                .Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("*****")]
        [InlineData("* * * * * * *")]
        public void GetNextOccurence_Theory_Throws(string cronText)
        {
            Action action = () => CronService.GetNextOccurence(cronText, DateTime.MinValue, TimeZoneInfo.Utc);
            action.Should().ThrowExactly<CronParseException>();
        }

        public static IEnumerable<object[]> GetNextOccurence_Theory_Data()
        {
            yield return new object[] {
                "* * * * *",
                new DateTime(2019, 8, 4, 14, 6, 0, DateTimeKind.Utc),
                TimeZoneInfo.Utc,
                new DateTime(2019, 8, 4, 14, 6, 0, DateTimeKind.Utc)
            };
            yield return new object[] {
                "0 7 * * *",
                new DateTime(2019, 8, 4, 14, 11, 0, DateTimeKind.Utc),
                TimeZoneInfo.Utc,
                new DateTime(2019, 8, 5, 7, 0, 0, DateTimeKind.Utc)
            };
            yield return new object[] {
                "0 7 * * *",
                new DateTime(2019, 8, 4, 14, 11, 0, DateTimeKind.Utc),
                TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen"),
                new DateTime(2019, 8, 5, 5, 0, 0, DateTimeKind.Utc)
            };
        }

        [Theory]
        [MemberData(nameof(ShouldRun_Theory_Data))]
        public void ShouldRun_Theory(DateTime utcNow, DateTime? nextOccurenceUtc, TimeSpan threshold, bool expected)
        {
            CronService.IsWithinThreshold(utcNow, nextOccurenceUtc, threshold)
                .Should().Be(expected);
        }

        public static IEnumerable<object[]> ShouldRun_Theory_Data()
        {
            yield return new object[] {
                DateTime.UtcNow,
                null,
                TimeSpan.FromMinutes(1),
                false
            };
            yield return new object[] {
                new DateTime(2019, 8, 4, 3, 37, 1, DateTimeKind.Utc),
                new DateTime(2019, 8, 4, 3, 38, 0, DateTimeKind.Utc),
                TimeSpan.FromMinutes(1),
                true
            };
            yield return new object[] {
                new DateTime(2019, 8, 4, 3, 37, 0, DateTimeKind.Utc),
                new DateTime(2019, 8, 4, 3, 38, 0, DateTimeKind.Utc),
                TimeSpan.FromMinutes(1),
                false
            };
            yield return new object[] {
                new DateTime(2019, 8, 4, 3, 37, 59, DateTimeKind.Utc),
                new DateTime(2019, 8, 4, 3, 39, 0, DateTimeKind.Utc),
                TimeSpan.FromMinutes(1),
                false };
        }
    }
}
