public partial class NodeRegionStart : NodeRegion
{
    public NodeRegionStart(SyntaxElementNode b) : base(b) { }
    public NodeRegionStart(string name) : base(new SyntaxElementNode(SyntaxKind.RegionDirectiveTrivia, $"\n#region {name}\n"))
    {
        this.TypeName = name;
    }
    public override string? TypeName { get; set; }
    public static new TreeNode<NodeSemantic> Factory(string regionName)
    {
        if (string.IsNullOrWhiteSpace(regionName))
            regionName = "Untitled Region";
        var text = $"\n#region {regionName.Trim()}\n";
        var element = new SyntaxElementNode(SyntaxKind.RegionDirectiveTrivia, text);
        var semantic = new NodeRegionStart(element);
        return new TreeNode<NodeSemantic>(semantic);
    }
}

