public partial class NodeProperty : NodeNamedMember
{
    public NodeProperty(SyntaxElementNode b) : base(b) { }
    public override TreeNode<NodeSemantic> NodeImplementation { get => tree.Children[1]; }
    public override TreeNode<NodeSemantic> NodeSignatureAndAttribute { get => tree.Children[0]; }
    public override TreeNode<NodeSemantic> CopyDeclaration()
    {
        return SerializerSemanticTree.DeserializeAndGroupCode(GetSignatureOnly());
    }
    public override void ExtractSignatureNode()
    {
        this.tree.SplitBefore(SyntaxKindGroups.PropertySigntureBoundaries);
    }
    public override string GetSignatureOnly()
    {
        var copy = tree.CleanCopy();
        copy.DeleteKinds(SyntaxKindGroups.MemberModifierKeywords);
        var a1 = copy
            .FindStopAt(
                new HashSet<SyntaxKind>()                {
                    SyntaxKind.ArrowExpressionClause,
                    SyntaxKind.OpenBraceToken,
                }
            )
            .ToList();
        var ret = String.Join("", a1.Select(t => t.Value.Text)) + "{get;set;}";
        return ret;
    }
    public override void SetNameNode()
    {
        this.TypeNameNode = this.tree.FindKindsStop(SyntaxKind.IdentifierToken.ItemToHashSet(), SyntaxKindGroups.PropertySigntureBoundaries).Last();
    }
}

