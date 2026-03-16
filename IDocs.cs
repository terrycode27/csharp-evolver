using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;


[ClassDoc("__Program")]
[Doc("Entry point and test harness for the Evolver5 semantic tree pipeline.")]
interface IDocument___Program
{
    [Doc("Primary entry point: loads Evolver5 project, extracts interfaces, and runs self-test.")]
    void Main();

    [Doc("Test helper that loads two trees, merges them, groups by kind/modifier/name, and writes result.")]
    void MergeTest();

    [Doc("Runs semantic serializer round-trip test and groups nodes by kind, modifier, and name.")]
    void SelfTest();
}

[ClassDoc("AddAttributeExtensions")]
[Doc("Extension methods that inject [ClassDoc] and [Doc] attributes into semantic tree nodes during interface extraction.")]
interface IDocument_AddAttributeExtensions
{
    [Doc("Adds a [ClassDoc(\"{className}\")] attribute to a NamedNode.")]
    void AddClassAttribute(NamedNode named, string className);

    [Doc("Adds a [Doc(\"{doc}\")] attribute (optional documentation text).")]
    void AddDocAttribute(NamedNode named, string doc = null);

    [Doc("Inserts a list of using directives at the top of the root compilation unit.")]
    void AddUsings(TreeNode<SemanticNode> root, List<UsingDirectiveNode> usings);

    [Doc("Core helper that adds an attribute syntax node if it does not already exist.")]
    void AddAttribute(NamedNode named, string attributeString);

    [Doc("Builds a minimal TreeNode containing an AttributeList from raw attribute text.")]
    TreeNode<SemanticNode> BuildAttributeNode(string attributeText);

    [Doc("Returns true when the target node exists and the attribute string is non-empty.")]
    bool ShouldAddAttribute(NamedNode named, string attributeString);
}

[ClassDoc("ArgumentListNode")]
[Doc("Represents an argument list in an invocation or call expression.")]
interface IDocument_ArgumentListNode
{
    [Doc("Collected ArgumentNode children.")]
    List<ArgumentNode> Arguments { get; }

    [Doc("Attaches an ArgumentNode child.")]
    void AttachChild(SemanticNode child);
}

[ClassDoc("ArgumentNode")]
[Doc("Single argument inside an argument list; extracts name, expression, and detects 'this' for extension methods.")]
interface IDocument_ArgumentNode
{
    [Doc("Optional named argument prefix (e.g. 'name:').")]
    string? ArgumentName { get; set; }

    [Doc("Tree node holding the argument expression.")]
    TreeNode<SemanticNode>? ExpressionNode { get; set; }

    [Doc("True when the argument uses the 'this' keyword (extension method).")]
    bool IsThisArgument { get; set; }

    [Doc("Initializes name, expression, and this-detection from the syntax tree.")]
    void SetTreeNode(TreeNode<SemanticNode> tree);

    [Doc("Scans for ThisKeyword token.")]
    void DetectThisArgument();

    [Doc("Extracts the name from a NameColon node.")]
    void ExtractArgumentName();

    [Doc("Captures the first non-delimiter child as the expression.")]
    void ExtractExpression();
}

[ClassDoc("AttributeListNode")]
[Doc("Holds multiple AttributeNodes and provides containment checks (ignoring the 'Attribute' suffix).")]
interface IDocument_AttributeListNode
{
    [Doc("List of individual attributes.")]
    List<AttributeNode> Attributes { get; }

    [Doc("Attaches AttributeNode children.")]
    void AttachChild(SemanticNode child);

    [Doc("Checks whether any attribute matches the supplied name (suffix-normalized).")]
    bool Contains(string name);

    [Doc("Removes the 'Attribute' suffix for comparison.")]
    string StripAttributeSuffix(string input);
}

[ClassDoc("AttributeNode")]
[Doc("Represents a single attribute usage; extracts the attribute type name.")]
interface IDocument_AttributeNode
{
    [Doc("Locates the identifier token that names the attribute.")]
    void SetNameNode();
}

[ClassDoc("BaseNode")]
[Doc("Generic base for typed tree nodes (SemanticNode / SyntaxElementNode / TokenNode).")]
interface IDocument_BaseNode<T>
    where T : BaseNode<T>
{ }

[ClassDoc("BlockNode")]
[Doc("Represents a { ... } block containing statements.")]
interface IDocument_BlockNode
{
    [Doc("Child statements inside the block.")]
    List<SemanticNode> Statements { get; }

    [Doc("Attaches statement children.")]
    void AttachChild(SemanticNode child);
}

[ClassDoc("ClassDocAttribute")]
[Doc("Custom attribute that records the original class name on generated interface declarations.")]
interface IDocument_ClassDocAttribute { }

[ClassDoc("ClassNode")]
[Doc("Semantic wrapper for ClassDeclarationSyntax with helpers for static/extension conversion, modifiers, and partial handling.")]
interface IDocument_ClassNode
{
    [Doc("Delegate declarations inside the class.")]
    List<DelegateNode> Delegates { get; }

    [Doc("True when the class contains any method declarations.")]
    bool HasMethods { get; }

    [Doc("Detects presence of the static keyword before the opening brace.")]
    bool IsStaticClass { get; }

    [Doc("Default access for members when none is specified.")]
    AccessModifier DefaultModifier { get; }

    [Doc("Attaches child nodes (including delegates).")]
    void AttachChild(SemanticNode child);

    [Doc("Returns the keyword that identifies this declaration type (ClassKeyword).")]
    SyntaxKind? GetTypeSpecificKeyword();

    [Doc("Locates the first modifier token before the opening brace.")]
    void SetModifierFromTree();

    [Doc("Inserts a keyword (e.g. static) before the class keyword.")]
    void AddKeywordToDeclaration(SyntaxKind kind);

    [Doc("Converts static extension methods back to instance methods.")]
    void ConvertMethodsFromStaticExtension();

    [Doc("Converts instance methods to static extension methods.")]
    void ConvertMethodsToStaticExtension();

    [Doc("Removes static keyword and converts methods to instance style.")]
    void ConvertToInstanceClass();

    [Doc("Adds static keyword and converts methods to extension style.")]
    void ConvertToStaticExtensionClass();

    [Doc("Removes the static keyword token from the declaration.")]
    void RemoveStaticKeywordFromDeclaration();
}

[ClassDoc("CommentNode")]
[Doc("Represents single-line or multi-line comment trivia.")]
interface IDocument_CommentNode { }

[ClassDoc("CompilationUnitNode")]
[Doc("Root node of a C# file; holds usings and top-level members.")]
interface IDocument_CompilationUnitNode
{
    [Doc("Top-level type/namespace members.")]
    List<SemanticNode> Members { get; }

    [Doc("Using directives at the top of the file.")]
    List<UsingDirectiveNode> Usings { get; }

    [Doc("Attaches either a UsingDirectiveNode or a member.")]
    void AttachChild(SemanticNode child);

    [Doc("Factory that creates a fresh compilation unit tree.")]
    TreeNode<SemanticNode> Factory();
}

[ClassDoc("CompilationUtil")]
[Doc("Helpers for compiling original vs. regenerated code against a Roslyn Project to verify round-tripping.")]
interface IDocument_CompilationUtil
{
    [Doc("Compiles code against a .csproj file.")]
    (bool Success, ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<Diagnostic> Errors) Compile(string code, string projectFile);

    [Doc("Compiles code using an already-loaded Roslyn Project.")]
    (bool Success, ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<Diagnostic> Errors) Compile(string code, Project project);

    [Doc("Verifies that both original and regenerated code compile successfully.")]
    bool RoundTripCompilation(string originalCode, string regeneratedCode, string csprojFileName);
}

[ClassDoc("ConstructorNode")]
[Doc("Semantic wrapper for constructor declarations (inherits parameter and body handling).")]
interface IDocument_ConstructorNode { }

[ClassDoc("CSharpierFormatter")]
[Doc("Isolated wrapper around CSharpier formatter; loads it in its own AssemblyLoadContext to avoid Roslyn version conflicts.")]
interface IDocument_CSharpierFormatter
{
    [Doc("Formats source code using CSharpier (static convenience method).")]
    string Format(string code);

    [Doc("Loads CSharpier.Core.dll into an isolated context because newer Roslyn breaks it.")]
    void BecauseNewerRoslynBreaksCSharpier(string path);

    [Doc("Initializes the formatter on static construction.")]
    void Initialize();

    [Doc("Unloads the isolated AssemblyLoadContext.")]
    void Unload();
}

[ClassDoc("DelegateNode")]
[Doc("Semantic wrapper for delegate declarations.")]
interface IDocument_DelegateNode
{
    [Doc("Parameter list of the delegate.")]
    ParameterListNode? ParameterList { get; set; }

    [Doc("Attaches the parameter list child.")]
    void AttachChild(SemanticNode child);

    [Doc("Returns DelegateKeyword.")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("DestructorNode")]
[Doc("Semantic wrapper for destructor (~Class) declarations.")]
interface IDocument_DestructorNode { }

[ClassDoc("DocAttribute")]
[Doc("Custom attribute that can store documentation text and an optional code-checkout reason.")]
interface IDocument_DocAttribute { }

[ClassDoc("EntryPoint")]
[Doc("Main orchestration class for loading, merging, grouping, splitting, and refactoring semantic trees for a given namespace/project.")]
interface IDocument_EntryPoint
{
    [Doc("Base namespace used for file/project naming.")]
    string BaseNamespace { get; }

