public partial class NodeRecord : NodeTypeDeclaration
{
    public NodeRecord(SyntaxElementNode b) : base(b) { }
    public override TreeNode<NodeSemantic> NodeSignatureAndAttribute
    {
        get
        {
            if (this.tree.Children.Count == 3)
                return base.NodeSignatureAndAttribute;
            return this.tree;
        }
    }
    public override TreeNode<NodeSemantic> CopyDeclaration()
    {
        var copy = tree.CleanCopy();
        var res = copy.DeleteStartingWithKinds(SyntaxKind.OpenBraceToken.ItemToHashSet());
        if (res.Count > 0)
            copy.AddChildTextNode(SyntaxKind.SemicolonToken, ";");
        return copy;
    }
}

