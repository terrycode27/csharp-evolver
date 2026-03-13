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

[ClassDoc("__Program")]
[Doc("Main entry point and self-refactoring harness for the Evolver5 semantic tree system")]
interface I__Program
{
    [Doc("Application entry point – runs interface extraction by default")]
    void Main();
}

[ClassDoc("AddAttributeExtensions")]
[Doc("Extensions responsible for injecting [ClassDocumentation] and [Documentation] attributes into generated interfaces")]
interface IAddAttributeExtensions
{
    [Doc("Prepends System.* usings to a compilation unit")]
    void AddUsings(TreeNode<SemanticNode> root, List<UsingDirectiveNode> usings);

    [Doc("Adds [ClassDocumentation(\"ClassName\")] attribute to a named node")]
    void AddClassAttribute(NamedNode named, string className);

    [Doc("Adds [Documentation(\"description\")] attribute (empty string by default)")]
    void AddDocAttribute(NamedNode named, string doc = null);

    [Doc("Generic attribute injection with duplicate-check guard")]
    void AddAttribute(NamedNode named, string attributeString);
}

[ClassDoc("_ExtractInterfaceExtensions")]
[Doc("Core engine that converts concrete classes into clean public-only interface declarations")]
interface I_ExtractInterfaceExtensions
{
    [Doc("Orchestrates interface extraction for every class in the tree")]
    TreeNode<SemanticNode> ExtractInterfaces(TreeNode<SemanticNode> root);

    [Doc("Extracts a single class → interface and decorates it with documentation attributes")]
    TreeNode<SemanticNode> ExtractInterfaceWithAttributes(ClassNode classNode);

    [Doc("Builds a property signature suitable for an interface (removes body, normalizes accessors)")]
    string BuildSinglePropertySignature(PropertyNode propNode);

    [Doc("Guarantees a trailing semicolon on interface method declarations")]
    void EnsureMethodSemicolon(MethodNode method);

    [Doc("Main Uncle-Bob-style orchestrator that assembles full interface source from signatures")]
    TreeNode<SemanticNode> ExtractInterface(ClassNode classNode);

    [Doc("Converts a MethodNode into its interface-compatible signature (strips body + illegal modifiers)")]
    string GetInterfaceSignature(MethodNode methodNode);

    [Doc("Removes modifiers that are illegal in interface context")]
    void RemoveInterfaceIllegalModifiers(MethodNode method);

    [Doc("Deletes method bodies (Block or ArrowExpressionClause)")]
    void RemoveMethodBody(MethodNode method);
}

[ClassDoc("BaseNode")]
[Doc("Generic base for all semantic and syntax tree nodes")]
interface IBaseNode<T>
    where T : BaseNode<T>
{ }

[ClassDoc("ClassDocumentationAttribute")]
[Doc("Attribute that records the original concrete class name on generated interfaces")]
interface IClassDocumentationAttribute { }

[ClassDoc("DocumentationAttribute")]
[Doc("Attribute that records human-readable documentation on members")]
interface IDocumentationAttribute { }

[ClassDoc("BlockNode")]
[Doc("Represents a { ... } block containing statements")]
interface IBlockNode
{
    [Doc("Child statements inside the block")]
    List<SemanticNode> Statements { get; }

    [Doc("Attaches a child node to the block")]
    void AttachChild(SemanticNode child);
}

[ClassDoc("ClassNode")]
[Doc("Semantic representation of a class declaration (including static/extension conversion helpers)")]
interface IClassNode
{
    [Doc("Delegate declarations inside the class")]
    List<DelegateNode> Delegates { get; }

    [Doc("True if the class contains any methods")]
    bool HasMethods { get; }

    [Doc("True if the class is marked static")]
    bool IsStaticClass { get; }

    [Doc("Default access modifier for members")]
    AccessModifier DefaultModifier { get; }

    [Doc("Attaches a child semantic node")]
    void AttachChild(SemanticNode child);

    [Doc("Converts a static class to an instance class")]
    void ConvertToInstanceClass();

    [Doc("Converts an instance class to a static extension class")]
    void ConvertToStaticExtensionClass();

    [Doc("Returns the keyword token specific to this type (class, struct, etc.)")]
    SyntaxKind? GetTypeSpecificKeyword();

    [Doc("Sets the modifier from the syntax tree")]
    void SetModifierFromTree();
}

[ClassDoc("CommentNode")]
[Doc("Represents single- or multi-line comments in the tree")]
interface ICommentNode { }

[ClassDoc("CompilationUnitNode")]
[Doc("Top-level compilation unit containing usings and members")]
interface ICompilationUnitNode
{
    [Doc("Top-level members")]
    List<SemanticNode> Members { get; }

    [Doc("Using directives")]
    List<UsingDirectiveNode> Usings { get; }

    [Doc("Attaches a child node")]
    void AttachChild(SemanticNode child);

    [Doc("Factory method for a new compilation unit")]
    TreeNode<SemanticNode> Factory();
}

[ClassDoc("CompilationUtil")]
[Doc("Roslyn-based compilation and round-trip testing utilities")]
interface ICompilationUtil
{
    [Doc("Compiles code against a .csproj file")]
    (bool Success, ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<Diagnostic> Errors) Compile(string code, string projectFile);

    [Doc("Compiles code against an open Project")]
    (bool Success, ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<Diagnostic> Errors) Compile(string code, Project project);

    [Doc("Verifies that original and regenerated code both compile successfully")]
    bool RoundTripCompilation(string originalCode, string regeneratedCode, string csprojFileName);
}

[ClassDoc("ConstructorNode")]
[Doc("Semantic node for constructor declarations")]
interface IConstructorNode { }

[ClassDoc("CSharpierFormatter")]
[Doc("Isolated CSharpier formatter (handles Roslyn version conflicts via separate AssemblyLoadContext)")]
interface ICSharpierFormatter
{
    [Doc("Formats C# source code")]
    string Format(string code);

    [Doc("Unloads the isolated formatter context")]
    void Unload();
}

[ClassDoc("DelegateNode")]
[Doc("Semantic node for delegate declarations")]
interface IDelegateNode
{
    [Doc("Parameter list of the delegate")]
    ParameterListNode? ParameterList { get; set; }

    [Doc("Attaches a child node")]
    void AttachChild(SemanticNode child);

    [Doc("Type-specific keyword (delegate)")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("DestructorNode")]
[Doc("Semantic node for destructor declarations")]
interface IDestructorNode { }

[ClassDoc("EntryPoint")]
[Doc("Project configuration, file paths, and high-level refactoring orchestrator")]
interface IEntryPoint
{
    [Doc("Base namespace for the current project")]
    string BaseNamespace { get; }

