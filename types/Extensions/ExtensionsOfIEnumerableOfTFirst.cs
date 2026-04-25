public static partial class ExtensionsOfIEnumerableOfTFirst
{
    public static IEnumerable<(TFirst First, TSecond Second)> JoinByIndex<TFirst, TSecond>(        this IEnumerable<TFirst> first,        IEnumerable<TSecond> second)
    {
        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();
        while (e1.MoveNext() && e2.MoveNext())
        {
            yield return (e1.Current, e2.Current);
        }
    }
}

