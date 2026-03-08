using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Evolver5;
using Evolver5.Language;
using Evolver5.Semantic.Code;
using Evolver5.Semantic.Types;
using Evolver5.SyntaxElement.Code;
using Evolver5.Token.Code;
using Evolver5.Tree;
using Evolver5.Util;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Shared;

namespace Evolver5
{
    public static class _Program
    {
        public static void Main()
        {
            throw new NotImplementedException();
        }

        static void Test(ProjectPaths path)
        {
            throw new NotImplementedException();
        }

        static void TestSemanticSerializer(ProjectPaths path)
        {
            throw new NotImplementedException();
        }
    }

    public class GrokTBD : NotImplementedException { }

    public static class RoslynTrivia
    {
        public static TreeNode<SemanticNode> Format(this TreeNode<SemanticNode> node)
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> NormalizeWhitespace(this TreeNode<SemanticNode> node)
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> NormalizeWhitespace(this SyntaxNode node)
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> NormalizeWhitespaceForReal(
            this TreeNode<SemanticNode> node
        )
        {
            throw new NotImplementedException();
        }

        public static string PreDeserialize(this string code)
        {
            throw new NotImplementedException();
        }

        public static SyntaxNode StripTrivia(this SyntaxNode node)
        {
            throw new NotImplementedException();
        }

        public static SyntaxNode ToSyntaxNode(this TreeNode<SemanticNode> node)
        {
            throw new NotImplementedException();
        }

        static string DontWashCode(this string code)
        {
            throw new NotImplementedException();
        }

        private static bool IsWhitespaceOrEndOfLine(this SyntaxTrivia trivia) =>
            trivia.IsKind(SyntaxKind.WhitespaceTrivia) || trivia.IsKind(SyntaxKind.EndOfLineTrivia);
    }
}

namespace Evolver5.Language
{
    public static class LanguageExtensions
    {
        public static SyntaxKindOrdering ToHandled(this SyntaxKind kind)
        {
            throw new NotImplementedException();
        }

        public static AccessModifier ToModifier(this SyntaxKind kind)
        {
            throw new NotImplementedException();
        }
    }

    public static class SyntaxKindGroups
    {
        public static readonly HashSet<SyntaxKind> AllTrivia = new HashSet<SyntaxKind>
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
        public static readonly HashSet<SyntaxKind> CommentTrivia = new HashSet<SyntaxKind>
        {
            SyntaxKind.MultiLineCommentTrivia,
            SyntaxKind.MultiLineDocumentationCommentTrivia,
            SyntaxKind.SingleLineCommentTrivia,
            SyntaxKind.SingleLineDocumentationCommentTrivia,
        };
        public static readonly HashSet<SyntaxKind> MemberDeclarations = new HashSet<SyntaxKind>
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
        public static readonly HashSet<SyntaxKind> MemberKinds = new()
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
        public static readonly HashSet<SyntaxKind> Region = new HashSet<SyntaxKind>
        {
            SyntaxKind.EndRegionDirectiveTrivia,
            SyntaxKind.EndRegionKeyword,
            SyntaxKind.RegionDirectiveTrivia,
            SyntaxKind.RegionKeyword,
        };
        public static readonly HashSet<SyntaxKind> TopLevel = new HashSet<SyntaxKind>
        {
            SyntaxKind.AttributeList,
            SyntaxKind.CompilationUnit,
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

        public static readonly HashSet<SyntaxKind> TypeKindsThatCanContainMethodImplementations =
            new()
            {
                SyntaxKind.ClassDeclaration,
                SyntaxKind.RecordDeclaration,
                SyntaxKind.RecordStructDeclaration,
                SyntaxKind.StructDeclaration,
            };
        public static readonly HashSet<SyntaxKind> WhitespaceTrivia = new HashSet<SyntaxKind>
        {
            SyntaxKind.EndOfLineTrivia,
            SyntaxKind.WhitespaceTrivia,
        };
    }

    public enum AccessModifier
    {
        None,
        Public,
        Protected,
        Private,
        Internal,
    }

