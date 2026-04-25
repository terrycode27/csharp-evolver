public static partial class ExtensionsOfAccessModifier
{
    public static SyntaxKind ToSyntaxKind(this AccessModifier modifier)
    {
        return modifier switch
        {
            AccessModifier.Public => SyntaxKind.PublicKeyword,
            AccessModifier.Protected => SyntaxKind.ProtectedKeyword,
            AccessModifier.Internal => SyntaxKind.InternalKeyword,
            AccessModifier.Private => SyntaxKind.PrivateKeyword,
            _ => throw new NotImplementedException(),
        };
    }
}

