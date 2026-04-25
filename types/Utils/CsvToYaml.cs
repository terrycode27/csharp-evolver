public partial class CsvToYaml
{
    private static Dictionary<string, List<string>> ParseCsvToDictionary(string csvPath)
    {
        var result = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var lines = File.ReadAllLines(csvPath)
            .Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line));
        foreach (var line in lines)
        {
            var parts = line.Split(',').Select(p => p.Trim()).ToArray();
            if (parts.Length < 2)
                continue;
            string category = parts[0];
            string typeName = parts[1];
            if (!result.ContainsKey(category))
                result[category] = new List<string>();
            result[category].Add(typeName);
        }
        foreach (var key in result.Keys.ToList())
        {
            result[key] = result[key].OrderBy(t => t).ToList();
        }
        return result;
    }
    private static void SerializeToYaml(Dictionary<string, List<string>> data, string outputPath)
    {
        var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        using var writer = new StringWriter();
        serializer.Serialize(writer, data);
        File.WriteAllText(outputPath, writer.ToString());
    }
}

