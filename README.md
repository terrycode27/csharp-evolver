# Evolver5


[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Evolver5 is a powerful C# source code analysis, transformation, and evolution framework. It uses a custom semantic AST (Abstract Syntax Tree) to parse, manipulate, refactor, and generate C# code with high precision. Designed for large-scale codebases, it supports merging duplicates, extracting partial classes, generating documentation, dependency analysis, and more—all while ensuring compilability via integrated checks.

## 🚀 Features

- **Custom Semantic Tree**: Roslyn-inspired parsing into a lightweight, mutable `TreeNode<NodeSemantic>` structure.
- **Code Merging & Deduplication**: Detect and consolidate duplicate types/methods across files (`DocMergeSelf`).
- **Partial Class Refactoring**: Automatically split large classes by interfaces into partials (`DivideIntoPartialClassesByInterfaces`).
- **Documentation Automation**: Generate doc interfaces with attributes (`CreateDocInterfaces`, `MergeAttributesFromDocs`).
- **Hierarchy & Dependency Analysis**: Class hierarchies, module dependencies, dead code detection (`GetAllDerivedClassesDictionary`, `ToModuleDictionary`).
- **Workflows & Formatting**: Chained transformations with CSharpier integration and `dotnet format`.
- **Compilation Validation**: Real-time `dotnet build` checks during evolutions (`TestCompile`, `CompileWithDotNetBuild`).
- **YAML Export**: Serialize hierarchies, types, and modules (`HierarchySave`, `ToYAMLFile`).
- **Advanced Grouping**: Group by kind/modifier/name, class hierarchies (`GroupByKindModifierName`, `BuildClassHierarchy`).
- **Extensibility**: 100+ extension methods for tree traversal, matching, and mutation.

## 📁 Project Structure

```
Evolver5/
├── Core/                 # Tree & syntax primitives
│   ├── SyntaxElementNode.cs
│   ├── SyntaxKindGroups.cs
│   ├── TokenNode.cs
│   ├── TreeNode.cs
│   └── ... (Tree<T>, yamlString)
├── Extensions/           # Fluent APIs (200+ methods)
│   ├── ExtensionsOfTreeNodeOfNodeSemantic.cs  # Core tree ops
│   ├── ExtensionsOfListOfTreeNodeOfNodeSemantic.cs
│   └── ... (IEnumerable, List, Node, String exts)
├── Models/               # Data models
│   ├── EntryPoint.cs     # Project manager
│   ├── ModuleInfo.cs     # Dependency graphs
│   ├── NodeSemantic.cs   # AST base (partial)
│   └── SignatureData.cs  # Type signatures
├── Nodes/                # AST nodes (40+)
│   ├── NodeClass.cs
│   ├── NodeMethod.cs
│   ├── NodeTypeDeclaration.cs
│   └── ... (CompilationUnit, Namespace, etc.)
├── Serializers/          # Code ↔ Tree
│   ├── SerializerSemanticTree.cs
│   └── ...
├── Utils/                # Helpers
│   ├── CompilationUtil.cs
│   ├── CSharpierFormatter.cs
│   └── StaticHelpers.cs
├── Evolver5.cs           # Consolidated entry (this file)
├── Evolver5.yaml         # File hierarchy manifest
├── Evolver5.csproj       # Project file
└── readme.md
```

Full hierarchy: [Evolver5.yaml](Evolver5.yaml)

## 🛠 Quick Start

### Prerequisites
- .NET 8+
- CSharpier (via `_dependency\CSharpier`)

### Build & Run
```bash
git clone https://github.com/yourusername/Evolver5.git
cd Evolver5
dotnet restore
dotnet build
dotnet run
```

This runs the `Main` evolution: generates this README via Grok API (meta!).

### Core Workflow
```csharp
using static KPP.Evolver5;  // EntryPoint "Evolver5"

// Load consolidated code
var tree = Load();  // Evolver5.cs

// Transform: merge duplicates, group, clean
tree = tree
    .DocMergeSelf()                    // Dedupe types
    .GroupByKindModifierName()         // Sort members
    .DocCleanWhitespaceAll();          // Normalize WS

// Save & format
tree.ToFile(ConsolidatedFileFullPath);
DotNetFormat();  // dotnet format

// Compile check
tree.TestCompile(ProjectFilePath, ConsolidatedFileFullPath);
```

## 💡 Examples

### 1. Extract Interface Implementations → Partials
```csharp
var tree = Load();
tree.DivideIntoPartialClassesByInterfaces("MyLargeClass", "IMyInterface");
// → MyLargeClass_Implements_IMyInterface.cs (partial)
```

### 2. Dependency Graph → YAML
```csharp
var modules = tree.ToModuleDictionary();  // { TypeName → ModuleInfo }
modules.ToYamlFile(WithYamlDir("modules.yaml"));
```

### 3. Class Hierarchy Regions
```csharp
var hierarchy = BuildClassHierarchy(GetClasses());
tree.Children.Clear();
hierarchy.ForEach(tree.AddChild);
tree.ToFile(TestFilePath);
```

### 4. Doc Interfaces
```csharp
CreateDocInterfaces();       // → IDocs.cs
DocSplitIntoFilesTree();     // Split to yaml/
DocMergeIntoOneFile();       // Merge back
```

### 5. Multi-File Consolidation
```bash
# From dir
di.MergeCodeDirectoryTo("Evolver5_Consolidated.cs");
```

## 🔧 API Highlights

- **EntryPoint**: Project orchestrator (`KPP.Evolver5`).
  - `Workflow(func)`: Apply transformations + format.
  - `Load()`, `ToFile()`, `DotNetFormat()`.

- **TreeNode<NodeSemantic>**: Core AST.
  - `FindWhere(predicate)`, `DeleteKinds(kinds)`.
  - `ToCode()`, `SerializeFile(path)`.

- **Nodes**: Strongly-typed (`NodeClass`, `NodeMethod`).
  - `ReadSignature()`, `WriteSignature(sig)`.
  - `GetMethods()`, `MemberNames`.

- **Matchers**: `CompareWithKeyResult`, `ListDiff`.

## 🧪 Testing & Validation

- **Compile**: `CompileWithDotNetBuild(projectPath)`.
- **Self-Merge Test**: `TestSelfMerge()`.
- **Tree Health**: `CheckTree()`.

## 🤝 Contributing

1. Fork & PR.
2. Add tests (e.g., new `Workflow`).
3. Run `dotnet format`.
4. Ensure `dotnet build` succeeds.


## 📄 License

MIT License. See [LICENSE](LICENSE) for details.

## 🙏 Acknowledgments

- [Roslyn](https://github.com/dotnet/roslyn): Syntax inspiration.
- [CSharpier](https://csharpier.com/): Formatting.
- [YamlDotNet](https://github.com/aaubry/YamlDotNet): YAML.
- Grok API: README generation (self-referential!).

---

⭐ **Star if useful!** Questions? Open an issue.
