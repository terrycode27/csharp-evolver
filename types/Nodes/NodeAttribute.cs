public partial class NodeAttribute : NodeNamed
{
    public NodeAttribute(SyntaxElementNode b) : base(b) { }
    public NodeAttributeList AttributeListNodeParent
    {
        get { return GetParentType<NodeAttributeList>(); }
    }
    public NodeNamedMember NamedMemberNodeParent
    {
        get { return GetParentType<NodeNamedMember>(); }
    }
    public static TreeNode<NodeSemantic> FactoryNodeAttribute(string attributeText)
    {
        if (!attributeText.TrimStart().StartsWith("["))
            attributeText = $"[{attributeText.Trim('[', ']')}]";
        var element = new SyntaxElementNode(SyntaxKind.AttributeList, attributeText);
        var semantic = new NodeUnknown(element);
        var node = new TreeNode<NodeSemantic>(semantic);
        semantic.SetTreeNode(node);
        return node;
    }
    public override void SetNameNode()
    {
        this.TypeNameNode = tree
            .FindWhereStop(
                t => t.Value.Kind == SyntaxKind.IdentifierToken,
                t =>
                    t.Value.Kind
                        is SyntaxKind.OpenParenToken
                            or SyntaxKind.CloseBracketToken
                            or SyntaxKind.CommaToken
            )
            .FirstOrDefault();
    }
}