    [Doc("Path to the class-specific generated file")]
    string ClassFilePath { get; }

    [Doc("Path to the consolidated source file")]
    string ConsolidatedFilePath { get; }

    [Doc("Path to the method-specific generated file")]
    string MethodFilePath { get; }

    [Doc("Consolidated namespace for all types")]
    string NamespaceConsolidated { get; }

    [Doc("Namespace used when generating one class per file")]
    string OneClassPerFileNamespace { get; }

    [Doc("Suffix appended for one-class-per-file generation")]
    string OneClassPerFileNamespaceSuffix { get; }

    [Doc("Namespace used when generating one namespace per file")]
    string OneNamespacePerFileNamespace { get; }

    [Doc("Suffix appended for one-namespace-per-file generation")]
    string OneNamespacePerFileNamespaceSuffix { get; }

    [Doc("Full path to the .csproj")]
    string ProjectFilePath { get; }

    [Doc("Root directory of the project")]
    string RootDirectory { get; }

    [Doc("Path used for test files")]
    string TestFilePath { get; }

    [Doc("Factory for a new EntryPoint instance")]
    EntryPoint Create(string baseNamespace);

    [Doc("Pulls types out of namespaces")]
    void ExtractFromNamespaces();

    [Doc("Formats the consolidated file with CSharpier")]
    void Format();

    [Doc("Helper to build full code path for a file")]
    string GetCodePath(string fileName);

    [Doc("Returns full file path given name and subdirectory")]
    string GetFilePath(string name, string subdir);

    [Doc("Returns path for test file at index i")]
    string GetTestFilePath(int i);

    [Doc("Groups members by class hierarchy")]
    void GroupByClassHierarchy();

    [Doc("Groups members by modifier/kind/name")]
    void GroupByModifierKindName();

    [Doc("Groups static extension methods into dedicated extension classes")]
    void GroupStaticExtensionMethods();

    [Doc("Loads a semantic tree by name (with optional subdir)")]
    TreeNode<SemanticNode> LoadTree(string name, string subdir = null);

    [Doc("Loads tree from code path")]
    TreeNode<SemanticNode> LoadTreeFromCodePath(string fileName);

    [Doc("Loads tree from full file path")]
    TreeNode<SemanticNode> LoadTreeFromPath(string fullPath);

    [Doc("Main entry for loading the primary semantic tree")]
    TreeNode<SemanticNode> LoadTreeMain();

    [Doc("Applies ordering to collection values in the tree")]
    void OrderCollectionValues();

    [Doc("Refactors a large class into partial classes split by interfaces")]
    void RefactorLargeClassIntoPartialsWithInterfaces(string className);

    [Doc("Runs semantic serializer round-trip test")]
    void TestSemanticSerializer();
}

[ClassDoc("EnumMemberNode")]
[Doc("Semantic node for individual enum members")]
interface IEnumMemberNode { }

[ClassDoc("EnumNode")]
[Doc("Semantic node for enum declarations")]
interface IEnumNode
{
    [Doc("Enum members")]
    List<EnumMemberNode> Members { get; }

    [Doc("Attaches a child node")]
    void AttachChild(SemanticNode child);

    [Doc("Type-specific keyword (enum)")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("EventFieldNode")]
[Doc("Semantic node for event-field declarations")]
interface IEventFieldNode
{
    [Doc("Sets the name node for the event field")]
    void SetNameNode();
}

[ClassDoc("EventNode")]
[Doc("Semantic node for event declarations")]
interface IEventNode { }

[ClassDoc("ExpressionBodyNode")]
[Doc("Semantic node for => expression-bodied members")]
interface IExpressionBodyNode { }

[ClassDoc("ExtensionsMethodNode")]
[Doc("Helper extensions for method-related tree navigation and static-extension handling")]
interface IExtensionsMethodNode
{
    [Doc("Returns all ancestor nodes of the given tree node")]
    IEnumerable<TreeNode<SemanticNode>> Ancestors(TreeNode<SemanticNode> node);

    [Doc("Finds the containing type declaration (class/struct/record/interface)")]
    TreeNode<SemanticNode> GetContainingTypeDeclaration(TreeNode<SemanticNode> node);

    [Doc("Checks whether a method is called from outside its declaring class")]
    bool IsCalledFromOutsideClass(MethodNode method, TreeNode<SemanticNode> solutionRoot);

    [Doc("Checks whether a node is inside the same class")]
    bool IsInsideSameClass(TreeNode<SemanticNode> site, TreeNode<SemanticNode> classNode);
}

[ClassDoc("ExtensionsOfAccessModifier")]
[Doc("Converts AccessModifier enum values to SyntaxKind")]
interface IExtensionsOfAccessModifier
{
    [Doc("Maps an AccessModifier to its corresponding SyntaxKind")]
    SyntaxKind ToSyntaxKind(AccessModifier modifier);
}

[ClassDoc("ExtensionsOfAppDomain")]
[Doc("AppDomain helper to locate the solution root folder")]
interface IExtensionsOfAppDomain
{
    [Doc("Walks up from BaseDirectory until a .cs file is found")]
    string GetSolutionBaseFolder(AppDomain domain);
}

[ClassDoc("ExtensionsOfIEnumerableOfT")]
[Doc("General-purpose LINQ-style extensions for grouping and iteration")]
interface IExtensionsOfIEnumerableOfT<T>
{
    [Doc("Simple ForEach that throws on null source/action")]
    void ForEach<T>(IEnumerable<T> source, Action<T> action);

    [Doc("Multi-level grouping by successive predicates")]
    IEnumerable<T> GroupByPredicates<T>(IEnumerable<T> items, List<Func<T, object>> predicates);

    [Doc("Groups consecutive items that satisfy the same predicate value")]
    List<List<T>> GroupConsecutive<T>(IEnumerable<T> items, Func<T, bool> predicate);
}

[ClassDoc("ExtensionsOfIEnumerableOfTreeNodeOfSemanticNode")]
[Doc("Tree-node collection extensions for deletion and dictionary conversion")]
interface IExtensionsOfIEnumerableOfTreeNodeOfSemanticNode
{
    [Doc("Deletes nodes of a given kind from the collection")]
    IEnumerable<TreeNode<SemanticNode>> DeleteKind(IEnumerable<TreeNode<SemanticNode>> en, SyntaxKind kind, DeleteType deleteType = DeleteType.SingleNode);

    [Doc("Deletes nodes of any kind in the supplied set")]
    IEnumerable<TreeNode<SemanticNode>> DeleteKinds(IEnumerable<TreeNode<SemanticNode>> en, HashSet<SyntaxKind> kinds, DeleteType deleteType = DeleteType.SingleNode);

