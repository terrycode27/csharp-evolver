using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json.Linq;
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

public class __Program
{
    public static void Main()
    {
        var path = KnownProjectPaths.Evolver5;
        var tree = path.LoadTree();

        SelfTest();
        MergeTest();
    }

    static void MergeTest()
    {
        var path = KnownProjectPaths.Evolver5;
        var source = path.LoadTree(@"MergeSource", "test");
        var dest = path.LoadTree(@"MergeDest", "test");
        dest.MergeSelf();
        dest.MergeFrom(source);
        dest.GroupByModifierKindName();
        dest.ToFormattedFile(path.GetFilePath("MergeResult", "test"));
    }

    static void SelfTest()
    {
        var path = KnownProjectPaths.Evolver5;
        path.TestSemanticSerializer();
        path.GroupByModifierKindName();
    }

    class KnownProjectPaths
    {
        public static _ProjectPaths Evolver5 => new("Evolver5");
    }
}

public partial class _ProjectPaths
{
    public static _ProjectPaths Create(string baseNamespace) => new(baseNamespace);

    public void ExtractFromNamespaces()
    {
        var root = LoadTree();
        root.PullChildrenOutOfNamespaces();
        root.TestCompile(ProjectFilePath);
        root.ToFile(ConsolidatedFilePath);
    }

    public void Format()
    {
        var code = File.ReadAllText(ConsolidatedFilePath);
        code = CSharpierFormatter.Format(code);
        File.WriteAllText(ConsolidatedFilePath, code);
    }

    public string GetFilePath(string name, string subdir)
    {
        if (!String.IsNullOrEmpty(subdir))
            subdir = $@"{subdir}\";
        var ret = Path.Combine(RootDirectory, $"{subdir}{BaseNamespace}_{name}.cs");
        return ret;
    }

    public string GetTestFilePath(int i)
    {
        return Path.Combine(RootDirectory, $"{BaseNamespace}_test_{i}.cs");
    }

    public void GroupByClassHierarchy()
    {
        var tree = LoadTree();
        var classes = tree.FindClasses();
        var grouped = BuildClassHierarchy(classes);
        tree.Children.Clear();
        grouped.ForEach(tree.AddChild);
        tree.ToFormattedFile(TestFilePath);
    }

    public void GroupByModifierKindName()
    {
        var originalTree = LoadTree();
        var peek = originalTree.GroupByModifierKindName();
        var treeWithConsecutiveGroups = peek.Tree;
        treeWithConsecutiveGroups.TestCompile(ProjectFilePath, ConsolidatedFilePath);
    }

    public void GroupStaticExtensionMethods()
    {
        var root = LoadTree();
        root.MoveNonExtensionStaticMethodsInStaticClassesIntoSingleClass();
        root.GroupStaticExtensionMethods();
        root = root.ReloadFormatted();
        // root.ToFormattedFile(this.ConsolidatedFilePath);
        //root = LoadTree();
        root.GroupByModifierKindName();
        root.SaveFile(this.ConsolidatedFilePath);
    }

    public TreeNode<SemanticNode> LoadTree()
    {
        return LoadTreeFromPath(ConsolidatedFilePath);
    }

    public TreeNode<SemanticNode> LoadTree(string name, string subdir = null)
    {
        var loadFile = GetFilePath(name, subdir);
        return LoadTreeFromPath(loadFile);
    }

    public TreeNode<SemanticNode> LoadTreeFromPath(string fullPath)
    {
        var tree = SemanticTree.DeserializeFile(fullPath);
        tree = tree.GroupConsecutiveChildren(t => t.Value.HasName);
        return tree;
    }

    public void OrderCollectionValues()
    {
        var originalTree = LoadTree();
        var treeWithConsecutiveGroups = originalTree.OrderCollectionValues();
        treeWithConsecutiveGroups.TestCompile(ProjectFilePath, ConsolidatedFilePath);
    }



    public void RefactorLargeClassIntoPartialsWithInterfaces(string className)
    {
        var tree = LoadTree();
        tree.DivideIntoPartialClassesByInterfaces(className);
        tree.GroupByModifierKindName();
        tree.ToFormattedFile(ConsolidatedFilePath);
    }

    public void TestSemanticSerializer()
    {
        var serializer = new SemanticTree();
        var fileGen = new FileGen<SemanticNode>(serializer);
        fileGen.TestRoundTripFile(ConsolidatedFilePath);
    }



    public _ProjectPaths(string baseNamespace, string root_directory = null)
    {
        if (root_directory == null)
            root_directory = AppDomain.CurrentDomain.GetSolutionBaseFolder();
        this.root_directory = root_directory;

        if (string.IsNullOrWhiteSpace(baseNamespace))
            throw new ArgumentException("Base namespace cannot be empty.", nameof(baseNamespace));

        BaseNamespace = baseNamespace.Trim();

        ProjectFilePath = Path.Combine(RootDirectory, $"{BaseNamespace}.csproj");
        ConsolidatedFilePath = Path.Combine(RootDirectory, $"{BaseNamespace}.cs");
        TestFilePath = Path.Combine(RootDirectory, $"{BaseNamespace}_test.cs");
        ClassFilePath = Path.Combine(RootDirectory, $"{BaseNamespace}_{Class_Prefix}.cs");
        MethodFilePath = Path.Combine(RootDirectory, $"{BaseNamespace}_{Method_Prefix}.cs");
    }

    public static string Class_Prefix = "Class";
    public static string Consolidated_Prefix = "";
    public static string Method_Prefix = "Method";
    public static string one_class_per_file_namespace = "CF";
    public static string one_namespace_per_file_namespace = "NF";

    public string BaseNamespace { get; }
    public string ClassFilePath { get; }
    public string ConsolidatedFilePath { get; }
    public string MethodFilePath { get; }

    public string NamespaceConsolidated => $"{BaseNamespace}";
    public string OneClassPerFileNamespace => $"{BaseNamespace}.{OneClassPerFileNamespaceSuffix}";
    public string OneClassPerFileNamespaceSuffix => one_class_per_file_namespace;
    public string OneNamespacePerFileNamespace =>
        $"{BaseNamespace}.{OneNamespacePerFileNamespaceSuffix}";
    public string OneNamespacePerFileNamespaceSuffix => one_namespace_per_file_namespace;
    public string ProjectFilePath { get; }
    public virtual string RootDirectory => Path.Combine(root_directory, BaseNamespace);
    public string TestFilePath { get; }

    private List<TreeNode<SemanticNode>> BuildClassHierarchy(List<ClassNode> classes)
    {
        var byName = classes.ToDictionary(c => c.Name, c => c.tree);
        var derivedMap = BuildDerivedMap(classes, byName);

        var roots = classes
            .Where(c => c.BaseType == null || !byName.ContainsKey(c.BaseType))
            .ToList();

        return roots
            .OrderBy(c => c.Name)
            .Select(r => r.tree.BuildRegionTree(derivedMap, byName))
            .ToList();
    }

    private Dictionary<string, List<TreeNode<SemanticNode>>> BuildDerivedMap(
        List<ClassNode> classes,
        Dictionary<string, TreeNode<SemanticNode>> byName
    )
    {
        var map = new Dictionary<string, List<TreeNode<SemanticNode>>>();
        foreach (var c in classes)
        {
            if (c.BaseType != null && byName.ContainsKey(c.BaseType))
            {
                if (!map.ContainsKey(c.BaseType))
                    map[c.BaseType] = new();
                map[c.BaseType].Add(c.tree);
            }
        }
        return map;
    }

    string root_directory;
}
public class BaseNode<T>
    where T : BaseNode<T>
{
    public TreeNode<T> tree;
}

public class BlockNode : SemanticNode
{
    public override void AttachChild(SemanticNode child)
    {
        Statements.Add(child);
    }

    public BlockNode(SyntaxElementNode b)
        : base(b) { }

    public List<SemanticNode> Statements { get; } = new();
}

public class ClassNode : TypeDeclarationNode
{
    public override void AttachChild(SemanticNode child)
    {
        base.AttachChild(child);
        if (child is DelegateNode del)
            Delegates.Add(del);
    }

    public void ConvertToInstanceClass()
    {
        if (!IsStaticClass)
            return; // already an instance class – nothing to do

        RemoveStaticKeywordFromDeclaration();
        ConvertMethodsFromStaticExtension();
    }

    public void ConvertToStaticExtensionClass()
    {
        if (IsStaticClass)
            return; // already static – nothing to do

        AddKeywordToDeclaration(SyntaxKind.StaticKeyword);
        ConvertMethodsToStaticExtension();
    }

    public ClassNode(SyntaxElementNode b)
        : base(b) { }

    public List<DelegateNode> Delegates { get; } = new();

    public bool HasMethods =>
        tree.FindWhere(t => t.Value.Kind == SyntaxKind.MethodDeclaration).Any();

    public bool IsStaticClass
    {
        get
        {
            var ret = tree.FindWhereStop(
                    t => t.Value.Kind == SyntaxKind.StaticKeyword,
                    t => t.Value.Kind == SyntaxKind.OpenBraceToken
                )
                .Any();

            return ret;
        }
    }

    protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.ClassKeyword;

    protected override void SetModifierFromTree()
    {
        ModifierNode = tree.FindWhereStop(
                t => SyntaxKindGroups.ModifierKinds.Contains(t.Value.Kind),
                t => t.Value.Kind == SyntaxKind.OpenBraceToken
            )
            .FirstOrDefault();
    }

    protected override AccessModifier DefaultModifier => AccessModifier.Private;

    void AddKeywordToDeclaration(SyntaxKind kind)
    {
        var classKeyword = tree.FindWhere(t => t.Value.Kind == SyntaxKind.ClassKeyword)
            .FirstOrDefault();
        var val = kind.ToString().ToLower().Replace("keyword", "");
        string keywordText = val + " ";

        var element = new SyntaxElementNode(kind, keywordText);
        var semantic = new UnknownNode(element);
        var newNode = new TreeNode<SemanticNode>(semantic);
        semantic.SetTreeNode(newNode);
        classKeyword.InsertBefore(newNode);
    }

    private void ConvertMethodsFromStaticExtension()
    {
        foreach (var m in tree.GetMethods())
        {
            ((MethodNode)m).ConvertFromStaticExtension();
        }
    }

    void ConvertMethodsToStaticExtension()
    {
        foreach (var m in tree.GetMethods())
        {
            m.ConvertToStaticExtension();
        }
    }

    private void RemoveStaticKeywordFromDeclaration()
    {
        var staticKeyword = tree.FindWhere(t => t.Value.Kind == SyntaxKind.StaticKeyword)
            .FirstOrDefault();

        staticKeyword?.Delete(DeleteType.SingleNode);
    }
}

public class CommentNode : TriviaNode
{
    public CommentNode(SyntaxElementNode b)
        : base(b) { }
}

public class CompilationUnitNode : SemanticNode
{
    public override void AttachChild(SemanticNode child)
    {
        if (child is UsingDirectiveNode u)
            Usings.Add(u);
        else
            Members.Add(child);
    }

    public static TreeNode<SemanticNode> Factory()
    {
        return new TreeNode<SemanticNode>(new CompilationUnitNode());
    }

    public CompilationUnitNode()
        : base(new SyntaxElementNode(SyntaxKind.CompilationUnit, null)) { }

    public CompilationUnitNode(SyntaxElementNode b)
        : base(b) { }

    public List<object> KeyPath;

    public List<SemanticNode> Members { get; } = new();
    public List<UsingDirectiveNode> Usings { get; } = new();
}

public class CompilationUtil
{
    public static (
        bool Success,
        ImmutableArray<Diagnostic> Diagnostics,
        ImmutableArray<Diagnostic> Errors
    ) Compile(string code, string projectFile)
    {
        var project = SyntaxLoader.OpenProject(projectFile);
        return Compile(code, project);
    }

    public static (
        bool Success,
        ImmutableArray<Diagnostic> Diagnostics,
        ImmutableArray<Diagnostic> Errors
    ) Compile(string code, Project project)
    {
        var projectCompilation = project.GetCompilationAsync().Result;
        if (projectCompilation == null)
        {
            throw new Exception();
        }

        var syntaxTree = CSharpSyntaxTree.ParseText(code, path: "DynamicCode.cs");
        var compilation = CSharpCompilation.Create(
            assemblyName: "RoundTripAssembly",
            syntaxTrees: new[] { syntaxTree },
            references: projectCompilation.References,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        using var ms = new MemoryStream();
        var emitResult = compilation.Emit(ms);
        var errors = emitResult
            .Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToImmutableArray();
        bool success = errors.IsEmpty;
        return (success, emitResult.Diagnostics, errors);
    }

    public static bool RoundTripCompilation(
        string originalCode,
        string regeneratedCode,
        string csprojFileName
    )
    {
        Microsoft.CodeAnalysis.Project project = SyntaxLoader.OpenProject(csprojFileName);
        var oRes = Compile(originalCode, project);
        if (!oRes.Success)
            throw new Exception("Original code not compiling");
        var rRes = Compile(regeneratedCode, project);
        if (!rRes.Success)
            throw new Exception("Stubbed code not compiling");
        var ret = oRes.Success == rRes.Success == true;
        return ret;
    }
}

public class ConstructorNode : ParameterizedMemberWithBodyNode
{
    public ConstructorNode(SyntaxElementNode b)
        : base(b) { }
}

public class CSharpierFormatter
{
    public static string Format(string code)
    {
        var result = formatMethod.Invoke(formatter, new[] { code, options });

        var formatted = (string)result.GetType().GetProperty("Code").GetValue(result);

        return formatted;
    }

    public static void Unload()
    {
        if (context != null)
        {
            context.Unload();
            context = null;
            formatMethod = null;
            formatter = null;
            options = null;
        }
    }

    private static void BecauseNewerRoslynBreaksCSharpier(string path)
    {
        var dll = Directory.Exists(path) ? Path.Combine(path, "CSharpier.Core.dll") : path;

        context = new AssemblyLoadContext("CSharpierIsolated", isCollectible: true);

        var assembly = context.LoadFromAssemblyPath(dll);
        foreach (var depName in new[] { "Microsoft.CodeAnalysis", "Microsoft.CodeAnalysis.CSharp" })
        {
            var depPath = Path.Combine(Path.GetDirectoryName(dll), depName + ".dll");
            if (File.Exists(depPath))
                context.LoadFromAssemblyPath(depPath);
        }

        var type = assembly.GetType("CSharpier.Core.CSharp.CSharpFormatter");

        formatMethod = type.GetMethods()
            .First(m => m.Name == "Format" && m.GetParameters().Length == 2);
        return;
    }

    private static void Initialize()
    {
        BecauseNewerRoslynBreaksCSharpier(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, csharpierPath)
        );
    }

