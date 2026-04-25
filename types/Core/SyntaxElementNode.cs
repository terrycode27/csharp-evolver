public partial class SyntaxElementNode : NodeBase<SyntaxElementNode>
{
    public SyntaxElementNode(SyntaxKind kind, string text) : this(kind, text, null) { }
    public SyntaxElementNode(SyntaxKind kind, string text, string identifier)
    {
        Kind = kind;
        Text = text;
        Identifier = identifier;
    }
    protected SyntaxElementNode(SyntaxNodeOrToken node)
    {
        Kind = node.Kind();
        if (node.IsToken)
        {
            var token = node.AsToken();
            Text = token.Text;
            if (token.IsKind(SyntaxKind.IdentifierToken))
                Identifier = token.Text;
        }
        else
        {
            Text = null;
            Identifier = null;
        }
    }
    public string? Identifier { get; }
    public SyntaxKind Kind { get; }
    public string? Text { get; }
    public NodeSemantic CreateSemantic()
    {
        return Kind switch
        {
            SyntaxKind.TypeArgumentList => new NodeTypeArgumentList(this),
            SyntaxKind.TypeOfExpression => new NodeTypeOfExpression(this),
            SyntaxKind.SimpleMemberAccessExpression => new NodeSimpleMemberAccess(this),
            SyntaxKind.InvocationExpression => new NodeInvocationExpression(this),
            SyntaxKind.ArgumentList => new NodeArgumentList(this),
            SyntaxKind.Argument => new NodeArgument(this),
            SyntaxKind.AttributeArgument => new NodeAttributeArgument(this),
            SyntaxKind.AttributeList => new NodeAttributeList(this),
            SyntaxKind.Attribute => new NodeAttribute(this),
            SyntaxKind.ArrowExpressionClause => new NodeExpressionBody(this),
            SyntaxKind.Block => new NodeBlock(this),
            SyntaxKind.ClassDeclaration => new NodeClass(this),
            SyntaxKind.CompilationUnit => new NodeCompilationUnit(this),
            SyntaxKind.ConstructorDeclaration => new NodeConstructor(this),
            SyntaxKind.DelegateDeclaration => new NodeDelegate(this),
            SyntaxKind.DestructorDeclaration => new NodeDestructor(this),
            SyntaxKind.EndRegionDirectiveTrivia => new NodeRegionEnd(this),
            SyntaxKind.EndOfLineTrivia => new NodeTrivia(this),
            SyntaxKind.EnumDeclaration => new NodeEnum(this),
            SyntaxKind.EnumMemberDeclaration => new NodeEnumMember(this),
            SyntaxKind.EventDeclaration => new NodeEvent(this),
            SyntaxKind.EventFieldDeclaration => new NodeEventField(this),
            SyntaxKind.FieldDeclaration => new NodeField(this),
            SyntaxKind.FileScopedNamespaceDeclaration => new NodeFileScopedNamespace(this),
            SyntaxKind.IndexerDeclaration => new NodeIndexer(this),
            SyntaxKind.InterfaceDeclaration => new NodeInterface(this),
            SyntaxKind.MethodDeclaration => new NodeMethod(this),
            SyntaxKind.MultiLineCommentTrivia => new NodeComment(this),
            SyntaxKind.NamespaceDeclaration => new NodeNamespace(this),
            SyntaxKind.None => new NodeTrivia(this),
            SyntaxKind.OperatorDeclaration => new NodeOperator(this),
            SyntaxKind.PropertyDeclaration => new NodeProperty(this),
            SyntaxKind.RecordDeclaration => new NodeRecord(this),
            SyntaxKind.RecordStructDeclaration => new NodeRecord(this),
            SyntaxKind.RegionDirectiveTrivia => new NodeRegionStart(this),
            SyntaxKind.SingleLineCommentTrivia => new NodeComment(this),
            SyntaxKind.StructDeclaration => new NodeStruct(this),
            SyntaxKind.UsingDirective => new NodeUsingDirective(this),
            SyntaxKind.WhitespaceTrivia => new NodeTrivia(this),
            SyntaxKind.Parameter => new NodeParameter(this),
            SyntaxKind.ParameterList => new NodeParameterList(this),
            SyntaxKind.IdentifierToken => new NodeIdentifierToken(this),
            _ => new NodeUnknown(this),
        };
    }
    public override string ToString()
    {
        var sb = new StringBuilder(Kind.ToString());
        if (Identifier is { } id)
            sb.Append($"({id})");
        return sb.ToString();
    }
}