    [Doc("Returns candidates not already present in the existing collection")]
    List<TreeNode<SemanticNode>> NotPresentIn(IEnumerable<TreeNode<SemanticNode>> candidates, IEnumerable<TreeNode<SemanticNode>> existing);

    [Doc("Builds a dictionary keyed by FullName")]
    Dictionary<string, TreeNode<SemanticNode>> ToDictionaryByFullName(IEnumerable<TreeNode<SemanticNode>> nodes);
}

[ClassDoc("ExtensionsOfIEnumerableOfTSource")]
[Doc("Single-key grouping extension")]
interface IExtensionsOfIEnumerableOfTSource<TSource, TKey>
{
    [Doc("Groups by key and returns a dictionary of single items")]
    Dictionary<TKey, TSource> GroupBySingle<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector);
}

[ClassDoc("ExtensionsOfListOfT")]
[Doc("List interleaving by key")]
interface IExtensionsOfListOfT<T, TKey>
    where TKey : IComparable<TKey>
{
    [Doc("Interleaves movable items according to their key order")]
    List<T> Interleave<T, TKey>(List<T> source, Func<T, TKey?> getKey)
        where TKey : IComparable<TKey>;
}

[ClassDoc("ExtensionsOfListOfTreeNodeOfSemanticNode")]
[Doc("High-level refactoring helpers for partial classes and grouping")]
interface IExtensionsOfListOfTreeNodeOfSemanticNode
{
    [Doc("Creates a new partial class containing the moved methods")]
    TreeNode<SemanticNode> CreateNewPartialClassForInterface(List<TreeNode<SemanticNode>> methodsToMove, string className, string interfaceName);

    [Doc("Finds delimited text segments")]
    IEnumerable<TreeNode<SemanticNode>> FindDelimitedText(List<TreeNode<SemanticNode>> roots, HashSet<SyntaxKind> delimeters);

    [Doc("Finds text segments stopping at a delimiter set")]
    IEnumerable<TreeNode<SemanticNode>> FindText(List<TreeNode<SemanticNode>> roots, HashSet<SyntaxKind> include, HashSet<SyntaxKind> stopAt);

    [Doc("Extracts string representations from nodes")]
    List<string> GetStrings(List<TreeNode<SemanticNode>> en);

    [Doc("Groups children by modifier + kind")]
    List<List<TreeNode<SemanticNode>>> GroupedByModifierKind(List<TreeNode<SemanticNode>> children);

    [Doc("Makes private static extension methods public")]
    void SetStaticExtensionMethodsPublic(List<TreeNode<SemanticNode>> extensionMethods);

    [Doc("Sorts children by modifier, kind, then name")]
    List<TreeNode<SemanticNode>> SortedByModifierKindName(List<TreeNode<SemanticNode>> children);

    [Doc("Wraps a list of methods inside a new declaration")]
    TreeNode<SemanticNode> WrapInDeclaration(List<TreeNode<SemanticNode>> methods, string declaration);
}

[ClassDoc("ExtensionsOfListOfTreeNodeOfT")]
[Doc("Generic recursive search helpers (FindWhere, FindSkip, etc.)")]
interface IExtensionsOfListOfTreeNodeOfT<T>
    where T : BaseNode<T>
{
    [Doc("Finds nodes matching predicate, skipping some")]
    IEnumerable<TreeNode<T>> FindSkip<T>(List<TreeNode<T>> roots, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> skipPredicate)
        where T : BaseNode<T>;

    [Doc("Finds nodes with both skip and stop predicates")]
    IEnumerable<TreeNode<T>> FindSkipStop<T>(List<TreeNode<T>> roots, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> skipPredicate, Func<TreeNode<T>, bool> stopPredicate)
        where T : BaseNode<T>;

    [Doc("Recursive FindWhere")]
    IEnumerable<TreeNode<T>> FindWhere<T>(List<TreeNode<T>> roots, Func<TreeNode<T>, bool> predicate)
        where T : BaseNode<T>;

    [Doc("Finds nodes stopping at a predicate")]
    IEnumerable<TreeNode<T>> FindWhereStop<T>(List<TreeNode<T>> roots, Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> untilPredicate)
        where T : BaseNode<T>;
}

[ClassDoc("ExtensionsOfstring")]
[Doc("String utilities for normalization and tree conversion")]
interface IExtensionsOfstring
{
    [Doc("Normalizes all line endings to Windows style")]
    string NormalizeLineEndings(string text);

    [Doc("Parses string into a semantic tree")]
    TreeNode<SemanticNode> ToTree(string code);
}

[ClassDoc("ExtensionsOfSyntaxKind")]
[Doc("SyntaxKind conversion helpers")]
interface IExtensionsOfSyntaxKind
{
    [Doc("Maps SyntaxKind to a handling order enum")]
    SyntaxKindOrdering ToHandled(SyntaxKind kind);

    [Doc("Maps SyntaxKind to AccessModifier")]
    AccessModifier ToModifier(SyntaxKind kind);
}

[ClassDoc("ExtensionsOfSyntaxNode")]
[Doc("Roslyn SyntaxNode → semantic tree conversion")]
interface IExtensionsOfSyntaxNode
{
    [Doc("Full pipeline: SyntaxNode → TokenNode → SyntaxElementNode → SemanticNode")]
    TreeNode<SemanticNode> ToTreeSemanticNode(SyntaxNode node);

    [Doc("SyntaxNode → TokenNode tree")]
    TreeNode<TokenNode> ToTreeTokenNode(SyntaxNode node);
}

[ClassDoc("ExtensionsOfT")]
[Doc("Generic collection wrapper extensions")]
interface IExtensionsOfT<T>
    where T : class
{
    [Doc("Wraps a single item as a 1-element array")]
    T[] AsArray<T>(T item);

    [Doc("Wraps a single item as a List")]
    List<T> AsList<T>(T item);

    [Doc("Returns empty list if item is null")]
    List<T> AsListOrEmpty<T>(T? item)
        where T : class;

    [Doc("Wraps item as a Queue")]
    Queue<T> AsQueue<T>(T item);

    [Doc("Wraps item as a singleton List")]
    List<T> ToSingletonList<T>(T item);

    [Doc("Yields a single item")]
    IEnumerable<T> Yield<T>(T item);
}

[ClassDoc("ExtensionsOfTreeNodeOfSemanticNode")]
[Doc("Core semantic tree manipulation extensions (merge, delete, group, refactor, etc.)")]
interface IExtensionsOfTreeNodeOfSemanticNode<T>
{
    [Doc("Adds missing containers at a specific depth during merge")]
    void AddMissingContainersAtDepth(TreeNode<SemanticNode> destination, TreeNode<SemanticNode> source, int depth);