    [Doc("Path to the per-class output file.")]
    string ClassFilePath { get; }

    [Doc("Path to the consolidated single-file output.")]
    string ConsolidatedFilePath { get; }

    [Doc("Path to the per-method stub file.")]
    string MethodFilePath { get; }

    [Doc("Namespace for the consolidated file.")]
    string NamespaceConsolidated { get; }

    [Doc("Namespace suffix used when splitting into one class per file.")]
    string OneClassPerFileNamespace { get; }

    [Doc("Suffix appended for one-class-per-file mode.")]
    string OneClassPerFileNamespaceSuffix { get; }

    [Doc("Namespace for one-namespace-per-file mode.")]
    string OneNamespacePerFileNamespace { get; }

    [Doc("Suffix for one-namespace-per-file mode.")]
    string OneNamespacePerFileNamespaceSuffix { get; }

    [Doc("Full path to the .csproj file.")]
    string ProjectFilePath { get; }

    [Doc("Root directory for the project (derived from AppDomain or supplied).")]
    string RootDirectory { get; }

    [Doc("Path used for test output.")]
    string TestFilePath { get; }

    [Doc("Factory method to create an EntryPoint instance.")]
    EntryPoint Create(string baseNamespace);

    [Doc("Pulls all types out of namespace declarations into a flat compilation unit.")]
    void ExtractFromNamespaces();

    [Doc("Extracts interface definitions from all classes and writes IDocs.cs.")]
    void ExtractInterfaces();

    [Doc("Applies CSharpier formatting to the consolidated file in-place.")]
    void Format();

    [Doc("Builds a subdirectory path under the root.")]
    string GetSubDir(string subDir);

    [Doc("Groups members by kind, modifier, then name for deterministic ordering.")]
    void GroupByKindModifierName();

    [Doc("Moves non-extension static methods into a single helper class and groups extension methods by 'this' type.")]
    void GroupStaticExtensionMethods();

    [Doc("Loads a previously serialized tree by name/subdir.")]
    TreeNode<SemanticNode> LoadTree(string name, string subdir = null);

    [Doc("Loads a tree from a file inside the code output directory.")]
    TreeNode<SemanticNode> LoadTreeFromCodePath(string fileName);

    [Doc("Deserializes a semantic tree from any full path.")]
    TreeNode<SemanticNode> LoadTreeFromPath(string fullPath);

    [Doc("Loads the main consolidated tree for the current namespace.")]
    TreeNode<SemanticNode> LoadTreeMain();

    [Doc("Merges all .cs files from a directory into one compilation unit.")]
    void MergeIntoOneFile(string inDir = null, string outFile = null);

    [Doc("Reorders collection initializer values.")]
    void OrderCollectionValues();

    [Doc("Splits a consolidated file into one type per file.")]
    void SplitIntoOneTypePerFile(string inFile = null, string outDir = null);

    [Doc("Tests that the serializer can round-trip a file without changing its text.")]
    void TestSemanticSerializer();

    [Doc("Builds a full output filename given a base name and optional subdir.")]
    string ToNameFile(string name, string subdir = null);

    [Doc("Combines the supplied name with the root directory.")]
    string WithRootDir(string name);

    [Doc("Builds a region-tree representing the inheritance hierarchy of classes.")]
    List<TreeNode<SemanticNode>> BuildClassHierarchy(List<ClassNode> classes);

    [Doc("Constructs a map from base type to derived classes.")]
    Dictionary<string, List<TreeNode<SemanticNode>>> BuildDerivedMap(
        List<ClassNode> classes,
        Dictionary<string, TreeNode<SemanticNode>> byName
    );

    [Doc("Returns a path inside the code output directory.")]
    string GetCodePath(string fileName);

    [Doc("Generates a numbered test file path.")]
    string GetTestFilePath(int i);

    [Doc("Groups classes into a hierarchy using regions.")]
    void GroupByClassHierarchy();

    [Doc("Divides a large class into partial classes, one per implemented interface.")]
    void RefactorLargeClassIntoPartialsWithInterfaces(string className);
}

[ClassDoc("EnumMemberNode")]
[Doc("Represents a single member inside an enum.")]
interface IDocument_EnumMemberNode { }

[ClassDoc("EnumNode")]
[Doc("Semantic wrapper for enum declarations.")]
interface IDocument_EnumNode
{
    [Doc("Enum members.")]
    List<EnumMemberNode> Members { get; }

    [Doc("Attaches enum members.")]
    void AttachChild(SemanticNode child);

    [Doc("Returns EnumKeyword.")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("EventFieldNode")]
[Doc("Field-like event declaration (event SomeDelegate MyEvent;).")]
interface IDocument_EventFieldNode
{
    [Doc("Locates the name after the type but before the semicolon.")]
    void SetNameNode();
}

[ClassDoc("EventNode")]
[Doc("Event declaration with add/remove accessors.")]
interface IDocument_EventNode { }

[ClassDoc("ExpressionBodyNode")]
[Doc("Represents => expression-bodied member syntax.")]
interface IDocument_ExpressionBodyNode { }

[ClassDoc("ExtensionsMethodNode")]
[Doc("Utility extension for walking ancestors of a tree node.")]
interface IDocument_ExtensionsMethodNode
{
    [Doc("Yields all ancestors from parent up to the root.")]
    IEnumerable<TreeNode<SemanticNode>> Ancestors(TreeNode<SemanticNode> node);
}

[ClassDoc("ExtensionsOfAccessModifier")]
[Doc("Converts AccessModifier enum values to the corresponding SyntaxKind.")]
interface IDocument_ExtensionsOfAccessModifier
{
    [Doc("Maps Public/Protected/Internal/Private to their keyword tokens.")]
    SyntaxKind ToSyntaxKind(AccessModifier modifier);
}

[ClassDoc("ExtensionsOfAppDomain")]
[Doc("Helper to locate the solution root from the current AppDomain base directory.")]
interface IDocument_ExtensionsOfAppDomain
{
    [Doc("Walks upward until a directory containing .cs files is found.")]
    string GetSolutionBaseFolder(AppDomain domain);
}

[ClassDoc("ExtensionsOfIEnumerableOfIEnumerableOfT")]
[Doc("Set-like comparison utilities for collections of collections.")]
interface IDocument_ExtensionsOfIEnumerableOfIEnumerableOfT<T>
{
    [Doc("Reports how many times each distinct item appears across the supplied collections.")]
    IEnumerable<(T Item, int Appearances, List<int> Indices)> CompareCollections<T>(
        IEnumerable<IEnumerable<T>> collections
    );
}

[ClassDoc("ExtensionsOfIEnumerableOfT")]
[Doc("General LINQ-style extensions for IEnumerable<T>.")]
interface IDocument_ExtensionsOfIEnumerableOfT
{
    [Doc("Executes an action for each item (null-safe).")]
    void ForEach<T>(IEnumerable<T> source, Action<T> action);

    [Doc("Groups consecutive items that share the same predicate result.")]
    List<List<T>> GroupConsecutive<T>(IEnumerable<T> items, Func<T, bool> predicate);

    [Doc("Applies a chain of grouping predicates in order.")]
    IEnumerable<T> OrderByPredicates<T>(IEnumerable<T> items, List<Func<T, object>> predicates);
}

[ClassDoc("ExtensionsOfIEnumerableOfTreeNodeOfSemanticNode")]
[Doc("Tree-node specific LINQ extensions used throughout the framework.")]
interface IDocument_ExtensionsOfIEnumerableOfTreeNodeOfSemanticNode<T>
{
    [Doc("Deletes nodes of a given kind from a sequence.")]
    IEnumerable<TreeNode<SemanticNode>> DeleteKind(
        IEnumerable<TreeNode<SemanticNode>> en,
        SyntaxKind kind,
        DeleteType deleteType = DeleteType.SingleNode
    );

    [Doc("Deletes nodes whose kind is in the supplied set.")]
    IEnumerable<TreeNode<SemanticNode>> DeleteKinds(
        IEnumerable<TreeNode<SemanticNode>> en,
        HashSet<SyntaxKind> kinds,
        DeleteType deleteType = DeleteType.SingleNode
    );

    [Doc("Marks methods as private/protected based on usage when they appear in only one class.")]
    void MarkMethodAccessByUsage(TreeNode<SemanticNode> tree);

    [Doc("Returns candidates that do not exist in the existing collection (by FullName).")]
    List<TreeNode<SemanticNode>> NotPresentIn(
        IEnumerable<TreeNode<SemanticNode>> candidates,
        IEnumerable<TreeNode<SemanticNode>> existing
    );

    [Doc("Builds a dictionary keyed by FullName.")]
    Dictionary<string, TreeNode<SemanticNode>> ToDictionaryByFullName(
        IEnumerable<TreeNode<SemanticNode>> nodes
    );

