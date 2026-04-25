public static partial class ExtensionsOfListOfNodeNamedMember
{
    public static List<(NodeNamedMember s, NodeNamedMember d)> MatchNodesFrom(this List<NodeNamedMember> destination, List<NodeNamedMember> source)
    {
        var groupResult = (
            from s in source
            join d in destination on s.FullName equals d.FullName into gj
            from sub in gj.DefaultIfEmpty()
            select new { D = sub, S = s }
        ).ToList();
        var notInSource = groupResult.Where(x => x.S == null).Select(x => x.D).ToList();
        var matched = groupResult.Where(t => t.D != null && t.S != null).ToList();
        return matched.Select(x => (s: x.S, d: x.D)).ToList();
    }
    public static TreeNode<NodeSemantic> MoveTypes(this List<NodeNamedMember> membersToMove)
    {
        var docNode = NodeCompilationUnit.Factory();
        foreach (var type in membersToMove.GroupBy(t => t.ParentFullName))
        {
            var tdp = type.First().TypeDeclarationParent;
            var decl = tdp.CopyDeclaration();
            var members = NodeCompilationUnit.Factory();
            foreach (var m in type)
            {
                members.AddChild(m.tree);
            }
            decl.Children.Insert(1, members);
            docNode.Children.Add(decl);
        }
        return docNode;
    }
}

