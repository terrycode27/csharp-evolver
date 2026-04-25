public partial class NodeParameterList : NodeSemantic
{
    public NodeParameterList(SyntaxElementNode b) : base(b) { }
    public List<NodeParameter> Parameters { get; } = new();
    public override void AttachChild(NodeSemantic child)
    {
        if (child is NodeParameter p)
            Parameters.Add(p);
    }
}

