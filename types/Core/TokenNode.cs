public partial class TokenNode : NodeBase<TokenNode>
{
    public TokenNode(SyntaxNodeOrToken node)
    {
        Kind = node.Kind();
        if (node.IsToken)
        {
            var token = node.AsToken();
            Text = token.Text;
            if (token.IsKind(SyntaxKind.IdentifierToken))
                Identifier = token.Text;
            LeadingTrivia = token
                .LeadingTrivia.Select(t => new SyntaxElementNode(t.Kind(), t.ToFullString()))
                .ToList()
                .AsReadOnly();
            TrailingTrivia = token
                .TrailingTrivia.Select(t => new SyntaxElementNode(t.Kind(), t.ToFullString()))
                .ToList()
                .AsReadOnly();
        }
        else
        {
            OriginalSyntaxNode = node.AsNode();
        }
    }
    public string? Identifier { get; }
    public SyntaxKind Kind { get; }
    public IReadOnlyList<SyntaxElementNode> LeadingTrivia { get; private set; } = Array.Empty<SyntaxElementNode>();
    public SyntaxNode? OriginalSyntaxNode { get; }
    public string? Text { get; }
    public IReadOnlyList<SyntaxElementNode> TrailingTrivia { get; private set; } = Array.Empty<SyntaxElementNode>();
    public string ToString(bool showTrivia)
    {
        var sb = new StringBuilder(Kind.ToString());
        if (Identifier is not null)
            sb.Append($"({Identifier})");
        if (showTrivia)
        {
            var interestingLeading = LeadingTrivia.Count(t => !IsTrivialWhitespace(t.Kind));
            var interestingTrailing = TrailingTrivia.Count(t => !IsTrivialWhitespace(t.Kind));
            if (interestingLeading > 0)
                sb.Append($" ↑{interestingLeading}");
            if (interestingTrailing > 0)
                sb.Append($" ↓{interestingTrailing}");
        }
        return sb.ToString();
    }
    public override string ToString() => ToString(false);
    public List<SyntaxElementNode> ToSyntaxElements()
    {
        var elements = new List<SyntaxElementNode>(LeadingTrivia.Count + 1 + TrailingTrivia.Count);
        elements.AddRange(LeadingTrivia);
        if (Text != null || Kind != SyntaxKind.None)
            elements.Add(new SyntaxElementNode(Kind, Text, Identifier));
        elements.AddRange(TrailingTrivia);
        return elements;
    }
    private SyntaxElementNode? GetFullTrivia(int index, bool isLeading = true)
    {
        var list = isLeading ? LeadingTrivia : TrailingTrivia;
        if (index < 0 || index >= list.Count)
            return null;
        return list[index];
    }
    private static bool IsTrivialWhitespace(SyntaxKind k) => k is SyntaxKind.WhitespaceTrivia or SyntaxKind.EndOfLineTrivia;
}

