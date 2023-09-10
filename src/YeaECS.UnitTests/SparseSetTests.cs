namespace YeaECS.UnitTests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

public class SparseSetTests
{
    private record Guts<T>(
        T[] DenseValues,
        DenseEntry[] DenseEntities,
        int DenseCount,
        int DenseFreeHead,
        SparseEntry[] SparseEntities,
        int Count)
    {
        private static readonly FieldInfo s_denseValuesField = typeof(SparseSet<T>).GetField("_denseValues", BindingFlags.NonPublic | BindingFlags.Instance)!;
        private static readonly FieldInfo s_denseEntitiesField = typeof(SparseSet<T>).GetField("_denseEntries", BindingFlags.NonPublic | BindingFlags.Instance)!;
        private static readonly FieldInfo s_denseCountField = typeof(SparseSet<T>).GetField("_denseCount", BindingFlags.NonPublic | BindingFlags.Instance)!;
        private static readonly FieldInfo s_denseFreeHeadField = typeof(SparseSet<T>).GetField("_denseFreeHead", BindingFlags.NonPublic | BindingFlags.Instance)!;
        private static readonly FieldInfo s_sparseEntitiesField = typeof(SparseSet<T>).GetField("_sparseEntities", BindingFlags.NonPublic | BindingFlags.Instance)!;

        public static Guts<T> GetSnapshot(SparseSet<T> sparseSet)
        {
            var denseValues = (T[])s_denseValuesField.GetValue(sparseSet)!;
            var denseEntities = (DenseEntry[])s_denseEntitiesField.GetValue(sparseSet)!;
            var denseCount = (int)s_denseCountField.GetValue(sparseSet)!;
            var denseFreeHead = (int)s_denseFreeHeadField.GetValue(sparseSet)!;
            var sparseEntities = (SparseEntry[])s_sparseEntitiesField.GetValue(sparseSet)!;

            return new Guts<T>(denseValues, denseEntities, denseCount, denseFreeHead, sparseEntities, sparseSet.Count);
        }

        public void AssertIntegrity()
        {
            // Nullability constraint
            Assert.NotNull(DenseValues);
            Assert.NotNull(DenseEntities);
            Assert.NotNull(SparseEntities);

            // Dense map equal length
            Assert.Equal(DenseValues.Length, DenseEntities.Length);

            // Dense count constraint
            Assert.InRange(DenseCount, 0, DenseValues.Length);

            // Dense head/tail constraint
            Assert.InRange(DenseFreeHead, 0, DenseValues.Length);

            // Check that dense values are set or unset correctly
            var entryCount = 0;
            for (var i = 0; i < DenseEntities.Length; i++)
            {
                var entity = DenseEntities[i];
                if (entity.Generation == 0)
                {
                    Assert.Equal(default, DenseValues[i]);
                }
                else
                {
                    entryCount++;
                    Assert.True(i < DenseCount, "Valid entry not in the region defined by DenseCount.");
                }
            }

            Assert.Equal(Count, entryCount);
            Assert.True(DenseCount >= Count);

            // Check next free map
            var freeCount = 0;
            var current = DenseFreeHead;
            while (current < DenseEntities.Length)
            {
                var entity = DenseEntities[current];
                Assert.Equal(0u, entity.Generation);

                freeCount++;

                var next = (int)entity.IdOrIndex;
                Assert.True(next > current);

                current = next;
            }

            Assert.Equal(DenseEntities.Length, entryCount + freeCount);

            // Check sparse entries
            var validSparseEntry = 0;
            foreach (var sparseSetEntry in SparseEntities)
            {
                if (sparseSetEntry is { Generation: 0, Index: 0 })
                    continue;

                validSparseEntry++;
                Assert.True(sparseSetEntry.Generation > 0);
                Assert.InRange(sparseSetEntry.Index, 0u, (uint)(DenseCount - 1));

                var targetEntity = DenseEntities[sparseSetEntry.Index];
                Assert.Equal(sparseSetEntry.Generation, targetEntity.Generation);
            }

            Assert.Equal(Count, validSparseEntry);
        }

        public IReadOnlyList<int> GetFreeNodesList()
        {
            var result = new List<int>();

            var current = DenseFreeHead;
            while (current < DenseEntities.Length)
            {
                result.Add(current);
                current = (int)DenseEntities[current].IdOrIndex;
            }

            result.Add(current);

            return result;
        }
    }