    static CSharpierFormatter()
    {
        Initialize();
    }

    private static AssemblyLoadContext context;
    private static string csharpierPath = @"_dependency\CSharpier";
    private static MethodInfo formatMethod;
    private static object formatter;
    private static object options;
}
public class DelegateNode : TypeDeclarationNode
{
    public override void AttachChild(SemanticNode child)
    {
        if (child is ParameterListNode pl)
            ParameterList = pl;
    }

    public DelegateNode(SyntaxElementNode b)
        : base(b) { }

    public ParameterListNode? ParameterList { get; set; }

    protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.DelegateKeyword;
}

public class DestructorNode : ParameterizedMemberWithBodyNode
{
    public DestructorNode(SyntaxElementNode b)
        : base(b) { }
}

public class EnumMemberNode : NamedNode
{
    public EnumMemberNode(SyntaxElementNode b)
        : base(b) { }
}

public class EnumNode : TypeDeclarationNode
{
    public override void AttachChild(SemanticNode child)
    {
        if (child is EnumMemberNode em)
            Members.Add(em);
    }

    public EnumNode(SyntaxElementNode b)
        : base(b) { }

    public List<EnumMemberNode> Members { get; } = new();

    protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.EnumKeyword;
}

public class EventFieldNode : NamedMemberNode
{
    public override void SetNameNode()
    {
        NameNode = tree.FindNameAfterTypeBeforeSemicolon();
    }

    public EventFieldNode(SyntaxElementNode b)
        : base(b) { }
}

public class EventNode : NamedMemberNode
{
    public EventNode(SyntaxElementNode b)
        : base(b) { }
}

public class ExpressionBodyNode : SemanticNode
{
    public ExpressionBodyNode(SyntaxElementNode b)
        : base(b) { }
}

public static class ExtensionsMethodNode
{

    // Also add this once (very useful)
    public static IEnumerable<TreeNode<SemanticNode>> Ancestors(this TreeNode<SemanticNode> node)
    {
        var current = node.Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    // Tiny helper you probably want anyway
    public static TreeNode<SemanticNode> GetContainingTypeDeclaration(this TreeNode<SemanticNode> node)
    {
        var current = node.Parent;
        while (current != null)
        {
            if (current.Value is ClassNode or StructNode or RecordNode or InterfaceNode)
                return current;
            current = current.Parent;
        }
        return null;
    }
    public static bool IsCalledFromOutsideClass(this MethodNode method, TreeNode<SemanticNode> solutionRoot)
    {
        if (string.IsNullOrEmpty(method.Name)) return false;

        var declaringClassNode = method.tree.GetContainingTypeDeclaration();
        if (declaringClassNode == null) return false; // weird case

        var methodName = method.Name;

        // Find every identifier with the same name
        foreach (var site in solutionRoot.FindWhere(t =>
            t.Value.Kind == SyntaxKind.IdentifierToken &&
            t.Value.Text == methodName))
        {
            if (!IsActualMethodCall(site)) continue;           // must be followed by (
            if (IsInsideSameMethod(site, method.tree)) continue; // ignore recursion inside itself
            if (IsInsideSameClass(site, declaringClassNode)) continue;

            return true; // found an external call!
        }

        return false;
    }

    public static bool IsInsideSameClass(this TreeNode<SemanticNode> site, TreeNode<SemanticNode> classNode)
    {
        return site.Ancestors().Any(a => a == classNode);
    }

    private static bool IsActualMethodCall(TreeNode<SemanticNode> identifierNode)
    {
        var next = identifierNode.GetNextNode();
        return next?.Value.Kind == SyntaxKind.OpenParenToken;
    }

    private static bool IsInsideSameMethod(TreeNode<SemanticNode> site, TreeNode<SemanticNode> methodNode)
    {
        return site.Ancestors().Any(a => a == methodNode);
    }
}

public static class ExtensionsOfAccessModifier
{
    public static SyntaxKind ToSyntaxKind(this AccessModifier modifier)
    {
        return modifier switch
        {
            AccessModifier.Public => SyntaxKind.PublicKeyword,
            AccessModifier.Protected => SyntaxKind.ProtectedKeyword,
            AccessModifier.Internal => SyntaxKind.InternalKeyword,
            AccessModifier.Private => SyntaxKind.PrivateKeyword,
            _ => throw new NotImplementedException(),
        };
    }
}

public static class ExtensionsOfAppDomain
{
    public static string GetSolutionBaseFolder(this AppDomain domain)
    {
        ArgumentNullException.ThrowIfNull(domain);

        var dir = new DirectoryInfo(domain.BaseDirectory);
        int maxLevels = 10;

        while (dir != null && maxLevels-- > 0)
        {
            if (dir.GetFiles("*.cs", SearchOption.TopDirectoryOnly).Any())
            {
                return dir.Parent.FullName;
            }

            dir = dir.Parent;
        }

        return domain.BaseDirectory.TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar
        );
    }
}

public static class ExtensionsOfIEnumerableOfT
{

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (action == null)
            throw new ArgumentNullException(nameof(action));
        foreach (var item in source)
            action(item);
    }

    public static IEnumerable<T> GroupByPredicates<T>(
        this IEnumerable<T> items,
        List<Func<T, object>> predicates
    )
    {
        IEnumerable<T> ApplyGrouping(int index, IEnumerable<T> currentItems)
        {
            if (index >= predicates.Count)
                return currentItems;

            return currentItems
                .GroupBy(predicates[index])
                .OrderBy(g => g.Key)
                .SelectMany(g => ApplyGrouping(index + 1, g));
        }

        return ApplyGrouping(0, items);
    }
    public static List<List<T>> GroupConsecutive<T>(
        this IEnumerable<T> items,
        Func<T, bool> predicate
    )
    {
        var result = new List<List<T>>();
        var current = new List<T>();

        foreach (var item in items)
        {
            if (current.Count == 0 || predicate(item) == predicate(current.Last()))
                current.Add(item);
            else
            {
                result.Add(current);
                current = new List<T> { item };
            }
        }

        if (current.Count > 0)
            result.Add(current);

        return result;
    }
}

public static class ExtensionsOfIEnumerableOfTreeNodeOfSemanticNode
{

    public static IEnumerable<TreeNode<SemanticNode>> DeleteKind(
        this IEnumerable<TreeNode<SemanticNode>> en,
        SyntaxKind kind,
        DeleteType deleteType = DeleteType.SingleNode
    )
    {
        return en.DeleteKinds(new HashSet<SyntaxKind>() { kind }, deleteType);
    }

    public static IEnumerable<TreeNode<SemanticNode>> DeleteKinds(
        this IEnumerable<TreeNode<SemanticNode>> en,
        HashSet<SyntaxKind> kinds,
        DeleteType deleteType = DeleteType.SingleNode
    )
    {
        foreach (var x in en)
        {
            yield return x;
            x.Delete(DeleteType.SingleNode);
        }
    }

    public static List<TreeNode<SemanticNode>> NotPresentIn(
        this IEnumerable<TreeNode<SemanticNode>> candidates,
        IEnumerable<TreeNode<SemanticNode>> existing
    )
    {
        var existingNames = existing.Select(e => e.Value.FullName).ToHashSet();
        return candidates.Where(c => !existingNames.Contains(c.Value.FullName)).ToList();
    }
    public static Dictionary<string, TreeNode<SemanticNode>> ToDictionaryByFullName(
        this IEnumerable<TreeNode<SemanticNode>> nodes
    ) => nodes.ToDictionary(t => t.Value.FullName);
}

public static class ExtensionsOfIEnumerableOfTSource
{
    public static Dictionary<TKey, TSource> GroupBySingle<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector
    )
    {
        return source
            .GroupBy(keySelector)
            .ToDictionary(group => group.Key, group => group.Single());
    }
}

public static class ExtensionsOfListOfT
{
    public static List<T> Interleave<T, TKey>(this List<T> source, Func<T, TKey?> getKey)
        where TKey : IComparable<TKey>
    {
        var movables = source.Where(item => getKey(item) != null).OrderBy(getKey).ToList();

        var index = 0;
        var result = new List<T>(source.Count);

        foreach (var item in source)
        {
            if (getKey(item) != null)
                result.Add(movables[index++]);
            else
                result.Add(item);
        }

        return result;
    }
}

public static class ExtensionsOfListOfTreeNodeOfSemanticNode
{

    public static TreeNode<SemanticNode> CreateNewPartialClassForInterface(
        this List<TreeNode<SemanticNode>> methodsToMove,
        string className,
        string interfaceName
    )
    {
        string template =
            $@"public partial class {className} : {interfaceName}
{{
}}";

        var partialRoot = SemanticTree.DeserializeCode(template);
        var newPartialClass = partialRoot
            .FindWhere(t => t.Value.Kind == SyntaxKind.ClassDeclaration)
            .First();
        var newBody = newPartialClass
            .FindWhere(t => t.Value.Kind == SyntaxKind.OpenBraceToken)
            .First();

        foreach (var method in methodsToMove)
        {
            method.RemoveSelf();
            newBody.AddChild(method);
        }

        newPartialClass.InitializeAllNodes();
        return newPartialClass;
    }

    public static IEnumerable<TreeNode<SemanticNode>> FindDelimitedText(
        this List<TreeNode<SemanticNode>> roots,
        HashSet<SyntaxKind> delimeters
    )
    {
        var include = new HashSet<SyntaxKind> { SyntaxKind.DotToken, SyntaxKind.IdentifierToken };
        return roots.FindText(include, delimeters);
    }

    public static IEnumerable<TreeNode<SemanticNode>> FindText(
        this List<TreeNode<SemanticNode>> roots,
        HashSet<SyntaxKind> include,
        HashSet<SyntaxKind> stopAt
    )
    {
        return roots
            .FindWhereStop(t => include.Contains(t.Value.Kind), t => stopAt.Contains(t.Value.Kind))
            .Where(t => t.Value != null && t.Value.Text != null && t.Value.Text != string.Empty)
            .ToList();
    }

    public static List<string> GetStrings(this List<TreeNode<SemanticNode>> en)
    {
        return en.Select(t => t.Value.Text.Trim()).ToList();
    }

    public static List<List<TreeNode<SemanticNode>>> GroupedByModifierKind(
        this List<TreeNode<SemanticNode>> children
    ) =>
        children
            .GroupBy(t => new { t.Value.Modifier, Kind = t.Value.Kind.ToHandled() })
            .Select(g => g.ToList())
            .ToList();

    public static void SetStaticExtensionMethodsPublic(
        this List<TreeNode<SemanticNode>> extensionMethods
    )
    {
        var casted = extensionMethods.Select(t => (MethodNode)t.Value).ToList();
        var privateMethods = casted.Where(t => t.Modifier == AccessModifier.Private).ToList();
        foreach (var p in privateMethods)
        {
            p.Modifier = AccessModifier.Public;
        }
    }

    public static List<TreeNode<SemanticNode>> SortedByModifierKindName(
        this List<TreeNode<SemanticNode>> children
    ) =>
        children
            .OrderBy(t => t.Value.Modifier)
            .ThenBy(t => t.Value.Kind.ToHandled())
            .ThenBy(t => t.Value.Name ?? "")
            .ToList();
    public static TreeNode<SemanticNode> WrapInDeclaration(
        this List<TreeNode<SemanticNode>> methods,
        string declaration
    )
    {
        var root = SemanticTree.DeserializeCode(declaration);
        var body = root.FindWhere(t => t.Value.Kind == SyntaxKind.OpenBraceToken).First();
        foreach (var m in methods.OrderBy(m => m.Value.Name))
        {
            m.RemoveSelf();
            body.InsertAfter(m);
        }
        root = root.GroupConsecutiveChildren(t => t.Value.HasName);
        return root.Children.First();
    }
}

public static class ExtensionsOfListOfTreeNodeOfT
{

    public static IEnumerable<TreeNode<T>> FindSkip<T>(
        this List<TreeNode<T>> roots,
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> skipPredicate
    )
        where T : BaseNode<T>
    {
        if (roots == null || predicate == null || skipPredicate == null)
            yield break;
        foreach (var root in roots)
        {
            if (root == null)
                continue;
            if (skipPredicate(root))
                continue;
            if (predicate(root))
                yield return root;
            foreach (var result in root.Children.FindSkip(predicate, skipPredicate))
                yield return result;
        }
    }

    public static IEnumerable<TreeNode<T>> FindSkipStop<T>(
        this List<TreeNode<T>> roots,
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> skipPredicate,
        Func<TreeNode<T>, bool> stopPredicate
    )
        where T : BaseNode<T>
    {
        if (roots == null || predicate == null || skipPredicate == null || stopPredicate == null)
            yield break;
        foreach (var root in roots)
        {
            if (root == null)
                continue;
            if (stopPredicate(root))
                yield break;
            if (skipPredicate(root))
                continue;
            if (predicate(root))
                yield return root;
            foreach (
                var result in root.Children.FindSkipStop(predicate, skipPredicate, stopPredicate)
            )
                yield return result;
        }
    }

    public static IEnumerable<TreeNode<T>> FindWhere<T>(
        this List<TreeNode<T>> roots,
        Func<TreeNode<T>, bool> predicate
    )
        where T : BaseNode<T>
    {
        if (roots == null || predicate == null)
            yield break;
        foreach (var root in roots)
        {
            if (root == null)
                continue;
            if (root.Value != null && predicate(root))
                yield return root;
            foreach (var found in root.Children.FindWhere(predicate))
                yield return found;
        }
    }
    public static IEnumerable<TreeNode<T>> FindWhereStop<T>(
        this List<TreeNode<T>> roots,
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> untilPredicate
    )
        where T : BaseNode<T>
    {
        if (roots == null || predicate == null || untilPredicate == null)
            yield break;
        foreach (var f in roots.FindWhere(t => true))
        {
            if (untilPredicate(f))
                yield break;
            if (predicate(f))
                yield return f;
        }
    }
}

public static class ExtensionsOfstring
{

    public static string NormalizeLineEndings(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return text.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", "\r\n");
    }
    public static TreeNode<SemanticNode> ToTree(this string code)
    {
        return SemanticTree.DeserializeCode(code);
    }
}

public static class ExtensionsOfSyntaxKind
{

