public partial class NodeCompilationUnit : NodeSemantic
{
    public NodeCompilationUnit() : base(new SyntaxElementNode(SyntaxKind.CompilationUnit, null)) { }
    public NodeCompilationUnit(SyntaxElementNode b) : base(b) { }
    public List<object> KeyPath;
    public List<NodeSemantic> Members { get; } = new();
    public List<NodeUsingDirective> Usings { get; } = new();
    public override void AttachChild(NodeSemantic child)
    {
        if (child is NodeUsingDirective u)
            Usings.Add(u);
        else
            Members.Add(child);
    }
    public static TreeNode<NodeSemantic> Factory(IEnumerable<TreeNode<NodeSemantic>> children)
    {
        var cu = Factory();
        cu.AddChildren(children);
        return cu;
    }
    public static TreeNode<NodeSemantic> Factory()
    {
        return new TreeNode<NodeSemantic>(new NodeCompilationUnit());
    }
}

