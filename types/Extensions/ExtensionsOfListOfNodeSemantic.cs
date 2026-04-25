public static partial class ExtensionsOfListOfNodeSemantic
{
    public static void Delete<T>(this List<T> classes) where T : NodeSemantic
    {
        classes.Select(t => t.tree).ToList().Delete();
    }
    public static Dictionary<T1, List<T>> ToGroupedDictionary<T, T1>(this List<T> list, Func<T, T1> selector) where T : NodeSemantic
    {
        var ret = list.GroupBy(selector).ToDictionary(t => t.Key, t => t.ToList());
        return ret;
    }
}