    public static SyntaxKindOrdering ToHandled(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.CompilationUnit => SyntaxKindOrdering.CompilationUnit,
            SyntaxKind.NamespaceDeclaration => SyntaxKindOrdering.NamespaceDeclaration,
            SyntaxKind.FileScopedNamespaceDeclaration =>
                SyntaxKindOrdering.FileScopedNamespaceDeclaration,
            SyntaxKind.ClassDeclaration => SyntaxKindOrdering.ClassDeclaration,
            SyntaxKind.StructDeclaration => SyntaxKindOrdering.StructDeclaration,
            SyntaxKind.RecordDeclaration => SyntaxKindOrdering.RecordDeclaration,
            SyntaxKind.InterfaceDeclaration => SyntaxKindOrdering.InterfaceDeclaration,
            SyntaxKind.EnumDeclaration => SyntaxKindOrdering.EnumDeclaration,
            SyntaxKind.DelegateDeclaration => SyntaxKindOrdering.DelegateDeclaration,
            SyntaxKind.MethodDeclaration => SyntaxKindOrdering.MethodDeclaration,
            SyntaxKind.ConstructorDeclaration => SyntaxKindOrdering.ConstructorDeclaration,
            SyntaxKind.DestructorDeclaration => SyntaxKindOrdering.DestructorDeclaration,
            SyntaxKind.PropertyDeclaration => SyntaxKindOrdering.PropertyDeclaration,
            SyntaxKind.FieldDeclaration => SyntaxKindOrdering.FieldDeclaration,
            SyntaxKind.EventFieldDeclaration => SyntaxKindOrdering.EventFieldDeclaration,
            SyntaxKind.EventDeclaration => SyntaxKindOrdering.EventDeclaration,
            SyntaxKind.OperatorDeclaration => SyntaxKindOrdering.OperatorDeclaration,
            SyntaxKind.IndexerDeclaration => SyntaxKindOrdering.IndexerDeclaration,
            SyntaxKind.EnumMemberDeclaration => SyntaxKindOrdering.EnumMemberDeclaration,
            SyntaxKind.Block => SyntaxKindOrdering.Block,
            SyntaxKind.ArrowExpressionClause => SyntaxKindOrdering.ArrowExpressionClause,
            SyntaxKind.UsingDirective => SyntaxKindOrdering.UsingDirective,
            SyntaxKind.SingleLineCommentTrivia => SyntaxKindOrdering.SingleLineCommentTrivia,
            SyntaxKind.MultiLineCommentTrivia => SyntaxKindOrdering.MultiLineCommentTrivia,
            SyntaxKind.RegionDirectiveTrivia => SyntaxKindOrdering.RegionDirectiveTrivia,
            SyntaxKind.EndRegionDirectiveTrivia => SyntaxKindOrdering.EndRegionDirectiveTrivia,
            SyntaxKind.WhitespaceTrivia => SyntaxKindOrdering.WhitespaceTrivia,
            SyntaxKind.EndOfLineTrivia => SyntaxKindOrdering.EndOfLineTrivia,
            SyntaxKind.None => SyntaxKindOrdering.None,
            _ => SyntaxKindOrdering.Unknown,
        };
    }
    public static AccessModifier ToModifier(this SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.PublicKeyword => AccessModifier.Public,
            SyntaxKind.ProtectedKeyword => AccessModifier.Protected,
            SyntaxKind.InternalKeyword => AccessModifier.Internal,
            SyntaxKind.PrivateKeyword => AccessModifier.Private,
            _ => throw new NotImplementedException(),
        };
    }
}

public static class ExtensionsOfSyntaxNode
{

    public static TreeNode<SemanticNode> ToTreeSemanticNode(this SyntaxNode node)
    {
        var ret = node.ToTreeTokenNode().ToTreeSyntaxElement().ToTreeSemanticNode();
        return ret;
    }
    public static TreeNode<TokenNode> ToTreeTokenNode(this SyntaxNode node)
    {
        var current = new TreeNode<TokenNode>(new TokenNode(node));
        foreach (var child in node.ChildNodesAndTokens())
        {
            if (child.IsNode)
            {
                var childNode = child.AsNode()!;
                if (childNode.FullSpan.Length == 0)
                    continue;
                current.AddChild(ToTreeTokenNode(childNode));
            }
            else if (child.IsToken)
            {
                var tok = child.AsToken();
                if (tok.Kind() is SyntaxKind.EndOfFileToken)
                    continue;
                current.AddChild(new TreeNode<TokenNode>(new TokenNode(tok)));
            }
        }

        return current;
    }
}

public static class ExtensionsOfT
{

    public static T[] AsArray<T>(this T item)
    {
        return [item];
    }

    public static List<T> AsList<T>(this T item)
    {
        return [item];
    }

    public static List<T> AsListOrEmpty<T>(this T? item)
        where T : class
    {
        return item is null ? [] : [item];
    }

    public static Queue<T> AsQueue<T>(this T item)
    {
        var queue = new Queue<T>();
        queue.Enqueue(item);
        return queue;
    }

    public static List<T> ToSingletonList<T>(this T item)
    {
        return [item];
    }
    public static IEnumerable<T> Yield<T>(this T item)
    {
        yield return item;
    }
}

public static class ExtensionsOfTreeNodeOfSemanticNode
{

    public static void AddMissingContainersAtDepth(
        this TreeNode<SemanticNode> destination,
        TreeNode<SemanticNode> source,
        int depth
    )
    {
        var missing = source
            .GetNamedNodesAtDepth(depth)
            .Where(t => !t.Value.ChildOnly)
            .NotPresentIn(destination.GetNamedNodes());

        foreach (var node in missing)
        {
            var parent = destination.FindParentByFullName(node.Value.ParentFullName);
            parent.Value.MembersNode.AddChild(node);
        }
    }

    public static void AddMissingContainersByDepthFrom(
        this TreeNode<SemanticNode> destination,
        TreeNode<SemanticNode> source
    )
    {
        var maxDepth = source.GetMaxParentCount();

        for (int depth = 0; depth <= maxDepth; depth++)
            destination.AddMissingContainersAtDepth(source, depth);
    }

    public static void AddNewLowestLevelTypesFrom(
        this TreeNode<SemanticNode> destination,
        List<TreeNode<SemanticNode>> source
    )
    {
        foreach (var sourceNode in source)
        {
            var parentFullName = sourceNode.Value.ParentFullName;
            var targetParent = destination.FindParentByFullName(parentFullName);
            targetParent.AddNewMember(sourceNode);
        }
    }

    public static void AddNewMember(
        this TreeNode<SemanticNode> parent,
        TreeNode<SemanticNode> member
    )
    {
        parent.Value.MembersNode.AddChild(member);
    }

    public static bool AlreadyHasPartialKeyword(this TreeNode<SemanticNode> classObj) =>
        classObj.FindWhere(t => t.Value.Kind == SyntaxKind.PartialKeyword).Any();

    public static List<string> AsCode<T>(this TreeNode<SemanticNode> tree)
    {
        var list = tree.Flatten().Select(t => t.ToCode()).ToList();
        return list;
    }

    public static TreeNode<SemanticNode> BuildRegionTree(
        this TreeNode<SemanticNode> classNode,
        Dictionary<string, List<TreeNode<SemanticNode>>> derivedMap,
        Dictionary<string, TreeNode<SemanticNode>> byName
    )
    {
        var className = ((ClassNode)classNode.Value).Name;
        var region = RegionStartNode.Factory(className);
        region.AddChild(classNode);

        if (derivedMap.TryGetValue(className, out var children))
        {
            children
                .OrderBy(c => ((ClassNode)c.Value).Name)
                .Select(c => BuildRegionTree(c, derivedMap, byName))
                .ToList()
                .ForEach(region.AddChild);
        }

        region.AddChild(RegionEndNode.Factory());
        return region;
    }

    public static int CountLinesOfCode(this TreeNode<SemanticNode> tree)
    {
        if (tree == null)
            return 0;

        string code = tree.ToCode();
        if (string.IsNullOrWhiteSpace(code))
            return 0;
        code = code.NormalizeLineEndings();
        var lines = code.Split(new[] { '\n' }, StringSplitOptions.None);
        return lines.Count(line => !string.IsNullOrWhiteSpace(line));
    }

    public static int CountMethods(this TreeNode<SemanticNode> tree)
    {
        if (tree == null)
            return 0;

        return tree.GetMethods().Count;
    }

    public static int CountStaticExtensionMethods(this TreeNode<SemanticNode> tree)
    {
        if (tree == null)
            return 0;

        return tree.FindStaticExtensionMethods().Count;
    }

    public static void DeleteComments<T>(this TreeNode<SemanticNode> root)
    {
        root.DeleteRecursive(t => SyntaxKindGroups.CommentTrivia.Contains(t.Value.Kind));
    }

    public static void DeleteCompilationUnit(this TreeNode<SemanticNode> tree)
    {
        foreach (
            var x in tree.FindWhere(t => t.Value.Kind == SyntaxKind.CompilationUnit)
                .Skip(1)
                .ToList()
        )
        {
            x.Delete(DeleteType.SingleNode);
        }
    }

    public static TreeNode<SemanticNode> DeleteExcessNewlines(this TreeNode<SemanticNode> root)
    {
        foreach (
            var group in root.FindConsecutive(t =>
                SyntaxKindGroups.WhitespaceTrivia.Contains(t.Value.Kind)
            )
        )
        {
            if (group.Count <= 2)
                continue;
            var lastNewline = group.FindLastIndex(t => t.Value.Kind == SyntaxKind.EndOfLineTrivia);
            if (lastNewline < 1)
                continue;
            for (int i = 0; i < lastNewline; i++)
                group[i].Delete(DeleteType.SingleNode);
        }

        return root;
    }

    public static IEnumerable<TreeNode<SemanticNode>> DeleteKind(
        this TreeNode<SemanticNode> tree,
        SyntaxKind kind,
        DeleteType deleteType = DeleteType.SingleNode
    )
    {
        return tree.DeleteKinds(new HashSet<SyntaxKind>() { kind }, deleteType);
    }

    public static IEnumerable<TreeNode<SemanticNode>> DeleteKinds(
        this TreeNode<SemanticNode> tree,
        HashSet<SyntaxKind> kinds,
        DeleteType deleteType = DeleteType.SingleNode
    )
    {
        foreach (var x in tree.FindWhere(t => kinds.Contains(t.Value.Kind)).ToList())
        {
            yield return x;
            x.Delete(DeleteType.SingleNode);
        }
    }

    public static void DivideIntoPartialClassByInterface(
        this TreeNode<SemanticNode> tree,
        TreeNode<SemanticNode> classObj,
        TreeNode<SemanticNode> interfaceDecl
    )
    {
        var className = classObj.Value.Name;
        var interfaceName = interfaceDecl.GetInterfaceName();

        if (StaticHelpers.NamesAreInvalid(className, interfaceName))
            return;

        classObj.EnsureClassIsMarkedPartial();

        var interfaceMethodNames = interfaceDecl.GetMethodNamesDeclaredInInterface();
        var methodsToExtract = classObj.FindImplementingMethodsInClass(interfaceMethodNames);

        if (methodsToExtract.Count == 0)
            return;

        var newPartialClass = methodsToExtract.CreateNewPartialClassForInterface(
            className,
            interfaceName
        );

        classObj.InsertAfter(newPartialClass);

        StaticHelpers.LogSuccessfulExtraction(interfaceName, methodsToExtract.Count);
    }

    public static void DivideIntoPartialClassesByInterfaces(
        this TreeNode<SemanticNode> tree,
        string className,
        string interfaceName = null
    )
    {
        var tnsn = tree.FindWhere(t =>
                t.Value.Kind == SyntaxKind.ClassDeclaration && t.Value.Name == className
            )
            .First();
        if (interfaceName == null)
            interfaceName = tnsn.Value.BaseType;
        var baseInterface = tree.FindWhere(t =>
                t.Value.Kind == SyntaxKind.InterfaceDeclaration && t.Value.Name == interfaceName
            )
            .First();
        var ia = (InterfaceNode)baseInterface.Value;
        foreach (var i in ia.BaseTypes)
        {
            var iDecl = tree.FindWhere(t =>
                    t.Value.Kind == SyntaxKind.InterfaceDeclaration && t.Value.Name == i
                )
                .First();
            DivideIntoPartialClassByInterface(tree, tnsn, iDecl);
        }
    }

    public static void EmptyMemberDeclarationsContents(
        this TreeNode<SemanticNode> tree,
        _ProjectPaths path
    )
    {
        var list = tree.FindWhere(t => t.Value.Kind == SyntaxKind.Block).ToList();
        foreach (var node in list)
        {
            node.Children.Where(t => SyntaxKindGroups.BodyElements.Contains(t.Value.Kind))
                .ToList()
                .ForEach(t => node.RemoveChild(t));
            var open = node.FindWhere(t => t.Value.Kind == SyntaxKind.OpenBraceToken).First();
            open.AddChild(
                new TreeNode<SemanticNode>(
                    new UnknownNode(
                        new SyntaxElementNode(
                            SyntaxKind.ThrowExpression,
                            "throw new NotImplementedException();"
                        )
                    )
                )
            );
        }

        tree.TestCompile(path.ProjectFilePath, path.MethodFilePath);
    }

    public static void EmptyTypeDeclarationsContents(
        this TreeNode<SemanticNode> tree,
        _ProjectPaths path
    )
    {
        var list = tree.FindWhere(t => SyntaxKindGroups.TypeDeclarations.Contains(t.Value.Kind))
            .ToList();
        foreach (var node in list)
        {
            node.Children.Where(t => t.Value.HasName).ToList().ForEach(t => node.RemoveChild(t));
        }

        tree.DeleteRecursive(t => t.Value.Kind == SyntaxKind.BaseList, DeleteType.NodeAndSubTree);
        tree.TestCompile(path.ProjectFilePath, path.ClassFilePath);
    }

    public static void EnsureClassIsMarkedPartial(this TreeNode<SemanticNode> classObj)
    {
        if (classObj.AlreadyHasPartialKeyword())
            return;

        var classKeyword = classObj.FindWhere(t => t.Value.Kind == SyntaxKind.ClassKeyword).First();
        classObj.InsertPartialKeywordBefore(classKeyword);
    }

    public static TreeNode<SemanticNode> ExtractFromAndRemoveNamespaces(this TreeNode<SemanticNode> tree)
    {
        var ns = tree.FindWhere(t => t.Value.Kind == SyntaxKind.NamespaceDeclaration).ToList();
        ns.ForEach(t =>
        {
            var memNode = t.Value.MembersNode;
            memNode.Parent.Parent.AddChild(memNode);
            t.RemoveSelf();
        });
        return tree;
    }

    public static List<ClassNode> FindClasses(this TreeNode<SemanticNode> tree)
    {
        return tree.FindKindVal(SyntaxKind.ClassDeclaration).Cast<ClassNode>().ToList();
    }

    public static List<TreeNode<SemanticNode>> FindClassNodes(this TreeNode<SemanticNode> node)
    {
        return node.FindWhere(t => t.Value.Kind == SyntaxKind.ClassDeclaration).ToList();
    }

