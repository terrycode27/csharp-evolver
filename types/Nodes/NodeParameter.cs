public partial class NodeParameter : NodeNamed
{
    public NodeParameter(SyntaxElementNode b) : base(b) { }
    public bool IsIn => Modifiers.Contains(SyntaxKind.InKeyword);
    public bool IsOut => Modifiers.Contains(SyntaxKind.OutKeyword);
    public bool IsParams => Modifiers.Contains(SyntaxKind.ParamsKeyword);
    public bool IsRef => Modifiers.Contains(SyntaxKind.RefKeyword);
    public bool IsThis => Modifiers.Contains(SyntaxKind.ThisKeyword);
    public List<SyntaxKind> Modifiers { get; } = new();
    public string ParameterName => this.tree.Children.Last().ToCode();
    public string ParameterTypeName
    {
        get
        {
            var list = ParameterTypeNode.GetTypedListKinds<NodeSemantic>(SyntaxKindGroups.TypeNameKinds).ToList();
            list = list.Where(t => !t.PathContains(SyntaxKind.TypeArgumentList)).ToList();
            return String.Join(".",list.Select(t=>t.tree.ToCode().Trim()));
        }
    }
    TreeNode<NodeSemantic> ParameterTypeNode => this.tree.Children.SkipLast(1).Last();
      public void RemoveDefaultAssignment()
    {
        var deleted = tree.DeleteStartingWithKinds(SyntaxKind.EqualsToken.ItemToHashSet());
    }
    public override void SetNameNode()
    {
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> t)
    {
        base.SetTreeNode(t);
        ParseModifiers();
    }
    private void ParseModifiers()
    {
        Modifiers.Clear();
        Modifiers.AddRange(
            tree
                .FindWhere(t => SyntaxKindGroups.MemberModifierKeywords.Contains(t.Value.Kind))
                .Select(t => t.Value.Kind)
        );
    }
}

