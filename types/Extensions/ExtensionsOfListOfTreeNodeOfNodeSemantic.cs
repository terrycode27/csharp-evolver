public static partial class ExtensionsOfListOfTreeNodeOfNodeSemantic
{
    public static void CompareObjectCount(this List<TreeNode<NodeSemantic>> treeNodes)
    {
        var lists = treeNodes
            .Select(t =>
                t.GetNodeNamedList()
                    .Where(t => !(t is NodeAttribute))
                    .Select(t => t.FullName)
                    .ToList()
            )
            .ToList();
        var res = lists.CompareCollections().ToList();
        var onlyExistInOne = res.Where(r => r.Appearances == 1).ToList().First();
    }
    public static TreeNode<NodeSemantic> CreateNewPartialClassForInterface(this List<TreeNode<NodeSemantic>> methodsToMove, string className, string interfaceName)
    {
        string template =
            $@"public partial class {className} : {interfaceName}
{{
}}";
        var partialRoot = SerializerSemanticTree.DeserializeAndGroupCode(template);
        var newPartialClass = partialRoot
            .FindWhere(t => t.Value.Kind == SyntaxKind.ClassDeclaration)
            .First();
        var newBody = newPartialClass
            .FindWhere(t => t.Value.Kind == SyntaxKind.OpenBraceToken)
            .First();
        foreach (var method in methodsToMove)
        {
            method.RemoveSelf();
            newBody.AddChild(method);
        }
        newPartialClass.InitializeAllNodes();
        return newPartialClass;
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindDelimitedText(this List<TreeNode<NodeSemantic>> roots, HashSet<SyntaxKind> delimeters)
    {
        var include = new HashSet<SyntaxKind> { SyntaxKind.DotToken, SyntaxKind.IdentifierToken };
        return roots.FindText(include, delimeters);
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindText(this List<TreeNode<NodeSemantic>> roots, HashSet<SyntaxKind> include, HashSet<SyntaxKind> stopAt)
    {
        return roots
            .FindWhereStop(t => include.Contains(t.Value.Kind), t => stopAt.Contains(t.Value.Kind))
            .Where(t => t.Value != null && t.Value.Text != null && t.Value.Text != string.Empty)
            .ToList();
    }
    public static List<string> GetStrings(this List<TreeNode<NodeSemantic>> en)
    {
        return en.Select(t => t.Value.Text.Trim()).ToList();
    }
    public static List<List<TreeNode<NodeSemantic>>> GroupByKindModifier(this List<TreeNode<NodeSemantic>> children) => children.GroupBy(t => new { Kind = t.Value.Kind.ToHandled(), t.Value.Modifier }).Select(g => g.ToList()).ToList();
    public static void SetStaticExtensionMethodsPublic(this List<TreeNode<NodeSemantic>> extensionMethods)
    {
        var casted = extensionMethods.Select(t => (NodeMethod)t.Value).ToList();
        var privateMethods = casted.Where(t => t.Modifier == AccessModifier.Private).ToList();
        foreach (var p in privateMethods)
        {
            p.Modifier = AccessModifier.Public;
        }
    }
    public static List<TreeNode<NodeSemantic>> SortedByKindModifierName(this List<TreeNode<NodeSemantic>> children) => children.OrderBy(t => t.Value.Kind.ToHandled()).ThenBy(t => t.Value.Modifier).ThenBy(t => t.Value.TypeName ?? "").ToList();
    public static TreeNode<NodeSemantic> ToSingleNode(this List<TreeNode<NodeSemantic>> list)
    {
        var cu = NodeCompilationUnit.Factory();
        foreach (var l in list)
            cu.AddChild(l);
        return cu;
    }
    public static List<T> ToValues<T>(this List<TreeNode<NodeSemantic>> list) where T : NodeSemantic
    {
        return list.Select(t => (T)t.Value).ToList();
    }
    public static TreeNode<NodeSemantic> WrapInDeclaration(this List<TreeNode<NodeSemantic>> members, string declaration)
    {
        var root = SerializerSemanticTree.DeserializeAndGroupCode(declaration);
        var type = root.GetTypeDeclarations().Single();
        foreach (var m in members.OrderBy(m => m.Value.TypeName))
        {
            type.MembersNode.AddChild(m);
        }
        return root;
    }
}