    public static IEnumerable<TreeNode<SemanticNode>> FindDelimitedText(
        this TreeNode<SemanticNode> root,
        HashSet<SyntaxKind> delimeters
    )
    {
        if (root == null)
            return Enumerable.Empty<TreeNode<SemanticNode>>();
        return root.AsList().FindDelimitedText(delimeters);
    }

    public static List<TreeNode<SemanticNode>> FindImplementingMethodsInClass(
        this TreeNode<SemanticNode> classObj,
        HashSet<string> interfaceMethodNames
    ) =>
        classObj
            .FindWhere(t => t.Value.Kind == SyntaxKind.MethodDeclaration)
            .Where(m => interfaceMethodNames.Contains(((MethodNode)m.Value).Name))
            .ToList();

    public static List<TreeNode<SemanticNode>> FindKind(
        this TreeNode<SemanticNode> tree,
        SyntaxKind kind
    )
    {
        return tree.FindWhere(t => t.Value.Kind == kind).ToList();
    }

    public static List<TreeNode<SemanticNode>> FindKinds(
        this TreeNode<SemanticNode> tree,
        HashSet<SyntaxKind> kinds
    )
    {
        return tree.FindWhere(t => kinds.Contains(t.Value.Kind)).ToList();
    }

    public static List<SemanticNode> FindKindVal(this TreeNode<SemanticNode> tree, SyntaxKind kind)
    {
        return tree.FindKind(kind).Select(t => t.Value).ToList();
    }

    public static TreeNode<SemanticNode> FindNameAfterType(
        this TreeNode<SemanticNode> tree,
        HashSet<SyntaxKind> stopKinds
    )
    {
        return tree.FindSkipStop(
                t => t.Value.Kind == SyntaxKind.IdentifierToken,
                t =>
                    SyntaxKindGroups.ModifierKeywords.Contains(t.Value.Kind)
                    || SyntaxKindGroups.KindsContainingParameterType.Contains(t.Value.Kind)
                    || t.Value.Kind == SyntaxKind.AttributeList,
                t => stopKinds.Contains(t.Value.Kind)
            )
            .FirstOrDefault();
    }

    public static TreeNode<SemanticNode> FindNameAfterTypeBeforeSemicolon(
        this TreeNode<SemanticNode> tree
    )
    {
        var stopKinds = new HashSet<SyntaxKind> { SyntaxKind.SemicolonToken };
        return tree.FindNameAfterType(stopKinds);
    }

    public static TreeNode<SemanticNode> FindParentByFullName(
        this TreeNode<SemanticNode> root,
        string parentFullName
    )
    {
        if (string.IsNullOrEmpty(parentFullName))
            return root;

        return root.FindWhere(t =>
                t.Value.FullName == parentFullName && t.Value is not UsingDirectiveNode
            )
            .First();
    }

    public static List<ClassNode> FindStaticClasses(this TreeNode<SemanticNode> tree)
    {
        return tree.FindClasses().Where(t => t.IsStaticClass).ToList();
    }

    public static List<TreeNode<SemanticNode>> FindStaticExtensionMethods(
        this TreeNode<SemanticNode> tree
    )
    {
        var ret = tree.GetMethods()
            .Where(t => t.IsStatic && t.IsExtension)
            .Select(t => t.tree)
            .ToList();
        return ret;
    }

    public static IEnumerable<TreeNode<SemanticNode>> FindSyntaxKind(
        this TreeNode<SemanticNode> node,
        SyntaxKind kind
    )
    {
        return node.FindSyntaxKinds(new HashSet<SyntaxKind>() { kind });
    }

    public static IEnumerable<TreeNode<SemanticNode>> FindSyntaxKinds(
        this TreeNode<SemanticNode> node,
        HashSet<SyntaxKind> kinds
    )
    {
        var ret = node.FindWhere(t => kinds.Contains(t.Value.Kind));
        return ret;
    }

    public static List<(string code, TreeNode<SemanticNode> node)> FlattenToCode(
        this TreeNode<SemanticNode> tree
    )
    {
        var list = tree.Flatten();
        List<(string code, TreeNode<SemanticNode> node)> ret =
            new List<(string code, TreeNode<SemanticNode> node)>();
        foreach (var l in list)
        {
            ret.Add((l.ToCode(), l));
        }

        return ret;
    }

    public static TreeNode<SemanticNode> Format(this TreeNode<SemanticNode> tree)
    {
        var code = tree.ToCode();
        code = CSharpierFormatter.Format(code);
        tree = SemanticTree.DeserializeCode(code);
        return tree;
    }

    public static Dictionary<string, List<TreeNode<SemanticNode>>> GetDerivedClasses(
        this TreeNode<SemanticNode> tree
    )
    {
        var classes = tree.FindWhere(t => t.Value.Kind == SyntaxKind.ClassDeclaration)
            .ToDictionary(t => ((ClassNode)t.Value).Name, t => t);

        var directChildren = new Dictionary<string, List<TreeNode<SemanticNode>>>();
        foreach (var c in classes.Values)
        {
            var baseName = ((ClassNode)c.Value).BaseType;
            if (baseName != null && classes.ContainsKey(baseName))
            {
                if (!directChildren.ContainsKey(baseName))
                    directChildren[baseName] = new();
                directChildren[baseName].Add(c);
            }
        }

        var result = new Dictionary<string, List<TreeNode<SemanticNode>>>();

        void CollectAllDerived(string name, List<TreeNode<SemanticNode>> list)
        {
            if (directChildren.TryGetValue(name, out var children))
                foreach (var child in children)
                {
                    list.Add(child);
                    CollectAllDerived(((ClassNode)child.Value).Name, list);
                }
        }

        foreach (var name in classes.Keys)
        {
            var list = new List<TreeNode<SemanticNode>>();
            CollectAllDerived(name, list);
            list.Add(classes[name]);
            result[name] = list;
        }

        return result;
    }

    public static string GetInterfaceName(this TreeNode<SemanticNode> interfaceDecl) =>
        ((InterfaceNode)interfaceDecl.Value).Name ?? string.Empty;

    public static List<TreeNode<SemanticNode>> GetLowestLevelTypes(
        this TreeNode<SemanticNode> root
    ) => root.FindWhere(t => t.Value.HasName && t.Value.ChildOnly).ToList();

    public static int GetMaxParentCount(this TreeNode<SemanticNode> root) =>
        root.GetNamedNodes().Max(t => t.Value.Parents.Count);

    public static HashSet<string> GetMethodNamesDeclaredInInterface(
        this TreeNode<SemanticNode> interfaceDecl
    ) =>
        interfaceDecl
            .FindWhere(t => t.Value.Kind == SyntaxKind.MethodDeclaration)
            .Select(t => ((MethodNode)t.Value).Name)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToHashSet(StringComparer.Ordinal);

    public static List<MethodNode> GetMethods(this TreeNode<SemanticNode> tree)
    {
        return tree.FindWhere(t => t.Value.Kind == SyntaxKind.MethodDeclaration)
            .Select(t => (MethodNode)t.Value)
            .ToList();
    }

    public static IEnumerable<TreeNode<SemanticNode>> GetNamedNodes(
        this TreeNode<SemanticNode> root
    ) => root.FindWhere(t => t.Value.HasName);

    public static IEnumerable<TreeNode<SemanticNode>> GetNamedNodesAtDepth(
        this TreeNode<SemanticNode> root,
        int depth
    ) => root.GetNamedNodes().Where(t => t.Value.Parents.Count == depth);

    public static List<TreeNode<SemanticNode>> GetNonExtensionMethodsInExtensionClass(
        this TreeNode<SemanticNode> root
    )
    {
        var staticClasses = root.FindClasses().Where(c => c.IsStaticClass).ToList();
        var ret = staticClasses
            .SelectMany(t =>
                t.tree.GetMethods().Where(m => m.IsStatic && !m.IsExtension).Select(m => m.tree)
            )
            .ToList();
        return ret;
    }



  

    public static List<UsingDirectiveNode> GetUsings(this TreeNode<SemanticNode> tree)
    {
        return tree.FindWhere(t => t.Value.Kind == SyntaxKind.UsingDirective)
            .Select(t => (UsingDirectiveNode)t.Value)
            .ToList();
    }

    public static (
        TreeNode<SemanticNode> Tree,
        List<List<TreeNode<SemanticNode>>> Groups
    ) GroupByModifierKindName(this TreeNode<SemanticNode> tree)
    {
        var groups = new List<List<TreeNode<SemanticNode>>>();

        foreach (var x in tree.FindWhere(t => t.Value.HasDirectNamedChildren))
        {
            var sorted = x.Children.SortedByModifierKindName();
            groups.AddRange(sorted.GroupedByModifierKind());
            x.Children = sorted;
        }

        return (tree, groups);
    }

    public static TreeNode<SemanticNode> GroupConsecutiveChildren(
        this TreeNode<SemanticNode> node,
        Func<TreeNode<SemanticNode>, bool> predicate
    )
    {
        if (node.Children.Count == 0)
            return new TreeNode<SemanticNode>(node.Value);

        var groups = node.Children.GroupConsecutive(predicate);

        var copy = new TreeNode<SemanticNode>(node.Value);

        if (groups.Count > 1)
        {
            groups.ForEach(group =>
            {
                var cu = CompilationUnitNode.Factory();
                group.ForEach(item => cu.AddChild(item.GroupConsecutiveChildren(predicate)));
                copy.AddChild(cu);
            });
        }
        else
        {
            node.Children.ForEach(c => copy.AddChild(c.GroupConsecutiveChildren(predicate)));
        }
        copy.InitializeAllNodes();
        return copy;
    }

    public static void GroupStaticExtensionMethods(this TreeNode<SemanticNode> root)
    {
        var ret = new List<TreeNode<SemanticNode>>();
        var extensionMethods = root.FindStaticExtensionMethods();
        extensionMethods.SetStaticExtensionMethodsPublic();

        var classes = root.FindClasses().Where(t => t.IsStaticClass).ToList();
        foreach (var c in classes)
        {
            c.tree.RemoveSelf();
        }

        var dict = extensionMethods
            .GroupBy(t =>((MethodNode) t.Value).GetStaticClassName())
            .ToDictionary(t => t.Key, t => t.ToList());

        foreach (var kv in dict)
        {
            string declaration = $"public static class {kv.Key}" + "{}";
            var newNode = kv.Value.WrapInDeclaration(declaration);
            root.Children.Add(newNode);
        }
    }

    public static void InitializeAllNodes(this TreeNode<SemanticNode> node)
    {
        foreach (var child in node.Children)
        {
            child.InitializeAllNodes();
        }

        node.Value.SetTreeNode(node);
    }

    public static void InsertPartialKeywordBefore(
        this TreeNode<SemanticNode> classObj,
        TreeNode<SemanticNode> classKeyword
    )
    {
        var partialElement = new SyntaxElementNode(SyntaxKind.PartialKeyword, "partial ");
        var partialSemantic = new UnknownNode(partialElement);
        var partialNode = new TreeNode<SemanticNode>(partialSemantic);
        partialSemantic.SetTreeNode(partialNode);
        classKeyword.InsertBefore(partialNode);
    }

    public static void MergeAtDepth(this TreeNode<SemanticNode> source, int index)
    {
        var toMerge = source
            .FindKinds(
                new HashSet<SyntaxKind>
                {
                    SyntaxKind.ClassDeclaration,
                    SyntaxKind.NamespaceDeclaration,
                }
            )
            .ToList();

        var groups = toMerge
            .Where(t => t.Value.Parents.Count == index)
            .GroupBy(t => t.Value.FullName)
            .Where(g => g.Count() > 1 && !string.IsNullOrEmpty(g.Key))
            .ToList();

        foreach (var group in groups)
        {
            var first = group.First();
            foreach (var rest in group.Skip(1).ToList())
            {
                first.MergeFrom(rest);
                rest.RemoveSelf();
            }
        }
    }

    public static TreeNode<SemanticNode> MergeFrom(
        this TreeNode<SemanticNode> destination,
        TreeNode<SemanticNode> source
    )
    {
        destination.AddMissingContainersByDepthFrom(source);
        var newLowLevel = destination.ReplaceExistingLowestLevelTypesFrom(source);
        destination.AddNewLowestLevelTypesFrom(newLowLevel);
        return destination;
    }

    public static TreeNode<SemanticNode> MergeFromList(
        this TreeNode<SemanticNode> destination,
        List<TreeNode<SemanticNode>> sources
    )
    {
        if (sources == null || sources.Count == 0)
            return destination;

        foreach (var source in sources)
        {
            if (source != null)
                destination.MergeFrom(source); // delegates to single-source version
        }

        return destination;
    }

    public static TreeNode<SemanticNode> MergePartials(
        this TreeNode<SemanticNode> tree,
        string className
    )
    {
        var partials = tree.FindClasses().Where(t => t.Name == className && t.HasMethods).ToList();
        var source = partials.Skip(1).Select(t => t.tree).ToList();
        var result = partials.First().tree.MergeFromList(source);
        return result;
    }

    public static TreeNode<SemanticNode> MergeSelf(this TreeNode<SemanticNode> source)
    {
        if (source == null)
            return source;

        var maxDepth = source.GetMaxParentCount();

        for (int i = 0; i <= maxDepth; i++)
        {
            source.MergeAtDepth(i);
        }
        var result = source.GroupByModifierKindName().Tree;
        result.DeleteExcessNewlines();
        return result;
    }

    public static TreeNode<SemanticNode> MoveCursorTo(
        this TreeNode<SemanticNode> treeNode,
        SyntaxKind kind
    )
    {
        return treeNode.FindWhere(t => t.Value.Kind == kind).First();
    }

    public static void MoveNonExtensionStaticMethodsInStaticClassesIntoSingleClass(
        this TreeNode<SemanticNode> root
    )
    {
        var existingClass = root.FindWhere(t =>
                t.Value.Kind == SyntaxKind.ClassDeclaration && t.Value.Name == "StaticHelpers"
            )
            .FirstOrDefault();
        var methodsToMove = root.GetNonExtensionMethodsInExtensionClass();

        foreach (var c in methodsToMove)
        {
            c.Value.Modifier = AccessModifier.Public;
            c.RemoveSelf();
        }

        var declaration = "public class StaticHelpers {}";
        if (existingClass != null)
        {
            methodsToMove.AddRange(existingClass.GetMethods().Select(t => t.tree).ToList());
            existingClass.RemoveSelf();
        }
        var newClassTree = methodsToMove.WrapInDeclaration(declaration);
        root.Children.Add(newClassTree);
    }

