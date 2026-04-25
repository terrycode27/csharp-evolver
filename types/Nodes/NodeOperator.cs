public partial class NodeOperator : NodeParameterizedMemberWithBody
{
    public NodeOperator(SyntaxElementNode b) : base(b) { }
    public override void SetNameNode()
    {
        TypeNameNode = FindOperatorToken();
    }
    private TreeNode<NodeSemantic> FindOperatorToken()
    {
        return tree
            .FindWhereStop(IsOperatorToken, t => t.Value.Kind == SyntaxKind.OpenParenToken)
            .FirstOrDefault();
    }
    private bool IsOperatorToken(TreeNode<NodeSemantic> t)
    {
        var text = t.Value?.Text?.Trim() ?? "";
        return text
            is "+"
                or "-"
                or "*"
                or "/"
                or "%"
                or "&"
                or "|"
                or "^"
                or "!"
                or "~"
                or "<"
                or ">"
                or "="
                or "=="
                or "?";
    }
}

