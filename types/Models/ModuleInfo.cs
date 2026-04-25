public partial class ModuleInfo
{
    public string TypeName;
    private IEnumerable<string>? _moduleTypesCache;
    static string FileName = "ModuleInfo.yaml";
    public List<string> Bases { get; set; } = new();
    [YamlIgnore]
    public int Count => Dependencies.Count + Dependents.Count + 1;
    public List<string> Dependencies { get; set; } = new();
    public List<string> Dependents { get; set; } = new();
    [YamlIgnore]
    public IEnumerable<string> ModuleTypes => _moduleTypesCache ??= new[] { TypeName }.Concat(Dependencies).Concat(Dependents);
    public static Dictionary<string, ModuleInfo> LoadYamlFile(string file)
    {
        return file.ToFI().FromYAMLFile<Dictionary<string, ModuleInfo>>();
    }
    public static Dictionary<string, ModuleInfo> LoadYamlFromPath(EntryPoint path)
    {
        var ret = LoadYamlFile(path.WithYamlDir(FileName));
        return ret;
    }
    public override string ToString()
    {
        return $"{this.TypeName} {Count}";
    }
    public void ToYamlFile(EntryPoint path)
    {
        var tree = path.Load();
        ExtensionsOfT.ToYAMLFile<Dictionary<string, ModuleInfo>>(
            tree.ToDependencyDictionary(),
            path.WithYamlDir(FileName)
        );
    }
}

