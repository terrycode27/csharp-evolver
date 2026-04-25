public static partial class ExtensionsOfTreeNodeOfTokenNode
{
    public static TreeNode<SyntaxElementNode> ToSyntaxElementTree(this TreeNode<TokenNode> tokenNode)
    {
        var token = tokenNode.Value;
        var elements = token.ToSyntaxElements();
        var allNodes = elements
            .Select(e => new TreeNode<SyntaxElementNode>(e))
            .ToList();
        var mainNodeIndex = token.LeadingTrivia.Count;
        var mainNode = allNodes[mainNodeIndex];
        foreach (var child in tokenNode.Children.Select(c => c.ToSyntaxElementTree()))
            mainNode.AddChild(child);
        for (int i = 0; i < allNodes.Count - 1; i++)
            allNodes[i].AddChild(allNodes[i + 1]);
        return allNodes[0];
    }
    public static TreeNode<SyntaxElementNode> ToTreeSyntaxElement(this TreeNode<TokenNode>? tokenRoot)
    {
        if (tokenRoot == null)
            throw new ArgumentNullException(nameof(tokenRoot));
        return tokenRoot.ToSyntaxElementTree();
    }
}

