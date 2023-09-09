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
    public void FindEntityInsertionIndex(uint[] data, uint insert, int expected)
    {
        var entities = data.Select(e => new Entity(1, e)).ToArray();
        var actual = ArrayUtility.FindEntityInsertionIndex(entities, entities.Length, new Entity(1, insert));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindEntityInsertionIndex_WithExistingEntityDefinition_Throws()
    {
        var data = new uint[] { 1, 2, 3};
        var entities = data.Select(e => new Entity(1, e)).ToArray();
        Assert.Throws<InvalidOperationException>(() => ArrayUtility.FindEntityInsertionIndex(entities, entities.Length, new Entity(1, 2)));
    }
}