    [Doc("Adds missing containers by walking depth from source")]
    void AddMissingContainersByDepthFrom(TreeNode<SemanticNode> destination, TreeNode<SemanticNode> source);

    [Doc("Adds lowest-level types from source")]
    void AddNewLowestLevelTypesFrom(TreeNode<SemanticNode> destination, List<TreeNode<SemanticNode>> source);

    [Doc("Adds a new member to a parent")]
    void AddNewMember(TreeNode<SemanticNode> parent, TreeNode<SemanticNode> member);

    [Doc("Checks whether a class already has the partial keyword")]
    bool AlreadyHasPartialKeyword(TreeNode<SemanticNode> classObj);

    [Doc("Converts tree to list of code strings")]
    List<string> AsCode<T>(TreeNode<SemanticNode> tree);

    [Doc("Builds a #region tree for class hierarchy visualization")]
    TreeNode<SemanticNode> BuildRegionTree(TreeNode<SemanticNode> classNode, Dictionary<string, List<TreeNode<SemanticNode>>> derivedMap, Dictionary<string, TreeNode<SemanticNode>> byName);

    [Doc("Counts physical lines of code")]
    int CountLinesOfCode(TreeNode<SemanticNode> tree);

    [Doc("Counts methods in the tree")]
    int CountMethods(TreeNode<SemanticNode> tree);

    [Doc("Counts static extension methods")]
    int CountStaticExtensionMethods(TreeNode<SemanticNode> tree);

    [Doc("Deletes all comment nodes")]
    void DeleteComments<T>(TreeNode<SemanticNode> root);

    [Doc("Removes extra compilation units")]
    void DeleteCompilationUnit(TreeNode<SemanticNode> tree);

    [Doc("Deletes excess consecutive newlines")]
    TreeNode<SemanticNode> DeleteExcessNewlines(TreeNode<SemanticNode> root);

    [Doc("Deletes nodes of a given kind")]
    IEnumerable<TreeNode<SemanticNode>> DeleteKind(TreeNode<SemanticNode> tree, SyntaxKind kind, DeleteType deleteType = DeleteType.SingleNode);

    [Doc("Deletes nodes of any kind in the set")]
    List<TreeNode<SemanticNode>> DeleteKinds(TreeNode<SemanticNode> tree, HashSet<SyntaxKind> kinds, DeleteType deleteType = DeleteType.SingleNode);

    [Doc("Seek-and-delete version for kinds")]
    IEnumerable<TreeNode<SemanticNode>> DeleteKindsSeek(TreeNode<SemanticNode> tree, HashSet<SyntaxKind> kinds, DeleteType deleteType = DeleteType.SingleNode);

    [Doc("Divides a class into partials based on one interface")]
    void DivideIntoPartialClassByInterface(TreeNode<SemanticNode> tree, TreeNode<SemanticNode> classObj, TreeNode<SemanticNode> interfaceDecl);

    [Doc("Divides a class into partials for each inherited interface")]
    void DivideIntoPartialClassesByInterfaces(TreeNode<SemanticNode> tree, string className, string interfaceName = null);

    [Doc("Replaces method bodies with NotImplementedException")]
    void EmptyMemberDeclarationsContents(TreeNode<SemanticNode> tree, EntryPoint path);

    [Doc("Removes member declarations from type declarations")]
    void EmptyTypeDeclarationsContents(TreeNode<SemanticNode> tree, EntryPoint path);

    [Doc("Ensures the class has the partial keyword")]
    void EnsureClassIsMarkedPartial(TreeNode<SemanticNode> classObj);

    [Doc("Removes namespace wrappers")]
    TreeNode<SemanticNode> ExtractFromAndRemoveNamespaces(TreeNode<SemanticNode> tree);

    [Doc("Extracts types from a namespace node")]
    void ExtractFromNamespaces(TreeNode<SemanticNode> root, List<TreeNode<SemanticNode>> usings, TreeNode<SemanticNode> ns);

    [Doc("Finds all ClassNode instances")]
    List<ClassNode> FindClasses(TreeNode<SemanticNode> tree);

    [Doc("Finds all class declaration nodes")]
    List<TreeNode<SemanticNode>> FindClassNodes(TreeNode<SemanticNode> node);

    [Doc("Finds delimited text segments")]
    IEnumerable<TreeNode<SemanticNode>> FindDelimitedText(TreeNode<SemanticNode> root, HashSet<SyntaxKind> delimeters);

    [Doc("Finds methods that implement a given interface")]
    List<TreeNode<SemanticNode>> FindImplementingMethodsInClass(TreeNode<SemanticNode> classObj, HashSet<string> interfaceMethodNames);

    [Doc("Finds nodes of a single kind")]
    List<TreeNode<SemanticNode>> FindKind(TreeNode<SemanticNode> tree, SyntaxKind kind);

    [Doc("Finds nodes of any kind in the set")]
    List<TreeNode<SemanticNode>> FindKinds(TreeNode<SemanticNode> tree, HashSet<SyntaxKind> kinds);

    [Doc("Finds semantic nodes of a kind (value form)")]
    List<SemanticNode> FindKindVal(TreeNode<SemanticNode> tree, SyntaxKind kind);

    [Doc("Finds name after a type declaration")]
    TreeNode<SemanticNode> FindNameAfterType(TreeNode<SemanticNode> tree, HashSet<SyntaxKind> stopKinds);

    [Doc("Finds name before semicolon")]
    TreeNode<SemanticNode> FindNameAfterTypeBeforeSemicolon(TreeNode<SemanticNode> tree);

    [Doc("Finds parent by full name path")]
    TreeNode<SemanticNode> FindParentByFullName(TreeNode<SemanticNode> root, string parentFullName);

    [Doc("Finds static classes")]
    List<ClassNode> FindStaticClasses(TreeNode<SemanticNode> tree);

    [Doc("Finds static extension methods")]
    List<TreeNode<SemanticNode>> FindStaticExtensionMethods(TreeNode<SemanticNode> tree);

    [Doc("Recursive find for a single kind")]
    IEnumerable<TreeNode<SemanticNode>> FindSyntaxKind(TreeNode<SemanticNode> node, SyntaxKind kind);

    [Doc("Recursive find for multiple kinds")]
    IEnumerable<TreeNode<SemanticNode>> FindSyntaxKinds(TreeNode<SemanticNode> node, HashSet<SyntaxKind> kinds);

