namespace Wildfire.Ecs.UnitTests;

using System;
using System.Linq;
using Xunit;

public class ArrayUtilityTest
{
    [Theory]
    [InlineData(new int[0], 4, 0)] 
    [InlineData(new[] { 1, 3, 4 }, 2, 1)] 
    [InlineData(new[] { 1, 3, 4 }, 0, 0)] 
    [InlineData(new[] { 1, 3, 4 }, 5, 3)] 
    [InlineData(new[] { 1, 3, 5, 6 }, 4, 2)] 
    [InlineData(new[] { 1, 3, 5, 6 }, 0, 0)] 
    [InlineData(new[] { 1, 3, 5, 6 }, 10, 4)] 
    public void FindEntityIdInsertionIndex(int[] data, int insert, int expected)
    {
        var entityIds = data.Select(e => new EntityId(e)).ToArray();
        var actual = ArrayUtility.FindEntityIdInsertionIndex(entityIds, entityIds.Length, new EntityId(insert));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindEntityIdInsertionIndex_WithExistingEntityDefinition_Throws()
    {
        var data = new int[] { 1, 2, 3};
        var entityIds = data.Select(e => new EntityId(e)).ToArray();
        Assert.Throws<InvalidOperationException>(() => ArrayUtility.FindEntityIdInsertionIndex(entityIds, entityIds.Length, new EntityId(2)));
    }
}
