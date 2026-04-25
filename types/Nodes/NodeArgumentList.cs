public partial class NodeArgumentList : NodeSemantic
{
    public NodeArgumentList(SyntaxElementNode b) : base(b) { }
    public List<NodeArgument> Arguments { get; } = new();
    public override void AttachChild(NodeSemantic child)
    {
        if (child is NodeArgument arg)
            Arguments.Add(arg);
    }
}