    [Doc("Filters and casts the sequence to the requested typed list.")]
    List<T> ToTypedList<T>(IEnumerable<TreeNode<SemanticNode>> nodeList)
        where T : SemanticNode;
}

[ClassDoc("ExtensionsOfIEnumerableOfTSource")]
[Doc("Extensions for grouping where each key must appear exactly once.")]
interface IDocument_ExtensionsOfIEnumerableOfTSource<TSource, TKey>
{
    [Doc("Groups by key and asserts that each key appears exactly once, returning a dictionary.")]
    Dictionary<TKey, TSource> GroupBySingle<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector
    );
}

[ClassDoc("ExtensionsOfListOfT")]
[Doc("Additional List<T> utilities for grouping and interleaving.")]
interface IDocument_ExtensionsOfListOfT<T>
{
    [Doc("Recursively groups a list by a sequence of predicates.")]
    List<(List<object> keys, List<T> vals)> GroupByPredicates<T>(
        List<T> list,
        List<Func<T, object>> predicates
    );

    [Doc("Stably interleaves movable items (those with a non-null key) into their original positions after sorting them.")]
    List<T> Interleave<T, TKey>(List<T> source, Func<T, TKey?> getKey)
        where TKey : IComparable<TKey>;
}

[ClassDoc("ExtensionsOfListOfTreeNodeOfSemanticNode")]
[Doc("List-specific helpers for grouping, wrapping, and comparing semantic trees.")]
interface IDocument_ExtensionsOfListOfTreeNodeOfSemanticNode
{
    [Doc("Compares multiple trees by counting named nodes (used for debugging).")]
    void CompareObjectCount(List<TreeNode<SemanticNode>> treeNodes);

    [Doc("Creates a new partial class that implements the given interface and moves the supplied methods into it.")]
    TreeNode<SemanticNode> CreateNewPartialClassForInterface(
        List<TreeNode<SemanticNode>> methodsToMove,
        string className,
        string interfaceName
    );

    [Doc("Finds text tokens delimited by the supplied delimiter kinds.")]
    IEnumerable<TreeNode<SemanticNode>> FindDelimitedText(
        List<TreeNode<SemanticNode>> roots,
        HashSet<SyntaxKind> delimeters
    );

    [Doc("Groups children first by kind then by modifier.")]
    List<List<TreeNode<SemanticNode>>> GroupByKindModifier(List<TreeNode<SemanticNode>> children);

    [Doc("Changes private extension methods to public so they can be called from outside the static class.")]
    void SetStaticExtensionMethodsPublic(List<TreeNode<SemanticNode>> extensionMethods);

    [Doc("Sorts children by (Kind, Modifier, Name).")]
    List<TreeNode<SemanticNode>> SortedByKindModifierName(List<TreeNode<SemanticNode>> children);

    [Doc("Wraps a list of methods inside a fresh class/struct declaration.")]
    TreeNode<SemanticNode> WrapInDeclaration(
        List<TreeNode<SemanticNode>> methods,
        string declaration
    );

    [Doc("Internal helper that extracts text tokens stopping at the supplied delimiter set.")]
    IEnumerable<TreeNode<SemanticNode>> FindText(
        List<TreeNode<SemanticNode>> roots,
        HashSet<SyntaxKind> include,
        HashSet<SyntaxKind> stopAt
    );

    [Doc("Extracts trimmed text from each node.")]
    List<string> GetStrings(List<TreeNode<SemanticNode>> en);
}

[ClassDoc("ExtensionsOfListOfTreeNodeOfT")]
[Doc("Generic recursive search helpers that support skipping and stopping predicates.")]
interface IDocument_ExtensionsOfListOfTreeNodeOfT
{
    [Doc("Recursively finds nodes satisfying predicate while skipping nodes that match skipPredicate.")]
    IEnumerable<TreeNode<T>> FindSkip<T>(
        List<TreeNode<T>> roots,
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> skipPredicate
    )
        where T : BaseNode<T>;

    [Doc("Like FindSkip but also stops recursion when stopPredicate returns true.")]
    IEnumerable<TreeNode<T>> FindSkipStop<T>(
        List<TreeNode<T>> roots,
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> skipPredicate,
        Func<TreeNode<T>, bool> stopPredicate
    )
        where T : BaseNode<T>;

    [Doc("Depth-first search returning all nodes where predicate is true.")]
    IEnumerable<TreeNode<T>> FindWhere<T>(
        List<TreeNode<T>> roots,
        Func<TreeNode<T>, bool> predicate
    )
        where T : BaseNode<T>;

    [Doc("Finds nodes while stopping recursion at nodes that match untilPredicate.")]
    IEnumerable<TreeNode<T>> FindWhereStop<T>(
        List<TreeNode<T>> roots,
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> untilPredicate
    )
        where T : BaseNode<T>;
}

[ClassDoc("ExtensionsOfstring")]
[Doc("String utilities used when converting between text and trees.")]
interface IDocument_ExtensionsOfstring
{
    [Doc("Normalizes all line endings to Windows style (CRLF).")]
    string NormalizeLineEndings(string text);

    [Doc("Parses a string into a fresh semantic tree (convenience helper).")]
    TreeNode<SemanticNode> ToTree(string code);
}

[ClassDoc("ExtensionsOfSyntaxKind")]
[Doc("Maps SyntaxKind values to ordering enums and to AccessModifier.")]
interface IDocument_ExtensionsOfSyntaxKind
{
    [Doc("Converts a SyntaxKind into the ordering category used for stable sorting.")]
    SyntaxKindOrdering ToHandled(SyntaxKind kind);

    [Doc("Converts a modifier keyword into the corresponding AccessModifier value.")]
    AccessModifier ToModifier(SyntaxKind kind);
}

[ClassDoc("ExtensionsOfSyntaxNode")]
[Doc("Bridges Roslyn SyntaxNode to the custom TokenNode / SyntaxElementNode / SemanticNode tree.")]
interface IDocument_ExtensionsOfSyntaxNode
{
    [Doc("Full conversion pipeline: SyntaxNode → Token tree → SyntaxElement tree → Semantic tree.")]
    TreeNode<SemanticNode> ToTreeSemanticNode(SyntaxNode node);

    [Doc("Builds a TokenNode tree that mirrors the Roslyn syntax tree structure.")]
    TreeNode<TokenNode> ToTreeTokenNode(SyntaxNode node);
}

[ClassDoc("ExtensionsOfT")]
[Doc("Simple generic wrapper extensions (AsList, AsArray, AsQueue).")]
interface IDocument_ExtensionsOfT<T>
{
    [Doc("Wraps a single item in a new List<T>.")]
    List<T> AsList<T>(T item);

    [Doc("Wraps a single item in a new array.")]
    T[] AsArray<T>(T item);

    [Doc("Wraps a single item in a new Queue<T>.")]
    Queue<T> AsQueue<T>(T item);
}

[ClassDoc("ExtensionsOfTreeNodeOfSemanticNode")]
[Doc("Core tree manipulation library – merging, grouping, formatting, interface extraction, partial-class refactoring, etc.")]
interface IDocument_ExtensionsOfTreeNodeOfSemanticNode<T>
{
    [Doc("Builds a #region tree representing an inheritance hierarchy.")]
    TreeNode<SemanticNode> BuildRegionTree(
        TreeNode<SemanticNode> classNode,
        Dictionary<string, List<TreeNode<SemanticNode>>> derivedMap,
        Dictionary<string, TreeNode<SemanticNode>> byName
    );

    [Doc("Deletes all nodes of a given kind from the tree.")]
    IEnumerable<TreeNode<SemanticNode>> DeleteKind(
        TreeNode<SemanticNode> tree,
        SyntaxKind kind,
        DeleteType deleteType = DeleteType.SingleNode
    );

    [Doc("Deletes nodes whose kind is in the supplied set.")]
    List<TreeNode<SemanticNode>> DeleteKinds(
        TreeNode<SemanticNode> tree,
        HashSet<SyntaxKind> kinds,
        DeleteType deleteType = DeleteType.SingleNode
    );

    [Doc("Internal iterator that yields and deletes nodes matching the kind set.")]
    IEnumerable<TreeNode<SemanticNode>> DeleteKindsSeek(
        TreeNode<SemanticNode> tree,
        HashSet<SyntaxKind> kinds,
        DeleteType deleteType = DeleteType.SingleNode
    );

    [Doc("Splits a large class into multiple partial classes, one per implemented interface.")]
    void DivideIntoPartialClassesByInterfaces(
        TreeNode<SemanticNode> tree,
        string className,
        string interfaceName = null
    );

    [Doc("Moves members out of namespace declarations into a flat compilation unit.")]
    void ExtractFromNamespaces(
        TreeNode<SemanticNode> root,
        List<TreeNode<SemanticNode>> usings,
        TreeNode<SemanticNode> ns
    );

    [Doc("Finds text tokens delimited by the supplied set of delimiter kinds.")]
    IEnumerable<TreeNode<SemanticNode>> FindDelimitedText(
        TreeNode<SemanticNode> root,
        HashSet<SyntaxKind> delimeters
    );

    [Doc("Returns all nodes of the exact given kind.")]
    List<TreeNode<SemanticNode>> FindKind(TreeNode<SemanticNode> tree, SyntaxKind kind);

    [Doc("Returns all nodes whose kind is in the supplied set.")]
    List<TreeNode<SemanticNode>> FindKinds(TreeNode<SemanticNode> tree, HashSet<SyntaxKind> kinds);

    [Doc("Finds an identifier token that appears after a type but before any stop kinds.")]
    TreeNode<SemanticNode> FindNameAfterType(
        TreeNode<SemanticNode> tree,
        HashSet<SyntaxKind> stopKinds
    );

