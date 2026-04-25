public static partial class ExtensionsOfNodeSemantic
{
    public static List<string> GetFullPath(this NodeSemantic node)
    {
        var path = new List<string>();
        var current = node.tree;
        while (current != null)
        {
            if (current.Value.HasName && !string.IsNullOrEmpty(current.Value.TypeName))
            {
                path.Add(current.Value.TypeName);
            }
            current = current.Parent;
        }
        path.Reverse();
        return path;
    }
    public static string GetFullPathString(this NodeSemantic node, string separator = ".")
    {
        return string.Join(separator, node.GetFullPath());
    }
}

