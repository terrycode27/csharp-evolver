public static partial class ExtensionsOfTreeNodeOfSyntaxElementNode
{
    public static TreeNode<NodeSemantic> BuildTree(this TreeNode<SyntaxElementNode> syntaxNode)
    {
        var semantic = syntaxNode.Value.CreateSemantic();
        var treeNode = new TreeNode<NodeSemantic>(semantic);
        foreach (var childSyntax in syntaxNode.Children)
        {
            var childTree = BuildTree(childSyntax);
            treeNode.AddChild(childTree);
            semantic.AttachChild(childTree.Value);
        }
        return treeNode;
    }
    public static TreeNode<NodeSemantic> ToTreeNodeSemantic(this TreeNode<SyntaxElementNode> syntaxRoot)
    {
        var tree = syntaxRoot.BuildTree();
        tree.InitializeAllNodes();
        return tree;
    }
}

