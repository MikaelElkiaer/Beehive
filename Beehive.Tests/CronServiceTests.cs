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
        [MemberData(nameof(ShouldRun_Theory_Data))]
        public void ShouldRun_Theory(DateTime utcNow, DateTime? nextOccurence, bool expected)
        {
            var cronService = new CronService(new Config.AppConfig(TimeSpan.FromMinutes(1), utcNow));
            cronService.IsWithinThreshold(utcNow, nextOccurence)
                .Should().Be(expected);
        }

        public static IEnumerable<object[]> ShouldRun_Theory_Data()
        {
            yield return new object[] { DateTime.UtcNow, null, false  };
            yield return new object[] { new DateTime(2019, 8, 4, 3, 37, 1, DateTimeKind.Utc), new DateTime(2019, 8, 4, 3, 38, 0, DateTimeKind.Utc), true };
            yield return new object[] { new DateTime(2019, 8, 4, 3, 37, 0, DateTimeKind.Utc), new DateTime(2019, 8, 4, 3, 38, 0, DateTimeKind.Utc), false };
            yield return new object[] { new DateTime(2019, 8, 4, 3, 37, 59, DateTimeKind.Utc), new DateTime(2019, 8, 4, 3, 39, 0, DateTimeKind.Utc), false };
        }
    }
}
