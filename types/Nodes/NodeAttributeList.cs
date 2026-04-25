public partial class NodeAttributeList : NodeSemantic
{
    public NodeAttributeList(SyntaxElementNode b) : base(b) { }
    public List<NodeAttribute> Attributes { get; } = new();
    public List<string> Names
    {
        get { return this.Attributes.Select(t => t.TypeName).ToList(); }
    }
    public override void AttachChild(NodeSemantic child)
    {
        if (child is NodeAttribute attr)
            Attributes.Add(attr);
    }
    public bool Contains(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        string search = StripAttributeSuffix(name);
        return Attributes.Any(a =>
            string.Equals(StripAttributeSuffix(a.TypeName ?? ""), search, StringComparison.Ordinal)
        );
    }
    private static string StripAttributeSuffix(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        const string suffix = "Attribute";
        return input.EndsWith(suffix, StringComparison.Ordinal)
            ? input.Substring(0, input.Length - suffix.Length)
            : input;
    }
}

