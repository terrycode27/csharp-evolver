public static partial class ExtensionsOfIEnumerableOfTSource
{
    public static Dictionary<TKey, TSource> GroupBySingle<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        return source
            .GroupBy(keySelector)
            .ToDictionary(group => group.Key, group => group.Single());
    }
}

