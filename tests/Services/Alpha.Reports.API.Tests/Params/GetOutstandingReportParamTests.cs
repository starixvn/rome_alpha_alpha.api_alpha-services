using Alpha.Reports.API.Params;
using Alpha.Utilities.Models;
using Alpha.ApiKey.API.Models;
using GrpcReports;
using NSubstitute;
using Xunit;

namespace Alpha.Reports.API.Tests.Params
{
    public class GetOutstandingReportParamTests
    {
        [Fact]
        public void Constructor_ShouldInitializeAllProperties()
        {
            // Arrange
            byte viewerLevel = UserLevelModel.Master;
            byte custLevel = UserLevelModel.Agent;
            int custId = 123;
            string productIds = "1,2,3";
            bool autoFollowBetIsDummyAgent = true;
            string siteName = "TestSite";
            int level = 1;

            // Act
            var param = new GetOutstandingReportParam(
                viewerLevel, custLevel, custId, productIds, 
                autoFollowBetIsDummyAgent, siteName, level);

            // Assert
            Assert.Equal(viewerLevel, param.ViewerLevel);
            Assert.Equal(custLevel, param.CustLevel);
            Assert.Equal(custId, param.CustId);
            Assert.Equal(productIds, param.ProductIds);
            Assert.Equal(autoFollowBetIsDummyAgent, param.AutoFollowBetIsDummyAgent);
            Assert.Equal(siteName, param.SiteName);
            Assert.Equal(level, param.Level);
        }

        [Theory]
        [InlineData(UserLevelModel.Master, UserLevelModel.Agent, 123, 1, true)]  // Valid downline
        [InlineData(UserLevelModel.Super, UserLevelModel.Master, 456, 0, true)] // Valid downline, level 0
        [InlineData(UserLevelModel.Super, UserLevelModel.Agent, 789, 2, true)]  // Valid downline, level 2
        public void IsValid_WithValidParameters_ShouldReturnTrue(
            byte viewerLevel, byte custLevel, int custId, int level, bool expected)
        {
            // Arrange
            var param = new GetOutstandingReportParam(
                viewerLevel, custLevel, custId, "1,2,3", true, "TestSite", level);

            // Act
            var result = param.IsValid();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(UserLevelModel.Master, UserLevelModel.Agent, 0, 1, false)]      // Invalid CustId
        [InlineData(UserLevelModel.Master, UserLevelModel.Agent, -1, 1, false)]     // Negative CustId
        [InlineData(UserLevelModel.Agent, UserLevelModel.Master, 123, 1, false)]    // Upline relationship
        [InlineData(UserLevelModel.Master, UserLevelModel.Agent, 123, -1, false)]   // Invalid level
        [InlineData(UserLevelModel.Master, UserLevelModel.Agent, 123, 3, false)]    // Invalid level
        public void IsValid_WithInvalidParameters_ShouldReturnFalse(
            byte viewerLevel, byte custLevel, int custId, int level, bool expected)
        {
            // Arrange
            var param = new GetOutstandingReportParam(
                viewerLevel, custLevel, custId, "1,2,3", true, "TestSite", level);

            // Act
            var result = param.IsValid();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Constructor_WithNullStrings_ShouldHandleCorrectly()
        {
            // Act
            var param = new GetOutstandingReportParam(
                UserLevelModel.Master, UserLevelModel.Agent, 123, null, false, null, 1);

            // Assert
            Assert.Null(param.ProductIds);
            Assert.Null(param.SiteName);
            Assert.False(param.AutoFollowBetIsDummyAgent);
        }

        [Fact]
        public void Constructor_WithEmptyStrings_ShouldHandleCorrectly()
        {
            // Act
            var param = new GetOutstandingReportParam(
                UserLevelModel.Super, UserLevelModel.Master, 456, "", false, "", 0);

            // Assert
            Assert.Equal("", param.ProductIds);
            Assert.Equal("", param.SiteName);
        }
    }
}
