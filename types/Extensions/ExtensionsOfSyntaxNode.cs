public static partial class ExtensionsOfSyntaxNode
{
    public static TreeNode<NodeSemantic> ToTreeNodeSemantic(this SyntaxNode node)
    {
        var ret = node.ToTreeTokenNode().ToTreeSyntaxElement().ToTreeNodeSemantic();
        return ret;
    }
    public static TreeNode<TokenNode> ToTreeTokenNode(this SyntaxNode node)
    {
        var current = new TreeNode<TokenNode>(new TokenNode(node));
        foreach (var child in node.ChildNodesAndTokens())
        {
            if (child.IsNode)
            {
                var childNode = child.AsNode()!;
                if (childNode.FullSpan.Length == 0)
                    continue;
                current.AddChild(ToTreeTokenNode(childNode));
            }
            else if (child.IsToken)
            {
                var tok = child.AsToken();
                if (tok.Kind() is SyntaxKind.EndOfFileToken)
                    continue;
                current.AddChild(new TreeNode<TokenNode>(new TokenNode(tok)));
            }
        }
        return current;
    }
}

