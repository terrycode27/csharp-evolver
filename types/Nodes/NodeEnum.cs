public partial class NodeEnum : NodeTypeDeclaration
{
    public NodeEnum(SyntaxElementNode b) : base(b) { }
    public List<NodeEnumMember> Members { get; } = new();
    public override TreeNode<NodeSemantic> NodeSignatureAndAttribute => null;
    public override TreeNode<NodeSemantic> NodeSignatureAttribute => null;
    public override TreeNode<NodeSemantic> NodeSignatureMain => null;
    public override void AttachChild(NodeSemantic child)
    {
        if (child is NodeEnumMember em)
            Members.Add(em);
    }
    public override TreeNode<NodeSemantic> CopyDeclaration()
    {
        var copy = tree.CleanCopy();
        var list = copy.DeleteKinds(
            SyntaxKindGroups.EnumMemberGroup,
            DeleteType.NodeAndSubTree
        );
        return copy;
    }
    public override void ExtractSignatureNode()
    {
    }
}