    public static TreeNode<SemanticNode> OrderCollectionValues(
        this TreeNode<SemanticNode> originalTree
    )
    {
        foreach (
            var x in originalTree.FindWhere(t =>
                SyntaxKindGroups.CollectionInitializerKinds.Contains(t.Value.Kind)
            )
        )
        {
            x.Children = x.Children.Interleave(t => t.Value.Value);
        }
        return originalTree;
    }

    public static void ExtractFromNamespaces(
        this TreeNode<SemanticNode> root,
        List<TreeNode<SemanticNode>> usings,
        TreeNode<SemanticNode> ns
    )
    {
        var types = ns.FindSyntaxKinds(SyntaxKindGroups.NamespaceTypeChildren).ToList();
        ns.RemoveSelf();
        var nsUsing = usings.Where(t => t.Value.Name == ns.Value.Name).FirstOrDefault();
        nsUsing.RemoveSelf();
        root.Children.AddRange(types);
    }

    public static void PullChildrenOutOfNamespaces(this TreeNode<SemanticNode> root)
    {
        var namespaces = root.FindWhere(t => t.Value.Kind == SyntaxKind.NamespaceDeclaration)
            .ToList();
        var usings = root.FindWhere(t => t.Value.Kind == SyntaxKind.UsingDirective).ToList();
        foreach (var ns in namespaces)
        {
            root.ExtractFromNamespaces(usings, ns);
        }
    }

    public static TreeNode<SemanticNode> ReloadFormatted(this TreeNode<SemanticNode> node)
    {
        var code = node.ToCode();
        code = CSharpierFormatter.Format(code);
        var ret = SemanticTree.DeserializeCode(code);
        ret.GroupConsecutiveChildren(t => t.Value.HasName);
        return ret;
    }

    public static void RemoveEmptyClasses(this TreeNode<SemanticNode> node)
    {
        var classes = node.FindClasses().Where(t => t.IsStaticClass).ToList();
        var emptyClasses = classes.Where(t => !t.HasMethods).ToList();
        foreach (var e in emptyClasses) { }

        emptyClasses.ForEach(t => t.tree.RemoveSelf());
    }
    
    public static void RenameNamespaces(
        this TreeNode<SemanticNode> tree,
        Func<string, string> replace
    )
    {
        var namespaces = tree
            .Children.Where(t => t.Value.Kind == SyntaxKind.NamespaceDeclaration)
            .ToList();
        namespaces.ForEach(t =>
        {
            t.Value.Name = replace(t.Value.Name);
        });
        var usings = tree.Children.Where(t => t.Value.Kind == SyntaxKind.UsingDirective).ToList();
        usings.ForEach(t => t.Value.Name = replace(t.Value.Name));
        usings
            .GroupBy(t => t.Value.Name)
            .Where(t => t.Count() > 1)
            .SelectMany(t => t.Skip(1))
            .ForEach(t => t.Delete(DeleteType.NodeAndSubTree));
    }

    public static List<TreeNode<SemanticNode>> ReplaceExistingLowestLevelTypesFrom(
        this TreeNode<SemanticNode> destination,
        TreeNode<SemanticNode> source
    )
    {
        var destByName = destination.GetLowestLevelTypes().ToDictionaryByFullName();
        var newLowLevelTypes = new List<TreeNode<SemanticNode>>();
        var sourceTypes = source.GetLowestLevelTypes();
        foreach (var sourceNode in sourceTypes)
        {
            if (destByName.TryGetValue(sourceNode.Value.FullName, out var destNode))
                destNode.ReplaceSelf(sourceNode);
            else
                newLowLevelTypes.Add(sourceNode);
        }

        return newLowLevelTypes;
    }

    public static void SaveFile(this TreeNode<SemanticNode> node, string filePath)
    {
        node.SerializeFile(filePath);
    }

    public static string Serialize(this TreeNode<SemanticNode>? root)
    {
        if (root == null)
            return string.Empty;
        var sb = new StringBuilder(8192 * 8);

        void Visit(TreeNode<SemanticNode> node)
        {
            if (node.Value?.Text != null)
                sb.Append(node.Value.Text);
            foreach (var child in node.Children)
                Visit(child);
        }

        Visit(root);
        return sb.ToString();
    }

    public static void SerializeFile(this TreeNode<SemanticNode> node, string filePath)
    {
        node.ToFile(filePath);
    }

    public static bool ShouldDivideLargeClass(this TreeNode<SemanticNode> tree)
    {
        if (tree == null || tree.Value is not ClassNode classNode)
            return false;

        string className = classNode.Name ?? "Unknown";

        int methodCount = tree.CountMethods();
        int locCount = tree.CountLinesOfCode();

        const int METHOD_THRESHOLD = 12;
        const int LOC_THRESHOLD = 320;

        bool isLarge = methodCount >= METHOD_THRESHOLD || locCount >= LOC_THRESHOLD;

        return isLarge;
    }

    public static bool TestCompile(
        this TreeNode<SemanticNode> node,
        string projectPath,
        string savePath = null
    )
    {
        var code = node.ToCode();

        if (savePath != null)
            File.WriteAllText(savePath, code);
        var ret = CompilationUtil.Compile(code, projectPath);
        if (!ret.Success)
            throw new Exception("FAILURE");
        return ret.Success;
    }

    public static string ToCode(this TreeNode<SemanticNode> node)
    {
        var code = node.Serialize();
        return code;
    }

    public static void ToFile(this TreeNode<SemanticNode> node, string filePath)
    {
        var code = node.ToCode();
        code = code.NormalizeLineEndings();
        File.WriteAllText(filePath, code);
    }
    public static void ToFormattedFile(this TreeNode<SemanticNode> node, string filePath)
    {
        var code = node.ToCode();
        code = CSharpierFormatter.Format(code);
        File.WriteAllText(filePath, code);
    }
}

public static class ExtensionsOfTreeNodeOfSyntaxElementNode
{

    public static TreeNode<SemanticNode> BuildTree(this TreeNode<SyntaxElementNode> syntaxNode)
    {
        var semantic = syntaxNode.Value.CreateSemantic();
        var treeNode = new TreeNode<SemanticNode>(semantic);
        foreach (var childSyntax in syntaxNode.Children)
        {
            var childTree = BuildTree(childSyntax);
            treeNode.AddChild(childTree);
            semantic.AttachChild(childTree.Value);
        }

        return treeNode;
    }
    public static TreeNode<SemanticNode> ToTreeSemanticNode(
        this TreeNode<SyntaxElementNode> syntaxRoot
    )
    {
        var tree = syntaxRoot.BuildTree();
        tree.InitializeAllNodes();
        return tree;
    }
}

public static class ExtensionsOfTreeNodeOfTokenNode
{

    public static TreeNode<SyntaxElementNode> ToSyntaxElementTree(
        this TreeNode<TokenNode> tokenNode
    )
    {
        if (tokenNode.Value == null)
            return new TreeNode<SyntaxElementNode>(null!);
        var token = tokenNode.Value;
        var flatElements = token.ToSyntaxElements();
        int leadingCount = token.LeadingTrivia.Count;
        var leadingNodes = flatElements
            .Take(leadingCount)
            .Select(e => new TreeNode<SyntaxElementNode>(e))
            .ToList();
        var mainNode = new TreeNode<SyntaxElementNode>(flatElements[leadingCount]);
        var trailingNodes = flatElements
            .Skip(leadingCount + 1)
            .Select(e => new TreeNode<SyntaxElementNode>(e))
            .ToList();
        foreach (var child in tokenNode.Children.Select(ToSyntaxElementTree))
            mainNode.AddChild(child);
        var siblings = new List<TreeNode<SyntaxElementNode>>();
        siblings.AddRange(leadingNodes);
        siblings.Add(mainNode);
        siblings.AddRange(trailingNodes);
        for (int i = 0; i < siblings.Count - 1; i++)
            siblings[i].AddChild(siblings[i + 1]);
        return siblings[0];
    }
    public static TreeNode<SyntaxElementNode> ToTreeSyntaxElement(
        this TreeNode<TokenNode>? tokenRoot
    )
    {
        if (tokenRoot == null)
            throw new ArgumentNullException(nameof(tokenRoot));
        return tokenRoot.ToSyntaxElementTree();
    }
}

public class FieldNode : NamedMemberNode
{
    public override void SetNameNode()
    {
        NameNode = tree.FindNameAfterTypeBeforeSemicolon();
    }

    public FieldNode(SyntaxElementNode b)
        : base(b) { }
}

public class FileGen<T>
    where T : BaseNode<T>
{
    public static string ProjectLoadToCode(string projectFile)
    {
        throw new NotImplementedException();
    }

    public static string ProjectLoadToFile(string projectFile, string outputFile)
    {
        throw new NotImplementedException();
    }

    public void ProjectRoundTripTest(string projectFile)
    {
        var code = ProjectLoadToCode(projectFile);
        TestRoundTrip(code);
    }

    public (string newCode, TreeNode<T> tree) RoundTripCode(string code)
    {
        var codeTree = tree.Deserialize(code);
        var serialized = tree.Serialize(codeTree);
        return (serialized, codeTree);
    }

    public (bool success, string newCode, TreeNode<T> tree) RoundTripFile(string file)
    {
        string code = File.ReadAllText(file);
        var newVals = RoundTripCode(code);
        File.WriteAllText(file, newVals.newCode);
        var ret = code == newVals.newCode;
        return (ret, newVals.newCode, newVals.tree);
    }

    public string TestRoundTrip(string code)
    {
        var regenerated = RoundTripCode(code);
        if (code != regenerated.newCode)
        {
            throw new Exception(
                $"Round-trip failed\n\nOriginal cleaned:\n{code}\n\nRegenerated:\n{regenerated}"
            );
        }

        Console.WriteLine("Round-trip success ✓");
        return regenerated.newCode;
    }

    public (bool success, string newCode, TreeNode<T> tree) TestRoundTripFile(string file)
    {
        var ret = RoundTripFile(file);
        if (!ret.success)
        {
            throw new Exception($"Round-trip failed for file: {file}");
        }

        return ret;
    }

    public FileGen(ISerializeCode<T> tree)
    {
        this.tree = tree;
    }

    ISerializeCode<T> tree;
}

public class FileScopedNamespaceNode : NamespaceNode
{
    public FileScopedNamespaceNode(SyntaxElementNode b)
        : base(b) { }
}

public class ImplementTBD : NotImplementedException
{
    public ImplementTBD(string description) { }
}

public class IndexerNode : ParameterizedMemberNode
{
    public override void SetNameNode() { }

    public IndexerNode(SyntaxElementNode b)
        : base(b) { }
}

public class InterfaceNode : TypeDeclarationNode
{
    public InterfaceNode(SyntaxElementNode b)
        : base(b) { }

    protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.InterfaceKeyword;
}

public class MethodNode : ParameterizedMemberWithBodyNode
{
    // Add this new method to the MethodNode class (right after ConvertToStaticExtension())
    public void ConvertFromStaticExtension()
    {
        if (!IsStatic || !IsExtension)
            return; // not a static extension method – nothing to do

        RemoveStaticKeyword();
        RemoveThisKeywordFromFirstParameter();
        tree.InitializeAllNodes();
    }

    public void ConvertToStaticExtension()
    {
        if (IsStatic && IsExtension)
            return;

        EnsureStatic();
        EnsureFirstParameterIsThis();
        tree.InitializeAllNodes();
    }

    public TreeNode<SemanticNode>? GetReturnTypeNode()
    {
        if (NameNode == null)
            return null;

        // Finds the (only) top-level type node that appears before the method name identifier.
        // Works for void (PredefinedType), simple types, generics, arrays, nullable types, qualified names, etc.
        return tree.FindWhereStop(
                t => SyntaxKindGroups.KindsContainingParameterType.Contains(t.Value.Kind),
                t => t == NameNode
            )
            .LastOrDefault();
    }
    public string GetStaticClassName()
    {
        return $"ExtensionsOf{GetThisParameterName()}";
    }
    public string GetThisParameterName()
    {
        var thisParam =tree.FindWhere(t => t.Value is ParameterNode p && p.IsThis)
            .Select(t => (ParameterNode)t.Value)
            .FirstOrDefault();

        if (thisParam == null || string.IsNullOrWhiteSpace(thisParam.TypeText))
            throw new InvalidOperationException(
                "Extension method must contain a 'this' parameter."
            );

        string typeName = thisParam.TypeText.Trim();

        int lastDot = typeName.LastIndexOf('.');
        if (lastDot >= 0)
            typeName = typeName.Substring(lastDot + 1);

        typeName = typeName.Replace("<", "Of").Replace(">", "").Replace(",", "");

        return Regex.Replace(typeName, @"[^a-zA-Z0-9]", "");
    }

    public override void SetNameNode()
    {
        this.NameNode = tree.FindSkipStop(
                t => t.Value.Kind == SyntaxKind.IdentifierToken,
                t => SyntaxKindGroups.MethodDeclarationNonNameKinds.Contains(t.Value.Kind),
                t => t.Value.Kind == SyntaxKind.OpenBracketToken
            )
            .FirstOrDefault();
    }

    public MethodNode(SyntaxElementNode b)
        : base(b) { }

    public bool IsExtension =>
        ParameterList.Parameters.Count > 0 && ParameterList.Parameters.First().IsThis;

    public bool IsStatic => tree.FindWhere(t => t.Value.Kind == SyntaxKind.StaticKeyword).Any();

    private void EnsureFirstParameterIsThis()
    {
        if (ParameterList?.Parameters.Count == 0)
            return;

        var firstParam = ParameterList.Parameters[0];
        if (firstParam.IsThis)
            return;

        var paramTree = firstParam.tree;

        var insertPoint =
            paramTree
                .FindWhere(t =>
                    SyntaxKindGroups.ModifierKeywords.Contains(t.Value.Kind)
                    || SyntaxKindGroups.KindsContainingParameterType.Contains(t.Value.Kind)
                )
                .FirstOrDefault()
            ?? paramTree.Children.FirstOrDefault();

        if (insertPoint == null)
            return;

        var element = new SyntaxElementNode(SyntaxKind.ThisKeyword, "this ");
        var semantic = new UnknownNode(element);
        var thisNode = new TreeNode<SemanticNode>(semantic);
        semantic.SetTreeNode(thisNode);

        insertPoint.InsertBefore(thisNode);
    }
    private void EnsureStatic()
    {
        if (IsStatic)
            return;

        var element = new SyntaxElementNode(SyntaxKind.StaticKeyword, " static ");
        var semantic = new UnknownNode(element);
        var newNode = new TreeNode<SemanticNode>(semantic);
        semantic.SetTreeNode(newNode);
        var lastKeyword = tree.FindWhereStop(
                t => SyntaxKindGroups.MethodModifierKeywords.Contains(t.Value.Kind),
                t => t.Value.Kind == SyntaxKind.OpenParenToken
            )
            .LastOrDefault();

        if (lastKeyword == null)
        {
            tree.InsertBefore(newNode);
            Console.WriteLine(tree.ToCode());
        }
        else
            lastKeyword.InsertAfter(newNode);
    }

