public static partial class ExtensionsOfIEnumerableOfIEnumerableOfT
{
    public static IEnumerable<(T Item, int Appearances, List<int> Indices)> CompareCollections<T>(this IEnumerable<IEnumerable<T>> collections) => (collections ?? Enumerable.Empty<IEnumerable<T>>()).SelectMany((col, idx) => (col ?? Enumerable.Empty<T>()).Distinct().Select(item => (Item: item, Index: idx))).GroupBy(x => x.Item).OrderBy(g => g.Key?.ToString() ?? string.Empty).Select(g => (Item: g.Key, Appearances: g.Count(), Indices: g.Select(x => x.Index).OrderBy(i => i).ToList()));
}

