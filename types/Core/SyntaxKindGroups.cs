public partial class SyntaxKindGroups
{
    public static readonly HashSet<SyntaxKind> AccessModifierKinds = new()
    {
        SyntaxKind.InternalKeyword,
        SyntaxKind.PrivateKeyword,
        SyntaxKind.ProtectedKeyword,
        SyntaxKind.PublicKeyword,
    };
    public static readonly HashSet<SyntaxKind> AllKeywords = new()
    {
        SyntaxKind.AbstractKeyword,
        SyntaxKind.AddKeyword,
        SyntaxKind.AliasKeyword,
        SyntaxKind.AndKeyword,
        SyntaxKind.AscendingKeyword,
        SyntaxKind.AsKeyword,
        SyntaxKind.AsyncKeyword,
        SyntaxKind.AwaitKeyword,
        SyntaxKind.BaseKeyword,
        SyntaxKind.BoolKeyword,
        SyntaxKind.BreakKeyword,
        SyntaxKind.ByKeyword,
        SyntaxKind.ByteKeyword,
        SyntaxKind.CaseKeyword,
        SyntaxKind.CatchKeyword,
        SyntaxKind.CharKeyword,
        SyntaxKind.CheckedKeyword,
        SyntaxKind.ClassKeyword,
        SyntaxKind.ConstKeyword,
        SyntaxKind.ContinueKeyword,
        SyntaxKind.DecimalKeyword,
        SyntaxKind.DefaultKeyword,
        SyntaxKind.DefineKeyword,
        SyntaxKind.DelegateKeyword,
        SyntaxKind.DescendingKeyword,
        SyntaxKind.DoKeyword,
        SyntaxKind.DoubleKeyword,
        SyntaxKind.ElifKeyword,
        SyntaxKind.ElseKeyword,
        SyntaxKind.ElseKeyword,
        SyntaxKind.EndIfKeyword,
        SyntaxKind.EndRegionKeyword,
        SyntaxKind.EnumKeyword,
        SyntaxKind.EqualsKeyword,
        SyntaxKind.ErrorKeyword,
        SyntaxKind.EventKeyword,
        SyntaxKind.ExplicitKeyword,
        SyntaxKind.ExternKeyword,
        SyntaxKind.FalseKeyword,
        SyntaxKind.FinallyKeyword,
        SyntaxKind.FixedKeyword,
        SyntaxKind.FloatKeyword,
        SyntaxKind.ForEachKeyword,
        SyntaxKind.ForKeyword,
        SyntaxKind.FromKeyword,
        SyntaxKind.GetKeyword,
        SyntaxKind.GlobalKeyword,
        SyntaxKind.GotoKeyword,
        SyntaxKind.GroupKeyword,
        SyntaxKind.IfKeyword,
        SyntaxKind.IfKeyword,
        SyntaxKind.ImplicitKeyword,
        SyntaxKind.InitKeyword,
        SyntaxKind.InKeyword,
        SyntaxKind.InterfaceKeyword,
        SyntaxKind.InternalKeyword,
        SyntaxKind.IntKeyword,
        SyntaxKind.IntoKeyword,
        SyntaxKind.IsKeyword,
        SyntaxKind.JoinKeyword,
        SyntaxKind.LetKeyword,
        SyntaxKind.LineKeyword,
        SyntaxKind.LoadKeyword,
        SyntaxKind.LockKeyword,
        SyntaxKind.LongKeyword,
        SyntaxKind.ManagedKeyword,
        SyntaxKind.NameOfKeyword,
        SyntaxKind.NamespaceKeyword,
        SyntaxKind.NewKeyword,
        SyntaxKind.NotKeyword,
        SyntaxKind.NullKeyword,
        SyntaxKind.ObjectKeyword,
        SyntaxKind.OnKeyword,
        SyntaxKind.OperatorKeyword,
        SyntaxKind.OrderByKeyword,
        SyntaxKind.OrKeyword,
        SyntaxKind.OutKeyword,
        SyntaxKind.OverrideKeyword,
        SyntaxKind.ParamsKeyword,
        SyntaxKind.PartialKeyword,
        SyntaxKind.PragmaKeyword,
        SyntaxKind.PrivateKeyword,
        SyntaxKind.ProtectedKeyword,
        SyntaxKind.PublicKeyword,
        SyntaxKind.ReadOnlyKeyword,
        SyntaxKind.RecordKeyword,
        SyntaxKind.RefKeyword,
        SyntaxKind.RegionKeyword,
        SyntaxKind.RemoveKeyword,
        SyntaxKind.RequiredKeyword,
        SyntaxKind.RestoreKeyword,
        SyntaxKind.ReturnKeyword,
        SyntaxKind.SByteKeyword,
        SyntaxKind.ScopedKeyword,
        SyntaxKind.SealedKeyword,
        SyntaxKind.SelectKeyword,
        SyntaxKind.SetKeyword,
        SyntaxKind.ShortKeyword,
        SyntaxKind.StackAllocKeyword,
        SyntaxKind.StaticKeyword,
        SyntaxKind.StringKeyword,
        SyntaxKind.StructKeyword,
        SyntaxKind.SwitchKeyword,
        SyntaxKind.ThisKeyword,
        SyntaxKind.ThrowKeyword,
        SyntaxKind.TrueKeyword,
        SyntaxKind.TryKeyword,
        SyntaxKind.TypeOfKeyword,
        SyntaxKind.UIntKeyword,
        SyntaxKind.ULongKeyword,
        SyntaxKind.UncheckedKeyword,
        SyntaxKind.UndefKeyword,
        SyntaxKind.UnmanagedKeyword,
        SyntaxKind.UnsafeKeyword,
        SyntaxKind.UShortKeyword,
        SyntaxKind.UsingKeyword,
        SyntaxKind.VarKeyword,
        SyntaxKind.VirtualKeyword,
        SyntaxKind.VoidKeyword,
        SyntaxKind.VolatileKeyword,
        SyntaxKind.WarningKeyword,
        SyntaxKind.WhenKeyword,
        SyntaxKind.WhereKeyword,
        SyntaxKind.WhileKeyword,
        SyntaxKind.WithKeyword,
        SyntaxKind.YieldKeyword,
    };
    public static readonly HashSet<SyntaxKind> ArgumentAndTypeArgumentListKinds = new()
    {
        SyntaxKind.ArgumentList,
        SyntaxKind.TypeArgumentList,
        SyntaxKind.TypeParameterList,
    };
    public static readonly HashSet<SyntaxKind> ArgumentListKind = new() { SyntaxKind.ArgumentList };
    public static readonly HashSet<SyntaxKind> AttributeKinds = new()
    {
        SyntaxKind.Attribute,
        SyntaxKind.AttributeList,
    };
    public static readonly HashSet<SyntaxKind> BodyElements = new()
    {
        SyntaxKind.ArrowExpressionClause,
        SyntaxKind.Block,
        SyntaxKind.BreakStatement,
        SyntaxKind.CheckedStatement,
        SyntaxKind.ContinueStatement,
        SyntaxKind.DoStatement,
        SyntaxKind.EmptyStatement,
        SyntaxKind.ExpressionStatement,
        SyntaxKind.FixedStatement,
        SyntaxKind.ForEachStatement,
        SyntaxKind.ForStatement,
        SyntaxKind.GotoStatement,
        SyntaxKind.IfStatement,
        SyntaxKind.LabeledStatement,
        SyntaxKind.LocalDeclarationStatement,
        SyntaxKind.LockStatement,
        SyntaxKind.ReturnStatement,
        SyntaxKind.SwitchStatement,
        SyntaxKind.ThrowStatement,
        SyntaxKind.TryStatement,
        SyntaxKind.UncheckedStatement,
        SyntaxKind.UnsafeStatement,
        SyntaxKind.UsingStatement,
        SyntaxKind.WhileStatement,
        SyntaxKind.YieldBreakStatement,
        SyntaxKind.YieldReturnStatement,
    };
    public static readonly HashSet<SyntaxKind> Brackets = new()
    {
        SyntaxKind.CloseBracketToken,
        SyntaxKind.OpenBracketToken,
    };
    public static readonly HashSet<SyntaxKind> ClassMemberKinds = new()
    {
        SyntaxKind.ConstructorDeclaration,
        SyntaxKind.ConversionOperatorDeclaration,
        SyntaxKind.DelegateDeclaration,
        SyntaxKind.DestructorDeclaration,
        SyntaxKind.EnumDeclaration,
        SyntaxKind.EventDeclaration,
        SyntaxKind.EventFieldDeclaration,
        SyntaxKind.FieldDeclaration,
        SyntaxKind.IndexerDeclaration,
        SyntaxKind.InterfaceDeclaration,
        SyntaxKind.MethodDeclaration,
        SyntaxKind.OperatorDeclaration,
        SyntaxKind.PropertyDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,
        SyntaxKind.StructDeclaration,
    };
    public static readonly HashSet<SyntaxKind> ClassOrInterface = new()
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.InterfaceDeclaration,
    };
    public static readonly HashSet<SyntaxKind> ClassTypeModifierKeywords = new()
    {
        SyntaxKind.AbstractKeyword,
        SyntaxKind.InternalKeyword,
        SyntaxKind.NewKeyword,
        SyntaxKind.PartialKeyword,
        SyntaxKind.PrivateKeyword,
        SyntaxKind.ProtectedKeyword,
        SyntaxKind.PublicKeyword,
        SyntaxKind.SealedKeyword,
        SyntaxKind.StaticKeyword,
        SyntaxKind.UnsafeKeyword,
    };
    public static readonly HashSet<SyntaxKind> CollectionDelimeterKinds = new()
    {
        SyntaxKind.CloseBraceToken,
        SyntaxKind.CommaToken,
        SyntaxKind.EndOfLineTrivia,
        SyntaxKind.OpenBraceToken,
        SyntaxKind.WhitespaceTrivia,
    };
    public static readonly HashSet<SyntaxKind> CollectionInitializerKinds = new()
    {
        SyntaxKind.ArrayInitializerExpression,
        SyntaxKind.CollectionInitializerExpression,
        SyntaxKind.ObjectInitializerExpression,
    };
    public static readonly HashSet<SyntaxKind> CommentTrivia = new()
    {
        SyntaxKind.MultiLineCommentTrivia,
        SyntaxKind.MultiLineDocumentationCommentTrivia,
        SyntaxKind.SingleLineCommentTrivia,
        SyntaxKind.SingleLineDocumentationCommentTrivia,
    };
    public static readonly Dictionary<SyntaxKind, SyntaxKind> DeclarationToKeywordMap = new()
    {
        [SyntaxKind.ClassDeclaration] = SyntaxKind.ClassKeyword,
        [SyntaxKind.DelegateDeclaration] = SyntaxKind.DelegateKeyword,
        [SyntaxKind.EnumDeclaration] = SyntaxKind.EnumKeyword,
        [SyntaxKind.InterfaceDeclaration] = SyntaxKind.InterfaceKeyword,
        [SyntaxKind.NamespaceDeclaration] = SyntaxKind.NamespaceKeyword,
        [SyntaxKind.RecordDeclaration] = SyntaxKind.RecordKeyword,
        [SyntaxKind.RecordStructDeclaration] = SyntaxKind.RecordKeyword,
        [SyntaxKind.StructDeclaration] = SyntaxKind.StructKeyword,
    };
    public static readonly HashSet<SyntaxKind> EnumMemberGroup = new()
    {
        SyntaxKind.CommaToken,
        SyntaxKind.EnumMemberDeclaration,
    };
    public static readonly HashSet<SyntaxKind> FieldSigntureBoundaries = new() { SyntaxKind.SemicolonToken, SyntaxKind.EqualsToken };
    public static readonly HashSet<SyntaxKind> GroupKinds = new()
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.InterfaceDeclaration,
        SyntaxKind.NamespaceDeclaration,
    };
    public static readonly HashSet<SyntaxKind> InvisibleOrWhitespaceTrivia = new()
    {
        SyntaxKind.BadDirectiveTrivia,
        SyntaxKind.ConflictMarkerTrivia,
        SyntaxKind.DisabledTextTrivia,
        SyntaxKind.EndOfLineTrivia,
        SyntaxKind.ShebangDirectiveTrivia,
        SyntaxKind.SkippedTokensTrivia,
        SyntaxKind.WhitespaceTrivia,
    };
    public static readonly HashSet<SyntaxKind> InvokedExpressionKinds = new()
    {
        SyntaxKind.GenericName,
        SyntaxKind.IdentifierName,
        SyntaxKind.MemberBindingExpression,
        SyntaxKind.QualifiedName,
        SyntaxKind.SimpleMemberAccessExpression,
    };
    public static readonly HashSet<SyntaxKind> KindsContainingParameterType = new()
    {
        SyntaxKind.AliasQualifiedName,
        SyntaxKind.ArrayType,
        SyntaxKind.FunctionPointerType,
        SyntaxKind.GenericName,
        SyntaxKind.IdentifierName,
        SyntaxKind.NullableType,
        SyntaxKind.OmittedTypeArgument,
        SyntaxKind.PointerType,
        SyntaxKind.PredefinedType,
        SyntaxKind.QualifiedName,
        SyntaxKind.RefType,
        SyntaxKind.TupleType,
    };
    public static readonly HashSet<SyntaxKind> MemberAccessKinds = new()
    {
        SyntaxKind.ConditionalAccessExpression,
        SyntaxKind.PointerMemberAccessExpression,
        SyntaxKind.SimpleMemberAccessExpression,
    };
    public static readonly HashSet<SyntaxKind> MemberAccessOperatorKinds = new()
    {
        SyntaxKind.DotToken,
        SyntaxKind.QuestionToken,
    };
    public static readonly HashSet<SyntaxKind> MemberModifierKeywords = new()
    {
        SyntaxKind.AbstractKeyword,
        SyntaxKind.AsyncKeyword,
        SyntaxKind.ConstKeyword,
        SyntaxKind.ExplicitKeyword,
        SyntaxKind.ExternKeyword,
        SyntaxKind.ImplicitKeyword,
        SyntaxKind.InKeyword,
        SyntaxKind.InternalKeyword,
        SyntaxKind.NewKeyword,
        SyntaxKind.OutKeyword,
        SyntaxKind.OverrideKeyword,
        SyntaxKind.PartialKeyword,
        SyntaxKind.PrivateKeyword,
        SyntaxKind.ProtectedKeyword,
        SyntaxKind.PublicKeyword,
        SyntaxKind.ReadOnlyKeyword,
        SyntaxKind.RefKeyword,
        SyntaxKind.RequiredKeyword,
        SyntaxKind.SealedKeyword,
        SyntaxKind.StaticKeyword,
        SyntaxKind.ThisKeyword,
        SyntaxKind.UnsafeKeyword,
        SyntaxKind.VirtualKeyword,
        SyntaxKind.VolatileKeyword,
    };
    public static readonly HashSet<SyntaxKind> MethodDeclarationNonNameKinds = new()
    {
        SyntaxKind.AbstractKeyword,
        SyntaxKind.ArrayType,
        SyntaxKind.ArrowExpressionClause,
        SyntaxKind.AsyncKeyword,
        SyntaxKind.AttributeList,
        SyntaxKind.Block,
        SyntaxKind.CloseParenToken,
        SyntaxKind.EqualsGreaterThanToken,
        SyntaxKind.ExplicitInterfaceSpecifier,
        SyntaxKind.ExplicitKeyword,
        SyntaxKind.ExternKeyword,
        SyntaxKind.FunctionPointerType,
        SyntaxKind.GenericName,
        SyntaxKind.IdentifierName,
        SyntaxKind.ImplicitKeyword,
        SyntaxKind.InternalKeyword,
        SyntaxKind.NewKeyword,
        SyntaxKind.NullableType,
        SyntaxKind.OpenParenToken,
        SyntaxKind.OperatorKeyword,
        SyntaxKind.OverrideKeyword,
        SyntaxKind.ParameterList,
        SyntaxKind.PartialKeyword,
        SyntaxKind.PointerType,
        SyntaxKind.PredefinedType,
        SyntaxKind.PrivateKeyword,
        SyntaxKind.ProtectedKeyword,
        SyntaxKind.PublicKeyword,
        SyntaxKind.QualifiedName,
        SyntaxKind.ReadOnlyKeyword,
        SyntaxKind.RefKeyword,
        SyntaxKind.RefType,
        SyntaxKind.RequiredKeyword,
        SyntaxKind.ScopedKeyword,
        SyntaxKind.SealedKeyword,
        SyntaxKind.SemicolonToken,
        SyntaxKind.StaticKeyword,
        SyntaxKind.TupleType,
        SyntaxKind.TypeParameterList,
        SyntaxKind.UnsafeKeyword,
        SyntaxKind.VirtualKeyword,
        SyntaxKind.WhereKeyword,
    };
    public static readonly HashSet<SyntaxKind> MethodSigntureBoundaries = new()
    {
        SyntaxKind.ArrowExpressionClause,
        SyntaxKind.Block,
    };
    public static readonly HashSet<SyntaxKind> NameReferenceKinds = new()
    {
        SyntaxKind.IdentifierName,
        SyntaxKind.IdentifierToken,
        SyntaxKind.SimpleMemberAccessExpression,
    };
    public static readonly HashSet<SyntaxKind> NamespaceLevelExclude = new() { SyntaxKind.NamespaceDeclaration, SyntaxKind.DelegateDeclaration };
    public static readonly HashSet<SyntaxKind> NamespaceMemberKinds = new()
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.DelegateDeclaration,
        SyntaxKind.EnumDeclaration,
        SyntaxKind.FileScopedNamespaceDeclaration,
        SyntaxKind.InterfaceDeclaration,
        SyntaxKind.NamespaceDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,
        SyntaxKind.StructDeclaration,
        SyntaxKind.UsingDirective,
    };
    public static readonly HashSet<SyntaxKind> NamespaceTypeChildren = new()
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.DelegateDeclaration,
        SyntaxKind.EnumDeclaration,
        SyntaxKind.InterfaceDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,
        SyntaxKind.StructDeclaration,
    };
    public static readonly HashSet<SyntaxKind> ObjectCreationKinds = new()
    {
        SyntaxKind.ArrayCreationExpression,
        SyntaxKind.ArrayInitializerExpression,
        SyntaxKind.CollectionExpression,
        SyntaxKind.CollectionInitializerExpression,
        SyntaxKind.ImplicitArrayCreationExpression,
        SyntaxKind.ObjectInitializerExpression,
        SyntaxKind.WithExpression,
        SyntaxKind.WithInitializerExpression,
    };
    public static readonly HashSet<SyntaxKind> ParameterDelimeterKinds = new()
    {
        SyntaxKind.CloseParenToken,
        SyntaxKind.CommaToken,
        SyntaxKind.OpenParenToken,
    };
    public static readonly HashSet<SyntaxKind> ParameterizedMemberWithBodySignatureBoundaries = new() { SyntaxKind.ArrowExpressionClause, SyntaxKind.Block };
    public static readonly HashSet<SyntaxKind> PropertySigntureBoundaries = new() { SyntaxKind.AccessorList, SyntaxKind.ArrowExpressionClause };
    public static readonly HashSet<SyntaxKind> RecordTypes = new()
    {
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,
    };
    public static readonly HashSet<SyntaxKind> RegionTrivia = new()
    {
        SyntaxKind.EndRegionDirectiveTrivia,
        SyntaxKind.EndRegionKeyword,
        SyntaxKind.RegionDirectiveTrivia,
        SyntaxKind.RegionKeyword,
    };
    public static readonly HashSet<SyntaxKind> SelfMergeKinds = new()
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.NamespaceDeclaration,
    };
    public static readonly HashSet<SyntaxKind> TypeArgumentListKind = new()
    {
        SyntaxKind.TypeArgumentList,
    };
    public static readonly HashSet<SyntaxKind> TypeArgumentListKinds = new()
    {
        SyntaxKind.TypeArgumentList,
        SyntaxKind.TypeParameterList,
    };
    public static readonly HashSet<SyntaxKind> TypeDeclarations = new()
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.DelegateDeclaration,
        SyntaxKind.InterfaceDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,
        SyntaxKind.StructDeclaration,
    };
    public static readonly HashSet<SyntaxKind> TypeNameKinds = new() { SyntaxKind.IdentifierToken, SyntaxKind.PredefinedType };
    public static readonly HashSet<SyntaxKind> TypeNameLeafKinds = new()
    {
        SyntaxKind.IdentifierName,
        SyntaxKind.IdentifierToken,
    };
    public static readonly HashSet<SyntaxKind> TypesWithMethods = new()
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.InterfaceDeclaration,
    };
    public static readonly HashSet<SyntaxKind> VirtualFunctionSyntaxKinds = new()
    {
        SyntaxKind.AbstractKeyword,
        SyntaxKind.OverrideKeyword,
        SyntaxKind.VirtualKeyword,
    };
    public static readonly HashSet<SyntaxKind> WhitespaceTrivia = new()
    {
        SyntaxKind.EndOfLineTrivia,
        SyntaxKind.WhitespaceTrivia,
    };
    static readonly HashSet<SyntaxKind> AllTrivia = new()
    {
        SyntaxKind.BadDirectiveTrivia,
        SyntaxKind.ConflictMarkerTrivia,
        SyntaxKind.DefineDirectiveTrivia,
        SyntaxKind.DisabledTextTrivia,
        SyntaxKind.ElifDirectiveTrivia,
        SyntaxKind.ElseDirectiveTrivia,
        SyntaxKind.EndIfDirectiveTrivia,
        SyntaxKind.EndOfLineTrivia,
        SyntaxKind.EndRegionDirectiveTrivia,
        SyntaxKind.ErrorDirectiveTrivia,
        SyntaxKind.IfDirectiveTrivia,
        SyntaxKind.LineDirectiveTrivia,
        SyntaxKind.LoadDirectiveTrivia,
        SyntaxKind.MultiLineCommentTrivia,
        SyntaxKind.MultiLineDocumentationCommentTrivia,
        SyntaxKind.MultiLineDocumentationCommentTrivia,
        SyntaxKind.NullableDirectiveTrivia,
        SyntaxKind.PragmaChecksumDirectiveTrivia,
        SyntaxKind.PragmaWarningDirectiveTrivia,
        SyntaxKind.ReferenceDirectiveTrivia,
        SyntaxKind.RegionDirectiveTrivia,
        SyntaxKind.ShebangDirectiveTrivia,
        SyntaxKind.SingleLineCommentTrivia,
        SyntaxKind.SingleLineDocumentationCommentTrivia,
        SyntaxKind.SkippedTokensTrivia,
        SyntaxKind.UndefDirectiveTrivia,
        SyntaxKind.WarningDirectiveTrivia,
        SyntaxKind.WhitespaceTrivia,
    };
    static readonly HashSet<SyntaxKind> MemberDeclarations = new()
    {
        SyntaxKind.ConstructorDeclaration,
        SyntaxKind.ConversionOperatorDeclaration,
        SyntaxKind.DestructorDeclaration,
        SyntaxKind.EventDeclaration,
        SyntaxKind.EventFieldDeclaration,
        SyntaxKind.FieldDeclaration,
        SyntaxKind.IndexerDeclaration,
        SyntaxKind.MethodDeclaration,
        SyntaxKind.OperatorDeclaration,
        SyntaxKind.PropertyDeclaration,
    };
    static readonly HashSet<SyntaxKind> MemberKinds = new()
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.ConstructorDeclaration,
        SyntaxKind.DelegateDeclaration,
        SyntaxKind.DestructorDeclaration,
        SyntaxKind.EnumDeclaration,
        SyntaxKind.EventDeclaration,
        SyntaxKind.EventFieldDeclaration,
        SyntaxKind.FieldDeclaration,
        SyntaxKind.IndexerDeclaration,
        SyntaxKind.InterfaceDeclaration,
        SyntaxKind.MethodDeclaration,
        SyntaxKind.OperatorDeclaration,
        SyntaxKind.PropertyDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,
        SyntaxKind.StructDeclaration,
    };
    static readonly HashSet<SyntaxKind> Region = new()
    {
        SyntaxKind.EndRegionDirectiveTrivia,
        SyntaxKind.EndRegionKeyword,
        SyntaxKind.RegionDirectiveTrivia,
        SyntaxKind.RegionKeyword,
    };
    static readonly HashSet<SyntaxKind> TypeKindsThatCanContainMethodImplementations = new()
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,
        SyntaxKind.StructDeclaration,
    };
}

