public static partial class ExtensionsOfDictionaryOfTKeyTVal
{
    public static TVal IfHas<TKey,TVal>(this Dictionary<TKey,TVal> dict,TKey key) where TVal:new()
    {
        if (!dict.ContainsKey(key))
            return new TVal();
        return dict[key];
    }
}

