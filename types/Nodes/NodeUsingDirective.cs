public partial class NodeUsingDirective : NodeNamedMember
{
    public NodeUsingDirective(SyntaxElementNode b) : base(b)
    {
        this.endDelimeter = new HashSet<SyntaxKind> { SyntaxKind.SemicolonToken };
        Modifier = AccessModifier.Public;
    }
    public override TreeNode<NodeSemantic> SetModifier(AccessModifier modifier)
    {
        return null;
    }
    public override void SetNameNode()
    {
        SetName(endDelimeter);
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> tree)
    {
        this.tree = tree;
        SetNameNode();
    }
}

