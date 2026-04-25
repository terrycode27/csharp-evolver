public static partial class ExtensionsOfNodeNamedMember
{
    public static void CleanWhitespace(this NodeNamedMember namedMemberNode)
    {
        if (!namedMemberNode.ShouldCleanWhitespace)
            return;
        if (namedMemberNode.NodeSignatureAndAttribute == null)
            return;
        if (namedMemberNode.NodeImplementation == null || namedMemberNode.NodeImplementation.FindKind(SyntaxKind.EndOfLineTrivia).Count <= 1)
        {
            if (!(namedMemberNode is NodeTypeDeclaration))
                namedMemberNode.tree.DeleteNewlinesAndAdd();
        }
        else
        {
            namedMemberNode.NodeSignatureMain.DeleteNewlinesAndAdd();
        }
        if (namedMemberNode.NodeSignatureAttribute.Children.Count > 0)
        {
            namedMemberNode.NodeSignatureAttribute.DeleteKind(SyntaxKind.EndOfLineTrivia);
            foreach (var x in namedMemberNode.NodeSignatureAttribute.FindKind(SyntaxKind.AttributeList))
                x.DeleteNewlinesAndAdd();
        }
    }
    public static void CopyAttributesFrom(this NodeNamedMember dest, NodeNamedMember source)
    {
        var attrList = source.GetAttributeList();
        dest.tree.Children.InsertRange(0, attrList.Select(t => t.tree));
    }
}