    [Doc("Convenience version that stops before a semicolon.")]
    TreeNode<SemanticNode> FindNameAfterTypeBeforeSemicolon(TreeNode<SemanticNode> tree);

    [Doc("Returns nodes that do NOT have any of the supplied kinds.")]
    List<TreeNode<SemanticNode>> FindNotKinds(
        TreeNode<SemanticNode> tree,
        HashSet<SyntaxKind> kinds
    );

    [Doc("Formats the tree using CSharpier and returns a new tree from the formatted text.")]
    TreeNode<SemanticNode> Format(TreeNode<SemanticNode> tree);

    [Doc("Retrieves all ClassNode instances in the tree.")]
    List<ClassNode> GetClasses(TreeNode<SemanticNode> tree);

    [Doc("Returns the name of an interface declaration node.")]
    string GetInterfaceName(TreeNode<SemanticNode> interfaceDecl);

    [Doc("Returns all MethodNode instances in the tree.")]
    List<MethodNode> GetMethods(TreeNode<SemanticNode> tree);

    [Doc("Collects all NamedNode descendants (excluding attributes).")]
    List<NamedNode> GetNamedNodeList(TreeNode<SemanticNode> tree);

    [Doc("Filters usings to those starting with Microsoft or System.")]
    List<UsingDirectiveNode> GetSystemUsings(TreeNode<SemanticNode> tree);

    [Doc("Generic typed filter for any SemanticNode subtype.")]
    List<T> GetTypedList<T>(TreeNode<SemanticNode> tree);

    [Doc("Typed filter restricted to nodes whose kind is in the supplied set.")]
    List<T> GetTypedListKinds<T>(TreeNode<SemanticNode> tree, HashSet<SyntaxKind> kinds);

    [Doc("Groups children by (Kind, Modifier, Name) and returns both the tree and the groups.")]
    (TreeNode<SemanticNode> Tree, List<List<TreeNode<SemanticNode>>> Groups) GroupByKindModifierName(TreeNode<SemanticNode> tree);

    [Doc("Recursively groups consecutive children that satisfy the predicate.")]
    TreeNode<SemanticNode> GroupConsecutiveChildren(
        TreeNode<SemanticNode> node,
        Func<TreeNode<SemanticNode>, bool> predicate
    );

    [Doc("Groups static extension methods into separate static helper classes named ExtensionsOfXXX.")]
    void GroupStaticExtensionMethods(TreeNode<SemanticNode> root);

    [Doc("Initializes the semantic view on every node in the subtree.")]
    void InitializeAllNodes(TreeNode<SemanticNode> node);

    [Doc("Merges another tree into this one, optionally replacing existing members.")]
    TreeNode<SemanticNode> MergeFrom(
        TreeNode<SemanticNode> destination,
        TreeNode<SemanticNode> source,
        bool replace = true
    );

    [Doc("Merges duplicate containers at every depth within the same tree.")]
    TreeNode<SemanticNode> MergeSelf(TreeNode<SemanticNode> source);

    [Doc("Moves non-extension static methods from static classes into a single StaticHelpers class.")]
    void MoveNonExtensionStaticMethodsInStaticClassesIntoSingleClass(TreeNode<SemanticNode> root);

    [Doc("Reorders values inside collection initializers.")]
    TreeNode<SemanticNode> OrderCollectionValues(TreeNode<SemanticNode> originalTree);

    [Doc("Removes namespace wrappers, pulling their children to the root.")]
    void PullChildrenOutOfNamespaces(TreeNode<SemanticNode> root);

    [Doc("Reformats the tree with CSharpier and reloads it as a new semantic tree.")]
    TreeNode<SemanticNode> ReloadFormatted(TreeNode<SemanticNode> node);

    [Doc("Serializes the tree to the supplied file path.")]
    void SaveFile(TreeNode<SemanticNode> node, string filePath);

    [Doc("Serializes the tree to a string (concatenates all node text).")]
    string Serialize(TreeNode<SemanticNode>? root);

    [Doc("Writes each top-level type to its own file inside the target directory.")]
    void SplitIntoOneTypePerFile(TreeNode<SemanticNode> tree, string dir);

    [Doc("Compiles the tree (optionally writing it to disk first) and throws on error.")]
    bool TestCompile(TreeNode<SemanticNode> node, string projectPath, string savePath = null);

    [Doc("Returns the concatenated text of the tree.")]
    string ToCode(TreeNode<SemanticNode> node);

    [Doc("Writes the tree to a file (no formatting).")]
    void ToFile(TreeNode<SemanticNode> node, string filePath);

    [Doc("Deletes excess newlines, formats with CSharpier, and writes the file.")]
    void ToFormattedFile(TreeNode<SemanticNode> node, string filePath);

    [Doc("Converts an IEnumerable of tree nodes into an ordered typed list.")]
    List<T> ToTypedList<T>(IEnumerable<TreeNode<SemanticNode>> tree);

    [Doc("Adds containers that exist in source but are missing from destination at a specific depth.")]
    void AddMissingContainersAtDepth(
        TreeNode<SemanticNode> destination,
        TreeNode<SemanticNode> source,
        int depth
    );

    [Doc("Adds all missing containers from source to destination by walking depth 0..maxDepth.")]
    void AddMissingContainersByDepthFrom(
        TreeNode<SemanticNode> destination,
        TreeNode<SemanticNode> source
    );

    [Doc("Adds newly discovered lowest-level named nodes from source into destination.")]
    void AddNewLowestLevelTypesFrom(
        TreeNode<SemanticNode> destination,
        List<TreeNode<SemanticNode>> source
    );

    [Doc("Adds a member to a parent's MembersNode.")]
    void AddNewMember(TreeNode<SemanticNode> parent, TreeNode<SemanticNode> member);

    [Doc("Checks whether a class already contains the partial keyword.")]
    bool AlreadyHasPartialKeyword(TreeNode<SemanticNode> classObj);

    [Doc("Returns a list of the code for every node in the subtree.")]
    List<string> AsCode<T>(TreeNode<SemanticNode> tree);

    [Doc("Counts non-blank lines of code in the tree.")]
    int CountLinesOfCode(TreeNode<SemanticNode> tree);

    [Doc("Counts the number of method declarations in the subtree.")]
    int CountMethods(TreeNode<SemanticNode> tree);

    [Doc("Counts static extension methods in the tree.")]
    int CountStaticExtensionMethods(TreeNode<SemanticNode> tree);

    [Doc("Deletes all comment trivia nodes.")]
    void DeleteComments<T>(TreeNode<SemanticNode> root);

    [Doc("Removes duplicate CompilationUnit nodes (keeps only the first).")]
    void DeleteCompilationUnit(TreeNode<SemanticNode> tree);

    [Doc("Removes extra consecutive newlines, leaving at most one blank line.")]
    TreeNode<SemanticNode> DeleteExcessNewlines(TreeNode<SemanticNode> root);

    [Doc("Creates a partial class for a single interface and moves implementing methods into it.")]
    void DivideIntoPartialClassByInterface(
        TreeNode<SemanticNode> tree,
        TreeNode<SemanticNode> classObj,
        TreeNode<SemanticNode> interfaceDecl
    );

    [Doc("Replaces method bodies with 'throw new NotImplementedException();' and writes the result.")]
    void EmptyMemberDeclarationsContents(TreeNode<SemanticNode> tree, EntryPoint path);

    [Doc("Strips member bodies from type declarations (used for skeleton generation).")]
    void EmptyTypeDeclarationsContents(TreeNode<SemanticNode> tree, EntryPoint path);

    [Doc("Ensures a class declaration contains the partial keyword.")]
    void EnsureClassIsMarkedPartial(TreeNode<SemanticNode> classObj);

    [Doc("Removes namespace declarations, moving their children to the root.")]
    TreeNode<SemanticNode> ExtractFromAndRemoveNamespaces(TreeNode<SemanticNode> tree);

    [Doc("Returns all class declaration nodes.")]
    List<TreeNode<SemanticNode>> FindClassNodes(TreeNode<SemanticNode> node);

    [Doc("Finds methods in a class that implement members of a given interface.")]
    List<TreeNode<SemanticNode>> FindImplementingMethodsInClass(
        TreeNode<SemanticNode> classObj,
        HashSet<string> interfaceMethodNames
    );

    [Doc("Returns all InterfaceNode instances.")]
    List<InterfaceNode> FindInterfaces(TreeNode<SemanticNode> tree);

    [Doc("Returns the SemanticNode values for nodes of a given kind.")]
    List<SemanticNode> FindKindVal(TreeNode<SemanticNode> tree, SyntaxKind kind);

    [Doc("Locates a parent node by its FullName (skipping using directives).")]
    TreeNode<SemanticNode> FindParentByFullName(TreeNode<SemanticNode> root, string parentFullName);

    [Doc("Returns all static classes in the tree.")]
    List<ClassNode> FindStaticClasses(TreeNode<SemanticNode> tree);

    [Doc("Returns tree nodes for static extension methods (methods that are static and have a 'this' parameter).")]
    List<TreeNode<SemanticNode>> FindStaticExtensionMethods(TreeNode<SemanticNode> tree);

    [Doc("Finds nodes of a single kind.")]
    IEnumerable<TreeNode<SemanticNode>> FindSyntaxKind(
        TreeNode<SemanticNode> node,
        SyntaxKind kind
    );