    private void RemoveStaticKeyword()
    {
        var staticNode = tree.FindWhere(t => t.Value.Kind == SyntaxKind.StaticKeyword)
            .FirstOrDefault();

        staticNode?.Delete(DeleteType.SingleNode);
    }

    private void RemoveThisKeywordFromFirstParameter()
    {
        if (ParameterList?.Parameters.Count == 0)
            return;

        var firstParam = ParameterList.Parameters[0];
        if (!firstParam.IsThis)
            return;

        var thisKeywordNode = firstParam
            .tree.FindWhere(t => t.Value.Kind == SyntaxKind.ThisKeyword)
            .FirstOrDefault();

        thisKeywordNode?.Delete(DeleteType.SingleNode);
    }
}

public abstract class NamedMemberNode : NamedNode
{
    public override void SetTreeNode(TreeNode<SemanticNode> tree)
    {
        base.SetTreeNode(tree);
        SetModifierFromTree();
    }

    public TreeNode<SemanticNode> ModifierNode;
    public override AccessModifier Modifier
    {
        get
        {
            if (ModifierNode == null)
                return DefaultModifier;
            else
                return ModifierNode.Value.Kind.ToModifier();
        }
        set { SetModifier(value); }
    }

    protected TreeNode<SemanticNode> SetModifier(AccessModifier modifier)
    {
        if (
            this is ConstructorNode
            || this is DestructorNode
            || this.tree.Parent.Value is InterfaceNode
        )
        {
            throw new Exception("");
        }
        string modString = $"{modifier.ToString().ToLower()}";
        if (ModifierNode != null)
        {
            ModifierNode.Value.Text = modString;
            return ModifierNode;
        }
        var syntaxKind = modifier.ToSyntaxKind();
        var modifierElement = new SyntaxElementNode(syntaxKind, modString);
        var modifierSemantic = new UnknownNode(modifierElement);
        var modifierNode = new TreeNode<SemanticNode>(modifierSemantic);
        modifierSemantic.SetTreeNode(modifierNode);

        tree.Children.Insert(0, modifierNode);
        ModifierNode = modifierNode;
        return ModifierNode;
    }

    protected virtual void SetModifierFromTree()
    {
        ModifierNode = tree.FindWhere(t => SyntaxKindGroups.ModifierKinds.Contains(t.Value.Kind))
            .FirstOrDefault();
    }

    protected NamedMemberNode(SyntaxElementNode backing)
        : base(backing) { }

    protected virtual AccessModifier DefaultModifier => AccessModifier.Private;
}

public abstract class NamedNode : SemanticNode
{
    public void SetName(HashSet<SyntaxKind> endDelimeters)
    {
        var list = tree.FindDelimitedText(endDelimeters).ToList();
        string tempName = String.Join("", list.Select(t => t.Value.Text));
        list.ForEach(t => t.Value.Text = string.Empty);
        this.NameNode = list.First();
        this.NameNode.Value.Text = tempName;
    }

    public virtual void SetNameNode()
    {
        NameNode = tree.FindWhere(n => n.Value.Kind == SyntaxKind.IdentifierToken).FirstOrDefault();
    }

    public override void SetTreeNode(TreeNode<SemanticNode> t)
    {
        base.SetTreeNode(t);
        SetNameNode();
    }

    protected NamedNode(SyntaxElementNode b)
        : base(b) { }

    protected HashSet<SyntaxKind> endDelimeter;
}

public class NamespaceNode : TypeDeclarationNode
{
    public override void AttachChild(SemanticNode child)
    {
        if (child is UsingDirectiveNode u)
            Usings.Add(u);
        else
            Members.Add(child);
    }

    public static TreeNode<SemanticNode> Factory(string name)
    {
        var code = string.Format("namespace {0}\n{{\n}}", name);
        var root = CSharpSyntaxTree.ParseText(code).GetRoot().ToTreeSemanticNode();
        return root.Children.First(t => t.Value.Kind == SyntaxKind.NamespaceDeclaration);
    }

    public override void SetNameNode()
    {
        this.SetName(endDelimeter);
    }

    public NamespaceNode(SyntaxElementNode b)
        : base(b)
    {
        endDelimeter = new HashSet<SyntaxKind>() { SyntaxKind.OpenBraceToken };
    }

    public List<SemanticNode> Members { get; } = new();
    public List<UsingDirectiveNode> Usings { get; } = new();

    protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.NamespaceKeyword;

    protected override AccessModifier DefaultModifier => AccessModifier.Public;
}

public class OperatorNode : ParameterizedMemberWithBodyNode
{
    public override void SetNameNode()
    {
        NameNode = FindOperatorToken();
    }

    public OperatorNode(SyntaxElementNode b)
        : base(b) { }

    TreeNode<SemanticNode> FindOperatorToken()
    {
        return tree.FindWhereStop(IsOperatorToken, t => t.Value.Kind == SyntaxKind.OpenParenToken)
            .FirstOrDefault();
    }

    bool IsOperatorToken(TreeNode<SemanticNode> t)
    {
        var text = t.Value?.Text?.Trim() ?? "";
        return text
            is "+"
                or "-"
                or "*"
                or "/"
                or "%"
                or "&"
                or "|"
                or "^"
                or "!"
                or "~"
                or "<"
                or ">"
                or "="
                or "=="
                or "?";
    }
}

public abstract class ParameterizedMemberNode : NamedMemberNode
{
    public override void AttachChild(SemanticNode child)
    {
        if (child is ParameterListNode pl)
            ParameterList = pl;
    }

    public ParameterListNode? ParameterList { get; set; }

    protected ParameterizedMemberNode(SyntaxElementNode b)
        : base(b) { }
}

public abstract class ParameterizedMemberWithBodyNode : ParameterizedMemberNode
{
    public override void AttachChild(SemanticNode child)
    {
        base.AttachChild(child);
        if (child is BlockNode b)
            Body = b;
    }

    public BlockNode? Body { get; set; }

    protected ParameterizedMemberWithBodyNode(SyntaxElementNode b)
        : base(b) { }
}

public class ParameterListNode : SemanticNode
{
    public override void AttachChild(SemanticNode child)
    {
        if (child is ParameterNode p)
            Parameters.Add(p);
    }

    public ParameterListNode(SyntaxElementNode b)
        : base(b) { }

    public List<ParameterNode> Parameters { get; } = new();
}

public class ParameterNode : NamedNode
{
    public override void SetNameNode()
    {
        var stopKinds = new HashSet<SyntaxKind>
        {
            SyntaxKind.CommaToken,
            SyntaxKind.CloseParenToken,
            SyntaxKind.EqualsToken,
        };
        ParameterNameNode = tree.FindNameAfterType(stopKinds);
    }

    public override void SetTreeNode(TreeNode<SemanticNode> t)
    {
        base.SetTreeNode(t);
        ParseModifiers();
        SetTypeNode();
        ParseType();
    }

    public ParameterNode(SyntaxElementNode b)
        : base(b) { }

    public TreeNode<SemanticNode> ParameterNameNode;

    public bool IsIn => Modifiers.Contains(SyntaxKind.InKeyword);
    public bool IsOut => Modifiers.Contains(SyntaxKind.OutKeyword);
    public bool IsParams => Modifiers.Contains(SyntaxKind.ParamsKeyword);
    public bool IsRef => Modifiers.Contains(SyntaxKind.RefKeyword);
    public bool IsThis => Modifiers.Contains(SyntaxKind.ThisKeyword);

    public List<SyntaxKind> Modifiers { get; } = new();

    public TreeNode<SemanticNode>? TypeNode { get; private set; }

    public string TypeText { get; private set; } = string.Empty;

    void ParseModifiers()
    {
        Modifiers.Clear();
        Modifiers.AddRange(
            tree.FindWhere(t => SyntaxKindGroups.ModifierKeywords.Contains(t.Value.Kind))
                .Select(t => t.Value.Kind)
        );
    }

    void ParseType()
    {
        TypeText = TypeNode?.ToCode().Trim() ?? string.Empty;
    }

    private void SetTypeNode()
    {
        TypeNode = tree.FindWhere(t =>
                SyntaxKindGroups.KindsContainingParameterType.Contains(t.Value.Kind)
            )
            .FirstOrDefault();
    }
}

public class PropertyNode : NamedMemberNode
{
    public override void SetNameNode()
    {
        NameNode = tree.FindNameAfterTypeBeforeSemicolon();
    }

    public PropertyNode(SyntaxElementNode b)
        : base(b) { }
}

public class RecordNode : TypeDeclarationNode
{
    public RecordNode(SyntaxElementNode b)
        : base(b) { }

    protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.RecordKeyword;
}

public class RegionEndNode : RegionNode
{
    public static TreeNode<SemanticNode> Factory()
    {
        var element = new SyntaxElementNode(SyntaxKind.EndRegionDirectiveTrivia, "\n#endregion\n");
        var semantic = new RegionEndNode(element);
        var root = new TreeNode<SemanticNode>(semantic);
        semantic.SetTreeNode(root);
        return root;
    }

    public RegionEndNode(SyntaxElementNode b)
        : base(b) { }
}

public abstract class RegionNode : TriviaNode
{
    public static TreeNode<SemanticNode> Factory(string name)
    {
        var cu = RegionStartNode.Factory(name);
        cu.AddChild(CompilationUnitNode.Factory());
        cu.AddChild(RegionEndNode.Factory());
        return cu;
    }

    protected RegionNode(SyntaxElementNode b)
        : base(b) { }
}

public class RegionStartNode : RegionNode
{
    public static new TreeNode<SemanticNode> Factory(string regionName)
    {
        if (string.IsNullOrWhiteSpace(regionName))
            regionName = "Untitled Region";
        var text = $"\n#region {regionName.Trim()}\n";
        var element = new SyntaxElementNode(SyntaxKind.RegionDirectiveTrivia, text);
        var semantic = new RegionStartNode(element);
        return new TreeNode<SemanticNode>(semantic);
    }

    public RegionStartNode(SyntaxElementNode b)
        : base(b) { }

    public RegionStartNode(string name)
        : base(new SyntaxElementNode(SyntaxKind.RegionDirectiveTrivia, $"\n#region {name}\n"))
    {
        this.Name = name;
    }

    public override string? Name { get; set; }
}

public abstract class SemanticNode : BaseNode<SemanticNode>
{
    public virtual void AttachChild(SemanticNode child) { }

    public void DeleteDeclaration()
    {
        var decNodes = GetDeclaration();
        decNodes.ForEach(t => t.DeleteRecursive(t => true));
    }

    public List<TreeNode<SemanticNode>> GetDeclaration()
    {
        return this.tree.Children.Where(t => !(t.Value is NamedNode)).ToList();
    }

    public virtual void SetBaseClass() { }

    public virtual void SetTreeNode(TreeNode<SemanticNode> tree)
    {
        this.tree = tree;
    }

    public override string ToString()
    {
        return $"{(Modifier == AccessModifier.None ? "" : Modifier)} {Kind} {Name}";
    }

    public string BaseType;
    public SyntaxKind Kind;

    public static List<Func<TreeNode<SemanticNode>, object>> ModifierKindName = new()
    {
        t => t.Value.Modifier,
        t => t.Value.Kind.ToHandled(),
        t => t.Value.Name,
    };

    public static Func<TreeNode<SemanticNode>, bool> moveable = t =>
        (t.Value.HasName && t.Value.Kind != SyntaxKind.EnumMemberDeclaration);

    public TreeNode<SemanticNode> NameNode;
    public string? Text;
    public bool ChildOnly
    {
        get { return !this.tree.FindWhere(t => t.Value.HasName).Skip(1).Any(); }
    }
    public string FullName
    {
        get
        {
            var list = new List<string>() { ParentFullName, Name };
            return string.Join(".", list.Where(t => !String.IsNullOrEmpty(t)));
        }
    }
    public bool HasDirectNamedChildren
    {
        get { return this.tree.Children.Where(t => t.Value.HasName).Any(); }
    }
    public bool HasName => Name != null && Name != string.Empty;
    public virtual TreeNode<SemanticNode> MembersNode
    {
        get { return this.tree; }
    }
    public virtual AccessModifier Modifier { get; set; } = AccessModifier.None;

    public virtual string? Name
    {
        get => NameNode?.Value.Text;
        set
        {
            if (NameNode != null)
                NameNode.Value.Text = value;
        }
    }
    public string ParentFullName
    {
        get
        {

            return String.Join(".", Parents);
        }
    }
    public TreeNode<SemanticNode> ParentNonContainer
    {
        get
        {
            var par = this.tree.Parent;
            while (par != null && par.Value.Kind == SyntaxKind.CompilationUnit)
            {
                par = par.Parent;
            }
            return par;
        }
    }
    public List<string> Parents
    {
        get
        {
            var x = this.tree.Parent;
            var names = new List<string>();
            while (x != null)
            {
                if (x.Value.HasName)
                    names.Add(x.Value.Name);
                x = x.Parent;
            }
            names.Reverse();
            return names;
        }
    }
    public virtual string Value => null;

    protected SemanticNode(SyntaxElementNode backing)
    {
        Kind = backing.Kind;
        Text = backing.Text;
    }
}

public class SemanticTree : ISerializeCode<SemanticNode>
{
    public TreeNode<SemanticNode> Deserialize(string code)
    {
        var syntaxSerializer = new Serializer_SyntaxElement();
        var tree = syntaxSerializer.Deserialize(code);
        return tree.ToTreeSemanticNode();
    }

    public static TreeNode<SemanticNode> DeserializeCode(string code, bool fmt = false)
    {
        var serializer = new SemanticTree();
        if (fmt == true)
            code = CSharpierFormatter.Format(code);
        return serializer.Deserialize(code);
    }

    public static TreeNode<SemanticNode> DeserializeFile(string file, bool fmt = false)
    {
        var code = File.ReadAllText(file);

        return DeserializeCode(code, fmt);
    }

    public static TreeNode<SemanticNode> FromFile(string file, bool fmt = false)
    {
        return DeserializeFile(file, fmt);
    }

    public string Serialize(TreeNode<SemanticNode> tree)
    {
        return tree.Serialize();
    }
}

public class Serializer_SyntaxElement : ISerializeCode<SyntaxElementNode>
{
    public TreeNode<SyntaxElementNode> Deserialize(string code)
    {
        var tokenSerializer = new Serializer_TokenNode();
        var tokenTree = tokenSerializer.Deserialize(code);
        var syntaxElementRoot = tokenTree.ToTreeSyntaxElement();
        return syntaxElementRoot;
    }

