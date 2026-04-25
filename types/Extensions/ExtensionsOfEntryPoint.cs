public static partial class ExtensionsOfEntryPoint
{
    public static void AddFileToProject(this EntryPoint path, string fileName) => path.ModifyProjectFile(fileName, ProjectFileAction.Add);
    public static void DeleteCacheFiles(this EntryPoint path)
    {
        var yamlDir = Path.Combine(path.RootDirectory, "yaml");
        if (!Directory.Exists(yamlDir))
            return;
        var files = new[]
        {
            "base_types_cache.yaml",
            "dependencies_cache.yaml",
            "dependents_cache.yaml",
        };
        foreach (var file in files)
        {
            var fullPath = Path.Combine(yamlDir, file);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
    public static Dictionary<string, List<string>> GetCatDictionary(this EntryPoint path)
    {
        var ret=path.WithRootDir(@"yaml\types.yaml").ToFI().FromYAMLFile<Dictionary<string, List<string>>>();
        return ret;
    }
    public static Dictionary<string, Dictionary<string, List<string>>> LoadCategoryTypeMethodMap(this EntryPoint path)
    {
        var tree = path.Load();
        var dict = tree.ToTypeMethodDictionary();
        var ret = new Dictionary<string, Dictionary<string, List<string>>>();
        var catTypes = path.WithRootDir("types.yaml").ToFI().FromYAMLFile<Dictionary<string, List<string>>>();
        foreach (var key in catTypes.Keys)
        {
            var types = new Dictionary<string, List<string>>();
            var typeSrc = catTypes[key];
            foreach (var t in typeSrc)
            {
                var methodList = dict[t];
                if (methodList.Count > 0)
                    types.Add(t, methodList);
            }
            ret.Add(key, types);
        }
        ret.ToYAMLFile(path.WithRootDir("cat_type_method.yaml"));
        return ret;
    }
    public static void ModifyProjectFile(this EntryPoint path, string fileName, ProjectFileAction action)
    {
        var csproj = XDocument.Load(path.ProjectFilePath);
        var relativePath = StaticHelpers.GetRelativePath(path.RootDirectory, fileName);
        var itemGroup = StaticHelpers.FindOrCreateItemGroup(csproj);
        if (action == ProjectFileAction.Add)
            StaticHelpers.AddFile(itemGroup, relativePath);
        else
            StaticHelpers.RemoveFile(itemGroup, relativePath);
        csproj.Save(path.ProjectFilePath);
    }
    public static void RemoveFileFromProject(this EntryPoint path, string fileName) => path.ModifyProjectFile(fileName, ProjectFileAction.Remove);
    public static void TestMerge(this EntryPoint path)
    {
        string spath = path.WithRootDir(@"Test\MergeSource.cs");
        var source = path.LoadPath(spath);
        source.ToFile(spath);
        string dpath = path.WithRootDir(@"Test\MergeDest.cs");
        var dest = path.LoadPath(dpath);
        dest.ToFile(dpath);
        dest = dest.DocMergeFrom(source);
        dest.ToFile(path.WithRootDir(@"Test\MergeResult.cs"));
    }
    public static void ToTypeDeclarationFile(this EntryPoint path)
    {
        var tree = path.Load();
        var cu = NodeCompilationUnit.Factory();
        foreach (var t in tree.GetTypeDeclarations())
        {
            var decl = t.CopyDeclaration();
            cu.Children.Add(decl);
        }
        var x = cu.Reload();
        foreach (NodeTypeDeclaration tdn in x.Children.Select(t => (NodeTypeDeclaration)t.Value))
        {
            tdn.AddNodeAttribute("[Next_Refactor_Slice(\"\")]\n");
        }
        x.ToFile(path.WithRootDir("revmo_declaration.cs"));
    }
}