    [Doc("Finds nodes whose kind is in the supplied set.")]
    IEnumerable<TreeNode<SemanticNode>> FindSyntaxKinds(
        TreeNode<SemanticNode> node,
        HashSet<SyntaxKind> kinds
    );

    [Doc("Returns pairs of (code, node) for every node in the tree.")]
    List<(string code, TreeNode<SemanticNode> node)> FlattenToCode(TreeNode<SemanticNode> tree);

    [Doc("Builds a full derivation map for every class in the tree.")]
    Dictionary<string, List<TreeNode<SemanticNode>>> GetDerivedClasses(TreeNode<SemanticNode> tree);

    [Doc("Returns the lowest-level named nodes (nodes with no other named descendants).")]
    List<TreeNode<SemanticNode>> GetLowestLevelTypes(TreeNode<SemanticNode> root);

    [Doc("Maximum number of parent containers any named node has in the tree.")]
    int GetMaxParentCount(TreeNode<SemanticNode> root);

    [Doc("Collects method names declared inside an interface.")]
    HashSet<string> GetMethodNamesDeclaredInInterface(TreeNode<SemanticNode> interfaceDecl);

    [Doc("Returns all nodes that have a non-empty TypeName.")]
    IEnumerable<TreeNode<SemanticNode>> GetNamedNodes(TreeNode<SemanticNode> root);

    [Doc("Filters named nodes to those at a specific parent depth.")]
    IEnumerable<TreeNode<SemanticNode>> GetNamedNodesAtDepth(
        TreeNode<SemanticNode> root,
        int depth
    );

    [Doc("Collects static non-extension methods that live inside static classes.")]
    List<TreeNode<SemanticNode>> GetNonExtensionMethodsInExtensionClass(
        TreeNode<SemanticNode> root
    );

    [Doc("Returns all using directives in the tree.")]
    List<UsingDirectiveNode> GetUsings(TreeNode<SemanticNode> tree);

    [Doc("Inserts a 'partial' keyword before the class keyword.")]
    void InsertPartialKeywordBefore(
        TreeNode<SemanticNode> classObj,
        TreeNode<SemanticNode> classKeyword
    );

    [Doc("Merges duplicate containers at a specific depth inside the tree.")]
    void MergeAtDepth(TreeNode<SemanticNode> source, int index);

    [Doc("Merges a list of source trees into the destination tree.")]
    TreeNode<SemanticNode> MergeFromList(
        TreeNode<SemanticNode> destination,
        List<TreeNode<SemanticNode>> sources
    );

    [Doc("Merges multiple partial classes with the same name into a single node.")]
    TreeNode<SemanticNode> MergePartials(TreeNode<SemanticNode> tree, string className);

    [Doc("Moves the cursor (returns first child of the given kind).")]
    TreeNode<SemanticNode> MoveCursorTo(TreeNode<SemanticNode> treeNode, SyntaxKind kind);

    [Doc("Reloads the tree from its own serialized text (round-trip).")]
    TreeNode<SemanticNode> Reload(TreeNode<SemanticNode> node);

    [Doc("Removes empty static classes that contain no methods.")]
    void RemoveEmptyClasses(TreeNode<SemanticNode> node);

    [Doc("Applies a replace function to all namespace and using names.")]
    void RenameNamespaces(TreeNode<SemanticNode> tree, Func<string, string> replace);

    [Doc("Replaces existing lowest-level nodes with those from source; returns the new ones that were added.")]
    List<TreeNode<SemanticNode>> ReplaceExistingLowestLevelTypesFrom(
        TreeNode<SemanticNode> destination,
        TreeNode<SemanticNode> source,
        bool replace = true
    );

    [Doc("Writes the tree to a file (alias for SerializeFile).")]
    void SerializeFile(TreeNode<SemanticNode> node, string filePath);

    [Doc("Decides whether a class is large enough to be split into partial classes (method count or LOC).")]
    bool ShouldDivideLargeClass(TreeNode<SemanticNode> tree);
}

[ClassDoc("ExtensionsOfTreeNodeOfSyntaxElementNode")]
[Doc("Converts a syntax-element tree into a fully initialized semantic tree.")]
interface IDocument_ExtensionsOfTreeNodeOfSyntaxElementNode
{
    [Doc("Builds semantic tree from syntax element tree and initializes all nodes.")]
    TreeNode<SemanticNode> ToTreeSemanticNode(TreeNode<SyntaxElementNode> syntaxRoot);

    [Doc("Recursively converts SyntaxElementNode tree into SemanticNode tree, attaching children.")]
    TreeNode<SemanticNode> BuildTree(TreeNode<SyntaxElementNode> syntaxNode);
}

[ClassDoc("ExtensionsOfTreeNodeOfTokenNode")]
[Doc("Converts a Roslyn token tree into a syntax-element tree (handles trivia).")]
interface IDocument_ExtensionsOfTreeNodeOfTokenNode
{
    [Doc("Full conversion from TokenNode tree to SyntaxElementNode tree.")]
    TreeNode<SyntaxElementNode> ToTreeSyntaxElement(TreeNode<TokenNode>? tokenRoot);

    [Doc("Builds a flat list of syntax elements from leading trivia, main token, and trailing trivia, then links them.")]
    TreeNode<SyntaxElementNode> ToSyntaxElementTree(TreeNode<TokenNode> tokenNode);
}

[ClassDoc("ExtractInterfaceExtensions")]
[Doc("Generates interface definitions from concrete classes, including attributes and signature normalization for properties/methods.")]
interface IDocument_ExtractInterfaceExtensions
{
    [Doc("Main entry point: deletes existing attributes, builds a new compilation unit containing an interface for each class.")]
    TreeNode<SemanticNode> ExtractInterfaces(TreeNode<SemanticNode> root);

    [Doc("Builds method signatures suitable for an interface (no body, no illegal modifiers).")]
    List<string> BuildAllMethodSignatures(ClassNode classNode);

    [Doc("Builds property signatures for an interface (getter/setter only, no modifiers).")]
    List<string> BuildAllPropertySignatures(ClassNode classNode);

    [Doc("Constructs the complete interface source text from name, generics, constraints, properties and methods.")]
    string BuildInterfaceSource(
        string interfaceName,
        string generics,
        string whereClause,
        List<string> propertySigs,
        List<string> methodSigs
    );

    [Doc("Converts a property node into an interface-compatible signature (removes body, adds accessors).")]
    string BuildSinglePropertySignature(PropertyNode propNode);

    [Doc("Ensures a method ends with a semicolon (required for interface declarations).")]
    void EnsureMethodSemicolon(MethodNode method);

    [Doc("Extracts an interface from a class, including generics and where clauses.")]
    TreeNode<SemanticNode> ExtractInterface(ClassNode classNode);

    [Doc("Adds [ClassDoc] to the generated interface and [Doc] to each member.")]
    TreeNode<SemanticNode> ExtractInterfaceWithAttributes(ClassNode classNode);

    [Doc("Returns the type parameter list text (e.g. <T>).")]
    string GetGenericParameters(ClassNode classNode);

    [Doc("Returns the interface name derived from the class (IDocument_ + class name).")]
    string GetInterfaceName(ClassNode classNode);

    [Doc("Converts a method into an interface signature (removes body, static/extension modifiers, etc.).")]
    string GetInterfaceSignature(MethodNode methodNode);

    [Doc("Extracts where constraints that appear before the opening brace.")]
    string GetWhereClause(ClassNode classNode);

    [Doc("Removes modifiers that are illegal on interface members.")]
    void RemoveInterfaceIllegalModifiers(MethodNode method);

    [Doc("Deletes the method body or arrow expression (for interface extraction).")]
    void RemoveMethodBody(MethodNode method);

    [Doc("Strips leading access/modifier keywords from a member header.")]
    string StripModifiers(string header);
}

[ClassDoc("FieldNode")]
[Doc("Semantic wrapper for field declarations.")]
interface IDocument_FieldNode
{
    [Doc("Locates the field name after its type but before the semicolon.")]
    void SetNameNode();
}

[ClassDoc("FileGen")]
[Doc("Generic round-trip serializer tester for any ISerializeCode<T> implementation.")]
interface IDocument_FileGen<T>
    where T : BaseNode<T>
{
    [Doc("Runs a full round-trip on a file and throws if the text changes.")]
    (bool success, string newCode, TreeNode<T> tree) TestRoundTripFile(string file);

    [Doc("Placeholder for project-to-code loading (not implemented).")]
    string ProjectLoadToCode(string projectFile);

    [Doc("Placeholder for project-to-file loading (not implemented).")]
    string ProjectLoadToFile(string projectFile, string outputFile);

    [Doc("Placeholder for full project round-trip test (not implemented).")]
    void ProjectRoundTripTest(string projectFile);

    [Doc("Deserializes code, serializes it back, and returns both the new text and the tree.")]
    (string newCode, TreeNode<T> tree) RoundTripCode(string code);

    [Doc("Performs a round-trip on a file and returns success flag, new text, and resulting tree.")]
    (bool success, string newCode, TreeNode<T> tree) RoundTripFile(string file);

    [Doc("Throws if the regenerated code differs from the original.")]
    string TestRoundTrip(string code);
}

[ClassDoc("FileScopedNamespaceNode")]
[Doc("Semantic wrapper for C# 10+ file-scoped namespace declarations.")]
interface IDocument_FileScopedNamespaceNode { }

[ClassDoc("IdentifierTokenNode")]
[Doc("Specialized node for identifier tokens that extracts and caches the text.")]
interface IDocument_IdentifierTokenNode
{
    [Doc("Trimmed text of the identifier token.")]
    string IdentifierText { get; set; }

