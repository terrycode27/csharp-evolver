public static partial class ExtensionsOfListOfNodeTypeDeclaration
{
    public static void CopyAttributesFrom(this List<NodeTypeDeclaration> dest, List<NodeTypeDeclaration> source)
    {
        var grp = (
            from d in dest
            join s in source on d.TypeName equals s.TypeName
            select new { d, s }
        ).ToList();
        foreach (var g in grp)
        {
            g.d.CopyAttributesFrom(g.s);
        }
    }
    public static TreeNode<NodeSemantic> SupersetTypeDeclarations(this List<NodeTypeDeclaration> types)
    {
        var sigs = types.Select(t => t.ReadSignature()).ToList();
        var sSig = sigs.SupersetSignatures();
        var members = types.SelectMany(t => t.MembersNode.Children).ToList();
        var supersetClass = members.WrapInDeclaration(sSig + "\n{\n\n}\n");
        return supersetClass;
    }
}

