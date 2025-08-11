using System;
using Alpha.Shared.Services.Models;
using Xunit;

namespace Alpha.Shared.Services.Tests.Models
{
    public class BalanceUpDayAndMoveBetsDayModelTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var balanceUpDay = new DateTime(2025, 8, 10);
            var moveBetsDay = new DateTime(2025, 8, 11);

            // Act
            var model = new BalanceUpDayAndMoveBetsDayModel(balanceUpDay, moveBetsDay);

            // Assert
            Assert.Equal(balanceUpDay, model.BalanceUpDay);
            Assert.Equal(moveBetsDay, model.MoveBetsDay);
        }
    }
}
