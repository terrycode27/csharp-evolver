public static partial class ExtensionsOfTreeNodeOfT
{
    public static TreeNode<T> ChildOrNextSibling<T>(this TreeNode<T> node) where T : NodeBase<T>
    {
        if (node.Children == null || node.Children.Count == 0)
            return node.NextSibling();
        return node.Children.First();
    }
    public static TreeNode<T> DeepClone<T>(this TreeNode<T> node) where T : NodeBase<T>
    {
        var clone = new TreeNode<T>(node.Value);
        foreach (var child in node.Children)
        {
            clone.AddChild(child.DeepClone());
        }
        return clone;
    }
    public static bool DeepEqual<T>(this TreeNode<T>? a, TreeNode<T>? b, IEqualityComparer<T>? comparer = null) where T : NodeBase<T>
    {
        comparer ??= EqualityComparer<T>.Default;
        if (ReferenceEquals(a, b))
            return true;
        if (a == null || b == null)
            return false;
        if (!comparer.Equals(a.Value, b.Value))
            return false;
        if (a.Children.Count != b.Children.Count)
            return false;
        for (int i = 0; i < a.Children.Count; i++)
        {
            if (!a.Children[i].DeepEqual(b.Children[i], comparer))
                return false;
        }
        return true;
    }
    public static TreeNode<T> Delete<T>(this TreeNode<T> node, DeleteType deleteType = DeleteType.SingleNode) where T : NodeBase<T>
    {
        if (node.Parent == null)
            return node;
        var parent = node.Parent;
        int index = parent.Children.IndexOf(node);
        parent.Children.RemoveAt(index);
        if (deleteType == DeleteType.SingleNode)
        {
            node.PromoteChildrenTo(parent, index);
        }
        return node;
    }
    public static void DeleteRecursive<T>(this TreeNode<T> root, Func<TreeNode<T>, bool> predicate, DeleteType deleteType = DeleteType.SingleNode) where T : NodeBase<T>
    {
        foreach (var node in root.FindWhere(predicate).ToList())
        {
            node.Delete(deleteType);
        }
    }
    public static void DeleteUntil<T>(this TreeNode<T> root, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> untilPredicate, DeleteType deleteType = DeleteType.SingleNode) where T : NodeBase<T>
    {
        foreach (var node in root.FindWhereStop(predicate, untilPredicate))
        {
            node.Delete(deleteType);
        }
    }
    public static IEnumerable<List<TreeNode<T>>> FindConsecutive<T>(this TreeNode<T> node, Func<TreeNode<T>, bool> predicate) where T : NodeBase<T>
    {
        var flat = node.Flatten().ToList();
        var current = new List<TreeNode<T>>();
        foreach (var n in flat)
        {
            if (predicate(n))
            {
                current.Add(n);
            }
            else if (current.Count > 0)
            {
                if (current.Count > 1)
                    yield return current;
                current = new List<TreeNode<T>>();
            }
        }
        if (current.Count > 1)
            yield return current;
    }
    public static IEnumerable<TreeNode<T>> FindSkip<T>(this TreeNode<T> node, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> skipPredicate) where T : NodeBase<T>
    {
        if (skipPredicate(node))
            yield break;
        if (predicate(node))
            yield return node;
        foreach (var result in node.Children.SelectMany(c => c.FindSkip(predicate, skipPredicate)))
            yield return result;
    }
    public static IEnumerable<TreeNode<T>> FindSkipStop<T>(this TreeNode<T> node, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> skipPredicate, Func<TreeNode<T>, bool> stopPredicate) where T : NodeBase<T>
    {
        if (stopPredicate(node) || skipPredicate(node))
            yield break;
        if (predicate(node))
            yield return node;
        foreach (
            var result in node.Children.SelectMany(c =>
                c.FindSkipStop(predicate, skipPredicate, stopPredicate)
            )
        )
            yield return result;
    }
    public static IEnumerable<TreeNode<T>> FindWhere<T>(this TreeNode<T> node, Func<TreeNode<T>, bool> predicate) where T : NodeBase<T>
    {
        if (predicate(node))
            yield return node;
        foreach (var found in node.Children.SelectMany(c => c.FindWhere(predicate)))
            yield return found;
    }
    public static IEnumerable<TreeNode<T>> FindWhereStop<T>(this TreeNode<T> tree, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> untilPredicate) where T : NodeBase<T>
    {
        foreach (var f in tree.Flatten())
        {
            if (untilPredicate(f))
                yield break;
            if (predicate(f))
                yield return f;
        }
    }
    public static IEnumerable<TreeNode<T>> Flatten<T>(this TreeNode<T> node) where T : NodeBase<T>
    {
        return node.FindWhere(_ => true);
    }
    public static void InsertAfter<T>(this TreeNode<T> node, TreeNode<T> newNode) where T : NodeBase<T>
    {
        node.InsertRelative(newNode, after: true);
    }
    public static void InsertBefore<T>(this TreeNode<T> node, TreeNode<T> newNode) where T : NodeBase<T>
    {
        node.InsertRelative(newNode, after: false);
    }
    public static void InsertRelative<T>(this TreeNode<T> node, TreeNode<T> newNode, bool after) where T : NodeBase<T>
    {
        if (node.Parent == null)
            return;
        var siblings = node.Parent.Children;
        var index = siblings.IndexOf(node);
        var insertAt = after ? index + 1 : index;
        siblings.Insert(insertAt, newNode);
        newNode.Parent = node.Parent;
    }
    public static TreeNode<T> NextSibling<T>(this TreeNode<T> node) where T : NodeBase<T>
    {
        if (node.Parent == null)
            return null;
        var idx = node.Parent.Children.IndexOf(node);
        return idx + 1 < node.Parent.Children.Count ? node.Parent.Children[idx + 1] : null;
    }
    public static TreeNode<T> PreviousSibling<T>(this TreeNode<T> node) where T : NodeBase<T>
    {
        if (node.Parent == null)
            return null;
        var idx = node.Parent.Children.IndexOf(node);
        if (idx <= 0)
            return null;
        return node.Parent.Children[idx - 1];
    }
    public static void PromoteChildrenTo<T>(this TreeNode<T> node, TreeNode<T> parent, int insertIndex) where T : NodeBase<T>
    {
        var childrenToPromote = node.Children.ToList();
        node.Children.Clear();
        foreach (var child in childrenToPromote)
        {
            parent.Children.Insert(insertIndex++, child);
            child.Parent = parent;
        }
    }
}

