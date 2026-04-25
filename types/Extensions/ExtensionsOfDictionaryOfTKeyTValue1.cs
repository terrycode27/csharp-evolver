public static partial class ExtensionsOfDictionaryOfTKeyTValue1
{
    public static Dictionary<TKey, (TValue1 First, TValue2 Second)> CombineDictionaries<TKey, TValue1, TValue2>(this Dictionary<TKey, TValue1> dict1, Dictionary<TKey, TValue2> dict2) 
    {
        var joined = (from x in dict1 join x2 in dict2 on x.Key equals x2.Key select (x, x2)).ToList();
        var grouped = joined.GroupBy(t => t.x.Key).ToDictionary(t => t.Key, t => t.Select(z => (z.x.Value, z.x2.Value)).Single());
        return grouped;
    }
}

