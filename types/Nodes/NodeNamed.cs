public partial class NodeNamed : NodeSemantic
{
    protected NodeNamed(SyntaxElementNode b) : base(b) { }
    protected HashSet<SyntaxKind> endDelimeter;
    public void SetName(HashSet<SyntaxKind> endDelimeters)
    {
        var list = tree.FindDelimitedText(endDelimeters).ToList();
        string tempName = String.Join("", list.Select(t => t.Value.Text));
        list.ForEach(t => t.Value.Text = string.Empty);
        this.TypeNameNode = list.First();
        this.TypeNameNode.Value.Text = tempName;
    }
    public virtual void SetNameNode()
    {
        if (this.tree.Value.Kind == SyntaxKind.ClassDeclaration)
        {
            int bp = 0;
        }
        TypeNameNode = tree
            .FindSkip(
                n => n.Value.Kind == SyntaxKind.IdentifierToken,
                t => t.Value.Kind == SyntaxKind.Attribute
            )
            .FirstOrDefault();
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> t)
    {
        base.SetTreeNode(t);
        SetNameNode();
    }
}