    [Doc("Always returns null (identifiers do not have a TypeName in this model).")]
    string? TypeName { get; }

    [Doc("Initializes IdentifierText from the underlying syntax element.")]
    void SetTreeNode(TreeNode<SemanticNode> tree);

    [Doc("Returns the identifier text when converted to string.")]
    string ToString();

    [Doc("Extracts and trims the token text into IdentifierText.")]
    void ExtractText();
}

[ClassDoc("ImplementTBD")]
[Doc("Custom exception used as a placeholder for unimplemented functionality.")]
interface IDocument_ImplementTBD { }

[ClassDoc("IndexerNode")]
[Doc("Semantic wrapper for indexer (this[...]) declarations.")]
interface IDocument_IndexerNode
{
    [Doc("Indexers do not use a conventional name node; this method is a no-op.")]
    void SetNameNode();
}

[ClassDoc("InterfaceNode")]
[Doc("Semantic wrapper for interface declarations.")]
interface IDocument_InterfaceNode
{
    [Doc("Returns InterfaceKeyword.")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("InvocationExpressionNode")]
[Doc("Rich semantic model for invocation expressions that parses receiver, method name, arguments, generics, and extension-method usage.")]
interface IDocument_InvocationExpressionNode
{
    [Doc("Parsed argument list node (if present).")]
    ArgumentListNode? ArgumentList { get; set; }

    [Doc("Convenience accessor for the arguments inside the argument list.")]
    List<ArgumentNode> Arguments { get; }

    [Doc("Type argument list (generics) used in the call.")]
    TreeNode<SemanticNode>? GenericTypeArgumentList { get; set; }

    [Doc("The expression that is being invoked (may be a member access).")]
    TreeNode<SemanticNode>? InvokedExpression { get; set; }

    [Doc("True when the call uses a 'this' argument (extension method syntax).")]
    bool IsExtensionMethodCall { get; set; }

    [Doc("True for calls with no receiver (static method call).")]
    bool IsStaticCall { get; set; }

    [Doc("Name of the method being called.")]
    string? MethodName { get; set; }

    [Doc("Receiver expression before the dot (for instance calls).")]
    TreeNode<SemanticNode>? Receiver { get; set; }

    [Doc("Total number of arguments, counting the implicit 'this' for extension methods.")]
    int TotalArgumentCount { get; }

    [Doc("Attaches an ArgumentListNode child.")]
    void AttachChild(SemanticNode child);

    [Doc("Parses the invocation on node attachment.")]
    void SetTreeNode(TreeNode<SemanticNode> tree);

    [Doc("Returns a compact description of the invocation.")]
    string ToString();

    [Doc("Detects whether any argument uses the 'this' keyword.")]
    void DetermineExtensionMethodCall();

    [Doc("Decides whether the call is a static call (no receiver and not an extension).")]
    void DetermineStaticCall();

    [Doc("Locates the invoked expression (everything except the argument list).")]
    void FindInvokedExpression();

    [Doc("Builds a human-readable summary including receiver, method name, generics, and argument count.")]
    string GetRichDescription();

    [Doc("Checks whether a node resides inside a type argument list (to avoid mis-identifying generic names).")]
    bool IsInsideTypeArgumentList(TreeNode<SemanticNode> node);

    [Doc("Orchestrates the full parsing of receiver, name, generics, and call style.")]
    void ParseInvocation();

    [Doc("Finds a type argument list inside the invoked expression.")]
    void ResolveGenericArguments();

    [Doc("Extracts the method name from the invoked expression, ignoring tokens inside type argument lists.")]
    void ResolveMethodName();

    [Doc("Extracts the receiver from a member access expression.")]
    void ResolveReceiver();
}

[ClassDoc("KnownProjectPaths")]
[Doc("Static factory providing preconfigured EntryPoint instances for known projects.")]
interface IDocument_KnownProjectPaths
{
    [Doc("EntryPoint configured for the Evolver5 project.")]
    EntryPoint Evolver5 { get; }
}

[ClassDoc("MethodNode")]
[Doc("Semantic wrapper for method declarations with rich support for extension methods, static conversion, parameters, and modifiers.")]
interface IDocument_MethodNode
{
    [Doc("Full name including parameter types (used for uniqueness).")]
    string FullName { get; }

    [Doc("True when the method has a 'this' parameter as its first argument.")]
    bool IsExtension { get; }

    [Doc("True when the method carries the static keyword.")]
    bool IsStatic { get; }

    [Doc("True when the method is abstract, virtual, or override.")]
    bool IsVirtual { get; }

    [Doc("Name combined with a normalized parameter signature string.")]
    string NameWithParameters { get; }

    [Doc("Underscore-joined list of parameter type names.")]
    string Parameters { get; }

    [Doc("List of parameter type names (for naming generated extension classes).")]
    List<string> ParameterTypeNames { get; }

    [Doc("Converts a static extension method back to a normal instance method.")]
    void ConvertFromStaticExtension();

    [Doc("Converts a normal method into a static extension method (adds static and this).")]
    void ConvertToStaticExtension();

    [Doc("Returns a generated helper class name like ExtensionsOfSomeType.")]
    string GetStaticClassName();

    [Doc("Locates the method name token, skipping modifiers, return type, and attributes.")]
    void SetNameNode();

    [Doc("Ensures the first parameter has a 'this' keyword (for extension methods).")]
    void EnsureFirstParameterIsThis();

    [Doc("Inserts the static keyword if the method is not already static.")]
    void EnsureStatic();

    [Doc("Locates the return-type node before the method name.")]
    TreeNode<SemanticNode>? GetReturnTypeNode();

    [Doc("Extracts a cleaned type name from the first 'this' parameter (used for naming extension helper classes).")]
    string GetThisParameterName();

    [Doc("Builds a list of parameter type names from the parameter list.")]
    List<string> ParseParameterTypeNames();

    [Doc("Removes the static keyword token.")]
    void RemoveStaticKeyword();

    [Doc("Removes the 'this' keyword from the first parameter.")]
    void RemoveThisKeywordFromFirstParameter();
}

[ClassDoc("NamedMemberNode")]
[Doc("Base for members that can have an access modifier (fields, properties, methods, etc.).")]
interface IDocument_NamedMemberNode
{
    [Doc("Access modifier of the member (defaults to Private when not specified).")]
    AccessModifier Modifier { get; set; }

    [Doc("Default modifier used when none is present on the syntax tree.")]
    AccessModifier DefaultModifier { get; }

    [Doc("Initializes modifier from the syntax tree on node attachment.")]
    void SetTreeNode(TreeNode<SemanticNode> tree);

    [Doc("Locates the first modifier keyword in the member.")]
    void SetModifierFromTree();

    [Doc("Sets or inserts an access modifier keyword at the start of the member.")]
    TreeNode<SemanticNode> SetModifier(AccessModifier modifier);
}

[ClassDoc("NamedNode")]
[Doc("Base class for any node that possesses a name (types, members, parameters, etc.).")]
interface IDocument_NamedNode
{
    [Doc("Collects identifier tokens until a delimiter and sets them as the name.")]
    void SetName(HashSet<SyntaxKind> endDelimeters);

    [Doc("Default implementation that finds the first identifier token (skipping attributes).")]
    void SetNameNode();

    [Doc("Initializes the name node when the tree node is attached.")]
    void SetTreeNode(TreeNode<SemanticNode> t);
}

[ClassDoc("NamespaceNode")]
[Doc("Semantic wrapper for namespace declarations (both block-scoped and file-scoped).")]
interface IDocument_NamespaceNode
{
    [Doc("Members declared inside the namespace.")]
    List<SemanticNode> Members { get; }

    [Doc("Using directives that appeared inside the namespace.")]
    List<UsingDirectiveNode> Usings { get; }

    [Doc("Default access for namespaces is Public.")]
    AccessModifier DefaultModifier { get; }

    [Doc("Attaches either usings or members.")]
    void AttachChild(SemanticNode child);

    [Doc("Factory that creates a namespace node from a name string.")]
    TreeNode<SemanticNode> Factory(string name);

    [Doc("Parses the namespace name by collecting tokens until the opening brace.")]
    void SetNameNode();

    [Doc("Returns NamespaceKeyword (or FileScopedNamespaceDeclaration for the other variant).")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("OperatorNode")]
[Doc("Semantic wrapper for user-defined operator overloads.")]
interface IDocument_OperatorNode
{
    [Doc("Locates the operator token (e.g. +, ==, implicit, etc.).")]
    void SetNameNode();

    [Doc("Finds the operator token before the parameter list.")]
    TreeNode<SemanticNode> FindOperatorToken();

    [Doc("Predicate that recognizes operator tokens.")]
    bool IsOperatorToken(TreeNode<SemanticNode> t);
}

[ClassDoc("ParameterizedMemberNode")]
[Doc("Base for members that accept a parameter list (methods, constructors, indexers, operators).")]
interface IDocument_ParameterizedMemberNode
{
    [Doc("The parameter list child node.")]
    ParameterListNode? ParameterList { get; set; }

