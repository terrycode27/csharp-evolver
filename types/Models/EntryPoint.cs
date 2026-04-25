public partial class EntryPoint
{
    public EntryPoint(string baseNamespace)
    {
        if (string.IsNullOrWhiteSpace(baseNamespace))
            throw new ArgumentException("Base namespace cannot be empty.", nameof(baseNamespace));
        BaseNamespace = baseNamespace.Trim();
        TestFilePath = Path.Combine(RootDirectory, $"{BaseNamespace}_test.cs");
        ClassFilePath = Path.Combine(RootDirectory, $"{BaseNamespace}_{Class_Prefix}.cs");
        MethodFilePath = Path.Combine(RootDirectory, $"{BaseNamespace}_{Method_Prefix}.cs");
    }
    public static string Class_Prefix = "Class";
    public static string Consolidated_Prefix = "";
    public static string Method_Prefix = "Method";
    public static string one_class_per_file_namespace = "CF";
    public static string one_namespace_per_file_namespace = "NF";
    public string BaseNamespace { get; }
    public string ClassFilePath { get; }
    public string ConsolidatedFileFullPath => Path.Combine(RootDirectory, ConsolidatedFileName);
    public string ConsolidatedFileName => $"{BaseNamespace}.cs";
    public string DocumentationFilePath => Path.Combine(RootDirectory, "IDocs.cs");
    public string EvolutionOutput => Path.Combine(RootDirectory, $"{BaseNamespace}_evolved_fragment.cs");
    public string HierarchyYamlFile => ConsolidatedFileFullPath.Replace(".cs",".yaml");
    public string MethodFilePath { get; }
    public string NamespaceConsolidated => $"{BaseNamespace}";
    public string OneClassPerFileNamespace => $"{BaseNamespace}.{OneClassPerFileNamespaceSuffix}";
    public string OneClassPerFileNamespaceSuffix => one_class_per_file_namespace;
    public string OneNamespacePerFileNamespace => $"{BaseNamespace}.{OneNamespacePerFileNamespaceSuffix}";
    public string OneNamespacePerFileNamespaceSuffix => one_namespace_per_file_namespace;
    public string PartialFile => WithRootDir(BaseNamespace + "_partial.cs");
    public string PartialFileOutput => WithRootDir(BaseNamespace + "_partial_output.cs");
    public string ProjectFileName => $"{BaseNamespace}.csproj";
    public string ProjectFilePath => Path.Combine(RootDirectory, ProjectFileName);
    public virtual string RootDirectory => Path.Combine(solutionBaseFolder, BaseNamespace);
    public string StageFilePath => Path.Combine(RootDirectory, $"{BaseNamespace}_Stage.cs");
    public string TestFilePath { get; }
    public string YamlDirectory => Path.Combine(RootDirectory, "yaml");
    string solutionBaseFolder
    {
        get { return AppDomain.CurrentDomain.GetSolutionBaseFolder(); }
    }
    public void CopyAttributesFrom(string sourceFile)
    {
        var tree = Load();
        var sliceTree = LoadPath(WithRootDir(sourceFile));
        var dest = tree.GetTypedList<NodeTypeDeclaration>();
        var source = sliceTree.GetTypedList<NodeTypeDeclaration>();
        var diff = source.ListDiff(
            dest,
            new KeyEqualityComparer<NodeTypeDeclaration, string>(t => t.TypeName));
        dest.CopyAttributesFrom(source);
        tree.ToFile(ConsolidatedFileFullPath);
    }
    public static EntryPoint Create(string baseNamespace) => new(baseNamespace);
    public void CreateDocInterfaces()
    {
        var tree = Load();
        var interfaces = tree.ExtractInterfaces(t => t + "_IDoc");
        foreach (var i in interfaces.GetTypedList<NodeInterface>())
        {
            i.AddClassAttribute(i.TypeName);
            i.AddDocAttribute();
            foreach (NodeNamedMember m in i.GetMembers())
            {
                m.AddDocAttribute();
            }
        }
        interfaces.ToFile(this.DocumentationFilePath);
    }
    public void DocMergeIntoOneFile(string outFile = null, string inDir = null)
    {
        inDir = inDir ?? GetSubDir("classes");
        outFile = outFile ?? ConsolidatedFileFullPath;
        inDir.ToDI().MergeCodeDirectoryTo(outFile);
    }
    public void DocSplitIntoFilesFlat(string inFile = null, string outDir = null)
    {
        inFile = inFile ?? ConsolidatedFileFullPath;
        outDir = outDir ?? GetSubDir("classes");
        var tree = SerializerSemanticTree.DeserializeFile(inFile);
        tree.DocSplitIntoFilesFlat(outDir);
    }
    public void DocSplitIntoFilesTree(string inFile = null, string outDir = null)
    {
        inFile = inFile ?? ConsolidatedFileFullPath;
        outDir = outDir ?? GetSubDir("classes");
        inFile.ToFI().DocSplitIntoFilesTree(this.HierarchyYamlFile, outDir);
    }
    public void DotNetFormat()
    {
        new FileInfo(ConsolidatedFileFullPath).DotNetFormat(this.ProjectFilePath);
    }
    public string GetSubDir(string subDir)
    {
        return Path.Combine(RootDirectory, subDir);
    }
    public void GroupByKindModifierName()
    {
        var originalTree = Load();
        var peek = originalTree.GroupByKindModifierName();
        var treeWithConsecutiveGroups = peek.Tree;
        treeWithConsecutiveGroups.TestCompile(ProjectFilePath, ConsolidatedFileFullPath);
    }
    public TreeNode<NodeSemantic> Load()
    {
        return LoadPath(ConsolidatedFileFullPath);
    }
    public TreeNode<NodeSemantic> LoadPath(string fullPath)
    {
        var tree = SerializerSemanticTree.DeserializeFile(fullPath);
        return tree;
    }
    public TreeNode<NodeSemantic> LoadRoot(string relativePath)
    {
        string path = WithRootDir(relativePath);
        return LoadPath(path);
    }
    public TreeNode<NodeSemantic> LoadTreeByTemplate(string name, string subdir = null)
    {
        var loadFile = ToNamespaceFile(name, subdir);
        return LoadPath(loadFile);
    }
    public TreeNode<NodeSemantic> LoadTreeProjectPath(string fileName)
    {
        string fullPath = WithRootDir(fileName);
        var tree = SerializerSemanticTree.DeserializeFile(fullPath);
        return tree;
    }
    public void MergeAttributesFromDocs()
    {
        var docSource = LoadTreeProjectPath("");
        var codeDest = Load();
        var interfaces = docSource.GetTypedList<NodeInterface>();
        foreach (var i in interfaces)
        {
            i.TypeName = i.TypeName.Replace("_IDocument", "");
        }
        var sourceNodeNameds = docSource.GetTypedList<NodeNamedMember>();
        var destNodeNameds = codeDest.GetTypedList<NodeNamedMember>();
        var res = destNodeNameds.MatchNodesFrom(sourceNodeNameds);
        foreach (var r in res)
        {
            r.d.CopyAttributesFrom(r.s);
        }
        codeDest.SaveFile(ConsolidatedFileFullPath);
    }
    public void PullChildrenOutOfNamespaces()
    {
        var root = Load();
        root.PullChildrenOutOfNamespaces();
        root.ToFile(ConsolidatedFileFullPath);
    }
    public void SaveRoot(TreeNode<NodeSemantic> tree, string relativePath)
    {
        tree.SaveFile(WithRootDir(relativePath));
    }
    public void SplitIntoBaseClassFile(string baseClass)
    {
        var tree = Load();
        var dict = tree.GetAllDerivedClassesDictionary();
        var l = dict.Values.OrderByDescending(t => t.Count).ToList();
        var sn = dict[baseClass];
        var cu = NodeCompilationUnit.Factory();
        sn.ForEach(t => t.RemoveSelf());
        cu.Children.AddRange(sn);
        cu.ToFile(WithRootDir($"{baseClass}.cs"));
        tree.ToFile(ConsolidatedFileFullPath);
    }
    public void SplitIntoStaticClassesFile()
    {
        var tree = Load();
        var sn = tree.GetClasses().Where(t => t.IsStatic).ToList();
        var cu = NodeCompilationUnit.Factory();
        sn.ForEach(t => t.tree.RemoveSelf());
        cu.Children.AddRange(sn.Select(t => t.tree));
        cu.ToFile(WithRootDir($"StaticClasses.cs"));
        tree.ToFile(ConsolidatedFileFullPath);
    }
    public static void TestSelfMerge()
    {
        string source = KPP.Evolver5.WithRootDir(@"test\evolver5_MergeSource.cs");
        var tree = KPP.Evolver5.LoadPath(source);
        tree = tree.DocMergeSelf();
        tree.SaveFile(source);
    }
    public void ToAbstractClasses()
    {
        var tree = Load();
        tree.DeleteKinds(SyntaxKindGroups.CommentTrivia);
        tree.DeleteKinds(SyntaxKindGroups.AttributeKinds, DeleteType.NodeAndSubTree);
        var abstractBaseClasses = tree.MakeIntoAbstractClasses(t => t);
        var cu = NodeCompilationUnit.Factory();
        foreach (var a in abstractBaseClasses.Children)
        {
            cu.Children.Add(a.Reload());
        }
        var ns = cu.Children.WrapInDeclaration(
            @"namespace Revmo.Abstracts 
            {
            }"
        );
    }
    public string ToNamespaceFile(string name, string subdir = null)
    {
        if (!String.IsNullOrEmpty(subdir))
            subdir = $@"{subdir}\";
        var ret = Path.Combine(RootDirectory, $"{subdir}{BaseNamespace}_{name}.cs");
        return ret;
    }
    public string WithRootDir(string name)
    {
        return Path.Combine(RootDirectory, name);
    }
    public string WithYamlDir(string name)
    {
        return Path.Combine(Path.Combine(RootDirectory, "yaml"), name);
    }
    public void Workflow(Func<TreeNode<NodeSemantic>, TreeNode<NodeSemantic>> func, bool format = false, string relativePath = null)
    {
        var di = new DirectoryInfo(this.RootDirectory);
        relativePath = relativePath ?? this.ConsolidatedFileName;
        di.Workflow(relativePath, func);
        if (format)
            di.DotNetFormat(this.ProjectFileName);
    }
    public void WorkflowRoot(Func<TreeNode<NodeSemantic>, TreeNode<NodeSemantic>> func, string path, bool format = false)
    {
        Workflow(func, format, WithRootDir(path));
    }
    public string WRD(string name)
    {
        return this.WithRootDir(name);
    }
    private List<TreeNode<NodeSemantic>> BuildClassHierarchy(List<NodeClass> classes)
    {
        var byName = classes.ToDictionary(c => c.TypeName, c => c.tree);
        var derivedMap = BuildDerivedMap(classes, byName);
        var roots = classes
            .Where(c => c.BaseType == null || !byName.ContainsKey(c.BaseType))
            .ToList();
        return roots
            .OrderBy(c => c.TypeName)
            .Select(r => r.tree.BuildRegionTree(derivedMap, byName))
            .ToList();
    }
    private Dictionary<string, List<TreeNode<NodeSemantic>>> BuildDerivedMap(List<NodeClass> classes, Dictionary<string, TreeNode<NodeSemantic>> byName)
    {
        var map = new Dictionary<string, List<TreeNode<NodeSemantic>>>();
        foreach (var c in classes)
        {
            if (c.BaseType != null && byName.ContainsKey(c.BaseType))
            {
                if (!map.ContainsKey(c.BaseType))
                    map[c.BaseType] = new();
                map[c.BaseType].Add(c.tree);
            }
        }
        return map;
    }
    private string GetTestFilePath(int i)
    {
        return Path.Combine(RootDirectory, $"{BaseNamespace}_test_{i}.cs");
    }
    private void GroupByClassHierarchy()
    {
        var tree = Load();
        var classes = tree.GetClasses();
        var grouped = BuildClassHierarchy(classes);
        tree.Children.Clear();
        grouped.ForEach(tree.AddChild);
        tree.ToFile(TestFilePath);
    }
    private void RefactorLargeClassIntoPartialsWithInterfaces(string className)
    {
        var tree = Load();
        tree.DivideIntoPartialClassesByInterfaces(className);
        tree.GroupByKindModifierName();
        tree.ToFile(ConsolidatedFileFullPath);
    }
}