    [Doc("Flattens tree to (code, node) pairs")]
    List<(string code, TreeNode<SemanticNode> node)> FlattenToCode(TreeNode<SemanticNode> tree);

    [Doc("Applies CSharpier formatting to the tree")]
    TreeNode<SemanticNode> Format(TreeNode<SemanticNode> tree);

    [Doc("Builds derived-class map")]
    Dictionary<string, List<TreeNode<SemanticNode>>> GetDerivedClasses(TreeNode<SemanticNode> tree);

    [Doc("Extracts interface name from an interface declaration")]
    string GetInterfaceName(TreeNode<SemanticNode> interfaceDecl);

    [Doc("Returns lowest-level type nodes")]
    List<TreeNode<SemanticNode>> GetLowestLevelTypes(TreeNode<SemanticNode> root);

    [Doc("Maximum parent depth in the tree")]
    int GetMaxParentCount(TreeNode<SemanticNode> root);

    [Doc("Method names declared in an interface")]
    HashSet<string> GetMethodNamesDeclaredInInterface(TreeNode<SemanticNode> interfaceDecl);

    [Doc("All MethodNode instances")]
    List<MethodNode> GetMethods(TreeNode<SemanticNode> tree);

    [Doc("All named nodes")]
    IEnumerable<TreeNode<SemanticNode>> GetNamedNodes(TreeNode<SemanticNode> root);

    [Doc("Named nodes at a specific depth")]
    IEnumerable<TreeNode<SemanticNode>> GetNamedNodesAtDepth(TreeNode<SemanticNode> root, int depth);

    [Doc("Non-extension static methods inside extension classes")]
    List<TreeNode<SemanticNode>> GetNonExtensionMethodsInExtensionClass(TreeNode<SemanticNode> root);

    [Doc("System.* usings")]
    List<UsingDirectiveNode> GetSystemUsings(TreeNode<SemanticNode> tree);

    [Doc("All using directives")]
    List<UsingDirectiveNode> GetUsings(TreeNode<SemanticNode> tree);

    [Doc("Groups tree by modifier/kind/name and returns groups")]
    (TreeNode<SemanticNode> Tree, List<List<TreeNode<SemanticNode>>> Groups) GroupByModifierKindName(TreeNode<SemanticNode> tree);

    [Doc("Groups consecutive children matching a predicate")]
    TreeNode<SemanticNode> GroupConsecutiveChildren(TreeNode<SemanticNode> node, Func<TreeNode<SemanticNode>, bool> predicate);

    [Doc("Groups static extension methods into dedicated classes")]
    void GroupStaticExtensionMethods(TreeNode<SemanticNode> root);

    [Doc("Initializes all nodes in the tree")]
    void InitializeAllNodes(TreeNode<SemanticNode> node);

    [Doc("Inserts partial keyword before the class keyword")]
    void InsertPartialKeywordBefore(TreeNode<SemanticNode> classObj, TreeNode<SemanticNode> classKeyword);

    [Doc("Merges at a specific depth")]
    void MergeAtDepth(TreeNode<SemanticNode> source, int index);

    [Doc("Merges source tree into destination (replace or add)")]
    TreeNode<SemanticNode> MergeFrom(TreeNode<SemanticNode> destination, TreeNode<SemanticNode> source, bool replace = true);

    [Doc("Merges a list of sources into destination")]
    TreeNode<SemanticNode> MergeFromList(TreeNode<SemanticNode> destination, List<TreeNode<SemanticNode>> sources);

    [Doc("Merges partial class pieces by name")]
    TreeNode<SemanticNode> MergePartials(TreeNode<SemanticNode> tree, string className);

    [Doc("Self-merge helper")]
    TreeNode<SemanticNode> MergeSelf(TreeNode<SemanticNode> source);

    [Doc("Moves cursor to the first node of a given kind")]
    TreeNode<SemanticNode> MoveCursorTo(TreeNode<SemanticNode> treeNode, SyntaxKind kind);

    [Doc("Moves non-extension static methods into a single class")]
    void MoveNonExtensionStaticMethodsInStaticClassesIntoSingleClass(TreeNode<SemanticNode> root);

    [Doc("Orders collection values")]
    TreeNode<SemanticNode> OrderCollectionValues(TreeNode<SemanticNode> originalTree);

    [Doc("Pulls children out of namespace declarations")]
    void PullChildrenOutOfNamespaces(TreeNode<SemanticNode> root);

    [Doc("Reloads tree from serialized code")]
    TreeNode<SemanticNode> Reload(TreeNode<SemanticNode> node);

    [Doc("Reloads tree and applies formatting")]
    TreeNode<SemanticNode> ReloadFormatted(TreeNode<SemanticNode> node);

    [Doc("Removes empty class declarations")]
    void RemoveEmptyClasses(TreeNode<SemanticNode> node);

    [Doc("Renames namespaces using a replace function")]
    void RenameNamespaces(TreeNode<SemanticNode> tree, Func<string, string> replace);

    [Doc("Replaces lowest-level types from source")]
    List<TreeNode<SemanticNode>> ReplaceExistingLowestLevelTypesFrom(TreeNode<SemanticNode> destination, TreeNode<SemanticNode> source, bool replace = true);

    [Doc("Saves tree to file")]
    void SaveFile(TreeNode<SemanticNode> node, string filePath);

    [Doc("Serializes tree to string")]
    string Serialize(TreeNode<SemanticNode>? root);

    [Doc("Serializes tree to file")]
    void SerializeFile(TreeNode<SemanticNode> node, string filePath);

    [Doc("Decides whether a class is large enough to split into partials")]
    bool ShouldDivideLargeClass(TreeNode<SemanticNode> tree);

    [Doc("Test-compiles the tree")]
    bool TestCompile(TreeNode<SemanticNode> node, string projectPath, string savePath = null);

    [Doc("Converts tree to C# code string")]
    string ToCode(TreeNode<SemanticNode> node);

    [Doc("Writes tree to file")]
    void ToFile(TreeNode<SemanticNode> node, string filePath);

    [Doc("Writes formatted tree to file")]
    void ToFormattedFile(TreeNode<SemanticNode> node, string filePath);
}

[ClassDoc("ExtensionsOfTreeNodeOfSyntaxElementNode")]
[Doc("Bridge from syntax-element tree to semantic tree")]
interface IExtensionsOfTreeNodeOfSyntaxElementNode
{
    [Doc("Builds full semantic tree from syntax-element tree")]
    TreeNode<SemanticNode> BuildTree(TreeNode<SyntaxElementNode> syntaxNode);

