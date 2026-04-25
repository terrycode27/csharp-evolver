public partial class NodeTypeOfExpression : NodeSemantic
{
    public NodeTypeOfExpression(SyntaxElementNode b) : base(b) { }
    public TreeNode<NodeSemantic>? TypeArgument { get; private set; }
    public override void SetTreeNode(TreeNode<NodeSemantic> tree)
    {
        base.SetTreeNode(tree);
        // The thing inside the parentheses
        TypeArgument = tree
            .FindNotKinds(
                new HashSet<SyntaxKind>                {
                    SyntaxKind.CloseParenToken
,
                    SyntaxKind.OpenParenToken,
                    SyntaxKind.TypeOfKeyword                })
            .FirstOrDefault();
    }
    public override string ToString() => $"typeof({TypeArgument?.ToCode() ?? ""})";
}