    [Doc("Attaches a ParameterListNode child.")]
    void AttachChild(SemanticNode child);
}

[ClassDoc("ParameterizedMemberWithBodyNode")]
[Doc("Extends ParameterizedMemberNode with a method body (BlockNode or expression body).")]
interface IDocument_ParameterizedMemberWithBodyNode
{
    [Doc("The body of the member (block or arrow expression).")]
    BlockNode? Body { get; set; }

    [Doc("Attaches either a parameter list or a body.")]
    void AttachChild(SemanticNode child);
}

[ClassDoc("ParameterListNode")]
[Doc("Container for the parameters of a method, constructor, delegate, etc.")]
interface IDocument_ParameterListNode
{
    [Doc("List of ParameterNode children.")]
    List<ParameterNode> Parameters { get; }

    [Doc("Attaches ParameterNode children.")]
    void AttachChild(SemanticNode child);
}

[ClassDoc("ParameterNode")]
[Doc("Represents a single method/constructor parameter, including modifiers (ref, out, this, etc.) and type.")]
interface IDocument_ParameterNode
{
    [Doc("True when the parameter has the 'in' modifier.")]
    bool IsIn { get; }

    [Doc("True when the parameter has the 'out' modifier.")]
    bool IsOut { get; }

    [Doc("True when the parameter has the 'params' modifier.")]
    bool IsParams { get; }

    [Doc("True when the parameter has the 'ref' modifier.")]
    bool IsRef { get; }

    [Doc("True when the parameter has the 'this' modifier (extension method).")]
    bool IsThis { get; }

    [Doc("List of modifier kinds present on the parameter.")]
    List<SyntaxKind> Modifiers { get; }

    [Doc("Node representing the parameter type.")]
    TreeNode<SemanticNode>? TypeNode { get; set; }

    [Doc("Text of the parameter type (cleaned).")]
    string TypeText { get; set; }

    [Doc("Locates the parameter name after the type.")]
    void SetNameNode();

    [Doc("Initializes modifiers, type node, and type text when attached to a tree.")]
    void SetTreeNode(TreeNode<SemanticNode> t);

    [Doc("Collects all modifier keywords on the parameter.")]
    void ParseModifiers();

    [Doc("Stores the text of the type node.")]
    void ParseType();

    [Doc("Finds the first node whose kind represents a type.")]
    void SetTypeNode();
}

[ClassDoc("PropertyNode")]
[Doc("Semantic wrapper for property declarations.")]
interface IDocument_PropertyNode
{
    [Doc("Locates the property name after its type but before any semicolon or brace.")]
    void SetNameNode();
}

[ClassDoc("RecordNode")]
[Doc("Semantic wrapper for record and record struct declarations.")]
interface IDocument_RecordNode
{
    [Doc("Returns RecordKeyword (or RecordStructDeclaration for struct records).")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("RegionEndNode")]
[Doc("Represents the #endregion directive.")]
interface IDocument_RegionEndNode
{
    [Doc("Factory that creates an #endregion node.")]
    TreeNode<SemanticNode> Factory();
}

[ClassDoc("RegionNode")]
[Doc("Base for #region / #endregion trivia nodes.")]
interface IDocument_RegionNode
{
    [Doc("Factory that creates a complete region pair with a name.")]
    TreeNode<SemanticNode> Factory(string name);
}

[ClassDoc("RegionStartNode")]
[Doc("Represents a #region directive with an optional name.")]
interface IDocument_RegionStartNode
{
    [Doc("Name of the region (used when the node is printed).")]
    string? TypeName { get; set; }

    [Doc("Factory that creates a #region directive with the supplied name.")]
    TreeNode<SemanticNode> Factory(string regionName);
}

[ClassDoc("SemanticNode")]
[Doc("Abstract base class for all nodes in the custom semantic tree. Provides name, modifier, parent, and tree traversal helpers.")]
interface IDocument_SemanticNode
{
    [Doc("True when the node has no other named children (leaf in the named hierarchy).")]
    bool ChildOnly { get; }

    [Doc("Name of the nearest containing class (walks parents).")]
    string ContainingClassName { get; }

    [Doc("Dot-separated full name including all parent containers.")]
    string FullName { get; }

    [Doc("True when this node has direct named children.")]
    bool HasDirectNamedChildren { get; }

    [Doc("True when the node has a non-empty TypeName.")]
    bool HasName { get; }

    [Doc("Node that should be used when adding members (usually the node itself or its body).")]
    TreeNode<SemanticNode> MembersNode { get; }

    [Doc("Access modifier of the node (None for nodes that do not support it).")]
    AccessModifier Modifier { get; set; }

    [Doc("Dot-separated list of parent names (excluding the node itself).")]
    string ParentFullName { get; }

    [Doc("First non-compilation-unit parent.")]
    TreeNode<SemanticNode> ParentNonContainer { get; }

    [Doc("List of ancestor names from root down to (but not including) this node.")]
    List<string> Parents { get; }

    [Doc("Optional extra text value used by some node types (e.g. collection initializers).")]
    string TextVal { get; }

    [Doc("Name of the node (backed by the TypeNameNode).")]
    string? TypeName { get; set; }

    [Doc("Default no-op child attachment (overridden by derived classes).")]
    void AttachChild(SemanticNode child);

    [Doc("Sets BaseType from the first entry in BaseTypes (overridden by type declarations).")]
    void SetBaseClass();

    [Doc("Stores the owning TreeNode and performs additional initialization.")]
    void SetTreeNode(TreeNode<SemanticNode> tree);

    [Doc("Returns a descriptive string containing modifier, kind, and name.")]
    string ToString();

    [Doc("Deletes all child nodes that are not themselves NamedNodes (i.e. the declaration part).")]
    void DeleteDeclaration();

    [Doc("Walks parents to find the nearest ClassNode and returns its TypeName.")]
    string? FindContainingClassName();

    [Doc("Returns child nodes that are not NamedNodes (declaration syntax).")]
    List<TreeNode<SemanticNode>> GetDeclaration();
}

[ClassDoc("SemanticTree")]
[Doc("High-level serializer that converts between C# text and the custom SemanticNode tree (uses the three-stage parser).")]
interface IDocument_SemanticTree
{
    [Doc("Deserializes text into a semantic tree (main public API).")]
    TreeNode<SemanticNode> Deserialize(string code);

    [Doc("Convenience factory: parses code (optionally formats first) and groups consecutive named children.")]
    TreeNode<SemanticNode> DeserializeCode(string code, bool fmt = false);

    [Doc("Parses a code snippet that is only a declaration (no full compilation unit).")]
    TreeNode<SemanticNode> DeserializeDeclaration(string code, bool fmt = false);

    [Doc("Loads a file, deserializes it, optionally formats, and returns the tree.")]
    TreeNode<SemanticNode> DeserializeFile(string file, bool fmt = false);

    [Doc("Serializes a semantic tree back to text by concatenating node text.")]
    string Serialize(TreeNode<SemanticNode> tree);

    [Doc("Alias for DeserializeFile (used internally).")]
    TreeNode<SemanticNode> FromFile(string file, bool fmt = false);
}

[ClassDoc("Serializer_SyntaxElement")]
[Doc("Converts text → Token tree → SyntaxElement tree (second stage of the parser).")]
interface IDocument_Serializer_SyntaxElement
{
    [Doc("Parses code into a TokenNode tree then converts it to a SyntaxElementNode tree.")]
    TreeNode<SyntaxElementNode> Deserialize(string code);

    [Doc("Serializes a syntax-element tree by concatenating Text values.")]
    string Serialize(TreeNode<SyntaxElementNode>? root);
}

[ClassDoc("Serializer_TokenNode")]
[Doc("Lowest-level serializer that uses Roslyn's CSharpSyntaxTree to build a TokenNode tree (including trivia).")]
interface IDocument_Serializer_TokenNode
{
    [Doc("Parses source text with Roslyn and builds a TokenNode tree representation.")]
    TreeNode<TokenNode> Deserialize(string code);

    [Doc("Serializes a token tree back to text, preserving trivia and trimming whitespace.")]
    string Serialize(TreeNode<TokenNode>? root);
}

[ClassDoc("SimpleMemberAccessNode")]
[Doc("Special node used inside collection initializers that collapses a member access chain into a single TextVal.")]
interface IDocument_SimpleMemberAccessNode
{
    [Doc("The collapsed text of the member access expression.")]
    string TextVal { get; }

    [Doc("When the parent is a collection initializer, collapses dot-chained identifiers into a single value.")]
    void SetTreeNode(TreeNode<SemanticNode> tree);
}

[ClassDoc("StaticHelpers")]
[Doc("Miscellaneous static utility methods used by the refactoring engine.")]
interface IDocument_StaticHelpers
{
    [Doc("Logs a message when methods are successfully moved into a partial class for an interface.")]
    void LogSuccessfulExtraction(string interfaceName, int methodCount);

    [Doc("Returns true when either className or interfaceName is null or empty.")]
    bool NamesAreInvalid(string className, string interfaceName);
}

[ClassDoc("StructNode")]
[Doc("Semantic wrapper for struct declarations.")]
interface IDocument_StructNode
{
    [Doc("Returns StructKeyword.")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("SyntaxElementNode")]
[Doc("Intermediate node that holds a SyntaxKind and optional text; used between the token tree and the semantic tree.")]
interface IDocument_SyntaxElementNode
{
    [Doc("Identifier text when the node represents an identifier token.")]
    string? Identifier { get; }

