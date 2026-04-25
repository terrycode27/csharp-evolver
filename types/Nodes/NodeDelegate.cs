public partial class NodeDelegate : NodeTypeDeclaration
{
    public NodeDelegate(SyntaxElementNode b) : base(b) { }
    public override TreeNode<NodeSemantic> NodeSignatureAndAttribute => null;
    public override TreeNode<NodeSemantic> NodeSignatureAttribute => null;
    public override TreeNode<NodeSemantic> NodeSignatureMain => null;
    public NodeParameterList? ParameterList { get; set; }
    public override void AttachChild(NodeSemantic child)
    {
        if (child is NodeParameterList pl)
            ParameterList = pl;
    }
}

