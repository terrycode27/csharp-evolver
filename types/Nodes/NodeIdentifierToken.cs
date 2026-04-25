public partial class NodeIdentifierToken : NodeSemantic
{
    public NodeIdentifierToken(SyntaxElementNode b) : base(b) { }
    public string IdentifierText { get; private set; } = string.Empty;
    public override string? TypeName => null;
    public override void SetTreeNode(TreeNode<NodeSemantic> tree)
    {
        base.SetTreeNode(tree);
        ExtractText();
    }
    public override string ToString() => IdentifierText;
    private void ExtractText()
    {
        IdentifierText = tree?.Value?.Text?.Trim() ?? string.Empty;
    }
}

