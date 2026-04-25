public partial class StaticHelpers
{
    public static string TFSTemp = "C:\\Users\\PC1\\AppData\\Local\\Temp\\TFSTemp";
    public static void AddFile(XElement itemGroup, string relativePath)
    {
        var ns = itemGroup.GetDefaultNamespace();
        var existing = FindCompileElement(itemGroup, relativePath);
        if (existing == null)
        {
            itemGroup.Add(new XElement(ns + "Compile", new XAttribute("Include", relativePath)));
        }
    }
    public static XElement CreateNewItemGroup(XDocument csproj, XNamespace ns)
    {
        var group = new XElement(ns + "ItemGroup");
        csproj.Root!.Add(group);
        return group;
    }
    public static void DeleteDuplicateTypes(TreeNode<NodeSemantic> tree)
    {
        var duplicate = tree.GetTypedList<NodeTypeDeclaration>()
            .GroupBy(t => t.TypeName)
            .Where(t => t.Count() > 1)
            .ToList();
        var delete = duplicate.SelectMany(t => t).ToList();
        foreach (var d in delete)
        {
            if (d.FullName.Contains("AllConstants"))
                d.tree.RemoveSelf();
        }
    }
    public static XElement? FindCompileElement(XElement itemGroup, string relativePath)
    {
        var ns = itemGroup.GetDefaultNamespace();
        return itemGroup
            .Elements(ns + "Compile")
            .FirstOrDefault(e =>
                string.Equals(
                    e.Attribute("Include")?.Value?.Replace('\\', '/'),
                    relativePath,
                    StringComparison.OrdinalIgnoreCase
                )
            );
    }
    public static XElement FindOrCreateItemGroup(XDocument csproj)
    {
        var ns = csproj.Root?.GetDefaultNamespace() ?? XNamespace.None;
        return csproj
                .Descendants(ns + "ItemGroup")
                .FirstOrDefault(g => g.Elements(ns + "Compile").Any())
            ?? CreateNewItemGroup(csproj, ns);
    }
    public static List<T> FromCSV<T>(string fileName)
    {
        using var reader = new StreamReader(fileName);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
        return csv.GetRecords<T>().ToList();
    }
    public static T FromJSON<T>(string fileName)
    {
        var json = File.ReadAllText(fileName);
        return JsonSerializer.Deserialize<T>(json);
    }
       public static List<string> GetFilesByPattern(string rootDirectory, string pattern)
    {
        var files = new List<string>();
        if (pattern.EndsWith("/**"))
        {
            string folderPath = Path.Combine(
                rootDirectory,
                pattern.Substring(0, pattern.Length - 3)
            );
            files.AddRange(Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories));
        }
        else if (pattern.EndsWith("/*"))
        {
            string folderPath = Path.Combine(
                rootDirectory,
                pattern.Substring(0, pattern.Length - 2)
            );
            files.AddRange(Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly));
        }
        else
        {
            string folderPath = Path.Combine(rootDirectory, pattern);
            files.AddRange(Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly));
        }
        return files;
    }
    public static string GetRelativePath(string rootDirectory, string fileName)
    {
        var fullPath = Path.Combine(rootDirectory, fileName);
        return Path.GetRelativePath(rootDirectory, fullPath).Replace('\\', '/');
    }
    public static void InsertAfterInterfaces(TreeNode<NodeSemantic> root, List<TreeNode<NodeSemantic>> abstractClasses)
    {
        var lastInterface = root.FindWhere(t => t.Value.Kind == SyntaxKind.InterfaceDeclaration)
            .LastOrDefault();
        var insertionPoint = lastInterface ?? root;
        foreach (var absClass in abstractClasses)
        {
            insertionPoint.InsertAfter(absClass);
        }
    }
    public static bool IsDisjoint<T>(HashSet<T> set, HashSet<T> covered)
    {
        return !set.Overlaps(covered);
    }
    public static void LogSuccessfulExtraction(string interfaceName, int methodCount) => Console.WriteLine($"✓ Extracted {methodCount} methods → partial class for {interfaceName}");
    public static bool NamesAreInvalid(string className, string interfaceName) => string.IsNullOrEmpty(className) || string.IsNullOrEmpty(interfaceName);
    public static TreeNode<NodeSemantic> NewNewLine()
    {
        return new TreeNode<NodeSemantic>(new NodeUnknown(new SyntaxElementNode(SyntaxKind.EndOfLineTrivia, "\n")));
    }
    public static TreeNode<NodeSemantic> NewTextNode(SyntaxKind kind, string text)
    {
        return new TreeNode<NodeSemantic>(new NodeUnknown(new SyntaxElementNode(kind, text)));
    }
    public static void RemoveFile(XElement itemGroup, string relativePath)
    {
        var existing = FindCompileElement(itemGroup, relativePath);
        existing?.Remove();
    }
    public static string StripAttributeSuffix(string input)
    {
        const string suffix = "Attribute";
        return input.EndsWith(suffix, StringComparison.Ordinal)
            ? input.Substring(0, input.Length - suffix.Length)
            : input;
    }
    public static List<string> ToCSV<T>(T val)
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
        if (val is IEnumerable<object> collection)
        {
            csv.WriteRecords(collection);
        }
        else
        {
            csv.WriteRecords(new[] { val });
        }
        return writer
            .ToString()
            .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }
    public static void ToCSVFile<T>(T val, string fileName)
    {
        var lines = ToCSV(val);
        File.WriteAllLines(fileName, lines);
    }
    public static List<string> ToJSON<T>(T val)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(val, options);
        return new List<string> { json };
    }
    public static void ToJSONFile<T>(T val, string fileName)
    {
        var lines = ToJSON(val);
        File.WriteAllLines(fileName, lines);
    }
}

