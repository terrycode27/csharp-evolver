public static partial class ExtensionsOfListOfModuleInfo
{
    public static List<ModuleInfo> OrderByDependents(this List<ModuleInfo> list)
    {
        return list.OrderByDescending(t => t.Dependents.Count()).ToList();
    }
}

