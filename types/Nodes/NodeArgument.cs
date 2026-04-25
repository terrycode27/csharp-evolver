public partial class NodeArgument : NodeSemantic
{
    public NodeArgument(SyntaxElementNode b) : base(b) { }
    public string? ArgumentName { get; private set; }
    public TreeNode<NodeSemantic>? ExpressionNode { get; private set; }
    public bool IsThisArgument { get; private set; }
    public override void SetTreeNode(TreeNode<NodeSemantic> tree)
    {
        base.SetTreeNode(tree);
        ExtractExpression();
        ExtractArgumentName();
        DetectThisArgument();
    }
    private void DetectThisArgument()
    {
        IsThisArgument = tree.FindKind(SyntaxKind.ThisKeyword).Any();
    }
    private void ExtractArgumentName()
    {
        var nameColon = tree.FindKind(SyntaxKind.NameColon).FirstOrDefault();
        if (nameColon != null)
            ArgumentName = nameColon.Children.FirstOrDefault()?.Value.Text?.Trim();
    }
    private void ExtractExpression()
    {
        ExpressionNode = tree
            .FindNotKinds(SyntaxKindGroups.ParameterDelimeterKinds)
            .FirstOrDefault();
    }
}