    [Doc("Converts syntax-element tree to semantic tree")]
    TreeNode<SemanticNode> ToTreeSemanticNode(TreeNode<SyntaxElementNode> syntaxRoot);
}

[ClassDoc("ExtensionsOfTreeNodeOfTokenNode")]
[Doc("Bridge from token tree to syntax-element tree")]
interface IExtensionsOfTreeNodeOfTokenNode
{
    [Doc("Converts token tree to syntax-element tree")]
    TreeNode<SyntaxElementNode> ToSyntaxElementTree(TreeNode<TokenNode> tokenNode);

    [Doc("Full conversion from token root to syntax-element tree")]
    TreeNode<SyntaxElementNode> ToTreeSyntaxElement(TreeNode<TokenNode>? tokenRoot);
}

[ClassDoc("FieldNode")]
[Doc("Semantic node for field declarations")]
interface IFieldNode
{
    [Doc("Sets the name node for the field")]
    void SetNameNode();
}

[ClassDoc("FileGen")]
[Doc("Generic file round-trip and project load utilities")]
interface IFileGen<T>
    where T : BaseNode<T>
{
    [Doc("Loads a project and converts it to code string")]
    string ProjectLoadToCode(string projectFile);

    [Doc("Loads a project and writes it to an output file")]
    string ProjectLoadToFile(string projectFile, string outputFile);

    [Doc("Round-trip test on a whole project")]
    void ProjectRoundTripTest(string projectFile);

    [Doc("Round-trips a code snippet")]
    (string newCode, TreeNode<T> tree) RoundTripCode(string code);

    [Doc("Round-trips a file")]
    (bool success, string newCode, TreeNode<T> tree) RoundTripFile(string file);

    [Doc("Simple round-trip test helper")]
    string TestRoundTrip(string code);

    [Doc("File round-trip test helper")]
    (bool success, string newCode, TreeNode<T> tree) TestRoundTripFile(string file);
}

[ClassDoc("FileScopedNamespaceNode")]
[Doc("Semantic node for file-scoped namespace declarations")]
interface IFileScopedNamespaceNode { }

[ClassDoc("ImplementTBD")]
[Doc("Placeholder for future implementation")]
interface IImplementTBD { }

[ClassDoc("IndexerNode")]
[Doc("Semantic node for indexer declarations")]
interface IIndexerNode
{
    [Doc("Sets the name node for the indexer")]
    void SetNameNode();
}

[ClassDoc("InterfaceNode")]
[Doc("Semantic node for interface declarations")]
interface IInterfaceNode
{
    [Doc("Type-specific keyword (interface)")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("KnownProjectPaths")]
[Doc("Hard-coded project paths for Evolver5")]
interface IKnownProjectPaths
{
    [Doc("EntryPoint instance for the Evolver5 project")]
    EntryPoint Evolver5 { get; }
}

[ClassDoc("MethodNode")]
[Doc("Semantic node for method declarations with static-extension helpers")]
interface IMethodNode
{
    [Doc("Full name including parameter type suffixes")]
    string FullName { get; }

    [Doc("True if the method is an extension method")]
    bool IsExtension { get; }

    [Doc("True if the method is static")]
    bool IsStatic { get; }

    [Doc("Converts a static extension method back to a normal instance method")]
    void ConvertFromStaticExtension();

    [Doc("Converts a normal method to a static extension method")]
    void ConvertToStaticExtension();

    [Doc("Extracts parameter type names")]
    List<string> GetParameterTypeNames();

    [Doc("Returns the return-type node")]
    TreeNode<SemanticNode>? GetReturnTypeNode();

    [Doc("Generates extension class name from 'this' parameter")]
    string GetStaticClassName();

    [Doc("Extracts the name of the 'this' parameter")]
    string GetThisParameterName();

    [Doc("Sets the name node for the method")]
    void SetNameNode();
}

[ClassDoc("NamedMemberNode")]
[Doc("Base for members that carry an access modifier")]
interface INamedMemberNode
{
    [Doc("Access modifier of the member")]
    AccessModifier Modifier { get; set; }

    [Doc("Default modifier when none is present")]
    AccessModifier DefaultModifier { get; }

    [Doc("Binds the tree node and sets modifier")]
    void SetTreeNode(TreeNode<SemanticNode> tree);

    [Doc("Sets modifier and inserts keyword if needed")]
    TreeNode<SemanticNode> SetModifier(AccessModifier modifier);

    [Doc("Reads modifier from the syntax tree")]
    void SetModifierFromTree();
}

[ClassDoc("NamedNode")]
[Doc("Base for any node that has a name")]
interface INamedNode
{
    [Doc("Sets name by collecting delimited tokens")]
    void SetName(HashSet<SyntaxKind> endDelimeters);

    [Doc("Sets the name node")]
    void SetNameNode();

    [Doc("Binds the tree node")]
    void SetTreeNode(TreeNode<SemanticNode> t);
}

[ClassDoc("NamespaceNode")]
[Doc("Semantic node for namespace declarations")]
interface INamespaceNode
{
    [Doc("Members inside the namespace")]
    List<SemanticNode> Members { get; }

    [Doc("Using directives inside the namespace")]
    List<UsingDirectiveNode> Usings { get; }

    [Doc("Default modifier for namespace members")]
    AccessModifier DefaultModifier { get; }

    [Doc("Attaches a child node")]
    void AttachChild(SemanticNode child);

    [Doc("Factory for a named namespace")]
    TreeNode<SemanticNode> Factory(string name);

    [Doc("Sets name from delimited tokens")]
    void SetNameNode();

    [Doc("Type-specific keyword (namespace)")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("OperatorNode")]
[Doc("Semantic node for operator overloads")]
interface IOperatorNode
{
    [Doc("Sets the name node for the operator")]
    void SetNameNode();
}

[ClassDoc("ParameterizedMemberNode")]
[Doc("Base for members that have a parameter list")]
interface IParameterizedMemberNode
{
    [Doc("Parameter list")]
    ParameterListNode? ParameterList { get; set; }

    [Doc("Attaches a child node")]
    void AttachChild(SemanticNode child);
}

[ClassDoc("ParameterizedMemberWithBodyNode")]
[Doc("Base for members that also have a body")]
interface IParameterizedMemberWithBodyNode
{
    [Doc("Body block")]
    BlockNode? Body { get; set; }

    [Doc("Attaches a child node")]
    void AttachChild(SemanticNode child);
}

[ClassDoc("ParameterListNode")]
[Doc("Semantic node for a parameter list")]
interface IParameterListNode
{
    [Doc("List of parameters")]
    List<ParameterNode> Parameters { get; }