    public string Serialize(TreeNode<SyntaxElementNode>? root)
    {
        if (root == null)
            return "";
        var sb = new StringBuilder(8192 * 4);

        void Visit(TreeNode<SyntaxElementNode> node)
        {
            if (node.Value?.Text != null)
            {
                sb.Append(node.Value.Text);
            }

            foreach (var child in node.Children)
            {
                Visit(child);
            }
        }

        Visit(root);
        return sb.ToString();
    }
}

public class Serializer_TokenNode : ISerializeCode<TokenNode>
{
    public TreeNode<TokenNode> Deserialize(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var tokenNodeTree = tree.GetRoot().ToTreeTokenNode();
        return tokenNodeTree;
    }

    public string Serialize(TreeNode<TokenNode>? root)
    {
        if (root == null)
            return "";
        var sb = new StringBuilder(8192 * 8);

        void Visit(TreeNode<TokenNode> node)
        {
            if (node.Value is null)
                return;
            var cn = node.Value;
            foreach (var trivia in cn.LeadingTrivia)
            {
                if (trivia.Text is not null)
                    sb.Append(trivia.Text);
            }

            if (cn.Text is not null)
            {
                sb.Append(cn.Text);
            }

            foreach (var child in node.Children)
            {
                Visit(child);
            }

            foreach (var trivia in cn.TrailingTrivia)
            {
                if (trivia.Text is not null)
                    sb.Append(trivia.Text);
            }
        }

        Visit(root);
        var raw = sb.ToString();
        var lines = raw.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].TrimEnd();
        }

        return string.Join("\n", lines).TrimEnd();
    }
}

public class SimpleMemberAccessNode : SemanticNode
{
    public override void SetTreeNode(TreeNode<SemanticNode> tree)
    {
        base.SetTreeNode(tree);

        if (
            tree.Parent == null
            || !SyntaxKindGroups.CollectionInitializerKinds.Contains(tree.Parent.Value.Kind)
        )
            return;

        var components = new HashSet<SyntaxKind>
        {
            SyntaxKind.DotToken,
            SyntaxKind.IdentifierToken,
        };
        var list = tree.FindWhere(t => components.Contains(t.Value.Kind)).ToList();
        var val = String.Join("", list.Select(t => t.Value.Text));
        foreach (var item in list)
            item.Value.Text = string.Empty;
        ValueNode = list.First();
        ValueNode.Value.Text = val;
    }

    public SimpleMemberAccessNode(SyntaxElementNode b)
        : base(b) { }

    public TreeNode<SemanticNode> ValueNode;
    public override string Value
    {
        get
        {
            if (ValueNode == null)
                return null;
            return ValueNode.Value.Text;
        }
    }
}

public class StaticHelpers
{

    public static void LogSuccessfulExtraction(string interfaceName, int methodCount) =>
        Console.WriteLine($"✓ Extracted {methodCount} methods → partial class for {interfaceName}");
    public static bool NamesAreInvalid(string className, string interfaceName) =>
        string.IsNullOrEmpty(className) || string.IsNullOrEmpty(interfaceName);
}

public class StructNode : TypeDeclarationNode
{
    public StructNode(SyntaxElementNode b)
        : base(b) { }

    protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.StructKeyword;
}

public class SyntaxElementNode : BaseNode<SyntaxElementNode>
{
    public SemanticNode CreateSemantic()
    {
        return Kind switch
        {
            SyntaxKind.ArrowExpressionClause => new ExpressionBodyNode(this),
            SyntaxKind.Block => new BlockNode(this),
            SyntaxKind.ClassDeclaration => new ClassNode(this),
            SyntaxKind.CompilationUnit => new CompilationUnitNode(this),
            SyntaxKind.ConstructorDeclaration => new ConstructorNode(this),
            SyntaxKind.DelegateDeclaration => new DelegateNode(this),
            SyntaxKind.DestructorDeclaration => new DestructorNode(this),
            SyntaxKind.EndRegionDirectiveTrivia => new RegionEndNode(this),
            SyntaxKind.EndOfLineTrivia => new TriviaNode(this),
            SyntaxKind.EnumDeclaration => new EnumNode(this),
            SyntaxKind.EnumMemberDeclaration => new EnumMemberNode(this),
            SyntaxKind.EventDeclaration => new EventNode(this),
            SyntaxKind.EventFieldDeclaration => new EventFieldNode(this),
            SyntaxKind.FieldDeclaration => new FieldNode(this),
            SyntaxKind.FileScopedNamespaceDeclaration => new FileScopedNamespaceNode(this),
            SyntaxKind.IndexerDeclaration => new IndexerNode(this),
            SyntaxKind.InterfaceDeclaration => new InterfaceNode(this),
            SyntaxKind.MethodDeclaration => new MethodNode(this),
            SyntaxKind.MultiLineCommentTrivia => new CommentNode(this),
            SyntaxKind.NamespaceDeclaration => new NamespaceNode(this),
            SyntaxKind.None => new TriviaNode(this),
            SyntaxKind.OperatorDeclaration => new OperatorNode(this),
            SyntaxKind.PropertyDeclaration => new PropertyNode(this),
            SyntaxKind.RecordDeclaration => new RecordNode(this),
            SyntaxKind.RecordStructDeclaration => new RecordNode(this),
            SyntaxKind.RegionDirectiveTrivia => new RegionStartNode(this),
            SyntaxKind.SimpleMemberAccessExpression => new SimpleMemberAccessNode(this),
            SyntaxKind.SingleLineCommentTrivia => new CommentNode(this),
            SyntaxKind.StructDeclaration => new StructNode(this),
            SyntaxKind.UsingDirective => new UsingDirectiveNode(this),
            SyntaxKind.WhitespaceTrivia => new TriviaNode(this),
            SyntaxKind.Parameter => new ParameterNode(this),
            SyntaxKind.ParameterList => new ParameterListNode(this),
            _ => new UnknownNode(this),
        };
    }

    public override string ToString()
    {
        var sb = new StringBuilder(Kind.ToString());
        if (Identifier is { } id)
            sb.Append($"({id})");
        return sb.ToString();
    }

    public SyntaxElementNode(SyntaxKind kind, string text)
        : this(kind, text, null) { }

    public SyntaxElementNode(SyntaxKind kind, string text, string identifier)
    {
        Kind = kind;
        Text = text;
        Identifier = identifier;
    }

    public string? Identifier { get; }
    public SyntaxKind Kind { get; }
    public string? Text { get; }

    protected SyntaxElementNode(SyntaxNodeOrToken node)
    {
        Kind = node.Kind();
        if (node.IsToken)
        {
            var token = node.AsToken();
            Text = token.Text;
            if (token.IsKind(SyntaxKind.IdentifierToken))
                Identifier = token.Text;
        }
        else
        {
            Text = null;
            Identifier = null;
        }
    }
}

public class SyntaxKindGroups
{
    public static readonly HashSet<SyntaxKind> AllKeywords = new HashSet<SyntaxKind>
    {
        SyntaxKind.AbstractKeyword,
        SyntaxKind.AddKeyword,
        SyntaxKind.AliasKeyword,
        SyntaxKind.AndKeyword,
        SyntaxKind.AsKeyword,
        SyntaxKind.AscendingKeyword,
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
        SyntaxKind.DelegateKeyword,
        SyntaxKind.DescendingKeyword,
        SyntaxKind.DoKeyword,
        SyntaxKind.DoubleKeyword,
        SyntaxKind.ElseKeyword,
        SyntaxKind.EnumKeyword,
        SyntaxKind.EqualsKeyword,
        SyntaxKind.EventKeyword,
        SyntaxKind.ExplicitKeyword,
        SyntaxKind.ExternKeyword,
        SyntaxKind.FalseKeyword,
        SyntaxKind.FinallyKeyword,
        SyntaxKind.FixedKeyword,
        SyntaxKind.FloatKeyword,
        SyntaxKind.ForKeyword,
        SyntaxKind.ForEachKeyword,
        SyntaxKind.FromKeyword,
        SyntaxKind.GetKeyword,
        SyntaxKind.GlobalKeyword,
        SyntaxKind.GotoKeyword,
        SyntaxKind.GroupKeyword,
        SyntaxKind.IfKeyword,
        SyntaxKind.ImplicitKeyword,
        SyntaxKind.InKeyword,
        SyntaxKind.InitKeyword,
        SyntaxKind.IntKeyword,
        SyntaxKind.InterfaceKeyword,
        SyntaxKind.InternalKeyword,
        SyntaxKind.IntoKeyword,
        SyntaxKind.IsKeyword,
        SyntaxKind.JoinKeyword,
        SyntaxKind.LetKeyword,
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
        SyntaxKind.OrKeyword,
        SyntaxKind.OrderByKeyword,
        SyntaxKind.OutKeyword,
        SyntaxKind.OverrideKeyword,
        SyntaxKind.ParamsKeyword,
        SyntaxKind.PartialKeyword,
        SyntaxKind.PrivateKeyword,
        SyntaxKind.ProtectedKeyword,
        SyntaxKind.PublicKeyword,
        SyntaxKind.ReadOnlyKeyword,
        SyntaxKind.RecordKeyword,
        SyntaxKind.RefKeyword,
        SyntaxKind.RemoveKeyword,
        SyntaxKind.RequiredKeyword,
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
        SyntaxKind.UnmanagedKeyword,
        SyntaxKind.UnsafeKeyword,
        SyntaxKind.UShortKeyword,
        SyntaxKind.UsingKeyword,
        SyntaxKind.VarKeyword,
        SyntaxKind.VirtualKeyword,
        SyntaxKind.VoidKeyword,
        SyntaxKind.VolatileKeyword,
        SyntaxKind.WhenKeyword,
        SyntaxKind.WhereKeyword,
        SyntaxKind.WhileKeyword,
        SyntaxKind.WithKeyword,
        SyntaxKind.YieldKeyword,
        // Preprocessor directives (also treated as keywords in many contexts)
        SyntaxKind.DefineKeyword,
        SyntaxKind.ElifKeyword,
        SyntaxKind.ElseKeyword, // already included
        SyntaxKind.EndIfKeyword,
        SyntaxKind.EndRegionKeyword,
        SyntaxKind.ErrorKeyword,
        SyntaxKind.IfKeyword, // already included
        SyntaxKind.LineKeyword,
        SyntaxKind.LoadKeyword,
        SyntaxKind.PragmaKeyword,
        SyntaxKind.RegionKeyword,
        SyntaxKind.RestoreKeyword,
        SyntaxKind.UndefKeyword,
        SyntaxKind.WarningKeyword,
    };
    public static readonly HashSet<SyntaxKind> BodyElements = new HashSet<SyntaxKind>
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
    public static readonly HashSet<SyntaxKind> CollectionInitializerKinds = new()
    {
        SyntaxKind.ArrayInitializerExpression,
        SyntaxKind.CollectionInitializerExpression,
        SyntaxKind.ObjectInitializerExpression,
    };

    public static readonly HashSet<SyntaxKind> CommentTrivia = new HashSet<SyntaxKind>
    {
        SyntaxKind.MultiLineCommentTrivia,
        SyntaxKind.MultiLineDocumentationCommentTrivia,
        SyntaxKind.SingleLineCommentTrivia,
        SyntaxKind.SingleLineDocumentationCommentTrivia,
    };

    public static readonly HashSet<SyntaxKind> KindsContainingParameterType =
        new HashSet<SyntaxKind>
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
    public static readonly HashSet<SyntaxKind> MethodModifierKeywords = new HashSet<SyntaxKind>
    {
        // Access modifiers
        SyntaxKind.PublicKeyword,
        SyntaxKind.ProtectedKeyword,
        SyntaxKind.InternalKeyword,
        SyntaxKind.PrivateKeyword,
        // Inheritance / polymorphism
        SyntaxKind.StaticKeyword,
        SyntaxKind.VirtualKeyword,
        SyntaxKind.AbstractKeyword,
        SyntaxKind.OverrideKeyword,
        SyntaxKind.SealedKeyword,
        SyntaxKind.NewKeyword,
        // Special method modifiers
        SyntaxKind.ExternKeyword,
        SyntaxKind.AsyncKeyword,
        SyntaxKind.UnsafeKeyword,
        SyntaxKind.PartialKeyword,
        SyntaxKind.ReadOnlyKeyword, // readonly methods (struct / ref returns)
        // Operator-specific (still method declarations)
        SyntaxKind.ImplicitKeyword,
        SyntaxKind.ExplicitKeyword,
    };

    public static readonly HashSet<SyntaxKind> ModifierKeywords = new HashSet<SyntaxKind>
    {
        SyntaxKind.AbstractKeyword,
        SyntaxKind.AsyncKeyword,
        SyntaxKind.ConstKeyword,
        SyntaxKind.ExternKeyword,
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
        SyntaxKind.SealedKeyword,
        SyntaxKind.StaticKeyword,
        SyntaxKind.ThisKeyword,
        SyntaxKind.UnsafeKeyword,
        SyntaxKind.VirtualKeyword,
        SyntaxKind.VolatileKeyword,
    };

    public static readonly HashSet<SyntaxKind> ModifierKinds = new()
    {
        SyntaxKind.InternalKeyword,
        SyntaxKind.PrivateKeyword,
        SyntaxKind.ProtectedKeyword,
        SyntaxKind.PublicKeyword,
    };
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

    public static readonly HashSet<SyntaxKind> NamespaceTypeChildren = new HashSet<SyntaxKind>
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.DelegateDeclaration,
        SyntaxKind.EnumDeclaration,
        SyntaxKind.InterfaceDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,
        SyntaxKind.StructDeclaration,
    };

    public static readonly HashSet<SyntaxKind> TopLevel = new HashSet<SyntaxKind>
    {
        SyntaxKind.AttributeList,
        SyntaxKind.FileScopedNamespaceDeclaration,
        SyntaxKind.GlobalStatement,
        SyntaxKind.NamespaceDeclaration,
        SyntaxKind.UsingDirective,
    };

    public static readonly HashSet<SyntaxKind> TypeDeclarations = new HashSet<SyntaxKind>
    {
        SyntaxKind.ClassDeclaration,
        SyntaxKind.DelegateDeclaration,
        SyntaxKind.InterfaceDeclaration,
        SyntaxKind.RecordDeclaration,
        SyntaxKind.RecordStructDeclaration,
        SyntaxKind.StructDeclaration,
    };

    public static readonly HashSet<SyntaxKind> WhitespaceTrivia = new HashSet<SyntaxKind>
    {
        SyntaxKind.EndOfLineTrivia,
        SyntaxKind.WhitespaceTrivia,
    };
    static readonly HashSet<SyntaxKind> AllTrivia = new HashSet<SyntaxKind>
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

    static readonly HashSet<SyntaxKind> CollectionDelimeterKinds = new()
    {
        SyntaxKind.CloseBraceToken,
        SyntaxKind.CommaToken,
        SyntaxKind.EndOfLineTrivia,
        SyntaxKind.OpenBraceToken,
        SyntaxKind.WhitespaceTrivia,
    };

    static readonly HashSet<SyntaxKind> MemberDeclarations = new HashSet<SyntaxKind>
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

    static readonly HashSet<SyntaxKind> Region = new HashSet<SyntaxKind>
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

