public static partial class ExtensionsOfFileInfo
{
    public static void CreateDirectoryIfNotExists(this FileInfo fi)
    {
        fi.DirectoryName.ToDI().CreateIfNotExists();
    }
    public static void DocSplitIntoFilesTree(this FileInfo fi,string yamlTree, string outputDir)
    {
        var tree= fi.ToTree();
        outputDir.ToDI().DeleteAndCreate();
        var tdn = tree.Children.Where(t => t.Value.Kind != SyntaxKind.UsingDirective).ToList();
        var ll=yamlTree.ToFI().FromYAMLFile<List<List<string>>>();
        var typeFileLoopup = ll.ToDictionary(t => t.First(), t => t.Skip(1).First());
        foreach (var t in tdn)
        {
            var file = typeFileLoopup[t.Value.TypeName];
            var fi2= (outputDir+file).ToFI();
            fi2.CreateDirectoryIfNotExists();
            StreamWriter sw = new StreamWriter(fi2.FullName, true);
            sw.WriteLine(t.ToCode());
            sw.Close();
        }
    }
    public static void DotNetFormat(this FileInfo fi, string projFile = null)
    {
        projFile = projFile ?? fi.Name.Replace(".cs", ".csproj");
        var psi = new ProcessStartInfo() { WorkingDirectory = fi.DirectoryName, FileName = "dotnet", Arguments = $" format --verbosity diagnostic {projFile}" };
        var p = System.Diagnostics.Process.Start(psi);
        p.WaitForExit();
    }
    public static T FromYAMLFile<T>(this FileInfo fi)
    {
        var yamlStr = File.ReadAllText(fi.FullName).ToYamlStr();
              return yamlStr.FromYAML<T>();
    }
    public static TreeNode<NodeSemantic> Load(this FileInfo fi)
    {
        return SerializerSemanticTree.DeserializeFile(fi.FullName);
    }
    public static void SplitIntoTypeFiles(this FileInfo fi,string directoryName=null)
    {
        directoryName= directoryName ?? Path.Combine(fi.DirectoryName,fi.Name.Replace(".cs",""));
        var tree= SerializerSemanticTree.DeserializeFile(fi.FullName);
        tree.DocSplitIntoFilesFlat(directoryName);
    }
    public static TreeNode<NodeSemantic> ToTree(this FileInfo fi)
    {
        return SerializerSemanticTree.DeserializeAndGroupCode(File.ReadAllText(fi.FullName));
    }
    public static void Workflow(this FileInfo fi, Func<TreeNode<NodeSemantic>, TreeNode<NodeSemantic>> func, string fullPath = null)
    {
        fullPath = fullPath ?? fi.FullName;
        var tree = SerializerSemanticTree.DeserializeFile(fullPath);
        tree = func(tree);
        tree.SaveFile(fullPath);
    }
}

