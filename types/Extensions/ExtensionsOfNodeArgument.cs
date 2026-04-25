public static partial class ExtensionsOfNodeArgument
{
    public static string StringValue(this NodeArgument arg)
    {
        if (arg == null)
            return null;
        return arg.ExpressionNode.ToCode().Trim().Trim('"');
    }
}

