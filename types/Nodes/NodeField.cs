public partial class NodeField : NodeNamedMember
{
    public NodeField(SyntaxElementNode b) : base(b) { }
    public bool HasInitializer => Initializer != null;
    public TreeNode<NodeSemantic> Initializer { get; private set; }
    public override bool ShouldCleanWhitespace => false;
    public override void ExtractSignatureNode()
    {
        this.tree.SplitBefore(SyntaxKindGroups.FieldSigntureBoundaries);
    }
        public string InitializerCode()
    {
        return HasInitializer ? Initializer.ToCode().Trim() : string.Empty;
    }
    public override bool IsStaticMember()
    {
        return base.IsStaticMember();
    }
    public (string container, string member)? ReferencedMember()
    {
        var ids = Initializer
            .Flatten()
            .Where(n => n.Value.Kind == SyntaxKind.IdentifierToken)
            .Select(n => n.Value.Text)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();
        return ids.Count == 2 ? (ids[0], ids[1]) : null;
    }
    public override void SetNameNode()
    {
        TypeNameNode = tree.FindNameAfterTypeBeforeSemicolon();
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> tree)
    {
        base.SetTreeNode(tree);
        CaptureInitializer();
    }
    private TreeNode<NodeSemantic> BuildInitializerTree(TreeNode<NodeSemantic> from)
    {
        var wrapper = new TreeNode<NodeSemantic>(new NodeUnknown(new SyntaxElementNode(SyntaxKind.None, null)));
        var current = from;
        while (current != null && current.Value.Kind != SyntaxKind.SemicolonToken)
        {
            wrapper.AddChild(current.DeepClone());
            current = current.NextSibling();
        }
        return wrapper.Children.Count == 1 ? wrapper.Children[0] : wrapper;
    }
    private void CaptureInitializer()
    {
        var equals = FindEqualsToken();
        if (equals == null)
            return;
        var start = equals.NextSibling();
        Initializer = BuildInitializerTree(start);
    }
    private TreeNode<NodeSemantic> FindEqualsToken()
    {
        return tree.FindWhere(t => t.Value.Kind == SyntaxKind.EqualsToken).FirstOrDefault();
    }
}

