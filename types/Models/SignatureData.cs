public partial class SignatureData
{
    private static readonly string[] TypeModifierOrder = { "new", "public", "protected", "internal", "private", "abstract", "sealed", "static", "partial", "unsafe", "readonly", "ref", "file" };
    public List<string> AccessModifiers { get; set; } = new();
    public List<string> Attributes { get; set; } = new();
    public List<string> BaseTypes { get; set; } = new();
    public string TypeKeyword { get; set; } = string.Empty;
    public List<string> TypeModifiers { get; set; } = new();
    public string TypeName { get; set; } = string.Empty;
    public List<string> TypeParameterConstraints { get; set; } = new();
    public List<string> TypeParameters { get; set; } = new();
    public SignatureData Combine(SignatureData second) => Superset(second);
    public static SignatureData Superset(params SignatureData[] signatures) => Superset((IEnumerable<SignatureData>)signatures);
    public static SignatureData Superset(IEnumerable<SignatureData> signatures)
    {
        if (!signatures.Any()) return new SignatureData();
        var list = signatures.ToList();
        return new SignatureData
        {
            Attributes = list.SelectMany(s => s.Attributes).Distinct().ToList(),
            AccessModifiers = list.SelectMany(s => s.AccessModifiers).Distinct().ToList(),
            TypeModifiers = list.SelectMany(s => s.TypeModifiers).Distinct().ToList(),
            TypeKeyword = list.Select(s => s.TypeKeyword).FirstOrDefault(s => !string.IsNullOrEmpty(s))
                          ?? list[0].TypeKeyword,
            TypeName = list.Select(s => s.TypeName).FirstOrDefault(s => !string.IsNullOrEmpty(s))
                       ?? list[0].TypeName,
            TypeParameters = list
                .SelectMany(s => s.TypeParameters)
                .Distinct()
                .OrderBy(p => p)
                .ToList(),
            TypeParameterConstraints = list
                .SelectMany(s => s.TypeParameterConstraints)
                .Distinct()
                .ToList(),
            BaseTypes = list.SelectMany(s => s.BaseTypes).Distinct().ToList(),
        };
    }
    public override string ToString()
    {
        var parts = new List<string>();
        parts.AddRange(Attributes.Select(a => a.Trim()));
        if (AccessModifiers.Count > 0)
            parts.Add(string.Join(" ", AccessModifiers));
        parts.AddRange(GetOrderedTypeModifiers(TypeModifiers));
        string typePart = TypeKeyword.TrimEnd();
        typePart += $" {TypeName}";
        if (TypeParameters.Count > 0)
            typePart += $"<{string.Join(", ", TypeParameters)}>";
        parts.Add(typePart.Trim());
        if (BaseTypes.Count > 0)
            parts.Add($": {string.Join(", ", BaseTypes)}");
        parts.AddRange(TypeParameterConstraints);
        return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }
    private static List<string> GetOrderedTypeModifiers(IEnumerable<string> modifiers)
    {
        var modSet = new HashSet<string>(modifiers.Select(m => m.Trim().ToLowerInvariant()), StringComparer.OrdinalIgnoreCase);
        return TypeModifierOrder
            .Where(m => modSet.Contains(m))
            .Select(m => modifiers.FirstOrDefault(orig =>
                string.Equals(orig.Trim(), m, StringComparison.OrdinalIgnoreCase)) ?? m)
            .ToList();
    }
}

