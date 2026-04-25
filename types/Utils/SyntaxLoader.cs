public partial class SyntaxLoader
{
    public SyntaxLoader() { }
    public static AdhocWorkspace CreateWorkspace()
    {
        return new AdhocWorkspace();
    }
    private static SyntaxTree LoadFile(string csFilePath)
    {
        if (!File.Exists(csFilePath))
            throw new FileNotFoundException("File not found", csFilePath);
        if (
            !string.Equals(Path.GetExtension(csFilePath), ".cs", StringComparison.OrdinalIgnoreCase)
        )
            throw new ArgumentException("Must be a .cs file", nameof(csFilePath));
        string text = File.ReadAllText(csFilePath);
        return CSharpSyntaxTree.ParseText(
            text,
            path: csFilePath,
            options: new CSharpParseOptions(LanguageVersion.Latest));
    }
}

