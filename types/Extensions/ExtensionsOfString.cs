public static partial class ExtensionsOfString
{
    public static string CapitalizeFirstLetter(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        return char.ToUpperInvariant(input[0]) + input[1..];
    }
    public static string Enquote(this string s)
    {
        return "\"" + s + "\"";
    }
    public static string NormalizeLineEndings(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        return text.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", "\r\n");
    }
    public static List<string> SplitCamelCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new List<string>();
        string result = Regex.Replace(input, @"(?<!^)(?=[A-Z])", " ");
        return result.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
    public static DirectoryInfo ToDI(this string s)
    {
        return new DirectoryInfo(s);
    }
    public static FileInfo ToFI(this string s)
    {
        return new FileInfo(s);
    }
    public static yamlString ToYamlStr(this string s)
    {
        var ys = new yamlString() { Value =s};
        return ys;
    }
}

