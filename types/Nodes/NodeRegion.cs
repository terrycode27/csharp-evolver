public partial class NodeRegion : NodeTrivia
{
    protected NodeRegion(SyntaxElementNode b) : base(b) { }
    public static TreeNode<NodeSemantic> Factory(string name)
    {
        var cu = NodeRegionStart.Factory(name);
        cu.AddChild(NodeCompilationUnit.Factory());
        cu.AddChild(NodeRegionEnd.Factory());
        return cu;
    }
}

