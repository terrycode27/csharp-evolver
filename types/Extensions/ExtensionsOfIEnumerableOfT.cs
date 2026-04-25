public static partial class ExtensionsOfIEnumerableOfT
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (action == null)
            throw new ArgumentNullException(nameof(action));
        foreach (var item in source)
            action(item);
    }
    public static List<List<T>> GroupConsecutive<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        var result = new List<List<T>>();
        var current = new List<T>();
        foreach (var item in items)
        {
            if (current.Count == 0 || predicate(item) == predicate(current.Last()))
                current.Add(item);
            else
            {
                result.Add(current);
                current = new List<T> { item };
            }
        }
        if (current.Count > 0)
            result.Add(current);
        return result;
    }
    public static IEnumerable<T> OrderByPredicates<T>(this IEnumerable<T> items, List<Func<T, object>> predicates)
    {
        IEnumerable<T> ApplyGrouping(int index, IEnumerable<T> currentItems)
        {
            if (index >= predicates.Count)
                return currentItems;
            return currentItems
                .GroupBy(predicates[index])
                .OrderBy(g => g.Key)
                .SelectMany(g => ApplyGrouping(index + 1, g));
        }
        return ApplyGrouping(0, items);
    }
}

