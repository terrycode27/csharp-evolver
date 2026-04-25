public partial class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
{
    public KeyEqualityComparer(Func<T, TKey> keySelector)
    {
        _keySelector = keySelector;
    }
    private readonly Func<T, TKey> _keySelector;
    public bool Equals(T x, T y) => Equals(_keySelector(x), _keySelector(y));
    public int GetHashCode(T obj) => 0;
}

