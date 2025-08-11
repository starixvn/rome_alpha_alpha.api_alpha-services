using Alpha.Customer.API.Models;
using Xunit;

namespace Alpha.Customer.API.Tests.Models;

public class DownlineInfoTests
{
    [Fact]
    public void DownlineInfo_CanBeCreated_WithAllProperties()
    {
        // Arrange & Act
        var downlineInfo = new DownlineInfo(
            CustId: 54321,
            RoleId: 3,
            IsSubAcc: true
        );

        // Assert
        Assert.Equal(54321, downlineInfo.CustId);
        Assert.Equal(3, downlineInfo.RoleId);
        Assert.True(downlineInfo.IsSubAcc);
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(999999, 255, false)]
    [InlineData(0, 0, false)]
    public void DownlineInfo_WorksWithVariousValues(int custId, byte roleId, bool isSubAcc)
    {
        // Arrange & Act
        var downlineInfo = new DownlineInfo(custId, roleId, isSubAcc);

        // Assert
        Assert.Equal(custId, downlineInfo.CustId);
        Assert.Equal(roleId, downlineInfo.RoleId);
        Assert.Equal(isSubAcc, downlineInfo.IsSubAcc);
    }

    [Fact]
    public void DownlineInfo_SupportsRecordEquality()
    {
        // Arrange
        var downlineInfo1 = new DownlineInfo(123, 2, true);
        var downlineInfo2 = new DownlineInfo(123, 2, true);
        var downlineInfo3 = new DownlineInfo(123, 2, false);

        // Act & Assert
        Assert.Equal(downlineInfo1, downlineInfo2);
        Assert.NotEqual(downlineInfo1, downlineInfo3);
        Assert.True(downlineInfo1 == downlineInfo2);
        Assert.False(downlineInfo1 == downlineInfo3);
    }
}
