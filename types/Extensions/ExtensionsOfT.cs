public static partial class ExtensionsOfT
{
    public static T[] AsArray<T>(this T item)
    {
        return [item];
    }
    public static List<T> AsList<T>(this T item)
    {
        return [item];
    }
    public static Queue<T> AsQueue<T>(this T item)
    {
        var queue = new Queue<T>();
        queue.Enqueue(item);
        return queue;
    }
    public static HashSet<T> ItemToHashSet<T>(this T item)
    {
        return new HashSet<T>() { item };
    }
    public static TreeNode<NodeSemantic> MergeTypeDeclaration<T>(this T dest, T source) where T : NodeTypeDeclaration
    {
        var res = dest.MemberNodeNameds.CompareWithKeyFrom(
            source.MemberNodeNameds,
            t => t.TypeName
        );
        var destSig = dest.ReadSignature();
        var sourceSig = source.ReadSignature();
        var sig = destSig.Combine(sourceSig);
        var merged = new List<NodeNamed>();
        merged.AddRange(res.OnlyInDestination);
        merged.AddRange(res.OnlyInSource);
        merged.AddRange(res.InBothPairs.Select(t => t.FromSource));
        var mergedClass = merged.Select(t => t.tree).ToList().WrapInDeclaration(sig + "{}");
        return mergedClass;
    }
    public static string ToYAML<T>(this T val)
    {
        var serializer = new SerializerBuilder().Build();
        var yaml = serializer.Serialize(val);
        return yaml;
    }
    public static void ToYAMLFile<T>(this T val, string fileName)
    {
        var lines = ToYAML(val);
        File.WriteAllText(fileName, lines);
    }
}

