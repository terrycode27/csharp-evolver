public partial class SerializerSyntaxElementTree
{
    public TreeNode<SyntaxElementNode> Deserialize(string code)
    {
        var tokenSerializer = new SerializerTokenNodeTree();
        var tokenTree = tokenSerializer.Deserialize(code);
        var syntaxElementRoot = tokenTree.ToTreeSyntaxElement();
        return syntaxElementRoot;
    }
    public string Serialize(TreeNode<SyntaxElementNode>? root)
    {
        if (root == null)
            return "";
        var sb = new StringBuilder(8192 * 4);
        void Visit(TreeNode<SyntaxElementNode> node)
        {
            if (node.Value?.Text != null)
            {
                sb.Append(node.Value.Text);
            }
            foreach (var child in node.Children)
            {
                Visit(child);
            }
        }
        Visit(root);
        return sb.ToString();
    }
}

