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
using Shared.CF.Core;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
namespace Evolver5
{

#region Public Class
    public class GrokTBD     {
    }

    public static class RoslynTrivia
    {

#region Public Method

#endregion

#region Private Method

#endregion
    }

#endregion
}

namespace Evolver5.Language
{

#region Public Class
    public static class LanguageExtensions
    {

#region Public Method

#endregion
    }

    public static class SyntaxKindGroups
    {

#region Public Field

#endregion
    }

#endregion

#region Public Enum

    public enum AccessModifier
    {
        None,
        Public,
        Protected,
        Private,
        Internal,
    }

    public enum HandledSyntaxKind
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
        Unknown
    }

#endregion
}

namespace Evolver5.Semantic.Code
{

#region Public Class
    public static class SemanticExtensions
    {

#region Public Method

#endregion
    }

    public static class SemanticExtensionsGroupAndRegion
    {

#region Public Method

#endregion

    }

    public class SemanticTree     {

#region Public Method

#endregion
    }

    public static class TreeConverter_SyntaxElementToSemanticNode
    {

#region Public Method

#endregion

#region Private Method

#endregion
    }

#endregion
}

namespace Evolver5.Semantic.Types
{

#region Public Class
    public partial class BlockNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion
    }

    public partial class ClassNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion

#region Protected Method

#endregion
    }

    public partial class CommentNode     {

#region Public Constructor

#endregion
    }

    public partial class CompilationUnitNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion
    }

    public partial class ConstructorNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion
    }

    public partial class DelegateNode     {

#region Public Constructor

#endregion

#region Protected Method

#endregion
    }

    public class DelimitedNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Protected Field

#endregion
    }

    public partial class DestructorNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion
    }

    public partial class EnumMemberNode     {

#region Public Constructor

#endregion
    }

    public partial class EnumNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion

#region Protected Method

#endregion
    }

    public partial class EventFieldNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion
    }

    public partial class EventNode     {

#region Public Constructor

#endregion
    }

    public partial class ExpressionBodyNode     {

#region Public Constructor

#endregion
    }

    public partial class FieldNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion
    }

    public partial class FileScopedNamespaceNode     {

#region Public Constructor

#endregion
    }

    public partial class FileScopedNamespaceNode
    {
    }

    public partial class IndexerNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion
    }

    public partial class InterfaceNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion

#region Protected Method

#endregion
    }

    public partial class MethodNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion
    }

    public abstract class NamedMemberNode     {

#region Public Method

#endregion

#region Protected Constructor

#endregion

#region Private Method

#endregion
    }

    public abstract class NamedNode     {

#region Public Method

#endregion

#region Protected Constructor

#endregion

#region Protected Method

#endregion
    }

    public partial class NamespaceNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion
    }

    public partial class OperatorNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion
    }

    public partial class OperatorNode     {

#region Public Method

#endregion
    }

    public partial class PropertyNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion
    }

    public partial class RecordNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion

#region Protected Method

#endregion
    }

    public partial class RegionEndNode     {

#region Public Constructor

#endregion
    }

    public abstract class RegionNode     {

#region Protected Constructor

#endregion
    }

    public partial class RegionStartNode     {

#region Public Constructor

#endregion
    }

    public abstract class SemanticNode
    {

#region Public Method

#endregion

#region Public Field

#endregion

#region Public Property

#endregion

#region Protected Constructor

#endregion
    }

    public class SimpleMemberAccessNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion
    }

    public partial class StructNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion

#region Protected Method

#endregion
    }

    public partial class TriviaNode     {

#region Public Constructor

#endregion
    }

    public abstract class TypeDeclarationNode     {

#region Protected Constructor

#endregion

#region Protected Method

#endregion
    }

    public partial class UnknownNode     {

#region Public Constructor

#endregion
    }

    public partial class UsingDirectiveNode     {

#region Public Constructor

#endregion
    }

#endregion
}

namespace Evolver5.SyntaxElement.Code
{

#region Public Class
    public class Serializer_SyntaxElement     {

#region Public Method

#endregion
    }

    public class SyntaxElementNode
    {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion

#region Protected Constructor

#endregion
    }

    public static class TreeConverter_TokenNodeToSyntaxElement
    {

#region Public Method

#endregion

#region Private Method

#endregion
    }

#endregion
}

namespace Evolver5.Token.Code
{

#region Public Class
    public class Serializer_TokenNode     {

#region Public Method

#endregion
    }

    public partial class TokenNode     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion

#region Private Method

#endregion
    }

#endregion
}

namespace Evolver5.Tree
{

#region Public Class
    public class DeepTreeCopier<T>
        where T : class
    {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Private Field

#endregion
    }

    public class DeepTreeCopierMulti<T>
        where T : class
    {

#region Public Method

#endregion
    }

    public static class TreeEquality
    {

#region Public Method

#endregion
    }

    public static class TreeExtensions
    {

#region Public Method

#endregion

#region Private Method

#endregion
    }

    public class TreeNode<T>
        where T : class
    {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Property

#endregion

#region Private Field

#endregion
    }

    public static class TreeSiblingExtensions
    {

#region Public Method

#endregion
    }

#endregion

#region Public Interface

    public interface ISerializeCode<T>
        where T : class
    {

#region Private Method

#endregion
    }

#endregion

#region Public Enum

    public enum DeleteType
    {
        NodeAndSubTree,
        SingleNode
    }

#endregion
}

namespace Evolver5.Util
{

#region Public Class
    public class CompilationUtil
    {

#region Public Method

#endregion
    }

    public class FileGen<T>
        where T : class
    {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Private Field

#endregion
    }

    public class SyntaxLoader     {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Private Constructor

#endregion
    }

#endregion

#region Public Enum

    public enum ProjectName
    {
        Evolver3,
        Evolver4,
        Evolver5,
        RevMo,
        Shared,
        Test
    }

#endregion

#region Public Record

    public partial record ProjectPaths
    {

#region Public Constructor

#endregion

#region Public Method

#endregion

#region Public Field

#endregion

#region Public Property

#endregion
    }

#endregion
}public static class _Program
{

#region Public Method

#endregion

#region Private Method

#endregion
}