    [Doc("Attaches a child parameter")]
    void AttachChild(SemanticNode child);
}

[ClassDoc("ParameterNode")]
[Doc("Semantic node for a single method/constructor parameter")]
interface IParameterNode
{
    [Doc("Whether the parameter has the 'in' modifier")]
    bool IsIn { get; }

    [Doc("Whether the parameter has the 'out' modifier")]
    bool IsOut { get; }

    [Doc("Whether the parameter has the 'params' modifier")]
    bool IsParams { get; }

    [Doc("Whether the parameter has the 'ref' modifier")]
    bool IsRef { get; }

    [Doc("Whether the parameter has the 'this' modifier")]
    bool IsThis { get; }

    [Doc("All modifiers on the parameter")]
    List<SyntaxKind> Modifiers { get; }

    [Doc("Type node")]
    TreeNode<SemanticNode>? TypeNode { get; set; }

    [Doc("Textual representation of the type")]
    string TypeText { get; set; }

    [Doc("Sets the name node")]
    void SetNameNode();

    [Doc("Binds the tree node and parses modifiers/type")]
    void SetTreeNode(TreeNode<SemanticNode> t);
}

[ClassDoc("PropertyNode")]
[Doc("Semantic node for property declarations")]
interface IPropertyNode
{
    [Doc("Sets the name node for the property")]
    void SetNameNode();
}

[ClassDoc("RecordNode")]
[Doc("Semantic node for record and record struct declarations")]
interface IRecordNode
{
    [Doc("Type-specific keyword")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("RegionEndNode")]
[Doc("Semantic node for #endregion")]
interface IRegionEndNode
{
    [Doc("Factory for a new #endregion node")]
    TreeNode<SemanticNode> Factory();
}

[ClassDoc("RegionNode")]
[Doc("Base for #region / #endregion nodes")]
interface IRegionNode
{
    [Doc("Factory for a named region")]
    TreeNode<SemanticNode> Factory(string name);
}

[ClassDoc("RegionStartNode")]
[Doc("Semantic node for #region")]
interface IRegionStartNode
{
    [Doc("Name of the region")]
    string? Name { get; set; }

    [Doc("Factory for a new #region node")]
    TreeNode<SemanticNode> Factory(string regionName);
}

[ClassDoc("SemanticNode")]
[Doc("Base semantic node with full-name, modifier, and tree binding logic")]
interface ISemanticNode
{
    [Doc("True if this node is only a child (no own members)")]
    bool ChildOnly { get; }

    [Doc("Full dotted name of the node")]
    string FullName { get; }

    [Doc("True if this node has direct named children")]
    bool HasDirectNamedChildren { get; }

    [Doc("True if the node has a name")]
    bool HasName { get; }

    [Doc("Node that holds members")]
    TreeNode<SemanticNode> MembersNode { get; }

    [Doc("Access modifier")]
    AccessModifier Modifier { get; set; }

    [Doc("Name of the node")]
    string? Name { get; set; }

    [Doc("Full name of the parent")]
    string ParentFullName { get; }

    [Doc("First non-compilation-unit parent")]
    TreeNode<SemanticNode> ParentNonContainer { get; }

    [Doc("List of parent names")]
    List<string> Parents { get; }

    [Doc("Value representation (for simple nodes)")]
    string Value { get; }

    [Doc("Attaches a child semantic node")]
    void AttachChild(SemanticNode child);

    [Doc("Deletes the entire declaration")]
    void DeleteDeclaration();

    [Doc("Returns declaration nodes")]
    List<TreeNode<SemanticNode>> GetDeclaration();

    [Doc("Sets base class from BaseTypes")]
    void SetBaseClass();

    [Doc("Binds the tree node")]
    void SetTreeNode(TreeNode<SemanticNode> tree);

    [Doc("Human-readable ToString")]
    string ToString();
}

[ClassDoc("SemanticTree")]
[Doc("Serializer that converts between C# text and SemanticNode trees")]
interface ISemanticTree
{
    [Doc("Deserializes code into a semantic tree")]
    TreeNode<SemanticNode> Deserialize(string code);

    [Doc("Deserializes code (optionally formatted)")]
    TreeNode<SemanticNode> DeserializeCode(string code, bool fmt = false);

    [Doc("Deserializes a declaration fragment")]
    TreeNode<SemanticNode> DeserializeDeclaration(string code, bool fmt = false);

    [Doc("Loads and deserializes a file")]
    TreeNode<SemanticNode> DeserializeFile(string file, bool fmt = false);

    [Doc("Alias for DeserializeFile")]
    TreeNode<SemanticNode> FromFile(string file, bool fmt = false);

    [Doc("Serializes a tree back to code")]
    string Serialize(TreeNode<SemanticNode> tree);
}

[ClassDoc("Serializer_SyntaxElement")]
[Doc("Serializer for the intermediate syntax-element tree")]
interface ISerializer_SyntaxElement
{
    [Doc("Deserializes code to syntax-element tree")]
    TreeNode<SyntaxElementNode> Deserialize(string code);

    [Doc("Serializes syntax-element tree to code")]
    string Serialize(TreeNode<SyntaxElementNode>? root);
}

[ClassDoc("Serializer_TokenNode")]
[Doc("Serializer for the lowest-level token tree")]
interface ISerializer_TokenNode
{
    [Doc("Deserializes code to token tree")]
    TreeNode<TokenNode> Deserialize(string code);

    [Doc("Serializes token tree to code")]
    string Serialize(TreeNode<TokenNode>? root);
}

[ClassDoc("SimpleMemberAccessNode")]
[Doc("Semantic node for simple member access expressions")]
interface ISimpleMemberAccessNode
{
    [Doc("Value of the member access")]
    string Value { get; }

    [Doc("Binds the tree node")]
    void SetTreeNode(TreeNode<SemanticNode> tree);
}

[ClassDoc("StaticHelpers")]
[Doc("Miscellaneous static helper functions")]
interface IStaticHelpers
{
    [Doc("Logs successful extraction of methods into a partial class")]
    void LogSuccessfulExtraction(string interfaceName, int methodCount);

    [Doc("Checks whether class or interface name is invalid")]
    bool NamesAreInvalid(string className, string interfaceName);
}

[ClassDoc("StructNode")]
[Doc("Semantic node for struct declarations")]
interface IStructNode
{
    [Doc("Type-specific keyword")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("SyntaxElementNode")]
[Doc("Intermediate node between token tree and semantic tree")]
interface ISyntaxElementNode
{
    [Doc("Identifier (if present)")]
    string? Identifier { get; }

    [Doc("Roslyn SyntaxKind")]
    SyntaxKind Kind { get; }

    [Doc("Text content")]
    string? Text { get; }

    [Doc("Creates the corresponding SemanticNode")]
    SemanticNode CreateSemantic();

