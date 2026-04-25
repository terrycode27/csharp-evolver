public partial class CompareWithKeyResult<T, TKey> where TKey : notnull
{
    public Dictionary<TKey, List<T>> DuplicatesInDestination { get; init; } = new();
    public Dictionary<TKey, List<T>> DuplicatesInSource { get; init; } = new();
    public bool HasDifferences => OnlyInSource.Count > 0 || OnlyInDestination.Count > 0;
    public bool HasDuplicates => DuplicatesInSource.Count > 0 || DuplicatesInDestination.Count > 0;
    public List<TKey> InBoth { get; init; } = new();
    public List<(T FromSource, T FromDestination)> InBothPairs { get; init; } = new();
    public List<T> OnlyInDestination { get; init; } = new();
    public List<T> OnlyInSource { get; init; } = new();
}

