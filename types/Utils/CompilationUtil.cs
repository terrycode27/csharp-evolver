public partial class CompilationUtil
{
    public static (bool Success, ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<Diagnostic> Errors) Compile(string code, string projectFile)
    {
        // For backward compatibility, return empty diagnostics
        // The actual compilation check should use CompileWithDotNetBuild
        return (false, ImmutableArray<Diagnostic>.Empty, ImmutableArray<Diagnostic>.Empty);
    }
    public static (bool Success, ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<Diagnostic> Errors) Compile(string code, Project project)
    {
        // For backward compatibility, return empty diagnostics
        return (false, ImmutableArray<Diagnostic>.Empty, ImmutableArray<Diagnostic>.Empty);
    }
    public static (bool Success, ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<Diagnostic> Errors) CompileProject(string projectFile)
    {
        var result = CompileWithDotNetBuild(projectFile);
        // Convert command-line result to diagnostic format for backward compatibility
        return (result.Success, ImmutableArray<Diagnostic>.Empty, ImmutableArray<Diagnostic>.Empty);
    }
    public static (bool Success, ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<Diagnostic> Errors) CompileProject(Project project)
    {
        // For backward compatibility, return empty diagnostics
        return (false, ImmutableArray<Diagnostic>.Empty, ImmutableArray<Diagnostic>.Empty);
    }
    public static (bool Success, string Output, string Errors) CompileWithDotNetBuild(string projectPath, string verbosity = "quiet")
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{projectPath}\" --verbosity {verbosity}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(projectPath)
        };
        using var process = Process.Start(psi);
        string output = process.StandardOutput.ReadToEnd();
        string errors = process.StandardError.ReadToEnd();
        process.WaitForExit();
        bool success = process.ExitCode == 0;
        return (success, output, errors);
    }
    private static bool RoundTripCompilation(string originalCode, string regeneratedCode, string csprojFileName)
    {
        // Use command-line compilation instead of in-process
        var result = CompilationUtil.CompileWithDotNetBuild(csprojFileName);
        return result.Success;
    }
}