    [Fact]
    public void Constructor_WithExplicitSizes()
    {
        var sparseSet = new SparseSet<string>(5, 10);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();

        Assert.Equal(5, guts.DenseValues.Length);
        Assert.Equal(5, guts.DenseEntities.Length);
        Assert.Equal(0, guts.DenseCount);
        Assert.Equal(0, guts.DenseFreeHead);
        Assert.Equal(10, guts.SparseEntities.Length);

        var expectedFreeNodesList = new[] { 0, 1, 2, 3, 4, 5 };
        Assert.Equal(expectedFreeNodesList, guts.GetFreeNodesList());
    }

    [Fact]
    public void Add_NotIncludedEntity_SetsValueAndReturnsRef()
    {
        var sparseSet = CreateSparseSet();

        var value = "asd";
        var valueRef = sparseSet.Add(new Entity(1, 1), in value);
        Assert.Equal("asd", valueRef);
        Assert.Equal(1, sparseSet.Count);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void Add_IncludedEntity_ReturnsRefAndTrue()
    {
        var sparseSet = CreateSparseSet();

        sparseSet.GetOrCreate(new Entity(1, 1), out _) = "asd";

        var value = "asd";
        var ex = Assert.Throws<InvalidOperationException>(() => sparseSet.Add(new Entity(1, 1), in value));
        Assert.Equal("A value for entity <1@1> already exists in the sparse set.", ex.Message);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void Clear()
    {
        var sparseSet = CreateSparseSet();
        sparseSet.GetOrCreate(new Entity(1, 1)) = "asd";

        sparseSet.Clear();
        Assert.Equal(0, sparseSet.Count);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void Get_NotIncludedEntity_Throws()
    {
        var sparseSet = CreateSparseSet();

        var ex = Assert.Throws<InvalidOperationException>(() => sparseSet.Get(new Entity(1, 1)));
        Assert.Equal("Cannot find a value for the specified entity <1@1> in the sparse set.", ex.Message);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void Get_IncludedEntity_ReturnsRefAndTrue()
    {
        var sparseSet = CreateSparseSet();

        sparseSet.GetOrCreate(new Entity(1, 1), out _) = "asd";

        ref var value = ref sparseSet.Get(new Entity(1, 1));
        Assert.False(Unsafe.IsNullRef(ref value));
        Assert.Equal("asd", value);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void GetEnumerator()
    {
        var sparseSet = CreateSparseSet();

        sparseSet.Add(new Entity(1, 1), "a");
        sparseSet.Add(new Entity(2, 2), "a");
        sparseSet.Add(new Entity(3, 3), "a");

        var expectedEntities = new Entity[]
        {
            new(1, 1),
            new(2, 2),
            new(3, 3),
        };
        Assert.Equal(expectedEntities, sparseSet.ToArray());
    }

    
    [Fact]
    public void GetOrCreate_NewEntity_CreatesEntity()
    {
        var sparseSet = CreateSparseSet();
        var entity = new Entity(1, 1);

        ref var value = ref sparseSet.GetOrCreate(entity, out var created);
        Assert.Null(value);
        Assert.True(created);

        Assert.Equal(1, sparseSet.Count);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();

        var expectedFreeNodesList = new[] { 1, 2, 3, 4, 5 };
        Assert.Equal(expectedFreeNodesList, guts.GetFreeNodesList());
    }

    [Fact]
    public void GetOrCreate_ExistingEntity_GetsCorrectReference()
    {
        var sparseSet = CreateSparseSet();
        var entity = new Entity(1, 1);

        sparseSet.GetOrCreate(entity, out var created) = "asd";
        Assert.True(created);
        Assert.Equal(1, sparseSet.Count);

        ref var value = ref sparseSet.GetOrCreate(entity, out created);
        Assert.Equal("asd", value);
        Assert.False(created);

        Assert.Equal(1, sparseSet.Count);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();

        Assert.Equal(1u, guts.SparseEntities[1].Generation);
        Assert.Equal(0u, guts.SparseEntities[1].Index);

        Assert.Equal(1u, guts.DenseEntities[0].Generation);
        Assert.Equal(1u, guts.DenseEntities[0].IdOrIndex);

        Assert.Equal("asd", guts.DenseValues[0]);

        var expectedFreeNodesList = new[] { 1, 2, 3, 4, 5 };
        Assert.Equal(expectedFreeNodesList, guts.GetFreeNodesList());
    }

    [Fact]
    public void GetOrCreate_ExistingEntityButDifferentGeneration_ThrowsException()
    {
        var sparseSet = CreateSparseSet();
        var entity = new Entity(1, 1);

        sparseSet.GetOrCreate(entity, out var created) = "asd";
        Assert.True(created);

        var ex = Assert.Throws<InvalidOperationException>(() => sparseSet.GetOrCreate(new Entity(2, 1), out _));
        Assert.Equal("There already exists a value from a different generation.", ex.Message);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void Has_NonIncludedEntity_ReturnsFalse()
    {
        var sparseSet = CreateSparseSet();

        Assert.False(sparseSet.Has(new Entity(1, 1)));

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void Has_IncludedEntity_ReturnsTrue()
    {
        var sparseSet = CreateSparseSet();
        var entity = new Entity(1, 1);

        sparseSet.GetOrCreate(entity, out _) = "asd";

        Assert.True(sparseSet.Has(entity));

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void Has_NotIncludedEntityButDifferentGeneration_ReturnsFalse()
    {
        var sparseSet = CreateSparseSet();

        sparseSet.GetOrCreate(new Entity(1, 1), out _) = "asd";

        Assert.False(sparseSet.Has(new Entity(2, 1)));

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void Remove_NonExistingEntity_ReturnsFalse()
    {
        var sparseSet = CreateSparseSet();

        sparseSet.GetOrCreate(new Entity(1, 2));
        Assert.Equal(1, sparseSet.Count);

        var result = sparseSet.Remove(new Entity(1, 1));
        Assert.False(result);
        Assert.Equal(1, sparseSet.Count);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();

        var expectedFreeNodesList = new[] { 1, 2, 3, 4, 5 };
        Assert.Equal(expectedFreeNodesList, guts.GetFreeNodesList());
    }

    [Fact]
    public void Remove_ExistingEntity_ReturnsTrue()
    {
        var sparseSet = CreateSparseSet();

        ref var value = ref sparseSet.GetOrCreate(new Entity(1, 1));
        value = "asd";

        var result = sparseSet.Remove(new Entity(1, 1));
        Assert.True(result);
        Assert.Null(value);
        Assert.Equal(0, sparseSet.Count);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();

        Assert.Equal(0, guts.DenseCount);

        var expectedFreeNodesList = new[] { 0, 1, 2, 3, 4, 5 };
        Assert.Equal(expectedFreeNodesList, guts.GetFreeNodesList());
    }

    [Fact]
    public void Remove_ExistingEntityButWrongGeneration_ReturnsFalse()
    {
        var sparseSet = CreateSparseSet();

        ref var value = ref sparseSet.GetOrCreate(new Entity(1, 1));
        value = "asd";

        var result = sparseSet.Remove(new Entity(2, 1));
        Assert.False(result);
        Assert.Equal("asd", value);
        Assert.Equal(1, sparseSet.Count);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();

        var expectedFreeNodesList = new[] { 1, 2, 3, 4, 5 };
        Assert.Equal(expectedFreeNodesList, guts.GetFreeNodesList());
    }

    [Fact]
    public void TryGet_NotIncludedEntity_ReturnsNullRefAndFalse()
    {
        var sparseSet = CreateSparseSet();

        ref var value = ref sparseSet.TryGet(new Entity(1, 1), out var found);
        Assert.True(Unsafe.IsNullRef(ref value));
        Assert.False(found);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }

    [Fact]
    public void TryGet_IncludedEntity_ReturnsRefAndTrue()
    {
        var sparseSet = CreateSparseSet();

        sparseSet.GetOrCreate(new Entity(1, 1), out _) = "asd";

        ref var value = ref sparseSet.TryGet(new Entity(1, 1), out var found);
        Assert.False(Unsafe.IsNullRef(ref value));
        Assert.True(found);
        Assert.Equal("asd", value);

        var guts = GetGutSnapshot(sparseSet);
        guts.AssertIntegrity();
    }


    private static SparseSet<string> CreateSparseSet() => new(5, 10);

    private static Guts<T> GetGutSnapshot<T>(SparseSet<T> sparseSet) => Guts<T>.GetSnapshot(sparseSet);
}
