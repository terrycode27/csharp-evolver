public partial class NodeClass : NodeTypeDeclaration
{
    public NodeClass(SyntaxElementNode b) : base(b) { }
    public List<NodeDelegate> Delegates { get; } = new();
    public bool HasMethods => tree.FindWhere(t => t.Value.Kind == SyntaxKind.MethodDeclaration).Any();
    public bool IsAbstract => this.tree.FindKind(SyntaxKind.AbstractKeyword).Any();
    public bool IsPartial => this.tree.FindKind(SyntaxKind.PartialKeyword).Any();
    public bool IsSealed => this.tree.FindKind(SyntaxKind.SealedKeyword).Any();
    public bool IsStatic
    {
        get
        {
            var ret = tree
                .FindWhereStop(
                    t => t.Value.Kind == SyntaxKind.StaticKeyword,
                    t => t.Value.Kind == SyntaxKind.OpenBraceToken
                )
                .Any();
            return ret;
        }
    }
    public List<string> MethodsAndPropertySignatures => propertySignatures.Concat(methodSignatures).ToList();
    public List<string> methodSignatures => MembersNode.GetTypedList<NodeMethod>().Select(m => m.GetSignatureOnly()).ToList();
    public List<NodeNamedMember> NonMethodNamedMembers
    {
        get
        {
            return MembersNode
                .GetTypedList<NodeNamedMember>()
                .Where(m => !(m is NodeMethod))
                .ToList();
        }
    }
    public List<NodeMethod> NonPublicMethods
    {
        get
        {
            return GetMethods()
                .Where(m => m.Modifier != AccessModifier.Public)
                .ToList();
        }
    }
    public List<string> propertySignatures => MembersNode.GetTypedList<NodeProperty>().Select(p => p.GetSignatureOnly()).ToList();
    public List<NodeMethod> PublicMethods
    {
        get
        {
            return GetMethods()
                .Where(m => m.Modifier == AccessModifier.Public)
                .ToList();
        }
    }
    protected override AccessModifier DefaultModifier => AccessModifier.Private;
    public override void AttachChild(NodeSemantic child)
    {
        base.AttachChild(child);
        if (child is NodeDelegate del)
            Delegates.Add(del);
    }
    public TreeNode<NodeSemantic> BuildAbstractClass(Func<string, string> rename)
    {
        this.TypeName = rename(this.TypeName);
        List<TreeNode<NodeSemantic>> children = new List<TreeNode<NodeSemantic>>();
        foreach (var m in this.tree.GetTypedList<NodeNamedMember>())
        {
            if (m is NodeTypeDeclaration)
                continue;
            string val = m.GetAbstractSignatureOnly();
            var add = SerializerSemanticTree.DeserializeAndGroupCode(val);
            children.Add(add);
        }
        string sig = this.GetAbstractSignatureOnly();
        return children.WrapInDeclaration($"{sig}" + "{}");
    }
    public void CleanOpenDeclarationWhitespace()
    {
        var first = this.tree.Children.First();
        first.DeleteKind(SyntaxKind.EndOfLineTrivia);
        var bracket = first.FindKind(SyntaxKind.OpenBraceToken).First();
        bracket.WrapInNewLines();
    }
    public NodeInterface ExtractInterface(Func<string, string> rename)
    {
        this.TypeName = rename(TypeName);
        this.TypeKindNode.Value.Text = "interface ";
        string source = BuildSource();
        return (NodeInterface)SerializerSemanticTree.DeserializeAndGroupCode(source).Children.First().Value;
    }
    public void MarkAbstract()
    {
        if (IsAbstract)
            return;
        InsertBeforeKindNode(SyntaxKind.AbstractKeyword, "abstract ");
    }
    public void MarkInstance()
    {
        if (!IsStatic)
            return;
        this.tree.FindKindsStop(SyntaxKind.StaticKeyword.ItemToHashSet(), SyntaxKind.OpenBraceToken.ItemToHashSet())
            .Single()
            .DeleteKind(SyntaxKind.StaticKeyword);
    }
    public void MarkPartial()
    {
        if (IsPartial)
            return;
        InsertBeforeKindNode(SyntaxKind.PartialKeyword, "partial ");
    }
    public void MarkSealed()
    {
        if (IsSealed)
            return;
        InsertBeforeKindNode(SyntaxKind.SealedKeyword, "sealed ");
    }
    public void PruneDeclarationCopy()
    {
        tree.DeleteKinds(SyntaxKindGroups.AttributeKinds, DeleteType.NodeAndSubTree);
        tree.DeleteKinds(new HashSet<SyntaxKind>() { SyntaxKind.BaseList }, DeleteType.NodeAndSubTree);
        tree.DeleteKind(SyntaxKind.ParameterList, DeleteType.NodeAndSubTree);
        CleanOpenDeclarationWhitespace();
    }
    public override TreeNode<NodeSemantic> SetModifier(AccessModifier modifier)
    {
        return base.SetModifier(modifier);
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> t)
    {
        base.SetTreeNode(t);
    }
    protected override void SetModifierFromTree()
    {
        ModifierNode = tree
            .FindWhereStop(
                t => SyntaxKindGroups.AccessModifierKinds.Contains(t.Value.Kind),
                t => t.Value.Kind == SyntaxKind.OpenBraceToken
            )
            .FirstOrDefault();
    }
    private void AddKeywordToDeclaration(SyntaxKind kind)
    {
        var classKeyword = tree
            .FindWhere(t => t.Value.Kind == SyntaxKind.ClassKeyword)
            .FirstOrDefault();
        var val = kind.ToString().ToLower().Replace("keyword", "");
        string keywordText = val + " ";
        var element = new SyntaxElementNode(kind, keywordText);
        var semantic = new NodeUnknown(element);
        var newNode = new TreeNode<NodeSemantic>(semantic);
        semantic.SetTreeNode(newNode);
        classKeyword.InsertBefore(newNode);
    }
    private string BuildSource()
    {
        return $@"{this.GetSignatureOnly()}
{{
    {string.Join("\n    ", MethodsAndPropertySignatures)}
}}";
    }
    void ClearModifiers()
    {
        this.tree.Children.First().DeleteKinds(SyntaxKindGroups.ClassTypeModifierKeywords);
        this.SetModifierFromTree();
    }
    private void ConvertMethodsFromStaticExtension()
    {
        foreach (var m in tree.GetMethods())
        {
            ((NodeMethod)m).ConvertFromStaticExtension();
        }
    }
    private void ConvertMethodsToStaticExtension()
    {
        foreach (var m in tree.GetMethods())
        {
            m.ConvertToStaticExtension();
        }
    }
    private void ConvertToInstanceClass()
    {
        if (!IsStatic)
            return;
        RemoveStaticKeywordFromDeclaration();
        ConvertMethodsFromStaticExtension();
    }
    private void ConvertToStaticExtensionClass()
    {
        if (IsStatic)
            return;
        AddKeywordToDeclaration(SyntaxKind.StaticKeyword);
        ConvertMethodsToStaticExtension();
    }
    private void RemoveStaticKeywordFromDeclaration()
    {
        var staticKeyword = tree
            .FindWhere(t => t.Value.Kind == SyntaxKind.StaticKeyword)
            .FirstOrDefault();
        staticKeyword?.Delete(DeleteType.SingleNode);
    }
}

