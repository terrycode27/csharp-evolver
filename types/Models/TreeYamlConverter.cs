public partial class TreeYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Tree<>);
    }
    public object ReadYaml(YamlDotNet.Core.IParser parser, Type type, ObjectDeserializer nestedObjectDeserializer)
    {
        throw new NotImplementedException("Tree<T> deserialization is not implemented.");
    }
    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer nestedObjectSerializer)
    {
        Tree<string> tree = (Tree<string>)value;
        string nodeValue = tree.Value?.ToString() ?? "null";
        if (tree.Children.Count == 0)
        {
            emitter.Emit(new Scalar(nodeValue));
        }
        else
        {
            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar(nodeValue));
            emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Block));
            foreach (var child in tree.Children)
            {
                WriteYaml(emitter, child, child.GetType(), nestedObjectSerializer);
            }
            emitter.Emit(new SequenceEnd());
            emitter.Emit(new MappingEnd());
        }
    }
}

