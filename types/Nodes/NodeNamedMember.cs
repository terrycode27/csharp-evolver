public partial class NodeNamedMember : NodeNamed
{
    protected NodeNamedMember(SyntaxElementNode backing) : base(backing) { }
    public TreeNode<NodeSemantic> ModifierNode;
    public override AccessModifier Modifier
    {
        get
        {
            if (ModifierNode == null)
                return DefaultModifier;
            else
                return ModifierNode.Value.Kind.ToModifier();
        }
        set { SetModifier(value); }
    }
    public virtual TreeNode<NodeSemantic> NodeImplementation
    {
        get { return null; }
    }
    public virtual TreeNode<NodeSemantic> NodeSignatureAndAttribute => tree;
    public virtual TreeNode<NodeSemantic> NodeSignatureAttribute => NodeSignatureAndAttribute.Children[0];
    public virtual TreeNode<NodeSemantic> NodeSignatureMain => NodeSignatureAndAttribute.Children[1];
    public virtual bool ShouldCleanWhitespace => true;
    protected virtual AccessModifier DefaultModifier => AccessModifier.Private;
    public void AddClassAttribute(string className)
    {
        InsertNodeAttribute($"[ClassDoc(\"{className}\")]");
    }
    public void AddDocAttribute(string doc = null)
    {
        InsertNodeAttribute($"[Doc(\"{doc ?? TypeName ?? ""}\")]");
    }
    public void AddNodeAttribute(string attributeString)
    {
        InsertNodeAttribute(attributeString);
    }
    public void AddNodeAttribute(TreeNode<NodeSemantic> attrNode)
    {
        var decl = tree;
        if (decl.Children.Count == 0)
            decl.AddChild(attrNode);
        else
        {
            decl.Children.Insert(0, attrNode);
            attrNode.Parent = decl;
        }
    }
    public virtual TreeNode<NodeSemantic> CopyDeclaration()
    {
        return this.tree.CleanCopy();
    }
    public virtual void ExtractSignatureNode()
    {
    }
    public virtual string GetAbstractSignatureOnly()
    {
        string sig = GetSignatureOnly();
        return "public  " + sig;
    }
    public List<NodeAttributeList> GetAttributeList()
    {
        var list = tree
            .FindWhereStop(t => t.Value is NodeAttributeList, t => t == this.TypeNameNode)
            .Select(t => (NodeAttributeList)t.Value)
            .ToList();
        return list;
    }
    public virtual string GetSignatureOnly()
    {
        var copy = tree.CleanCopy();
        var vals = copy.DeleteStartingWithKinds(SyntaxKind.EqualsToken.ItemToHashSet()).ToList();
        var x = copy.DeleteKinds(SyntaxKindGroups.MemberModifierKeywords);
        var ret = copy.ToCode();
        if (!ret.Contains(";"))
            ret = ret + ";";
        return ret;
    }
    public void InsertNodeAttribute(string name, string label)
    {
        InsertNodeAttribute($"[{name}(\"{label}\")]");
    }
    public void InsertNodeAttribute(string attributeString)
    {
        var attrNode = NodeAttribute.FactoryNodeAttribute(attributeString);
        AddNodeAttribute(attrNode);
    }
    public bool IsExtensionMethod()
    {
        return this is NodeMethod method && method.IsExtension;
    }
    public virtual bool IsStaticMember()
    {
        var node = this.NodeSignatureMain;
          if (node.FindKind(SyntaxKind.StaticKeyword).Any())
            return true;
        return false;
    }
    public virtual TreeNode<NodeSemantic> SetModifier(AccessModifier modifier)
    {
        if (
            this is NodeConstructor
            || this is NodeDestructor
            || this.tree.Parent.Value is NodeInterface
        )
        {
            throw new Exception("");
        }
        string modString = $"{modifier.ToString().ToLower()}";
        if (ModifierNode != null)
        {
            ModifierNode.Value.Text = modString;
            return ModifierNode;
        }
        var syntaxKind = modifier.ToSyntaxKind();
        var modifierElement = new SyntaxElementNode(syntaxKind, modString);
        var modifierSemantic = new NodeUnknown(modifierElement);
        var modifierNode = new TreeNode<NodeSemantic>(modifierSemantic);
        modifierSemantic.SetTreeNode(modifierNode);
        tree.Children.Insert(0, modifierNode);
        ModifierNode = modifierNode;
        return ModifierNode;
    }
    public override void SetNameNode()
    {
        base.SetNameNode();
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> tree)
    {
        base.SetTreeNode(tree);
        SetModifierFromTree();
    }
    protected virtual void SetModifierFromTree()
    {
        ModifierNode = tree
            .FindWhere(t => SyntaxKindGroups.AccessModifierKinds.Contains(t.Value.Kind))
            .FirstOrDefault();
    }
}

