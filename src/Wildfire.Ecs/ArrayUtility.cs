﻿namespace Wildfire.Ecs;

internal static class ArrayUtility
{
    /// <summary>
    /// Finds the correct insertion index in an <see cref="Entity"/> array so that the array stays sorted.
    /// Throws if the <paramref name="entity"/> already exists.
    /// </summary>
    public static int FindEntityInsertionIndex(Entity[] data, int size, Entity entity)
    {
        int left = 0, right = size;
        if (right == 0)
            return 0;

        // optimization for the append case
        if (entity > data[right - 1])
            return right;

        while (left < right)
        {
            var middle = (left + right) / 2;
            var value = data[middle];
            if (value < entity)
            {
                left = middle + 1;
            }
            else if (value > entity)
            {
                right = middle;
            }
            else
            {
                throw new InvalidOperationException("The specified entity already has a component registered.");
            }
        }

        return right;
    }

    public static void InsertAt<T>(T[] items, int size, int index, in T value)
    {
        for (var i = size; i > index; i--)
            items[i] = items[i - 1];

        items[index] = value;
    }

    public static void RemoveAt<T>(T[] items, int size, int index)
    {
        for (var i = index; i < size - 1; i++)
            items[i] = items[i + 1];
    }
}
