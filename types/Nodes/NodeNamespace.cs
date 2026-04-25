public partial class NodeNamespace : NodeTypeDeclaration
{
    public NodeNamespace(SyntaxElementNode b) : base(b)
    {
        endDelimeter = new HashSet<SyntaxKind>() { SyntaxKind.OpenBraceToken };
    }
    public List<NodeSemantic> Members { get; } = new();
    public List<NodeUsingDirective> Usings { get; } = new();
    protected override AccessModifier DefaultModifier => AccessModifier.Public;
    public override void AttachChild(NodeSemantic child)
    {
        if (child is NodeUsingDirective u)
            Usings.Add(u);
        else
            Members.Add(child);
    }
    public static TreeNode<NodeSemantic> Factory(string name)
    {
        var code = string.Format("namespace {0}\n{{\n}}", name);
        var root = CSharpSyntaxTree.ParseText(code).GetRoot().ToTreeNodeSemantic();
        return root.Children.First(t => t.Value.Kind == SyntaxKind.NamespaceDeclaration);
    }
    public override SignatureData ReadSignature()
    {
        return new SignatureData
        {
            Attributes = ReadAttributes(),
            AccessModifiers = new List<string>(),
            TypeModifiers = new List<string>(),
            TypeName = this.TypeName,
            TypeKeyword = "namespace",
            BaseTypes = new List<string>(),
        };
    }
    public override void SetNameNode()
    {
        this.SetName(endDelimeter);
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> t)
    {
        base.SetTreeNode(t);
    }
    protected override void ParseBaseList() { }
}

