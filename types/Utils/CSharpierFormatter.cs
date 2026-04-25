public partial class CSharpierFormatter
{
    static CSharpierFormatter()
    {
        Initialize();
    }
    private static AssemblyLoadContext context;
    private static string csharpierPath = @"_dependency\CSharpier";
    private static MethodInfo formatMethod;
    private static object formatter;
    private static object options;
    public static string Format(string code)
    {
        var result = formatMethod.Invoke(formatter, new[] { code, options });
        var formatted = (string)result.GetType().GetProperty("Code").GetValue(result);
        return formatted;
    }
    private static void BecauseNewerRoslynBreaksCSharpier(string path)
    {
        var dll = Directory.Exists(path) ? Path.Combine(path, "CSharpier.Core.dll") : path;
        context = new AssemblyLoadContext("CSharpierIsolated", isCollectible: true);
        var assembly = context.LoadFromAssemblyPath(dll);
        foreach (var depName in new[] { "Microsoft.CodeAnalysis", "Microsoft.CodeAnalysis.CSharp" })
        {
            var depPath = Path.Combine(Path.GetDirectoryName(dll), depName + ".dll");
            if (File.Exists(depPath))
                context.LoadFromAssemblyPath(depPath);
        }
        var type = assembly.GetType("CSharpier.Core.CSharp.CSharpFormatter");
        formatMethod = type.GetMethods()
            .First(m => m.Name == "Format" && m.GetParameters().Length == 2);
        return;
    }
    private static void Initialize()
    {
        BecauseNewerRoslynBreaksCSharpier(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, csharpierPath)
        );
    }
}

