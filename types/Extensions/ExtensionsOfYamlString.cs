public static partial class ExtensionsOfYamlString
{
    public static T FromYAML<T>(this yamlString yaml)
    {
        var deserializer = new DeserializerBuilder().Build();
        var single = deserializer.Deserialize<T>(yaml.Value);
        return single;
    }
}

