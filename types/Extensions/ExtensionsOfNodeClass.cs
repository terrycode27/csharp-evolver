public static partial class ExtensionsOfNodeClass
{
    public static void CompareMembers(this NodeClass dest, NodeClass source)
    {
        var res = dest.MemberNames.CompareWithKeyFrom(source.MemberNames, t => t);
    }
}

