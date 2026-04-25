public static partial class ExtensionsOfNodeTypeDeclaration
{
    public static void CopyAttributesFrom(this NodeTypeDeclaration dest, NodeTypeDeclaration source)
    {
        foreach (var attr in source.tree.GetTypedList<NodeAttributeList>())
        {
            dest.AddNodeAttribute(attr.tree);
        }
    }
    public static NodeAttribute SliceNodeAttribute(this NodeTypeDeclaration node) => node.tree.FindKind(SyntaxKind.AttributeList).SelectMany(a => a.GetTypedList<NodeAttribute>()).First(a => a.IsSlice());
}

