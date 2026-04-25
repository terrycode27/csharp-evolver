public partial class NodeInvocationExpression : NodeSemantic
{
    public NodeInvocationExpression(SyntaxElementNode b) : base(b) { }
    public NodeArgumentList? ArgumentList { get; set; }
    public List<NodeArgument> Arguments => ArgumentList?.Arguments ?? new();
    public TreeNode<NodeSemantic>? GenericTypeArgumentList { get; private set; }
    public TreeNode<NodeSemantic>? InvokedExpression { get; private set; }
    public bool IsExtensionMethodCall { get; private set; }
    public bool IsStaticCall { get; private set; }
    public string? MethodName { get; private set; }
    public TreeNode<NodeSemantic>? Receiver { get; private set; }
    public int TotalArgumentCount
    {
        get
        {
            int i = this.Arguments.Count;
            if (this.IsExtensionMethodCall)
                i++;
            return i;
        }
    }
    public override void AttachChild(NodeSemantic child)
    {
        if (child is NodeArgumentList al)
            ArgumentList = al;
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> tree)
    {
        base.SetTreeNode(tree);
        ParseInvocation();
    }
    public override string ToString() => MethodName ?? "unknown-invocation";
    private void DetermineExtensionMethodCall()
    {
        IsExtensionMethodCall = Arguments.Any(a => a.IsThisArgument);
    }
    private void DetermineStaticCall()
    {
        IsStaticCall = Receiver == null && !IsExtensionMethodCall;
    }
    private void FindInvokedExpression()
    {
        InvokedExpression =
            tree.Children.FirstOrDefault(c =>
                !SyntaxKindGroups.ArgumentListKind.Contains(c.Value.Kind)
            )
            ?? tree
                .FindWhere(t => SyntaxKindGroups.InvokedExpressionKinds.Contains(t.Value.Kind))
                .FirstOrDefault();
    }
    private string GetRichDescription()
    {
        var receiver = Receiver?.ToCode()?.Trim() ?? (IsStaticCall ? "static" : "local");
        var generics = GenericTypeArgumentList != null ? "<…>" : "";
        return $"{receiver}.{MethodName}{generics}({Arguments.Count} args)";
    }
    private static bool IsInsideTypeArgumentList(TreeNode<NodeSemantic> node)
    {
        var current = node;
        while (current != null)
        {
            if (SyntaxKindGroups.TypeArgumentListKind.Contains(current.Value.Kind))
                return true;
            current = current.Parent;
        }
        return false;
    }
    private void ParseInvocation()
    {
        FindInvokedExpression();
        ResolveMethodName();
        ResolveReceiver();
        ResolveGenericArguments();
        DetermineExtensionMethodCall();
        DetermineStaticCall();
    }
    private void ResolveGenericArguments()
    {
        GenericTypeArgumentList = InvokedExpression
            ?.FindWhere(t => SyntaxKindGroups.TypeArgumentListKind.Contains(t.Value.Kind))
            .FirstOrDefault();
    }
    private void ResolveMethodName()
    {
        if (InvokedExpression == null)
        {
            MethodName = null;
            return;
        }
        MethodName = InvokedExpression
            .FindWhereStop(
                t => t.Value.Kind == SyntaxKind.IdentifierToken,
                t => SyntaxKindGroups.ArgumentListKind.Contains(t.Value.Kind)
            )
            .Where(t => !IsInsideTypeArgumentList(t))
            .LastOrDefault()
            ?.Value?.Text?.Trim();
        if (string.IsNullOrWhiteSpace(MethodName))
            MethodName = InvokedExpression.ToCode()?.Trim();
    }
    private void ResolveReceiver()
    {
        if (
            !SyntaxKindGroups.MemberAccessKinds.Contains(
                InvokedExpression?.Value.Kind ?? SyntaxKind.None
            )
        )
        {
            Receiver = null;
            return;
        }
        Receiver = InvokedExpression!.Children.FirstOrDefault(c =>
            !SyntaxKindGroups.MemberAccessOperatorKinds.Contains(c.Value.Kind)
        );
    }
}