public class SyntaxLoader : IDisposable
{
    public static MSBuildWorkspace CreateWorkspace()
    {
        var msbuildProps = new Dictionary<string, string>
        {
            ["DesignTimeBuild"] = "true",
            ["ProvideCommandLineArgs"] = "true",
            ["CheckForSystemRuntimeDependency"] = "true",
            ["ResolveNuGetPackages"] = "false",
        };
        return MSBuildWorkspace.Create(msbuildProps);
    }

    public void Dispose() { }

    public static SyntaxTree LoadFile(string csFilePath)
    {
        if (!File.Exists(csFilePath))
            throw new FileNotFoundException("File not found", csFilePath);
        if (
            !string.Equals(Path.GetExtension(csFilePath), ".cs", StringComparison.OrdinalIgnoreCase)
        )
            throw new ArgumentException("Must be a .cs file", nameof(csFilePath));
        string text = File.ReadAllText(csFilePath);
        return CSharpSyntaxTree.ParseText(
            text,
            path: csFilePath,
            options: new CSharpParseOptions(LanguageVersion.Latest)
        );
    }

    public static IReadOnlyList<SyntaxTree> LoadSingleProject(string projectPath)
    {
        Project project = OpenProject(projectPath);
        if (project == null)
            throw new InvalidOperationException("Failed to load project");
        var syntaxTrees = new List<SyntaxTree>();
        foreach (var document in project.Documents)
        {
            if (
                document.FilePath == null
                || !document.FilePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
            )
            {
                continue;
            }

            var syntaxTree = document.GetSyntaxTreeAsync().GetAwaiter().GetResult();
            if (syntaxTree != null)
            {
                syntaxTrees.Add(syntaxTree);
            }
        }

        return syntaxTrees.AsReadOnly();
    }

    public static Project OpenProject(string projectPath)
    {
        var workspace = CreateWorkspace();
        if (string.IsNullOrWhiteSpace(projectPath))
            throw new ArgumentException("Project path is required", nameof(projectPath));
        string ext = Path.GetExtension(projectPath).ToLowerInvariant();
        if (ext != ".csproj")
            throw new ArgumentException("File must be a .csproj file", nameof(projectPath));
        if (!File.Exists(projectPath))
            throw new FileNotFoundException("Project file not found", projectPath);
        var ret = workspace.OpenProjectAsync(projectPath).GetAwaiter().GetResult();
        return ret;
    }

    public SyntaxLoader() { }

    static SyntaxLoader()
    {
        MSBuildLocator.RegisterDefaults();
    }
}

public partial class TokenNode : BaseNode<TokenNode>
{
    public SyntaxElementNode? GetFullTrivia(int index, bool isLeading = true)
    {
        var list = isLeading ? LeadingTrivia : TrailingTrivia;
        if (index < 0 || index >= list.Count)
            return null;
        return list[index];
    }

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
    public IReadOnlyList<SyntaxElementNode> LeadingTrivia { get; private set; } =
        Array.Empty<SyntaxElementNode>();
    public SyntaxNode? OriginalSyntaxNode { get; }
    public string? Text { get; }
    public IReadOnlyList<SyntaxElementNode> TrailingTrivia { get; private set; } =
        Array.Empty<SyntaxElementNode>();

    private static bool IsTrivialWhitespace(SyntaxKind k) =>
        k is SyntaxKind.WhitespaceTrivia or SyntaxKind.EndOfLineTrivia;
}

public partial class TreeNode<T>
    where T : BaseNode<T>
{
    public TreeNode<T> AddChild(T child)
    {
        var ret = new TreeNode<T>(child);
        AddChild(ret);
        return ret;
    }

    public void AddChild(TreeNode<T> child)
    {
        if (child.Parent != null)
            child.Parent._children.Remove(child);
        child.Parent = this;
        _children.Add(child);
    }

    public TreeNode<T> DeepClone()
    {
        var clone = new TreeNode<T>(Value);
        foreach (var child in Children)
            clone.AddChild(child.DeepClone());
        return clone;
    }

    public bool DeepEqual(TreeNode<T>? other, IEqualityComparer<T>? valueComparer = null)
    {
        valueComparer ??= EqualityComparer<T>.Default;
        if (ReferenceEquals(this, other))
            return true;
        if (other == null)
            return false;
        if (!valueComparer.Equals(Value, other.Value))
            return false;
        if (Children.Count != other.Children.Count)
            return false;

        for (int i = 0; i < Children.Count; i++)
        {
            if (!Children[i].DeepEqual(other.Children[i], valueComparer))
                return false;
        }

        return true;
    }

    public void Delete(DeleteType deleteType)
    {
        var parent = Parent;
        int index = parent.Children.IndexOf(this);
        parent.Children.RemoveAt(index);

        if (deleteType == DeleteType.SingleNode)
            PromoteChildrenTo(parent, index);
    }

    public void DeleteConsecutive(
        Func<TreeNode<T>, bool> predicate,
        DeleteType deleteType = DeleteType.SingleNode,
        int nStart = 1
    )
    {
        var toDelete = new List<TreeNode<T>>();
        foreach (var group in FindConsecutive(predicate))
            for (int i = nStart; i < group.Count; i++)
                toDelete.Add(group[i]);

        for (int i = toDelete.Count - 1; i >= 0; i--)
            toDelete[i].Delete(deleteType);
    }

    public void DeleteRecursive(
        Func<TreeNode<T>, bool> predicate,
        DeleteType deleteType = DeleteType.SingleNode
    )
    {
        foreach (var node in FindWhere(predicate).ToList())
            node.Delete(deleteType);
    }

    public void DeleteUntil(
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> untilPredicate,
        DeleteType deleteType = DeleteType.SingleNode
    )
    {
        foreach (var node in FindWhereStop(predicate, untilPredicate))
            node.Delete(deleteType);
    }

    public IEnumerable<List<TreeNode<T>>> FindConsecutive(Func<TreeNode<T>, bool> predicate)
    {
        var flat = Flatten().ToList();
        var current = new List<TreeNode<T>>();
        foreach (var node in flat)
        {
            if (predicate(node))
                current.Add(node);
            else if (current.Count > 0)
            {
                if (current.Count > 1)
                    yield return current;
                current = new List<TreeNode<T>>();
            }
        }

        if (current.Count > 1)
            yield return current;
    }

    public List<(TreeNode<T> First, TreeNode<T> Last)> FindFirstLastChildPairs()
    {
        var pairs = new List<(TreeNode<T> First, TreeNode<T> Last)>();

        void AddPairs(TreeNode<T> n)
        {
            if (n.Children.Count > 0)
            {
                pairs.Add((n.Children[0], n.Children[n.Children.Count - 1]));
                foreach (var c in n.Children)
                    AddPairs(c);
            }
        }

        AddPairs(this);
        return pairs;
    }

    public IEnumerable<TreeNode<T>> FindSkip(
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> skipPredicate
    )
    {
        if (skipPredicate(this))
            yield break;
        if (predicate(this))
            yield return this;

        foreach (var result in Children.SelectMany(c => c.FindSkip(predicate, skipPredicate)))
            yield return result;
    }

    public IEnumerable<TreeNode<T>> FindSkipStop(
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> skipPredicate,
        Func<TreeNode<T>, bool> stopPredicate
    )
    {
        if (stopPredicate(this))
            yield break;
        if (skipPredicate(this))
            yield break;

        if (predicate(this))
            yield return this;

        foreach (
            var result in Children.SelectMany(c =>
                c.FindSkipStop(predicate, skipPredicate, stopPredicate)
            )
        )
            yield return result;
    }

    public IEnumerable<TreeNode<T>> FindWhere(Func<TreeNode<T>, bool> predicate)
    {
        if (predicate(this))
            yield return this;

        foreach (var found in Children.SelectMany(c => c.FindWhere(predicate)))
            yield return found;
    }

    public IEnumerable<TreeNode<T>> FindWhereStop(
        Func<TreeNode<T>, bool> predicate,
        Func<TreeNode<T>, bool> untilPredicate
    )
    {
        if (untilPredicate(this))
            yield break;

        if (predicate(this))
            yield return this;

        foreach (var found in this.Children.FindWhereStop(predicate, untilPredicate))
            yield return found;
    }

    public IEnumerable<TreeNode<T>> Flatten()
    {
        return FindWhere(_ => true);
    }

    public TreeNode<T>? GetNextNode()
    {
        var current = this;
        while (current.Parent != null)
        {
            var parent = current.Parent;
            int index = parent.Children.IndexOf(current);
            if (index < parent.Children.Count - 1)
                return parent.Children[index + 1];
            current = parent;
        }

        return null;
    }

    public TreeNode<T>? GetPreviousNode()
    {
        if (Parent == null)
            return null;

        var parent = Parent;
        int currentIndex = parent.Children.IndexOf(this);
        if (currentIndex > 0)
            return parent.Children[currentIndex - 1];
        return parent;
    }

    public void InsertAfter(TreeNode<T> newNode)
    {
        InsertRelative(newNode, true);
    }

    public void InsertBefore(TreeNode<T> newNode)
    {
        InsertRelative(newNode, false);
    }

    public void InsertRelative(TreeNode<T> newNode, bool after = false)
    {
        if (Parent == null)
            return;

        var siblings = Parent.Children;
        var index = siblings.IndexOf(this);

        var insertAt = after ? index + 1 : index;
        siblings.Insert(insertAt, newNode);
        newNode.Parent = Parent;
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

    public void ReplaceNode(TreeNode<T> newNode)
    {
        var parent = Parent;
        int index = parent.Children.IndexOf(this);
        if (index >= 0)
        {
            parent.Children[index] = newNode;
            newNode.Parent = parent;
        }
    }

    public void ReplaceSelf(TreeNode<T> replacement)
    {
        var parent = Parent;
        int index = parent.Children.IndexOf(this);
        parent.Children.RemoveAt(index);
        parent.Children.Insert(index, replacement);
        replacement.Parent = parent;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    public TreeNode(T? value)
    {
        Value = value;
    }

    public List<TreeNode<T>> Children
    {
        get { return _children; }
        set { _children = value; }
    }

    public bool IsRoot => Parent == null;
    public TreeNode<T>? Parent { get; set; }
    public T? Value { get; set; }

    private void PromoteChildrenTo(TreeNode<T> parent, int insertIndex)
    {
        var childrenToPromote = Children.ToList();
        Children.Clear();
        foreach (var child in childrenToPromote)
        {
            parent.Children.Insert(insertIndex++, child);
            child.Parent = parent;
        }
    }

    private List<TreeNode<T>> _children = new();
}

public class TriviaNode : SemanticNode
{
    public TriviaNode(SyntaxElementNode b)
        : base(b) { }
}

public abstract class TypeDeclarationNode : NamedMemberNode
{
    public override void SetBaseClass()
    {
        if (BaseTypes.Count == 0)
            return;
        this.BaseType = BaseTypes.First();
    }

    public override void SetTreeNode(TreeNode<SemanticNode> t)
    {
        base.SetTreeNode(t);
        SetBaseList();
        SetBaseClass();
    }

    public HashSet<string> BaseTypes { get; } = new(StringComparer.Ordinal);
    public override TreeNode<SemanticNode> MembersNode
    {
        get { return this.tree.Children[1]; }
    }

    protected abstract SyntaxKind? GetTypeSpecificKeyword();

    protected TypeDeclarationNode(SyntaxElementNode b)
        : base(b) { }

    void SetBaseList()
    {
        BaseTypes.Clear();
        var baseListNode = tree.FindWhere(t => t.Value.Kind == SyntaxKind.BaseList)
            .FirstOrDefault();
        if (baseListNode == null)
            return;

        var simpleBases = baseListNode.FindWhere(t => t.Value.Kind == SyntaxKind.SimpleBaseType);
        foreach (var sb in simpleBases)
        {
            var typeName = sb.ToCode().Trim();
            typeName = StripGenerics(typeName);
            if (typeName.Length > 0)
                BaseTypes.Add(typeName);
        }
    }

    private static string StripGenerics(string s)
    {
        int i = s.IndexOf('<');
        return i < 0 ? s : s.Substring(0, i).TrimEnd();
    }
}

public class UnknownNode : SemanticNode
{
    public UnknownNode(SyntaxElementNode b)
        : base(b) { }
}

public class UsingDirectiveNode : NamedNode
{
    public override void SetNameNode()
    {
        SetName(endDelimeter);
    }

    public UsingDirectiveNode(SyntaxElementNode b)
        : base(b)
    {
        this.endDelimeter = new HashSet<SyntaxKind> { SyntaxKind.SemicolonToken };
        Modifier = AccessModifier.Public;
    }
}

public interface ISerializeCode<T>
    where T : BaseNode<T>
{
    TreeNode<T> Deserialize(string code);
    string Serialize(TreeNode<T> tree);
}

public enum AccessModifier
{
    None,
    Public,
    Protected,
    Private,
    Internal,
}

public enum DeleteType
{
    NodeAndSubTree,
    SingleNode,
}

public enum ProjectName
{
    Evolver3,
    Evolver4,
    Evolver5,
    RevMo,
    Shared,
    Test,
}

public enum SyntaxKindOrdering
{
    UsingDirective,
    FileScopedNamespaceDeclaration,
    MethodDeclaration,
    ClassDeclaration,
    NamespaceDeclaration,
    InterfaceDeclaration,
    EnumDeclaration,
    RecordDeclaration,
    StructDeclaration,
    DelegateDeclaration,
    ConstructorDeclaration,

    FieldDeclaration,
    EventFieldDeclaration,
    DestructorDeclaration,
    EventDeclaration,
    PropertyDeclaration,
    IndexerDeclaration,
    OperatorDeclaration,
    EnumMemberDeclaration,
    Block,
    ArrowExpressionClause,
    SingleLineCommentTrivia,
    MultiLineCommentTrivia,
    RegionDirectiveTrivia,
    EndRegionDirectiveTrivia,
    WhitespaceTrivia,
    EndOfLineTrivia,
    CompilationUnit,
    None,
    Unknown,
}
