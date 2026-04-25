public partial class NodeParameterizedMemberWithBody : NodeNamedMember
{
    protected NodeParameterizedMemberWithBody(SyntaxElementNode b) : base(b) { }
    bool hasBody;
    public NodeBlock? Body { get; set; }
    public override string FullName
    {
        get { return base.FullName + Parameters; }
    }
    public bool IsExtension => ParameterList.Parameters.Count > 0 && ParameterList.Parameters.First().IsThis;
    public bool IsStatic => tree.FindWhere(t => t.Value.Kind == SyntaxKind.StaticKeyword).Any();
    public bool IsVirtual
    {
        get { return tree.FindKinds(SyntaxKindGroups.VirtualFunctionSyntaxKinds).Any(); }
    }
    public override TreeNode<NodeSemantic> NodeImplementation { get => tree.Children[1]; }
    public override TreeNode<NodeSemantic> NodeSignatureAndAttribute
    {
        get
        {
            if (!hasBody)
                return tree;
            return tree.Children[0];
        }
    }
    public NodeParameterList? ParameterList { get; set; }
    public Dictionary<string,string> WhereClauseDictionary
    {
        get
        {
            if (this.WhereClauseMap == null)
                return new Dictionary<string,string>();
            return WhereClauseMap.ToDictionary(t => t.Item1, t => t.Item2);
        }
    }
    public List<(string, string)> WhereClauseMap
    {
        get
        {
            if (WhereClauseTypeList == null || WhereClauseNameList == null)
                return null;
            var ret=WhereClauseNameList.JoinByIndex(WhereClauseTypeList).ToList();
            return ret;
        }
    }
    public List<string> WhereClauseNameList => KindDictionary.IfHas("TypeParameterConstraintClause.IdentifierName.IdentifierToken");
    public List<string> WhereClauseTypeList => KindDictionary.IfHas("TypeParameterConstraintClause.TypeConstraint.IdentifierName.IdentifierToken");
    List<string> Parameters
    {
        get
        {
            throw new NotImplementedException();
        }
    }
    public override void AttachChild(NodeSemantic child)
    {
        if (child is NodeParameterList pl)
            ParameterList = pl;
        if (child is NodeBlock b)
            Body = b;
    }
    public void ConvertDefaultParameters()
    {
        if (ParameterList == null)
            return;
        foreach (var p in ParameterList.Parameters)
            p.RemoveDefaultAssignment();
    }
    public void ConvertFromStaticExtension()
    {
        if (!IsStatic || !IsExtension)
            return;
        RemoveStaticKeyword();
        RemoveThisKeywordFromFirstParameter();
        tree.InitializeAllNodes();
    }
    public void ConvertToStaticExtension()
    {
        if (IsStatic && IsExtension)
            return;
        EnsureStatic();
        EnsureFirstParameterIsThis();
        tree.InitializeAllNodes();
    }
    public override void ExtractSignatureNode()
    {
        hasBody = this.tree.SplitBefore(SyntaxKindGroups.MethodSigntureBoundaries);
    }
    public override string GetAbstractSignatureOnly()
    {
        string sig = GetSignatureOnly();
        return "public  abstract " + sig;
    }
    public string GetBaseSignature()
    {
        this.ConvertDefaultParameters();
        tree.DeleteKinds(SyntaxKindGroups.MemberModifierKeywords);
        var a1 = tree
            .FindStopAt(
                new HashSet<SyntaxKind>()                {
                    SyntaxKind.ArrowExpressionClause,
                    SyntaxKind.OpenBraceToken,
                }
            )
            .ToList();
        var ret = String.Join("", a1.Select(t => t.Value.Text));
        return ret;
    }
       public override string GetSignatureOnly()
    {
        this.ConvertDefaultParameters();
        string ret = GetBaseSignature();
        if (!ret.Contains(";"))
            ret = ret + ";";
        return ret;
    }
    public string GetStaticExtensionClassName()
    {
        var thisParam = tree
        .FindWhere(t => t.Value is NodeParameter p && p.IsThis)
        .Select(t => (NodeParameter)t.Value)
        .FirstOrDefault();
        var whereDictionary = this.WhereClauseDictionary;
        var list = new List<string>();
        list.Add("Extensions");
        list.Add(thisParam.ParameterTypeName.Replace(".",""));
        var nodeArgumentList=thisParam.tree.GetTypedList<NodeTypeArgumentList>().FirstOrDefault();
        if(nodeArgumentList!=null)
        {
            foreach(var x in nodeArgumentList.ArgumentList)
            {
                list.Add(String.Join("", x.Select(t=>whereDictionary.ValOrSearchKey(t).Replace(".", "").CapitalizeFirstLetter())));
            }
        }
        var ret = string.Join("Of", list.Select(t=>t.CapitalizeFirstLetter()));
        return ret;
    }
      public override void SetNameNode()
    {
        base.SetNameNode();
    }
    public TreeNode<NodeSemantic> WrapInClassDeclaration()
    {
        return this.tree.WrapInDeclaration($"public partial class {TypeName}" + "{}");
    }
    private void EnsureFirstParameterIsThis()
    {
        if (ParameterList?.Parameters.Count == 0)
            return;
        var firstParam = ParameterList.Parameters[0];
        if (firstParam.IsThis)
            return;
        var paramTree = firstParam.tree;
        var insertPoint =
            paramTree
                .FindWhere(t =>
                    SyntaxKindGroups.MemberModifierKeywords.Contains(t.Value.Kind)
                    || SyntaxKindGroups.KindsContainingParameterType.Contains(t.Value.Kind)
                )
                .FirstOrDefault()
            ?? paramTree.Children.FirstOrDefault();
        if (insertPoint == null)
            return;
        var element = new SyntaxElementNode(SyntaxKind.ThisKeyword, "this ");
        var semantic = new NodeUnknown(element);
        var thisNode = new TreeNode<NodeSemantic>(semantic);
        semantic.SetTreeNode(thisNode);
        insertPoint.InsertBefore(thisNode);
    }
    private void EnsureStatic()
    {
        if (IsStatic)
            return;
        var element = new SyntaxElementNode(SyntaxKind.StaticKeyword, " static ");
        var semantic = new NodeUnknown(element);
        var newNode = new TreeNode<NodeSemantic>(semantic);
        semantic.SetTreeNode(newNode);
        var lastKeyword = tree
            .FindWhereStop(
                t => SyntaxKindGroups.MemberModifierKeywords.Contains(t.Value.Kind),
                t => t.Value.Kind == SyntaxKind.OpenParenToken
            )
            .LastOrDefault();
        if (lastKeyword == null)
        {
            tree.InsertBefore(newNode);
        }
        else
            lastKeyword.InsertAfter(newNode);
    }
    private TreeNode<NodeSemantic>? GetReturnTypeNode()
    {
        if (TypeNameNode == null)
            return null;
        return tree
            .FindWhereStop(
                t => SyntaxKindGroups.KindsContainingParameterType.Contains(t.Value.Kind),
                t => t == TypeNameNode
            )
            .LastOrDefault();
    }
    private string GetSignature(Action removeIllegal)
    {
        ConvertFromStaticExtension();
        RemoveMethodBody();
        removeIllegal();
        var ret = tree.ToCode();
        if (!ret.Contains(";"))
            ret = ret + ";";
        return ret;
    }
    private void RemoveMethodBody()
    {
        var bodies = tree
            .FindWhere(t =>
                t.Value.Kind == SyntaxKind.Block || t.Value.Kind == SyntaxKind.ArrowExpressionClause
            )
            .ToList();
        foreach (var body in bodies)
            body.Delete(DeleteType.NodeAndSubTree);
    }
    private void RemoveStaticKeyword()
    {
        var staticNode = tree
            .FindWhere(t => t.Value.Kind == SyntaxKind.StaticKeyword)
            .FirstOrDefault();
        staticNode?.Delete(DeleteType.SingleNode);
    }
    private void RemoveThisKeywordFromFirstParameter()
    {
        if (ParameterList?.Parameters.Count == 0)
            return;
        var firstParam = ParameterList.Parameters[0];
        if (!firstParam.IsThis)
            return;
        var thisKeywordNode = firstParam
            .tree.FindWhere(t => t.Value.Kind == SyntaxKind.ThisKeyword)
            .FirstOrDefault();
        thisKeywordNode?.Delete(DeleteType.SingleNode);
    }
}

