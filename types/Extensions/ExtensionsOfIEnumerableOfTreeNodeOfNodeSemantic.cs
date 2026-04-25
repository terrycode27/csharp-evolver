public static partial class ExtensionsOfIEnumerableOfTreeNodeOfNodeSemantic
{
    public static IEnumerable<TreeNode<NodeSemantic>> DeleteKind(this IEnumerable<TreeNode<NodeSemantic>> en, SyntaxKind kind, DeleteType deleteType = DeleteType.SingleNode)
    {
        return en.DeleteKinds(new HashSet<SyntaxKind>() { kind }, deleteType);
    }
    public static IEnumerable<TreeNode<NodeSemantic>> DeleteKinds(this IEnumerable<TreeNode<NodeSemantic>> en, HashSet<SyntaxKind> kinds, DeleteType deleteType = DeleteType.SingleNode)
    {
        foreach (var x in en)
        {
            yield return x;
            x.Delete(DeleteType.SingleNode);
        }
    }
    public static List<TreeNode<NodeSemantic>> NotPresentIn(this IEnumerable<TreeNode<NodeSemantic>> candidates, IEnumerable<TreeNode<NodeSemantic>> existing)
    {
        var existingNames = existing.Select(e => e.Value.FullName).ToHashSet();
        return candidates.Where(c => !existingNames.Contains(c.Value.FullName)).ToList();
    }
    public static string ToCode(this IEnumerable<TreeNode<NodeSemantic>> list)
    {
        var sw = new StringWriter();
        list.ForEach(t => sw.WriteLine(t.ToCode()));
        return sw.ToString();
    }
    public static Dictionary<string, TreeNode<NodeSemantic>> ToDictionaryByFullName(this IEnumerable<TreeNode<NodeSemantic>> nodes) => nodes.ToDictionary(t => t.Value.FullName);
    public static List<T> ToTypedList<T>(this IEnumerable<TreeNode<NodeSemantic>> tree)
    {
        return tree.Select(t => t.Value).OrderBy(t => t.TypeName).Cast<T>().ToList();
    }
}

