public partial class NodeTypeArgumentList : NodeSemantic
{
    public NodeTypeArgumentList(SyntaxElementNode b) : base(b) { }
    public List<string> TypeArguments;
    public List<List<string>> ArgumentList
    {
        get
        { 
               var ret=new List<List<string>>();
                var add = new List<string>();
                foreach (var x in tree.Flatten())
                {
                    if (x.Value is NodeTypeArgumentList)
                    {
                        if (add.Count > 0)
                        {
                            ret.Add(add);
                            add = new List<string>();
                        }
                    }
                    if (SyntaxKindGroups.TypeNameKinds.Contains(x.Value.Kind))
                        add.Add(x.ToCode().Trim());
                }
                ret.Add(add);
                return ret;
        }
    }
    public string MapExtensionClassName(Dictionary<string,string> map)
    {
        var ret = String.Join("", this.TypeArguments.Select(t => map.ValOrSearchKey(t)));
        return ret;
    }
    public override void SetTreeNode(TreeNode<NodeSemantic> tree)
    {
        base.SetTreeNode(tree);
    }
}

