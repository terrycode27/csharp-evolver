public partial class NodeConstructor : NodeParameterizedMemberWithBody
{
    public NodeConstructor(SyntaxElementNode b) : base(b) { }
    public override string GetAbstractSignatureOnly()
    {
        string sig = GetSignatureOnly();
        return "public " + sig;
    }
    public override string GetSignatureOnly()
    {
        string ret = GetBaseSignature();
        if (!ret.Contains("{"))
            ret = ret + "{}";
        return ret;
    }
}

