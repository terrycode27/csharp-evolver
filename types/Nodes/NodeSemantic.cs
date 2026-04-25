public partial class NodeSemantic : NodeBase<NodeSemantic>
{
    protected NodeSemantic(SyntaxElementNode backing)
    {
        Kind = backing.Kind;
        Text = backing.Text;
    }
    public SyntaxKind Kind;
    public static List<Func<TreeNode<NodeSemantic>, object>> ModifierKindName = new() { t => t.Value.Modifier, t => t.Value.Kind.ToHandled(), t => t.Value.TypeName, };
    public static Func<TreeNode<NodeSemantic>, bool> moveable = t => (t.Value.HasName && t.Value.Kind != SyntaxKind.EnumMemberDeclaration);
    public string? Text;
    string kindFullName;
    List<SyntaxKind> kindPath;
    public bool ChildOnly
    {
        get { return !this.tree.FindWhere(t => t.Value.HasName).Skip(1).Any(); }
    }
    public string ContainingClassName => FindContainingClassName();
    public IEnumerable<TreeNode<NodeSemantic>> ContainingTypeDeclarationNesting
    {
        get
        {
            var parent = this.tree;
            while (parent != null)
            {
                if (parent.Value is NodeTypeDeclaration)
                    yield return parent;
                parent = parent.Parent;
            }
        }
    }
    public virtual string FullName
    {
        get
        {
            var list = new List<string>()
            {
                ParentFullName,
                Kind == SyntaxKind.CompilationUnit ? "" : Kind.ToString(),
                TypeName,
            };
            return string.Join(".", list.Where(t => !String.IsNullOrEmpty(t)));
        }
    }
    public bool HasDirectNamedChildren
    {
        get { return this.tree.Children.Where(t => t.Value is NodeNamedMember).Any(); }
    }
    public virtual bool HasName => TypeName != null && TypeName != string.Empty;
    public bool IsWhitespace
    {
        get
        {
            if (Text == null)
                return true;
            if (Text.Trim() == string.Empty)
                return true;
            return false;
        }
    }
    public Dictionary<string, List<string>> KindDictionary => tree.ToKindDictionary();
    public string KindFullName
    {
        get
        {
             if (kindFullName == null)
                kindFullName = ComputeKindFullName(null);
            return kindFullName;
        }
    }
    public virtual int LineCount
    {
        get
        {
            string code = tree.ToCode().NormalizeLineEndings();
            if (string.IsNullOrWhiteSpace(code))
                return 0;
            return Regex.Matches(code, @"^\s*\S", RegexOptions.Multiline).Count;
        }
    }
    public virtual AccessModifier Modifier { get; set; } = AccessModifier.None;
    public string ParentFullName
    {
        get
        {
            if (this.tree.Parent == null)
                return string.Empty;
            return this.tree.Parent.Value.FullName;
        }
    }
    public TreeNode<NodeSemantic> ParentNonContainer
    {
        get
        {
            var par = this.tree.Parent;
            while (par != null && par.Value.Kind == SyntaxKind.CompilationUnit)
            {
                par = par.Parent;
            }
            return par;
        }
    }
    public List<string> Parents
    {
        get
        {
            var x = this.tree.Parent;
            var names = new List<string>();
            while (x != null)
            {
                if (x.Value.HasName)
                    names.Add(x.Value.TypeName);
                x = x.Parent;
            }
            names.Reverse();
            return names;
        }
    }
    public TreeNode<NodeSemantic> SemanticParent
    {
        get
        {
            var parent = this.tree.Parent;
            while (parent != null)
            {
                if (parent.Value.Kind != SyntaxKind.CompilationUnit)
                    break;
                parent = parent.Parent;
            }
            return parent;
        }
    }
    public NodeTypeDeclaration TypeDeclarationParent => (NodeTypeDeclaration)ContainingTypeDeclarationNesting.First().Value;
    public virtual string? TypeName
    {
        get => TypeNameNode?.Value.Text;
        set
        {
            if (TypeNameNode != null)
                TypeNameNode.Value.Text = value;
        }
    }
    public virtual TreeNode<NodeSemantic> TypeNameNode { get; set; }
    List<SyntaxKind> KindPath
    {
        get
        {
            if (kindPath == null)
                kindPath = tree.ComputeKindPath(null);
            return kindPath;
        }
    }
    public void AddKind(SyntaxKind kind, string str)
    {
        if (tree.FindKind(kind).Any())
            return;
        tree.InsertAfter(GetKindNode(kind, str));
    }
    public void AddSemicolon()
    {
        AddKind(SyntaxKind.SemicolonToken, ";");
    }
    public virtual void AttachChild(NodeSemantic child) { }
    public string ComputeKindFullName(TreeNode<NodeSemantic>topLevel)
    {
        var kinds = tree.ComputeKindPath(topLevel);
        var ret= string.Join(".", kinds);
        return ret;
    }
    public T GetParentType<T>() where T : NodeSemantic
    {
        var ret = this.tree.Parent;
        while (!(ret.Value is T))
            ret = ret.Parent;
        return (T)ret.Value;
    }
    public bool IsTreeHealthy()
    {
        return this.tree.Flatten().All(n => n.Parent != null || n.IsRoot)
            && !string.IsNullOrWhiteSpace(this.tree.ToCode());
    }
    public virtual void ParseBaseClass() { }
    public bool PathContains(SyntaxKind kind)
    {
        return tree.ComputeKindPath().Contains(kind);
    }
    public virtual void SetTreeNode(TreeNode<NodeSemantic> tree)
    {
        this.tree = tree;
    }
    public override string ToString()
    {
        return $"{(Modifier == AccessModifier.None ? "" : Modifier)} {Kind} {TypeName}";
    }
    private string? FindContainingClassName()
    {
        if (tree == null)
            return null;
        var current = tree.Parent;
        while (current != null)
        {
            if (current.Value is NodeClass NodeClass && !string.IsNullOrEmpty(NodeClass.TypeName))
                return NodeClass.TypeName;
            current = current.Parent;
        }
        return null;
    }
    private TreeNode<NodeSemantic> GetKindNode(SyntaxKind kind, string str)
    {
        return new TreeNode<NodeSemantic>(new NodeUnknown(new SyntaxElementNode(kind, str)));
    }
}

