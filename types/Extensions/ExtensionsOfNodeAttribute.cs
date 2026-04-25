public static partial class ExtensionsOfNodeAttribute
{
    public static List<NodeAttributeArgument> ArgumentNodes(this NodeAttribute attr)
    {
        var ret = attr.tree.GetTypedList<NodeAttributeArgument>();
        return ret;
    }
    public static bool IsSlice(this NodeAttribute a) => a.TypeName == "Slice" || a.TypeName == "SliceAttribute";
}

