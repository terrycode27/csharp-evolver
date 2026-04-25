public partial class NodeEventField : NodeField
{
    public NodeEventField(SyntaxElementNode b) : base(b) { }
    public override void SetNameNode()
    {
        TypeNameNode = tree.FindNameAfterTypeBeforeSemicolon();
    }
}

