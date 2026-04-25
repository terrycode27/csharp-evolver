public static partial class ExtensionsOfAppDomain
{
    public static string GetSolutionBaseFolder(this AppDomain domain)
    {
        ArgumentNullException.ThrowIfNull(domain);
        var dir = new DirectoryInfo(domain.BaseDirectory);
        int maxLevels = 10;
        while (dir != null && maxLevels-- > 0)
        {
            if (dir.GetFiles("*.cs", SearchOption.TopDirectoryOnly).Any())
            {
                return dir.Parent.FullName;
            }
            dir = dir.Parent;
        }
        return domain.BaseDirectory.TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar
        );
    }
}