    public enum SyntaxKindOrdering
    {
        UsingDirective,
        FileScopedNamespaceDeclaration,
        ClassDeclaration,
        NamespaceDeclaration,
        InterfaceDeclaration,
        EnumDeclaration,
        RecordDeclaration,
        StructDeclaration,
        DelegateDeclaration,
        ConstructorDeclaration,
        MethodDeclaration,
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
}

namespace Evolver5.Semantic.Code
{
    public static class SemanticExtensions
    {
        public static string NormalizeLineEndings(string text)
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> RemoveNamespaces(this TreeNode<SemanticNode> tree)
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> RewriteRecursive(
            this TreeNode<SemanticNode> semTree,
            Queue<Func<TreeNode<SemanticNode>, bool>> rewriteList
        )
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> RewriteRecursive(
            this TreeNode<SemanticNode> node,
            Func<TreeNode<SemanticNode>, bool> rewrite
        )
        {
            throw new NotImplementedException();
        }

        public static void SaveFile(this TreeNode<SemanticNode> node, string filePath)
        {
            throw new NotImplementedException();
        }

        public static string Serialize(this TreeNode<SemanticNode>? root)
        {
            throw new NotImplementedException();
            void Visit(TreeNode<SemanticNode> node)
            {
                throw new NotImplementedException();
            }
        }

        public static void Serialize(this TreeNode<SemanticNode> node, string filePath)
        {
            throw new NotImplementedException();
        }

        public static bool TestCompile(
            this TreeNode<SemanticNode> node,
            string projectPath,
            string savePath = null
        )
        {
            throw new NotImplementedException();
        }

        public static string ToCode(this TreeNode<SemanticNode> node)
        {
            throw new NotImplementedException();
        }

        public static void ToFile(this TreeNode<SemanticNode> node, string filePath)
        {
            throw new NotImplementedException();
        }
    }

    public static class SemanticExtensionsGroupAndRegion
    {
        public static TreeNode<SemanticNode> CreateRegionEndNode()
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> CreateRegionStartNode(string regionName)
        {
            throw new NotImplementedException();
        }

        public static void EmptyMemberDeclarationsContents(
            this TreeNode<SemanticNode> tree,
            ProjectPaths path
        )
        {
            throw new NotImplementedException();
        }

        public static void EmptyTypeDeclarationsContents(
            this TreeNode<SemanticNode> tree,
            ProjectPaths path
        )
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> Format(this TreeNode<SemanticNode> tree)
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> GroupAndAddRegion(
            this TreeNode<SemanticNode> tree,
            ProjectPaths path
        )
        {
            throw new NotImplementedException();
        }

        public static List<TreeNode<SemanticNode>> GroupAndAddRegions(
            this IEnumerable<TreeNode<SemanticNode>> children
        )
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> GroupByModifierKindName(
            this TreeNode<SemanticNode> tree,
            ProjectPaths path
        )
        {
            throw new NotImplementedException();
        }

        public static List<TreeNode<SemanticNode>> GroupByModifierKindName(
            this IEnumerable<TreeNode<SemanticNode>> en
        )
        {
            throw new NotImplementedException();
        }

        public static (
            Func<TreeNode<SemanticNode>, bool> moveable,
            List<Func<TreeNode<SemanticNode>, object>> predicates
        ) ReturnGroupByKindModifierNamePredicates()
        {
            throw new NotImplementedException();
        }
    }

    public class SemanticTree : ISerializeCode<SemanticNode>
    {
        public TreeNode<SemanticNode> Deserialize(string code)
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> DeserializeCode(string code)
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> DeserializeFile(string file)
        {
            throw new NotImplementedException();
        }

        public string Serialize(TreeNode<SemanticNode> tree)
        {
            throw new NotImplementedException();
        }
    }

    public static class TreeConverter_SyntaxElementToSemanticNode
    {
        public static TreeNode<SemanticNode> Convert(TreeNode<SyntaxElementNode> syntaxRoot)
        {
            throw new NotImplementedException();
        }

        private static TreeNode<SemanticNode> BuildTree(TreeNode<SyntaxElementNode> syntaxNode)
        {
            throw new NotImplementedException();
        }

        private static SemanticNode CreateSemantic(SyntaxElementNode elem)
        {
            throw new NotImplementedException();
        }

