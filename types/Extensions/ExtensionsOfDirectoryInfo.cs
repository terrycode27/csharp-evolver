public static partial class ExtensionsOfDirectoryInfo
{
    public static void ConcatenateCodeDirectoryTo(this DirectoryInfo di, string fullPath)
    {
        var files = di.GetFiles("*.cs", SearchOption.AllDirectories);
        var sw = new StringWriter();
        List<string> usings = new List<string>();
        foreach (var f in files.OrderBy(t => t.FullName))
        {
            if (f.DirectoryName.Contains("Debug"))
                continue;
            foreach (var line in File.ReadAllLines(f.FullName))
            {
                if (line.Contains("using "))
                {
                    usings.Add(line);
                    continue;
                }
                sw.WriteLine(line);
            }
        }
        usings = usings.Distinct().OrderBy(t => t).ToList();
        var code = sw.ToString();
        var usingStr = String.Join("\n", usings);
        File.WriteAllText(fullPath, usingStr + "\n" + code);
    }
    public static void CreateIfNotExists(this DirectoryInfo di)
    {
        string dir = di.FullName;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }
    public static void DeleteAndCreate(this DirectoryInfo di)
    {
        string dir = di.FullName;
        if (Directory.Exists(dir))
            Directory.Delete(dir, true);
        Directory.CreateDirectory(dir);
    }
    public static void DotNetFormat(this DirectoryInfo di, string projectRelativePath)
    {
        var projectPath = Path.Combine(di.FullName, projectRelativePath);
        var psi = new ProcessStartInfo() { WorkingDirectory = di.FullName, FileName = "dotnet", Arguments = $" format --verbosity diagnostic {projectRelativePath}" };
        var p = System.Diagnostics.Process.Start(psi);
        p.WaitForExit();
    }
    public static List<FileInfo> GetCSFiles(this DirectoryInfo di)
    {
        return di.GetFiles("*.cs", SearchOption.AllDirectories).ToList();
    }
    public static List<List<string>> HierarchyRead(this DirectoryInfo di)
    {
        var ret = new List<List<string>>();
        foreach (var cs in di.GetCSFiles())
        {
            ret.Add(new List<string>() { cs.Name.Replace(".cs",""),cs.FullName.Replace(di.FullName,"") });
        }
        return ret;
    }
    public static void HierarchySave(this DirectoryInfo di,string outputFile=null)
    {
        if (outputFile == null)
            outputFile = Path.Combine(di.FullName, "hierarchy.yaml");
        var list= di.HierarchyRead();
        list.ToYAMLFile(outputFile);
    }
    public static TreeNode<NodeSemantic> LoadAndConsolidateCodeDirectory(this DirectoryInfo di)
    {
        var tree = di.LoadCodeDirectory();
        var ret = tree.ConsolidateMultiFileCode();
        return ret;
    }
    public static TreeNode<NodeSemantic> LoadCodeDirectory(this DirectoryInfo di)
    {
        var files = di.GetFiles("*.cs", SearchOption.AllDirectories);
        var cu = NodeCompilationUnit.Factory();
        foreach (var f in files.OrderBy(t => t.FullName))
        {
            var code = File.ReadAllText(f.FullName);
            var tree = SerializerSemanticTree.DeserializeAndGroupCode(code);
            tree.DeleteKinds(SyntaxKindGroups.RegionTrivia);
            tree.DeleteKinds(SyntaxKindGroups.CommentTrivia);
            cu.AddChild(tree);
        }
        return cu;
    }
    public static TreeNode<NodeSemantic> LoadPath(this DirectoryInfo di, string relative)
    {
        var tree = SerializerSemanticTree.DeserializeFile(Path.Combine(di.FullName, relative));
        tree.CheckTree();
        return tree;
    }
    public static void MergeCodeDirectory(this DirectoryInfo di)
    {
        string newFile = di.Parent.FullName + "\\" + di.Name + ".cs";
        di.MergeCodeDirectoryTo(newFile);
    }
    public static void MergeCodeDirectoryTo(this DirectoryInfo di, string fullPath)
    {
        var tree = di.LoadAndConsolidateCodeDirectory();
        tree.SaveFile(fullPath);
    }
    public static void MergeIntoOneFile(this DirectoryInfo di)
    {
    }
    public static void ReplaceWithGlobalUsings(this DirectoryInfo di)
    {
        di.WriteGlobalUsings();
        di.WorkflowInPlace(t => t.DeleteUsings());
    }
    public static void Workflow(this DirectoryInfo di, string relativePath, Func<TreeNode<NodeSemantic>, TreeNode<NodeSemantic>> func)
    {
        string fullPath = Path.Combine(di.FullName, relativePath);
        fullPath.ToFI().Workflow(func);
    }
    public static TreeNode<NodeSemantic> WorkflowConsolidate(this DirectoryInfo di, Func<TreeNode<NodeSemantic>, TreeNode<NodeSemantic>> func)
    {
        var cu = NodeCompilationUnit.Factory();
        foreach (var f in di.GetCSFiles())
        {
            var tree = SerializerSemanticTree.DeserializeFile(f.FullName);
            tree = func(tree);
            cu.AddChild(tree);
        }
        return cu;
    }
    public static void WorkflowInPlace(this DirectoryInfo di,Func<TreeNode<NodeSemantic>, TreeNode<NodeSemantic>> func)
    {
        foreach(var cs in di.GetCSFiles())
        {
            var tree= cs.Load();
            tree = func(tree);
            tree.SaveFile(cs.FullName);
        }
    }
    public static void WriteGlobalUsings(this DirectoryInfo di, string globalUsingsFullPath = null)
    {
        globalUsingsFullPath = globalUsingsFullPath ?? di.Parent.FullName + @"\usings.cs";
        var usings = new List<string>();
        foreach (var f in di.GetFiles("*.cs", SearchOption.AllDirectories))
        {
            var tree = SerializerSemanticTree.DeserializeFile(f.FullName);
            tree.DeleteKinds(SyntaxKindGroups.CommentTrivia);
            foreach (var u in tree.GetUsings())
            {
                u.tree.DeleteNewlinesAndAdd();
                var usin = u.tree.ToCode().Trim();
                if (!usin.Contains("global"))
                {
                    usin = "global " + usin;
                }
                usings.Add(usin);
            }
        }
        usings = usings.Distinct().OrderBy(t => t).ToList();
        string usingsStr = String.Join("\n", usings);
        File.WriteAllText(globalUsingsFullPath, usingsStr);
    }
}

