public static partial class ExtensionsOfIEnumerableOfSignatureData
{
    public static SignatureData SupersetSignatures(this IEnumerable<SignatureData> signatures)
    {
        if (signatures == null || !signatures.Any())
            return new SignatureData();
        return new SignatureData
        {
            Attributes = signatures.SelectMany(s => s.Attributes).Distinct().ToList(),
            BaseTypes = signatures.SelectMany(s => s.BaseTypes).Distinct().ToList(),
            TypeModifiers = signatures.SelectMany(s => s.TypeModifiers).Distinct().ToList(),
            AccessModifiers = signatures.SelectMany(s => s.AccessModifiers).Distinct().ToList(),
            TypeName = signatures.Select(t => t.TypeName).Distinct().Single(),
            TypeKeyword = signatures.Select(t => t.TypeKeyword).Distinct().Single(),
            TypeParameters = signatures
                .SelectMany(s => s.TypeParameters)
                .Distinct()
                .OrderBy(p => p)
                .ToList(),
            TypeParameterConstraints = signatures
                .SelectMany(s => s.TypeParameterConstraints)
                .Distinct()
                .ToList()
        };
    }
}

