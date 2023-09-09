namespace Wildfire.Ecs.UnitTests;

using System;
using System.Linq;
using Xunit;

public class ArrayUtilityTest
{
    [Theory]
    [InlineData(new uint[0], 4, 0)] 
    [InlineData(new uint[] { 1, 3, 4 }, 2, 1)] 
    [InlineData(new uint[] { 1, 3, 4 }, 0, 0)] 
    [InlineData(new uint[] { 1, 3, 4 }, 5, 3)] 
    [InlineData(new uint[] { 1, 3, 5, 6 }, 4, 2)] 
    [InlineData(new uint[] { 1, 3, 5, 6 }, 0, 0)] 
    [InlineData(new uint[] { 1, 3, 5, 6 }, 10, 4)] 
    public void FindEntityIdInsertionIndex(uint[] data, uint insert, int expected)
    {
        var entityIds = data.Select(e => new EntityId(e)).ToArray();
        var actual = ArrayUtility.FindEntityIdInsertionIndex(entityIds, entityIds.Length, new EntityId(insert));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindEntityIdInsertionIndex_WithExistingEntityDefinition_Throws()
    {
        var data = new uint[] { 1, 2, 3};
        var entityIds = data.Select(e => new EntityId(e)).ToArray();
        Assert.Throws<InvalidOperationException>(() => ArrayUtility.FindEntityIdInsertionIndex(entityIds, entityIds.Length, new EntityId(2)));
    }
}