        private static void InitializeAllNodes(TreeNode<SemanticNode> node)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Evolver5.Semantic.Types
{
    public partial class BlockNode : SemanticNode
    {
        public BlockNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public List<SemanticNode> Statements { get; } = new();
    }

    public partial class ClassNode : TypeDeclarationNode
    {
        public ClassNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public override void SetNameNode()
        {
            throw new NotImplementedException();
        }

        public List<ConstructorNode> Constructors { get; } = new();
        public List<DelegateNode> Delegates { get; } = new();
        public List<DestructorNode> Destructors { get; } = new();
        public List<EventNode> Events { get; } = new();
        public List<FieldNode> Fields { get; } = new();
        public List<IndexerNode> Indexers { get; } = new();
        public List<MethodNode> Methods { get; } = new();
        public List<PropertyNode> Properties { get; } = new();

        protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.ClassKeyword;
    }

    public partial class CommentNode : TriviaNode
    {
        public CommentNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public partial class CompilationUnitNode : SemanticNode
    {
        public CompilationUnitNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public List<SemanticNode> Members { get; } = new();
        public List<UsingDirectiveNode> Usings { get; } = new();
    }

    public partial class ConstructorNode : NamedMemberNode
    {
        public ConstructorNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public BlockNode? Body { get; set; }
    }

    public partial class DelegateNode : TypeDeclarationNode
    {
        public DelegateNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.DelegateKeyword;
    }

    public class DelimitedNode : NamedNode
    {
        public DelimitedNode(SyntaxElementNode backing, HashSet<SyntaxKind> endDelimeter)
            : base(backing)
        {
            throw new NotImplementedException();
        }

        public override void SetNameNode()
        {
            throw new NotImplementedException();
        }

        protected HashSet<SyntaxKind> endDelimeter;
    }

    public partial class DestructorNode : NamedMemberNode
    {
        public DestructorNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public BlockNode? Body { get; set; }
    }

    public partial class EnumMemberNode : NamedNode
    {
        public EnumMemberNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public partial class EnumNode : TypeDeclarationNode
    {
        public EnumNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public List<EnumMemberNode> Members { get; } = new();

        protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.EnumKeyword;
    }

    public partial class EventFieldNode : NamedMemberNode
    {
        public EventFieldNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void SetNameNode()
        {
            throw new NotImplementedException();
        }
    }

    public partial class EventNode : NamedMemberNode
    {
        public EventNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ExpressionBodyNode : SemanticNode
    {
        public ExpressionBodyNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public partial class FieldNode : NamedMemberNode
    {
        public FieldNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void SetNameNode()
        {
            throw new NotImplementedException();
        }
    }

    public partial class FileScopedNamespaceNode : NamespaceNode
    {
        public FileScopedNamespaceNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public partial class FileScopedNamespaceNode { }

    public partial class IndexerNode : NamedMemberNode
    {
        public IndexerNode(SyntaxElementNode backing)
            : base(backing)
        {
            throw new NotImplementedException();
        }

        public override void SetNameNode()
        {
            throw new NotImplementedException();
        }
    }

    public partial class InterfaceNode : TypeDeclarationNode
    {
        public InterfaceNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public List<EventNode> Events { get; } = new();
        public List<IndexerNode> Indexers { get; } = new();
        public List<MethodNode> Methods { get; } = new();
        public List<PropertyNode> Properties { get; } = new();

        protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.InterfaceKeyword;
    }

    public partial class MethodNode : NamedMemberNode
    {
        public MethodNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public override void SetNameNode()
        {
            throw new NotImplementedException();
        }

        public BlockNode? Body { get; set; }
    }

    public abstract class NamedMemberNode : NamedNode
    {
        public override void SetTreeNode(TreeNode<SemanticNode> tree)
        {
            throw new NotImplementedException();
        }

        protected NamedMemberNode(SyntaxElementNode backing)
            : base(backing)
        {
            throw new NotImplementedException();
        }

        void SetModifierFromTree()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class NamedNode : SemanticNode
    {
        public void SetNameFromTree(HashSet<SyntaxKind> endDelimeter)
        {
            throw new NotImplementedException();
        }

        public virtual void SetNameNode()
        {
            throw new NotImplementedException();
        }

        public override void SetTreeNode(TreeNode<SemanticNode> tree)
        {
            throw new NotImplementedException();
        }

        protected NamedNode(SyntaxElementNode backing)
            : base(backing)
        {
            throw new NotImplementedException();
        }

        protected virtual string? ExtractNameCustom(
            IReadOnlyList<TreeNode<SyntaxElementNode>> syntaxChildren
        ) => null;
    }

    public partial class NamespaceNode : DelimitedNode
    {
        public NamespaceNode(SyntaxElementNode b)
            : base(b, new HashSet<SyntaxKind>() { SyntaxKind.OpenBraceToken })
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public List<SemanticNode> Members { get; } = new();
        public List<UsingDirectiveNode> Usings { get; } = new();
    }

    public partial class OperatorNode : NamedMemberNode
    {
        public OperatorNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public BlockNode? Body { get; set; }
    }

    public partial class OperatorNode : NamedMemberNode
    {
        public override void SetNameNode()
        {
            throw new NotImplementedException();
        }
    }

    public partial class PropertyNode : NamedMemberNode
    {
        public PropertyNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void SetNameNode()
        {
            throw new NotImplementedException();
        }
    }

    public partial class RecordNode : TypeDeclarationNode
    {
        public RecordNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public List<ConstructorNode> Constructors { get; } = new();
        public List<FieldNode> Fields { get; } = new();
        public List<MethodNode> Methods { get; } = new();
        public List<PropertyNode> Properties { get; } = new();

        protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.RecordKeyword;
    }

    public partial class RegionEndNode : RegionNode
    {
        public RegionEndNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class RegionNode : TriviaNode
    {
        protected RegionNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public partial class RegionStartNode : RegionNode
    {
        public RegionStartNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class SemanticNode
    {
        public virtual void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public void DeleteDeclaration()
        {
            throw new NotImplementedException();
        }

        public List<TreeNode<SemanticNode>> GetDeclaration()
        {
            throw new NotImplementedException();
        }

        public virtual void SetTreeNode(TreeNode<SemanticNode> tree)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public SyntaxKind Kind;
        public TreeNode<SemanticNode> NameNode;
        public string? Text;
        public TreeNode<SemanticNode> tree;
        public AccessModifier Modifier { get; set; } = AccessModifier.None;

        public string? Name
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Type => this.GetType().Name;

        protected SemanticNode(SyntaxElementNode backing)
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleMemberAccessNode : NamedNode
    {
        public SimpleMemberAccessNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void SetNameNode()
        {
            throw new NotImplementedException();
        }
    }

    public partial class StructNode : TypeDeclarationNode
    {
        public StructNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }

        public override void AttachChild(SemanticNode child)
        {
            throw new NotImplementedException();
        }

        public List<ConstructorNode> Constructors { get; } = new();
        public List<EventNode> Events { get; } = new();
        public List<FieldNode> Fields { get; } = new();
        public List<IndexerNode> Indexers { get; } = new();
        public List<MethodNode> Methods { get; } = new();
        public List<PropertyNode> Properties { get; } = new();

        protected override SyntaxKind? GetTypeSpecificKeyword() => SyntaxKind.StructKeyword;
    }

    public partial class TriviaNode : SemanticNode
    {
        public TriviaNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public abstract partial class TypeDeclarationNode : NamedMemberNode
    {
        public override void SetTreeNode(TreeNode<SemanticNode> tree)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> BaseTypes { get; } = new HashSet<string>(StringComparer.Ordinal);

        protected TypeDeclarationNode(SyntaxElementNode backing)
            : base(backing)
        {
            throw new NotImplementedException();
        }

        protected abstract SyntaxKind? GetTypeSpecificKeyword();

        void SetBaseTypes()
        {
            throw new NotImplementedException();
        }
    }

    public partial class UnknownNode : SemanticNode
    {
        public UnknownNode(SyntaxElementNode b)
            : base(b)
        {
            throw new NotImplementedException();
        }
    }

    public partial class UsingDirectiveNode : DelimitedNode
    {
        public UsingDirectiveNode(SyntaxElementNode backing)
            : base(backing, new HashSet<SyntaxKind>() { SyntaxKind.SemicolonToken })
        {
            throw new NotImplementedException();
        }
    }
}

namespace Evolver5.SyntaxElement.Code
{
    public class Serializer_SyntaxElement : ISerializeCode<SyntaxElementNode>
    {
        public TreeNode<SyntaxElementNode> Deserialize(string code)
        {
            throw new NotImplementedException();
        }

        public string Serialize(TreeNode<SyntaxElementNode>? root)
        {
            throw new NotImplementedException();
            void Visit(TreeNode<SyntaxElementNode> node)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class SyntaxElementNode
    {
        public SyntaxElementNode(SyntaxKind kind, string text)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public string? Identifier { get; }
        public SyntaxKind Kind { get; }
        public string? Text { get; }

        protected SyntaxElementNode(SyntaxNodeOrToken node)
        {
            throw new NotImplementedException();
        }
    }

    public static class TreeConverter_TokenNodeToSyntaxElement
    {
        public static TreeNode<SyntaxElementNode> Convert(TreeNode<TokenNode>? tokenRoot)
        {
            throw new NotImplementedException();
        }

        private static TreeNode<SyntaxElementNode> DeepConvert(TreeNode<TokenNode> tokenNode)
        {
            throw new NotImplementedException();
        }

        private static TreeNode<SyntaxElementNode> MergeList(
            this List<TreeNode<SyntaxElementNode>> tokenNode
        )
        {
            throw new NotImplementedException();
        }
    }
}

namespace Evolver5.Token.Code
{
    public class Serializer_TokenNode : ISerializeCode<TokenNode>
    {
        public TreeNode<TokenNode> Deserialize(string code)
        {
            throw new NotImplementedException();
        }

        public string Serialize(TreeNode<TokenNode>? root)
        {
            throw new NotImplementedException();
            void Visit(TreeNode<TokenNode> node)
            {
                throw new NotImplementedException();
            }
        }

        public TreeNode<TokenNode> ToTokenNodeTree(SyntaxNode node)
        {
            throw new NotImplementedException();
        }
    }

    public partial class TokenNode : SyntaxElementNode
    {
        public TokenNode(SyntaxNodeOrToken node)
            : base(node)
        {
            throw new NotImplementedException();
        }

        public TokenNode(SyntaxTrivia trivia)
            : base(trivia.Kind(), trivia.ToFullString())
        {
            throw new NotImplementedException();
        }

        public TokenNode? GetFullTrivia(int index, bool isLeading = true)
        {
            throw new NotImplementedException();
        }

        public new string ToString() => ToString(showTrivia: false);

        public string ToString(bool showTrivia)
        {
            throw new NotImplementedException();
        }

        public List<SyntaxElementNode> ToSyntaxElements()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<SyntaxElementNode> LeadingTrivia { get; private set; } =
            Array.Empty<SyntaxElementNode>();
        public SyntaxNode? OriginalSyntaxNode { get; }
        public IReadOnlyList<SyntaxElementNode> TrailingTrivia { get; private set; } =
            Array.Empty<SyntaxElementNode>();

        private static bool IsTrivialWhitespace(SyntaxKind k) =>
            k is SyntaxKind.WhitespaceTrivia or SyntaxKind.EndOfLineTrivia;
    }
}

namespace Evolver5.Tree
{
    public class DeepTreeCopier<T>
        where T : class
    {
        public DeepTreeCopier(Func<TreeNode<T>, bool> preCopy)
        {
            throw new NotImplementedException();
        }

        public TreeNode<T>? Copy(TreeNode<T>? source)
        {
            throw new NotImplementedException();
        }

        private readonly Func<TreeNode<T>, bool> _preCopy;
    }

    public class DeepTreeCopierMulti<T>
        where T : class
    {
        public static TreeNode<T> Copy(
            TreeNode<T>? source,
            Queue<Func<TreeNode<T>, bool>> rewriteList
        )
        {
            throw new NotImplementedException();
        }
    }

    public static class TreeEquality
    {
        public static bool DeepEqual<T>(
            TreeNode<T>? a,
            TreeNode<T>? b,
            IEqualityComparer<T>? valueComparer = null
        )
            where T : class
        {
            throw new NotImplementedException();
        }
    }

    public static class TreeExtensions
    {
        public static Queue<T> AsQueue<T>(this T item)
        {
            throw new NotImplementedException();
        }

        public static TreeNode<T>? DeepClone<T>(
            this TreeNode<T>? source,
            Func<TreeNode<T>, bool>? exitPredicate = null
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static void Delete<T>(this TreeNode<T> node, DeleteType deleteType)
            where T : class
        {
            throw new NotImplementedException();
        }

        public static void DeleteComments<T>(
            this TreeNode<T> root,
            Func<TreeNode<T>, bool> predicate,
            DeleteType deleteType = DeleteType.SingleNode
        )
            where T : SemanticNode
        {
            throw new NotImplementedException();
        }

        public static void DeleteConsecutive<T>(
            this TreeNode<T> root,
            Func<TreeNode<T>, bool> predicate,
            DeleteType deleteType = DeleteType.SingleNode,
            int nStart = 1
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static TreeNode<SemanticNode> DeleteExcessNewlines(this TreeNode<SemanticNode> root)
        {
            throw new NotImplementedException();
        }

        public static void DeleteRecursive<T>(
            this TreeNode<T> root,
            Func<TreeNode<T>, bool> predicate,
            DeleteType deleteType = DeleteType.SingleNode
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static void DeleteUntil<T>(
            this TreeNode<T> root,
            Func<TreeNode<T>, bool> predicate,
            Func<TreeNode<T>, bool> untilPredicate,
            DeleteType deleteType = DeleteType.SingleNode
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<List<TreeNode<T>>> FindConsecutive<T>(
            this TreeNode<T> root,
            Func<TreeNode<T>, bool> predicate
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<SemanticNode>> FindDelimitedText(
            this TreeNode<SemanticNode> root,
            HashSet<SyntaxKind> delimeters
        )
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<SemanticNode>> FindDelimitedText(
            this List<TreeNode<SemanticNode>> roots,
            HashSet<SyntaxKind> delimeters
        )
        {
            throw new NotImplementedException();
        }

        public static List<(TreeNode<T> First, TreeNode<T> Last)> FindFirstLastChildPairs<T>(
            this TreeNode<T> root
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static List<(TreeNode<T> First, TreeNode<T> Last)> FindFirstLastChildPairs<T>(
            this List<TreeNode<T>> roots
        )
            where T : class
        {
            throw new NotImplementedException();
            void AddPairs(TreeNode<T> n)
            {
                throw new NotImplementedException();
            }
        }

        public static IEnumerable<TreeNode<T>> FindNodes<T>(
            this List<TreeNode<T>> roots,
            Func<TreeNode<T>, bool> predicate
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<T>> FindNodes<T>(
            this TreeNode<T> root,
            Func<TreeNode<T>, bool> predicate
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<T>> FindSkip<T>(
            this TreeNode<T> root,
            Func<TreeNode<T>, bool> predicate,
            Func<TreeNode<T>, bool> skipPredicate
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<T>> FindSkip<T>(
            this List<TreeNode<T>> roots,
            Func<TreeNode<T>, bool> predicate,
            Func<TreeNode<T>, bool> skipPredicate
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<T>> FindSkipStop<T>(
            this TreeNode<T> root,
            Func<TreeNode<T>, bool> predicate,
            Func<TreeNode<T>, bool> skipPredicate,
            Func<TreeNode<T>, bool> stopPredicate
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<T>> FindSkipStop<T>(
            this List<TreeNode<T>> roots,
            Func<TreeNode<T>, bool> predicate,
            Func<TreeNode<T>, bool> skipPredicate,
            Func<TreeNode<T>, bool> stopPredicate
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<T>> FindStop<T>(
            this TreeNode<T> root,
            Func<TreeNode<T>, bool> predicate,
            Func<TreeNode<T>, bool> untilPredicate
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<T>> FindStop<T>(
            this List<TreeNode<T>> roots,
            Func<TreeNode<T>, bool> predicate,
            Func<TreeNode<T>, bool> untilPredicate
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static TreeNode<T>? GetNextNode<T>(this TreeNode<T> node)
            where T : class
        {
            throw new NotImplementedException();
        }

        public static TreeNode<T>? GetPreviousNode<T>(this TreeNode<T> node)
            where T : class
        {
            throw new NotImplementedException();
        }

        public static List<string> GetStrings(this IEnumerable<TreeNode<SemanticNode>> en)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<T> GroupByPredicates<T>(
            this IEnumerable<T> items,
            List<Func<T, object>> groupByPredicates
        )
        {
            throw new NotImplementedException();
            IEnumerable<T> ApplyGrouping(int index, IEnumerable<T> currentItems)
            {
                throw new NotImplementedException();
            }
        }

        public static (IEnumerable<T> all, IEnumerable<T> grouped) GroupByPredicatesInterleave<T>(
            this IEnumerable<T> items,
            Func<T, bool> moveable,
            List<Func<T, object>> groupByPredicates
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<(T First, T Last)> GroupSpans<T>(
            this IEnumerable<T> items,
            List<Func<T, object>> predicates
        )
        {
            throw new NotImplementedException();
        }

        public static void InsertAfter<T>(
            this List<TreeNode<T>> list,
            TreeNode<T> item,
            TreeNode<T> insert
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static void InsertAfter<T>(this TreeNode<T> node, TreeNode<T> newNode)
            where T : class
        {
            throw new NotImplementedException();
        }

        public static void InsertBefore<T>(
            this List<TreeNode<T>> list,
            TreeNode<T> item,
            TreeNode<T> insert
        )
            where T : class
        {
            throw new NotImplementedException();
        }

        public static void InsertBefore<T>(this TreeNode<T> node, TreeNode<T> newNode)
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<T> Interleave<T>(
            this IEnumerable<T> en,
            Func<T, bool> isMoveable,
            List<T> sortedMovable
        )
        {
            throw new NotImplementedException();
        }

        public static void RenameNamespaces(
            this TreeNode<SemanticNode> tree,
            Func<string, string> replace
        )
        {
            throw new NotImplementedException();
        }

        public static void ReplaceNode<T>(this TreeNode<T> oldNode, TreeNode<T> newNode)
            where T : class
        {
            throw new NotImplementedException();
        }

        static IEnumerable<TreeNode<SemanticNode>> FindText(
            List<TreeNode<SemanticNode>> roots,
            HashSet<SyntaxKind> include,
            HashSet<SyntaxKind> stopAt
        )
        {
            throw new NotImplementedException();
        }

        private static void PromoteChildren<T>(
            TreeNode<T> node,
            TreeNode<T> parent,
            int insertIndex
        )
            where T : class
        {
            throw new NotImplementedException();
        }
    }

    public class TreeNode<T>
        where T : class
    {
        public TreeNode(T? value)
        {
            throw new NotImplementedException();
        }

        public void AddChild(TreeNode<T> child)
        {
            throw new NotImplementedException();
        }

        public bool RemoveChild(TreeNode<T> child)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public List<TreeNode<T>> Children
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool IsRoot => Parent == null;
        public TreeNode<T>? Parent { get; set; }
        public T? Value { get; set; }

        private List<TreeNode<T>> _children = new();
    }

    public static class TreeSiblingExtensions
    {
        public static IEnumerable<TreeNode<T>> NextSiblings<T>(this TreeNode<T> node)
            where T : class
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TreeNode<T>> PreviousSiblings<T>(this TreeNode<T> node)
            where T : class
        {
            throw new NotImplementedException();
        }
    }

    public interface ISerializeCode<T>
        where T : class
    {
        TreeNode<T> Deserialize(string code);
        string Serialize(TreeNode<T> tree);
    }

    public enum DeleteType
    {
        NodeAndSubTree,
        SingleNode,
    }
}

namespace Evolver5.Util
{
    public class CompilationUtil
    {
        public static (
            bool Success,
            ImmutableArray<Diagnostic> Diagnostics,
            ImmutableArray<Diagnostic> Errors
        ) Compile(string code, string projectFile)
        {
            throw new NotImplementedException();
        }

        public static (
            bool Success,
            ImmutableArray<Diagnostic> Diagnostics,
            ImmutableArray<Diagnostic> Errors
        ) Compile(string code, Project project)
        {
            throw new NotImplementedException();
        }

        public static bool RoundTripCompilation(
            string originalCode,
            string regeneratedCode,
            string csprojFileName
        )
        {
            throw new NotImplementedException();
        }
    }

    public class FileGen<T>
        where T : class
    {
        public FileGen(ISerializeCode<T> tree)
        {
            throw new NotImplementedException();
        }

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
            throw new NotImplementedException();
        }

        public (string newCode, TreeNode<T> tree) RoundTripCode(string code)
        {
            throw new NotImplementedException();
        }

        public (bool success, string newCode, TreeNode<T> tree) RoundTripFile(string file)
        {
            throw new NotImplementedException();
        }

        public string TestRoundTrip(string code)
        {
            throw new NotImplementedException();
        }

        public (bool success, string newCode, TreeNode<T> tree) TestRoundTripFile(string file)
        {
            throw new NotImplementedException();
        }

        ISerializeCode<T> tree;
    }

    public class SyntaxLoader : IDisposable
    {
        public SyntaxLoader()
        {
            throw new NotImplementedException();
        }

        public static MSBuildWorkspace CreateWorkspace()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public static SyntaxTree LoadFile(string csFilePath)
        {
            throw new NotImplementedException();
        }

        public static IReadOnlyList<SyntaxTree> LoadSingleProject(string projectPath)
        {
            throw new NotImplementedException();
        }

        public static Project OpenProject(string projectPath)
        {
            throw new NotImplementedException();
        }

        static SyntaxLoader()
        {
            throw new NotImplementedException();
        }
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

    public partial record ProjectPaths
    {
        public ProjectPaths(string baseNamespace)
        {
            throw new NotImplementedException();
        }

        public static ProjectPaths Create(string baseNamespace) => new(baseNamespace);

        public void PrintPaths(TextWriter? writer = null)
        {
            throw new NotImplementedException();
            foreach (var (label, path) in ToPathDictionary())
            {
                throw new NotImplementedException();
            }
        }

        public IReadOnlyDictionary<string, string> ToPathDictionary() =>
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Base namespace"] = BaseNamespace,
                [".csproj"] = ProjectFilePath,
                ["Consolidated (.cs)"] = ConsolidatedFilePath,
                ["Skeleton (.cs)"] = ClassFilePath,
                ["One-class-per-file namespace"] = OneClassPerFileNamespace,
                ["One-namespace-per-file ns"] = OneNamespacePerFileNamespace,
            };

        public static string Class_Prefix = "Class";
        public static string Consolidated_Prefix = "";
        public static string Method_Prefix = "Method";
        public static string one_class_per_file_namespace = "CF";
        public static string one_namespace_per_file_namespace = "NF";
        public string BaseNamespace { get; }
        public string ClassFilePath { get; }
        public string ConsolidatedFilePath { get; }
        public static ProjectPaths Evolver3 => new(ProjectName.Evolver3.ToString());
        public static ProjectPaths Evolver5 => new(ProjectName.Evolver5.ToString());
        public string MethodFilePath { get; }
        public string NamespaceConsolidated => $"{BaseNamespace}";
        public string OneClassPerFileNamespace =>
            $"{BaseNamespace}.{OneClassPerFileNamespaceSuffix}";
        public string OneClassPerFileNamespaceSuffix => ProjectPaths.one_class_per_file_namespace;
        public string OneNamespacePerFileNamespace =>
            $"{BaseNamespace}.{OneNamespacePerFileNamespaceSuffix}";
        public string OneNamespacePerFileNamespaceSuffix =>
            ProjectPaths.one_namespace_per_file_namespace;
        public string ProjectFilePath { get; }
        public static ProjectPaths RevMo => new(ProjectName.RevMo.ToString());
        public static ProjectPaths Shared => new(ProjectName.Shared.ToString());
        public static ProjectPaths Test => new(ProjectName.Test.ToString());
        public string TestFilePath { get; }
    }

    public static class CSharpierFormatter
    {
        private static MethodInfo formatMethod;
        private static object formatter;
        private static object options;
        private static AssemblyLoadContext context;

        public static string Format(string code, string csharpierPath = @"_dependency\CSharpier")
        {
            throw new NotImplementedException();
        }

        private static void Load(string path)
        {
            throw new NotImplementedException();
        }

        // Call this when you're done with a batch of formats (optional, but frees memory)
        public static void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
