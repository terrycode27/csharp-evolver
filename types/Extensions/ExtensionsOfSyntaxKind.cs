public static partial class ExtensionsOfSyntaxKind
{
    public static SyntaxKindOrdering ToHandled(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.CompilationUnit => SyntaxKindOrdering.CompilationUnit,
            SyntaxKind.NamespaceDeclaration => SyntaxKindOrdering.NamespaceDeclaration,
            SyntaxKind.FileScopedNamespaceDeclaration =>
                SyntaxKindOrdering.FileScopedNamespaceDeclaration,
            SyntaxKind.ClassDeclaration => SyntaxKindOrdering.ClassDeclaration,
            SyntaxKind.StructDeclaration => SyntaxKindOrdering.StructDeclaration,
            SyntaxKind.RecordDeclaration => SyntaxKindOrdering.RecordDeclaration,
            SyntaxKind.InterfaceDeclaration => SyntaxKindOrdering.InterfaceDeclaration,
            SyntaxKind.EnumDeclaration => SyntaxKindOrdering.EnumDeclaration,
            SyntaxKind.DelegateDeclaration => SyntaxKindOrdering.DelegateDeclaration,
            SyntaxKind.MethodDeclaration => SyntaxKindOrdering.MethodDeclaration,
            SyntaxKind.ConstructorDeclaration => SyntaxKindOrdering.ConstructorDeclaration,
            SyntaxKind.DestructorDeclaration => SyntaxKindOrdering.DestructorDeclaration,
            SyntaxKind.PropertyDeclaration => SyntaxKindOrdering.PropertyDeclaration,
            SyntaxKind.FieldDeclaration => SyntaxKindOrdering.FieldDeclaration,
            SyntaxKind.EventFieldDeclaration => SyntaxKindOrdering.EventFieldDeclaration,
            SyntaxKind.EventDeclaration => SyntaxKindOrdering.EventDeclaration,
            SyntaxKind.OperatorDeclaration => SyntaxKindOrdering.OperatorDeclaration,
            SyntaxKind.IndexerDeclaration => SyntaxKindOrdering.IndexerDeclaration,
            SyntaxKind.EnumMemberDeclaration => SyntaxKindOrdering.EnumMemberDeclaration,
            SyntaxKind.Block => SyntaxKindOrdering.Block,
            SyntaxKind.ArrowExpressionClause => SyntaxKindOrdering.ArrowExpressionClause,
            SyntaxKind.UsingDirective => SyntaxKindOrdering.UsingDirective,
            SyntaxKind.SingleLineCommentTrivia => SyntaxKindOrdering.SingleLineCommentTrivia,
            SyntaxKind.MultiLineCommentTrivia => SyntaxKindOrdering.MultiLineCommentTrivia,
            SyntaxKind.RegionDirectiveTrivia => SyntaxKindOrdering.RegionDirectiveTrivia,
            SyntaxKind.EndRegionDirectiveTrivia => SyntaxKindOrdering.EndRegionDirectiveTrivia,
            SyntaxKind.WhitespaceTrivia => SyntaxKindOrdering.WhitespaceTrivia,
            SyntaxKind.EndOfLineTrivia => SyntaxKindOrdering.EndOfLineTrivia,
            SyntaxKind.None => SyntaxKindOrdering.None,
            _ => SyntaxKindOrdering.Unknown,
        };
    }
    public static AccessModifier ToModifier(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.PublicKeyword => AccessModifier.Public,
            SyntaxKind.ProtectedKeyword => AccessModifier.Protected,
            SyntaxKind.InternalKeyword => AccessModifier.Internal,
            SyntaxKind.PrivateKeyword => AccessModifier.Private,
            _ => throw new NotImplementedException(),
        };
    }
}

