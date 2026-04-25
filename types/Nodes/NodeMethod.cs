public partial class NodeMethod : NodeParameterizedMemberWithBody
{
    public NodeMethod(SyntaxElementNode b) : base(b) { }
    public override void SetNameNode()
    {
        this.TypeNameNode = tree
            .FindSkipStop(
                t => t.Value.Kind == SyntaxKind.IdentifierToken,
                t => SyntaxKindGroups.MethodDeclarationNonNameKinds.Contains(t.Value.Kind),
                t => t.Value.Kind == SyntaxKind.OpenBracketToken
            )
            .FirstOrDefault();
    }
}

