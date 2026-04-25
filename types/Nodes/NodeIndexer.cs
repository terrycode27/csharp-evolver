public partial class NodeIndexer : NodeProperty
{
    public NodeIndexer(SyntaxElementNode b) : base(b) { }
    public override string? TypeName { get => "this"; set { } }
    public override void SetNameNode() { }
}

