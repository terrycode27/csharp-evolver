public partial class SerializerSemanticTree
{
    public TreeNode<NodeSemantic> Deserialize(string code)
    {
        var syntaxSerializer = new SerializerSyntaxElementTree();
        var tree = syntaxSerializer.Deserialize(code);
        return tree.ToTreeNodeSemantic();
    }
    public static TreeNode<NodeSemantic> DeserializeAndGroupCode(string code, bool fmt = false)
    {
        var serializer = new SerializerSemanticTree();
        var ret = serializer.Deserialize(code);
        ret = ret.GroupNodes();
        return ret;
    }
    public static TreeNode<NodeSemantic> DeserializeCodeNoGroup(string code, bool fmt = false)
    {
        var serializer = new SerializerSemanticTree();
        var ret = serializer.Deserialize(code);
        return ret;
    }
    public static TreeNode<NodeSemantic> DeserializeFile(string file, bool fmt = false)
    {
        var code = File.ReadAllText(file);
        var ret = DeserializeAndGroupCode(code, fmt);
        ret.CheckTree();
        return ret;
    }
    public string Serialize(TreeNode<NodeSemantic> tree)
    {
        return tree.Serialize();
    }
}