    [Doc("ToString representation")]
    string ToString();
}

[ClassDoc("SyntaxKindGroups")]
[Doc("Static sets of SyntaxKind used throughout the system")]
interface ISyntaxKindGroups { }

[ClassDoc("SyntaxLoader")]
[Doc("MSBuild workspace and project loader")]
interface ISyntaxLoader
{
    [Doc("Creates an MSBuildWorkspace with design-time properties")]
    MSBuildWorkspace CreateWorkspace();

    [Doc("Disposes workspace resources")]
    void Dispose();

    [Doc("Loads a single .cs file")]
    SyntaxTree LoadFile(string csFilePath);

    [Doc("Loads all syntax trees from a project")]
    IReadOnlyList<SyntaxTree> LoadSingleProject(string projectPath);

    [Doc("Opens a project")]
    Project OpenProject(string projectPath);
}

[ClassDoc("TokenNode")]
[Doc("Lowest-level token node with trivia")]
interface ITokenNode
{
    [Doc("Identifier if present")]
    string? Identifier { get; }

    [Doc("Roslyn SyntaxKind")]
    SyntaxKind Kind { get; }

    [Doc("Leading trivia")]
    IReadOnlyList<SyntaxElementNode> LeadingTrivia { get; set; }

    [Doc("Original syntax node (if not a token)")]
    SyntaxNode? OriginalSyntaxNode { get; }

    [Doc("Token text")]
    string? Text { get; }

    [Doc("Trailing trivia")]
    IReadOnlyList<SyntaxElementNode> TrailingTrivia { get; set; }

    [Doc("Gets full trivia element at index")]
    SyntaxElementNode? GetFullTrivia(int index, bool isLeading = true);

    [Doc("ToString with optional trivia info")]
    string ToString(bool showTrivia);

    [Doc("Converts token + trivia to SyntaxElement list")]
    List<SyntaxElementNode> ToSyntaxElements();
}

[ClassDoc("TreeNode")]
[Doc("Generic tree node with children, parent, and traversal helpers")]
interface ITreeNode<T>
    where T : BaseNode<T>
{
    [Doc("Child nodes")]
    List<TreeNode<T>> Children { get; set; }

    [Doc("True if this is the root")]
    bool IsRoot { get; }

    [Doc("Parent node")]
    TreeNode<T>? Parent { get; set; }

    [Doc("Value payload")]
    T? Value { get; set; }

    [Doc("Adds a new child value")]
    TreeNode<T> AddChild(T child);

    [Doc("Adds an existing child tree node")]
    void AddChild(TreeNode<T> child);

    [Doc("Deep clone of the subtree")]
    TreeNode<T> DeepClone();

    [Doc("Deep equality check")]
    bool DeepEqual(TreeNode<T>? other, IEqualityComparer<T>? valueComparer = null);

    [Doc("Deletes the node (and optionally subtree)")]
    void Delete(DeleteType deleteType);

    [Doc("Deletes consecutive matching nodes")]
    void DeleteConsecutive(Func<TreeNode<T>, bool> predicate, DeleteType deleteType = DeleteType.SingleNode, int nStart = 1);

    [Doc("Recursive delete matching predicate")]
    void DeleteRecursive(Func<TreeNode<T>, bool> predicate, DeleteType deleteType = DeleteType.SingleNode);

    [Doc("Deletes nodes until a stop predicate")]
    void DeleteUntil(Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> untilPredicate, DeleteType deleteType = DeleteType.SingleNode);

    [Doc("Finds consecutive runs matching predicate")]
    IEnumerable<List<TreeNode<T>>> FindConsecutive(Func<TreeNode<T>, bool> predicate);

    [Doc("Finds first/last child pairs at every level")]
    List<(TreeNode<T> First, TreeNode<T> Last)> FindFirstLastChildPairs();

    [Doc("Find-skip helper")]
    IEnumerable<TreeNode<T>> FindSkip(Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> skipPredicate);

    [Doc("Find-skip-stop helper")]
    IEnumerable<TreeNode<T>> FindSkipStop(Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> skipPredicate, Func<TreeNode<T>, bool> stopPredicate);

    [Doc("FindWhere helper")]
    IEnumerable<TreeNode<T>> FindWhere(Func<TreeNode<T>, bool> predicate);

    [Doc("FindWhereStop helper")]
    IEnumerable<TreeNode<T>> FindWhereStop(Func<TreeNode<T>, bool> predicate, Func<TreeNode<T>, bool> untilPredicate);

    [Doc("Flattens the entire subtree")]
    IEnumerable<TreeNode<T>> Flatten();

    [Doc("Next sibling")]
    TreeNode<T>? GetNextNode();

    [Doc("Previous sibling or parent")]
    TreeNode<T>? GetPreviousNode();

    [Doc("Inserts a new node after this one")]
    void InsertAfter(TreeNode<T> newNode);

    [Doc("Inserts a new node before this one")]
    void InsertBefore(TreeNode<T> newNode);

    [Doc("Relative insert (before/after)")]
    void InsertRelative(TreeNode<T> newNode, bool after = false);

    [Doc("Removes a direct child")]
    bool RemoveChild(TreeNode<T> child);

    [Doc("Removes this node from its parent")]
    void RemoveSelf();

    [Doc("Replaces this node with another")]
    void ReplaceNode(TreeNode<T> newNode);

    [Doc("Replaces self in parent")]
    void ReplaceSelf(TreeNode<T> replacement);

    [Doc("ToString (value.ToString())")]
    string ToString();
}

[ClassDoc("TriviaNode")]
[Doc("Base for trivia (comments, whitespace, directives)")]
interface ITriviaNode { }

[ClassDoc("TypeDeclarationNode")]
[Doc("Base for class/struct/record/interface/enum/namespace declarations")]
interface ITypeDeclarationNode
{
    [Doc("Base types inherited by this declaration")]
    HashSet<string> BaseTypes { get; }

    [Doc("Members container node")]
    TreeNode<SemanticNode> MembersNode { get; }

    [Doc("Sets BaseType from BaseTypes")]
    void SetBaseClass();

    [Doc("Binds tree and parses base list")]
    void SetTreeNode(TreeNode<SemanticNode> t);

    [Doc("Type-specific keyword")]
    SyntaxKind? GetTypeSpecificKeyword();
}

[ClassDoc("UnknownNode")]
[Doc("Fallback node for unrecognized syntax")]
interface IUnknownNode { }

[ClassDoc("UsingDirectiveNode")]
[Doc("Semantic node for using directives")]
interface IUsingDirectiveNode
{
    [Doc("Sets the name node")]
    void SetNameNode();
}