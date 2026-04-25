public partial class SerializerTokenNodeTree
{
    public TreeNode<TokenNode> Deserialize(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var tokenNodeTree = tree.GetRoot().ToTreeTokenNode();
        return tokenNodeTree;
    }
    public string Serialize(TreeNode<TokenNode>? root)
    {
        if (root == null)
            return "";
        var sb = new StringBuilder(8192 * 8);
        void Visit(TreeNode<TokenNode> node)
        {
            if (node.Value is null)
                return;
            var cn = node.Value;
            foreach (var trivia in cn.LeadingTrivia)
            {
                if (trivia.Text is not null)
                    sb.Append(trivia.Text);
            }
            if (cn.Text is not null)
            {
                sb.Append(cn.Text);
            }
            foreach (var child in node.Children)
            {
                Visit(child);
            }
            foreach (var trivia in cn.TrailingTrivia)
            {
                if (trivia.Text is not null)
                    sb.Append(trivia.Text);
            }
        }
        Visit(root);
        var raw = sb.ToString();
        var lines = raw.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].TrimEnd();
        }
        return string.Join("\n", lines).TrimEnd();
    }
}

