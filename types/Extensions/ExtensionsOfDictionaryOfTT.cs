public static partial class ExtensionsOfDictionaryOfTT
{
    public static T ValOrSearchKey<T>(this Dictionary<T,T> dictionary,T key)
    {
        if(dictionary.ContainsKey(key))
            return dictionary[key];
        return key;
    }
}

