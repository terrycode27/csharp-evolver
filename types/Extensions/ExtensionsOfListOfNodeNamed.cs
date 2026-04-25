public static partial class ExtensionsOfListOfNodeNamed
{
    public static List<TreeNode<NodeSemantic>> FirstParentChildren(this List<NodeNamed> nodes)
    {
        return nodes.First().tree.Parent.Children;
    }
}

