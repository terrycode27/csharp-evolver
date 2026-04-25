public partial class NodeRegionEnd : NodeRegion
{
    public NodeRegionEnd(SyntaxElementNode b) : base(b) { }
    public static TreeNode<NodeSemantic> Factory()
    {
        var element = new SyntaxElementNode(SyntaxKind.EndRegionDirectiveTrivia, "\n#endregion\n");
        var semantic = new NodeRegionEnd(element);
        var root = new TreeNode<NodeSemantic>(semantic);
        semantic.SetTreeNode(root);
        return root;
    }
}

