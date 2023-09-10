namespace YeaECS;

internal static class MinimumUtil
{
    public static unsafe (int minimum1, int minimum2) GetTwoMinimaIndices(int* data, int count)
    {
        if (count <= 3)
            throw new ArgumentException("Count cannot be smaller than 3.");
        
        var minimum1 = data[0];
        var minimum1Index = 0;
        var minimum2 = data[1];
        var minimum2Index = 1;
        for (var i = 1; i < count; i++)
        {
            var value = data[i];
            if (value < minimum1)
            {
                minimum2 = minimum1;
                minimum2Index = minimum1Index;
                minimum1 = value;
                minimum1Index = i;
            }
            else if (value == minimum1 || value < minimum2)
            {
                minimum2 = value;
                minimum2Index = i;
            }
        }

        return (minimum1Index, minimum2Index);
    }
}
