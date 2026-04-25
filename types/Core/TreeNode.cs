public partial class TreeNode<T>
{
    public TreeNode(T? value)
    {
        Value = value;
    }
    private List<TreeNode<T>> _children = new();
    public List<TreeNode<T>> Children
    {
        get => _children;
        set => _children = value ?? new List<TreeNode<T>>();
    }
    public bool IsRoot => Parent == null;
    public TreeNode<T>? Parent { get; set; }
    public T? Value { get; set; }
    public void AddChild(TreeNode<T> child)
    {
        if (child.Parent != null)
        {
            child.Parent._children.Remove(child);
        }
        child.Parent = this;
        _children.Add(child);
    }
    public void AddChildren(IEnumerable<TreeNode<T>> children)
    {
        foreach (var c in children)
            AddChild(c);
    }
    public void InsertChild(int index, TreeNode<T> child)
    {
        child.Parent = this;
        _children.Insert(index, child);
    }
    public void InsertFirstChild(TreeNode<T> child)
    {
        InsertChild(0, child);
    }
    public bool RemoveChild(TreeNode<T> child)
    {
        if (_children.Remove(child))
        {
            child.Parent = null;
            return true;
        }
        return false;
    }
    public void RemoveSelf()
    {
        Parent?.Children.Remove(this);
    }
    public void ReplaceSelf(TreeNode<T> replacement)
    {
        int index = Parent.Children.IndexOf(this);
        if (index >= 0)
        {
            Parent.Children.RemoveAt(index);
            Parent.Children.Insert(index, replacement);
            replacement.Parent = Parent;
        }
        else
        {
            throw new Exception();
        }
    }
    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
}

