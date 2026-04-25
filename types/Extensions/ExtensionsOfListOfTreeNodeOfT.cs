public static partial class ExtensionsOfListOfTreeNodeOfT
{
    public static void Delete<T>(this List<TreeNode<T>> nodes) where T : NodeBase<T>
    {
        nodes.ForEach(t => t.RemoveSelf());
    }
    public static IEnumerable<TreeNode<T>> FindSkip<T>(this List<TreeNode<T>> roots, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> skipPredicate) where T : NodeBase<T>
    {
        if (roots == null || predicate == null || skipPredicate == null)
            yield break;
        foreach (var root in roots)
        {
            if (root == null)
                continue;
            if (skipPredicate(root))
                continue;
            if (predicate(root))
                yield return root;
            foreach (var result in root.Children.FindSkip(predicate, skipPredicate))
                yield return result;
        }
    }
    public static IEnumerable<TreeNode<T>> FindSkipStop<T>(this List<TreeNode<T>> roots, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> skipPredicate, Func<TreeNode<T>, bool> stopPredicate) where T : NodeBase<T>
    {
        if (roots == null || predicate == null || skipPredicate == null || stopPredicate == null)
            yield break;
        foreach (var root in roots)
        {
            if (root == null)
                continue;
            if (stopPredicate(root))
                yield break;
            if (skipPredicate(root))
                continue;
            if (predicate(root))
                yield return root;
            foreach (
                var result in root.Children.FindSkipStop(predicate, skipPredicate, stopPredicate)
            )
                yield return result;
        }
    }
    public static IEnumerable<TreeNode<T>> FindWhere<T>(this List<TreeNode<T>> roots, Func<TreeNode<T>, bool> predicate) where T : NodeBase<T>
    {
        if (roots == null || predicate == null)
            yield break;
        foreach (var root in roots)
        {
            if (root == null)
                continue;
            if (root.Value != null && predicate(root))
                yield return root;
            foreach (var found in root.Children.FindWhere(predicate))
                yield return found;
        }
    }
    public static IEnumerable<TreeNode<T>> FindWhereStop<T>(this List<TreeNode<T>> roots, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> untilPredicate) where T : NodeBase<T>
    {
        if (roots == null || predicate == null || untilPredicate == null)
            yield break;
        foreach (var f in roots.FindWhere(t => true))
        {
            if (untilPredicate(f))
                yield break;
            if (predicate(f))
                yield return f;
        }
    }
}

