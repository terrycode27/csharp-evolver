public static partial class ExtensionsOfListOfT
{
    public static bool AreListsEqualIgnoringOrder<T>(this List<T> list1, List<T> list2)
    {
        if (list1 == null || list2 == null)
            return list1 == list2;
        var ordered1 = list1.OrderBy(x => x).ToList();
        var ordered2 = list2.OrderBy(x => x).ToList();
        var ret = ordered1.SequenceEqual(ordered2);
        return ret;
    }
    public static CompareWithKeyResult<T, TKey> CompareWithKeyFrom<T, TKey>(this List<T> destination, List<T> source, Func<T, TKey> keySelector) where TKey : notnull
    {
        source ??= new();
        destination ??= new();
        var groupSource = source.GroupBy(keySelector).ToDictionary(g => g.Key, g => g.ToList());
        var groupDestination = destination
            .GroupBy(keySelector)
            .ToDictionary(g => g.Key, g => g.ToList());
        var allKeys = groupSource.Keys.Union(groupDestination.Keys).ToHashSet();
        var onlyInSource = groupSource
            .Where(kv => !groupDestination.ContainsKey(kv.Key))
            .SelectMany(kv => kv.Value)
            .ToList();
        var onlyInDestination = groupDestination
            .Where(kv => !groupSource.ContainsKey(kv.Key))
            .SelectMany(kv => kv.Value)
            .ToList();
        var inBothKeys = allKeys
            .Where(k => groupSource.ContainsKey(k) && groupDestination.ContainsKey(k))
            .ToList();
        var inBothPairs = inBothKeys
            .Select(k =>
                (FromSource: groupSource[k].First(), FromDestination: groupDestination[k].First())
            )
            .ToList();
        var duplicatesInSource = groupSource
            .Where(g => g.Value.Count > 1)
            .ToDictionary(g => g.Key, g => g.Value);
        var duplicatesInDestination = groupDestination
            .Where(g => g.Value.Count > 1)
            .ToDictionary(g => g.Key, g => g.Value);
        return new CompareWithKeyResult<T, TKey>
        {
            OnlyInSource = onlyInSource,
            OnlyInDestination = onlyInDestination,
            InBoth = inBothKeys,
            InBothPairs = inBothPairs,
            DuplicatesInSource = duplicatesInSource,
            DuplicatesInDestination = duplicatesInDestination,
        };
    }
    public static List<T> GetDups<T>(this List<T> list, IEqualityComparer<T> comparer)
    {
        return list.GroupBy(t => t, comparer)
            .Where(t => t.Count() > 1)
            .Select(t => t.First())
            .ToList();
    }
    public static T GetOrNull<T>(this List<T> list, int index)
    {
        if (list.Count <= index)
            return default(T);
        return list[index];
    }
    public static List<(List<object> keys, List<T> vals)> GroupByPredicates<T>(this List<T> list, List<Func<T, object>> predicates)
    {
        List<(List<object> keys, List<T> vals)> Rec(List<T> items, int depth, List<object> keys)
        {
            if (depth == predicates.Count)
            {
                return new List<(List<object>, List<T>)> { (new List<object>(keys), new List<T>(items)) };
            }
            return items.GroupBy(predicates[depth])
                .SelectMany(g => Rec(g.ToList(), depth + 1, new List<object>(keys) { g.Key }))
                .ToList();
        }
        return Rec(list, 0, new List<object>());
    }
    public static int IndexOf<T>(this List<T> list, Func<T, bool> predicate)
    {
        if (list == null || predicate == null)
            return -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
                return i;
        }
        return -1;
    }
    public static List<T> Interleave<T, TKey>(this List<T> source, Func<T, TKey?> getVal, Func<T, bool> moveable) where TKey : IComparable<TKey>
    {
        var movables = source.Where(moveable).OrderBy(getVal).ToList();
        var index = 0;
        var result = new List<T>(source.Count);
        foreach (var item in source)
        {
            if (moveable(item))
                result.Add(movables[index++]);
            else
                result.Add(item);
        }
        return result;
    }
    public static (List<T> notInA, List<T> notInB, List<T> dupA, List<T> dupB) ListDiff<T>(this List<T> listA, List<T> listB, IEqualityComparer<T>? comparer)
    {
        var notInB = listA.NotIn(listB, comparer);
        var notInA = listB.NotIn(listA, comparer);
        var dupA = listA.GetDups(comparer);
        var dupB = listB.GetDups(comparer);
        return (notInA, notInB, dupA, dupB);
    }
    public static List<T> NotIn<T>(this List<T> listA, List<T> listB, IEqualityComparer<T> comparer)
    {
        var notInB = new List<T>();
        var setB = new HashSet<T>(listB, comparer);
        foreach (var item in listA)
        {
            if (!setB.Contains(item))
                notInB.Add(item);
        }
        return notInB;
    }
    public static HashSet<T> ToHS<T>(this List<T> list)
    {
        return new HashSet<T>(list);
    }
}

