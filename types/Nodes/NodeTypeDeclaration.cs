public partial class NodeTypeDeclaration : NodeNamedMember
{
    private enum SplitPhase
    {
        Before,
        Middle,
        After,
    }
    protected NodeTypeDeclaration(SyntaxElementNode b) : base(b) { }
    public TreeNode<NodeSemantic> BaseListNode;
    public List<string> BaseListRaw = new List<string>();
    public string BaseType;
    public TreeNode<NodeSemantic> TypeKindNode;
    public HashSet<string> BaseTypes { get; } = new(StringComparer.Ordinal);
    public virtual TreeNode<NodeSemantic> ClosingNode
    {
        get { return this.tree.Children[2]; }
    }
    public string FullNameWithTypeParameters
    {
        get
        {
            return FullName + "_" + string.Join("_", this.TypeParameters);
        }
    }
    public bool IsMergeKind => SyntaxKindGroups.SelfMergeKinds.Contains(this.Kind);
    public List<string> MemberNames
    {
        get { return MemberNodeNameds.Select(t => t.TypeName).ToList(); }
    }
    public List<NodeNamed> MemberNodeNameds
    {
        get { return this.MembersNode.GetTypedList<NodeNamed>(); }
    }
    public virtual TreeNode<NodeSemantic> MembersNode
    {
        get { return this.tree.Children[1]; }
    }
    public override TreeNode<NodeSemantic> NodeImplementation => MembersNode;
    public override TreeNode<NodeSemantic> NodeSignatureAndAttribute
    {
        get { return tree.Children[0].Children[0]; }
    }
    public List<string> TypeParameterConstraints { get; private set; } = new();
    public List<string> TypeParameters { get; private set; } = new();
    public override TreeNode<NodeSemantic> CopyDeclaration()
    {
        var decl = this.tree.Children.Where(t => t != MembersNode).ToList();
        string code = decl.ToCode();
        var ret = SerializerSemanticTree.DeserializeAndGroupCode(code);
        return ret.Children.First();
    }
    public void ExtractMembersNode()
    {
        var en = tree.Children.ToList().GetEnumerator();
        var cuOpening = NodeCompilationUnit.Factory();
        var cuMiddle = NodeCompilationUnit.Factory();
        var cuLast = NodeCompilationUnit.Factory();
        while (en.MoveNext())
        {
            cuOpening.AddChild(en.Current);
            if (en.Current.FindKind(SyntaxKind.OpenBraceToken).Any())
                break;
        }
        while (en.MoveNext())
        {
            if (en.Current.FindKind(SyntaxKind.CloseBraceToken).Any())
                break;
            cuMiddle.AddChild(en.Current);
        }
        while (true)
        {
            cuLast.AddChild(en.Current);
            if (!en.MoveNext())
                break;
        }
        this.tree.AddChild(cuOpening);
        this.tree.AddChild(cuMiddle);
        this.tree.AddChild(cuLast);
        this.tree.InitializeAllNodes();
    }
    public override void ExtractSignatureNode()
    {
        this.tree.Children[0].SplitBefore(SyntaxKind.OpenBraceToken.ItemToHashSet());
    }
    public override string GetAbstractSignatureOnly()
    {
        string sig = GetSignatureOnly();
        return "public  abstract " + sig;
    }
    public List<NodeNamed> GetMembers()
    {
        return this.tree.GetTypedListKinds<NodeNamed>(SyntaxKindGroups.ClassMemberKinds);
    }
    public List<NodeMethod> GetMethods()
    {
        return this.tree.GetTypedList<NodeMethod>();
    }
    public override string GetSignatureOnly()
    {
        var copy = tree.CleanCopy();
        var vals = copy.DeleteKinds(
            SyntaxKindGroups.MemberModifierKeywords,
            DeleteType.NodeAndSubTree
        );
        var hasPublic = copy.FindKind(SyntaxKind.PublicKeyword).Any();
        if (this.BaseListNode != null)
            this.BaseListNode.Delete(DeleteType.NodeAndSubTree);
        var a1 = copy.FindStopAt(SyntaxKind.OpenBraceToken.ItemToHashSet()).ToList();
        var ret = String.Join("", a1.Select(t => t.Value.Text));
        return ret;
    }
    public void InsertBeforeKindNode(SyntaxKind kwKind, string kwStr)
    {
        var partialNode = new TreeNode<NodeSemantic>(new NodeUnknown(new SyntaxElementNode(kwKind, kwStr)));
        this.TypeKindNode.InsertBefore(partialNode);
    }
    public override void ParseBaseClass()
    {
        if (BaseTypes.Count == 0)
            return;
        this.BaseType = BaseTypes.First();
    }
    public List<string> ReadAccessModifiers()
    {
        var modifier = Modifier.ToString().ToLower();
        return modifier == "none" ? new List<string>() : new List<string> { modifier };
    }
    public List<string> ReadAttributes()
    {
        return GetAttributeList().Select(attrList => attrList.tree.ToCode().Trim()).ToList();
    }
    public List<string> ReadBaseTypes()
    {
        return BaseListRaw.ToList();
    }
    public virtual SignatureData ReadSignature()
    {
        var ret = new SignatureData
        {
            Attributes = ReadAttributes(),
            AccessModifiers = ReadAccessModifiers(),
            TypeModifiers = ReadTypeModifiers(),
            TypeKeyword = this.GetTypeKeyword(),
            TypeName = this.TypeName,
            BaseTypes = ReadBaseTypes(),
            TypeParameters = TypeParameters.ToList(),
            TypeParameterConstraints = TypeParameterConstraints.ToList()
        };
        return ret;
    }
    public List<string> ReadTypeModifiers()
    {
        var modifiers = new List<string>();
        var typeModifierKinds = new HashSet<SyntaxKind>        {
            SyntaxKind.AbstractKeyword,
            SyntaxKind.PartialKeyword,
            SyntaxKind.SealedKeyword,
            SyntaxKind.StaticKeyword,
        };
        foreach (var kind in typeModifierKinds)
        {
            if (this.NodeSignatureAndAttribute.FindKind(kind).Any())
            {
                modifiers.Add(kind.ToString().Replace("Keyword", "").ToLower());
            }
        }
        return modifiers;
    }
    public void RegenerateBaseList()
    {
        if (BaseListRaw.Count == 0)
        {
            RemoveExistingBaseListIfPresent();
            return;
        }
        var newBaseListNode = SerializerSemanticTree.DeserializeAndGroupCode(GetBaseListCodeString());
        if (BaseListNode != null)
        {
            BaseListNode.ReplaceSelf(newBaseListNode);
        }
        else
        {
            InsertBaseListBeforeOpeningBrace(newBaseListNode);
        }
        ParseBaseList();
        ParseBaseClass();
    }
    public override void SetNameNode()
    {
        this.TypeNameNode = this.tree.FindKindsAfter(this.GetTypeSpecificKeyword().Value.ItemToHashSet(), SyntaxKind.IdentifierToken.ItemToHashSet()).First();
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> t)
    {
        base.SetTreeNode(t);
        ParseBaseList();
        ParseBaseClass();
        ParseTypeKindNode();
        ParseTypeParameters();
        ParseTypeParameterConstraints();
    }
    public void WriteAccessModifiers(List<string> accessModifiers)
    {
        if (accessModifiers.Count == 0)
        {
            if (ModifierNode != null)
            {
                ModifierNode.RemoveSelf();
                ModifierNode = null;
            }
        }
        else
        {
            var modifierStr = accessModifiers[0];
            var modifier = modifierStr.ToLower() switch
            {
                "public" => AccessModifier.Public,
                "private" => AccessModifier.Private,
                "protected" => AccessModifier.Protected,
                "internal" => AccessModifier.Internal,
                _ => AccessModifier.Private,
            };
            SetModifier(modifier);
        }
    }
    public void WriteAttributes(List<string> attributes)
    {
        var existingAttrs = tree.FindKind(SyntaxKind.AttributeList);
        foreach (var attr in existingAttrs.ToList())
        {
            attr.RemoveSelf();
        }
        foreach (var attr in attributes)
        {
            AddNodeAttribute(attr);
        }
    }
    public void WriteBaseTypes(List<string> baseTypes)
    {
        BaseListRaw = baseTypes.ToList();
        RegenerateBaseList();
    }
    public void WriteSignature(SignatureData signature)
    {
        string signatureText = signature.ToString();
        WriteSignature(signatureText);
    }
    public void WriteSignature(string signature)
    {
        var parsed = SerializerSemanticTree.DeserializeCodeNoGroup(signature);
        this.NodeSignatureAndAttribute.ReplaceSelf(parsed);
    }
    public void WriteTypeModifiers(List<string> typeModifiers)
    {
        var typeModifierKinds = new HashSet<SyntaxKind>        {
            SyntaxKind.AbstractKeyword,
            SyntaxKind.PartialKeyword,
            SyntaxKind.SealedKeyword,
            SyntaxKind.StaticKeyword,
        };
        foreach (var kind in typeModifierKinds)
        {
            var nodes = tree.FindKind(kind);
            foreach (var node in nodes.ToList())
            {
                node.RemoveSelf();
            }
        }
        foreach (var modifier in typeModifiers)
        {
            var kind = modifier.ToLower() switch
            {
                "abstract" => SyntaxKind.AbstractKeyword,
                "sealed" => SyntaxKind.SealedKeyword,
                "partial" => SyntaxKind.PartialKeyword,
                "static" => SyntaxKind.StaticKeyword,
                _ => (SyntaxKind?)null,
            };
            if (kind.HasValue)
            {
                InsertBeforeKindNode(kind.Value, modifier.ToLower());
            }
        }
    }
    public void WriteTypeParameters(List<string> typeParameters)
    {
    }
    protected virtual SyntaxKind? GetTypeSpecificKeyword()
    {
        if (SyntaxKindGroups.DeclarationToKeywordMap.TryGetValue(this.Kind, out var keyword))
            return keyword;
        return SyntaxKind.CompilationUnit;   // fallback
    }
    protected virtual void ParseBaseList()
    {
        BaseTypes.Clear();
        BaseListNode = tree.FindWhere(t => t.Value.Kind == SyntaxKind.BaseList).FirstOrDefault();
        if (BaseListNode == null)
            return;
        var simpleBases = BaseListNode.FindWhere(t => t.Value.Kind == SyntaxKind.SimpleBaseType);
        foreach (var sb in simpleBases)
        {
            var typeName = sb.ToCode().Trim();
            BaseListRaw.Add(typeName);
            typeName = StripGenerics(typeName);
            if (typeName.Length > 0)
            {
                if (!BaseTypes.Contains(typeName))
                    BaseTypes.Add(typeName);
            }
        }
        BaseListRaw = BaseListRaw.Distinct().ToList();
    }
    string GetBaseListCodeString()
    {
        string bases = string.Join(", ", BaseListRaw);
        return $": {bases}";
    }
    private List<TreeNode<NodeSemantic>> GetHeaderNodes()
    {
        return tree
            .FindStopAt(new HashSet<SyntaxKind> { SyntaxKind.OpenBraceToken })
            .Where(n => n.Value.Kind != SyntaxKind.OpenBraceToken)
            .ToList();
    }
    private string GetTypeKeyword()
    {
        if (TypeKindNode?.Value?.Text != null)
            return TypeKindNode.Value.Text.Trim().TrimEnd();
        return this switch
        {
            NodeClass => "class",
            NodeInterface => "interface",
            NodeStruct => "struct",
            NodeRecord => "record",
            NodeEnum => "enum",
            NodeDelegate => "delegate",
            NodeNamespace => "namespace",
            _ => "class",
        };
    }
    private void InsertBaseListBeforeOpeningBrace(TreeNode<NodeSemantic> baseListNode)
    {
        var openBrace = tree.FindKind(SyntaxKind.OpenBraceToken).First();
        openBrace.InsertBefore(baseListNode);
    }
    private void InsertNewHeader(string signature)
    {
        var temp = SerializerSemanticTree.DeserializeAndGroupCode(signature.TrimEnd() + "\n{}");
        var newHeader = temp.Children[0]
            .Children.TakeWhile(n => n.Value.Kind != SyntaxKind.OpenBraceToken)
            .ToList();
        foreach (var node in newHeader.AsEnumerable().Reverse())
        {
            tree.Children.Insert(0, node);
            node.Parent = tree;
        }
    }
    void ParseTypeKindNode()
    {
        var keywordKind = GetTypeSpecificKeyword();
        if (keywordKind.HasValue)
        {
            TypeKindNode = tree.FindWhere(t => t.Value.Kind == keywordKind.Value).FirstOrDefault();
        }
    }
    private void ParseTypeParameterConstraints()
    {
        TypeParameterConstraints = tree
            .FindWhere(t => t.Value.Kind == SyntaxKind.WhereClause)
            .Select(t => t.ToCode().Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();
    }
    private void ParseTypeParameters()
    {
        TypeParameters.Clear();
        var typeParamList = tree
               .FindWhereStop(
                   t => t.Value.Kind == SyntaxKind.TypeParameterList,
                   t => t.Value.Kind == SyntaxKind.OpenBraceToken ||
                        t.Value.Kind == SyntaxKind.SemicolonToken ||
                        t.Value.Kind == SyntaxKind.BaseList  // also stop at base list if present
               )
               .FirstOrDefault();
        if (typeParamList == null)
            return;
        TypeParameters = typeParamList
            .FindWhere(t => t.Value.Kind == SyntaxKind.TypeParameter)
            .Select(t => t.ToCode().Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();
    }
    private void RemoveExistingBaseListIfPresent()
    {
        if (BaseListNode != null)
        {
            BaseListNode.RemoveSelf();
            BaseListNode = null;
        }
    }
    private void RemoveHeader(List<TreeNode<NodeSemantic>> header)
    {
        foreach (var node in header)
            node.Delete(DeleteType.SingleNode);
    }
    private static string StripGenerics(string s)
    {
        int i = s.IndexOf('<');
        return i < 0 ? s : s.Substring(0, i).TrimEnd();
    }
}

