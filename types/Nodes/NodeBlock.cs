public partial class NodeBlock : NodeSemantic
{
    public NodeBlock(SyntaxElementNode b) : base(b) { }
    public List<NodeSemantic> Statements { get; } = new();
    public override void AttachChild(NodeSemantic child)
    {
        Statements.Add(child);
    }
}

