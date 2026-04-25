public partial class Tree<T>
{
    public Tree() { }
    public Tree(T value)
    {
        Value = value;
    }
    public List<Tree<T>> Children { get; set; } = new List<Tree<T>>();
    public T Value { get; set; }
    public Tree<T> Add(T value)
    {
        var child = new Tree<T>(value);
        Children.Add(child);
        return child;
    }
    public Tree<T> Add(Tree<T> child)
    {
        Children.Add(child);
        return child;
    }
    public IEnumerable<Tree<T>> Flatten()
    {
        yield return this;
        foreach (var descendant in Children.SelectMany(c => c.Flatten()))
            yield return descendant;
    }
    public void Print(string indent = "")
    {
        Console.WriteLine(indent + (Value?.ToString() ?? "null"));
        foreach (var child in Children)
        {
            child.Print(indent + "  ");
        }
    }
    public string ThisToYAML()
    {
        var serializer = new SerializerBuilder().WithTypeConverter(new TreeYamlConverter()).Build();
        return serializer.Serialize(this);
    }
    public void ThisToYamlFile(string file)
    {
        File.WriteAllText(file, this.ThisToYAML());
    }
}