    [Doc("SyntaxKind of the element.")]
    SyntaxKind Kind { get; }

    [Doc("Text content of the token or trivia.")]
    string? Text { get; }

    [Doc("Factory method that creates the appropriate SemanticNode subclass for this element.")]
    SemanticNode CreateSemantic();

    [Doc("Returns a debug string containing the kind and optional identifier.")]
    string ToString();
}

[ClassDoc("SyntaxKindGroups")]
[Doc("Static sets of SyntaxKind values used for filtering, stopping recursion, and classifying nodes throughout the framework.")]
interface IDocument_SyntaxKindGroups { }

[ClassDoc("SyntaxLoader")]
[Doc("Helpers for opening Roslyn projects and loading syntax trees from .csproj or individual .cs files.")]
interface IDocument_SyntaxLoader
{
    [Doc("Opens a Roslyn Project from a .csproj path using MSBuildWorkspace.")]
    Project OpenProject(string projectPath);

    [Doc("Creates an MSBuildWorkspace with design-time build properties.")]
    MSBuildWorkspace CreateWorkspace();

    [Doc("Loads a single C# file into a Roslyn SyntaxTree.")]
    SyntaxTree LoadFile(string csFilePath);

    [Doc("Loads all C# documents from a Roslyn Project into syntax trees.")]
    IReadOnlyList<SyntaxTree> LoadSingleProject(string projectPath);
}

[ClassDoc("TokenNode")]
[Doc("Represents a Roslyn SyntaxNodeOrToken with its leading and trailing trivia separated.")]
interface IDocument_TokenNode
{
    [Doc("Identifier text when the token is an IdentifierToken.")]
    string? Identifier { get; }

    [Doc("SyntaxKind of the token or node.")]
    SyntaxKind Kind { get; }

    [Doc("Leading trivia elements.")]
    IReadOnlyList<SyntaxElementNode> LeadingTrivia { get; set; }

    [Doc("Original Roslyn SyntaxNode when the token was created from a node (instead of a token).")]
    SyntaxNode? OriginalSyntaxNode { get; }

    [Doc("Text of the token (for tokens) or null for nodes.")]
    string? Text { get; }

    [Doc("Trailing trivia elements.")]
    IReadOnlyList<SyntaxElementNode> TrailingTrivia { get; set; }

    [Doc("Returns a debug string; optionally includes trivia counts.")]
    string ToString(bool showTrivia);

    [Doc("Converts the token (with trivia) into a flat list of SyntaxElementNode objects.")]
    List<SyntaxElementNode> ToSyntaxElements();

    [Doc("Retrieves a specific trivia element by index (leading or trailing).")]
    SyntaxElementNode? GetFullTrivia(int index, bool isLeading = true);

    [Doc("Returns true for whitespace or end-of-line trivia that can be ignored.")]
    bool IsTrivialWhitespace(SyntaxKind k);
}

[ClassDoc("TreeNode")]
[Doc("Generic tree container that holds a Value (SemanticNode, SyntaxElementNode, or TokenNode) and a list of children.")]
interface IDocument_TreeNode<T>
    where T : BaseNode<T>
{
    [Doc("Child nodes.")]
    List<TreeNode<T>> Children { get; set; }

    [Doc("True when the node has no parent.")]
    bool IsRoot { get; }

    [Doc("Parent node (null for root).")]
    TreeNode<T>? Parent { get; set; }

    [Doc("The payload (SemanticNode, SyntaxElementNode, etc.).")]
    T? Value { get; set; }

    [Doc("Creates a new child node from a value and adds it.")]
    TreeNode<T> AddChild(T child);

    [Doc("Adds an existing TreeNode as a child (re-parenting it).")]
    void AddChild(TreeNode<T> child);

    [Doc("Deep-clones the entire subtree.")]
    TreeNode<T> DeepClone();

    [Doc("Removes the node from its parent, optionally promoting children.")]
    void Delete(DeleteType deleteType);

    [Doc("Recursively deletes nodes that match a predicate.")]
    void DeleteRecursive(
        Func<TreeNode<T>, bool> predicate,
        DeleteType deleteType = DeleteType.SingleNode
    );

    [Doc("Yields groups of consecutive nodes that all satisfy the predicate.")]
    IEnumerable<List<TreeNode<T>>> FindConsecutive(Func<TreeNode<T>, bool> predicate);

    [Doc("Recursively searches while skipping nodes that match skipPredicate.")]
    IEnumerable<TreeNode<T>> FindSkip(
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> skipPredicate
    );

    [Doc("Recursively searches with both skip and stop predicates.")]
    IEnumerable<TreeNode<T>> FindSkipStop(
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> skipPredicate,
        Func<TreeNode<T>, bool> stopPredicate
    );

    [Doc("Depth-first search returning every node where the predicate returns true.")]
    IEnumerable<TreeNode<T>> FindWhere(Func<TreeNode<T>, bool> predicate);

    [Doc("Searches until a node matches untilPredicate (does not go deeper).")]
    IEnumerable<TreeNode<T>> FindWhereStop(
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> untilPredicate
    );

    [Doc("Returns every node in the subtree (including this one).")]
    IEnumerable<TreeNode<T>> Flatten();

    [Doc("Inserts the new node immediately after this node in the parent's child list.")]
    void InsertAfter(TreeNode<T> newNode);

    [Doc("Inserts the new node immediately before this node in the parent's child list.")]
    void InsertBefore(TreeNode<T> newNode);

    [Doc("Removes a direct child (null-safe).")]
    bool RemoveChild(TreeNode<T> child);

    [Doc("Removes this node from its parent.")]
    void RemoveSelf();

    [Doc("Replaces this node with another node in the parent's child list.")]
    void ReplaceSelf(TreeNode<T> replacement);

    [Doc("Returns the string representation of the contained Value.")]
    string ToString();

    [Doc("Deep equality comparison of two trees (including values and children).")]
    bool DeepEqual(TreeNode<T>? other, IEqualityComparer<T>? valueComparer = null);

    [Doc("Deletes all but the first N consecutive nodes that match the predicate.")]
    void DeleteConsecutive(
        Func<TreeNode<T>, bool> predicate,
        DeleteType deleteType = DeleteType.SingleNode,
        int nStart = 1
    );

    [Doc("Deletes nodes matching predicate until a stop node is reached.")]
    void DeleteUntil(
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> untilPredicate,
        DeleteType deleteType = DeleteType.SingleNode
    );

    [Doc("Collects first/last child pairs for every node that has children (used internally).")]
    List<(TreeNode<T> First, TreeNode<T> Last)> FindFirstLastChildPairs();

    [Doc("Returns the next sibling or null.")]
    TreeNode<T>? GetNextNode();

    [Doc("Returns the previous sibling or the parent if this is the first child.")]
    TreeNode<T>? GetPreviousNode();

    [Doc("Inserts a node before or after this node (internal helper).")]
    void InsertRelative(TreeNode<T> newNode, bool after = false);

    [Doc("Promotes children of a deleted node into the parent's list at the deletion point.")]
    void PromoteChildrenTo(TreeNode<T> parent, int insertIndex);

    [Doc("Replaces this node in its parent's list with a new node.")]
    void ReplaceNode(TreeNode<T> newNode);
}

[ClassDoc("TriviaNode")]
[Doc("Base for nodes that represent trivia (comments, whitespace, regions).")]
interface IDocument_TriviaNode { }

[ClassDoc("TypeDeclarationNode")]
[Doc("Common base for all type-like declarations (class, struct, interface, enum, record, delegate, namespace).")]
interface IDocument_TypeDeclarationNode
{
    [Doc("Set of base type names (after stripping generics).")]
    HashSet<string> BaseTypes { get; }

    [Doc("Node that should receive new members (usually the body).")]
    TreeNode<SemanticNode> MembersNode { get; }

    [Doc("Sets BaseType to the first entry in BaseTypes.")]
    void SetBaseClass();

    [Doc("Initializes base list and base class on tree attachment.")]
    void SetTreeNode(TreeNode<SemanticNode> t);

    [Doc("Returns the specific keyword that identifies the type (ClassKeyword, InterfaceKeyword, etc.).")]
    SyntaxKind? GetTypeSpecificKeyword();

    [Doc("Parses the BaseList syntax to populate BaseTypes.")]
    void SetBaseList();

    [Doc("Strips generic type arguments from a base type name.")]
    string StripGenerics(string s);
}

[ClassDoc("UnknownNode")]
[Doc("Fallback node used when no more specific SemanticNode subclass matches the SyntaxKind.")]
interface IDocument_UnknownNode { }

[ClassDoc("UsingDirectiveNode")]
[Doc("Represents a using directive; treated as a named node whose name is the namespace being imported.")]
interface IDocument_UsingDirectiveNode
{
    [Doc("Parses the namespace name by collecting tokens until the semicolon.")]
    void SetNameNode();
}
public class ClassDocAttribute : Attribute
{
    public ClassDocAttribute(string className)
    {
        this.ClassName = className;
    }

    public string ClassName;
}
public class DocAttribute : Attribute
{
    public DocAttribute(string documentation)
    {
        this.documentation = documentation;
    }
    public DocAttribute(string documentation, string codeCheckoutReason) : this(documentation)
    {
        this.codeCheckoutReason = codeCheckoutReason;
    }
    public string codeCheckoutReason;
    public string documentation;
}