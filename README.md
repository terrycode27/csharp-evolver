# csharp-evolver
csharp-evolver is a Roslyn-powered C# transformation engine focused on split and merge capabilities. It intelligently merges partial classes and namespaces while also splitting code into skeletons, grouping extensions, and reorganizing hierarchies. Includes round-trip compilation and CSharpier formatting. Ideal for large-scale .NET refactoring.
# Evolver5

https://grok.com/share/c2hhcmQtMg_83e2433f-461c-4b85-83d5-57d90d183035

**Semantic C# Code Tree & Automated Refactoring Engine**

A powerful, Roslyn-based library that turns any C# source code into a rich **semantic tree** for safe, high-level refactoring and code evolution.

Perfect for:
- Reorganizing messy codebases
- Standardizing member ordering
- Auto-grouping extension methods
- Merging partial classes/namespaces
- Generating clean skeletons
- Round-trip compilation verification

---

## ✨ Features

- **Full round-trip parsing** — parse → manipulate → regenerate identical code
- **Smart grouping** — by access modifier → kind → name
- **Extension method magic** — automatically creates `ExtensionsOfXXX` static classes
- **Namespace flattening** — collapse everything into one clean file
- **Safe merging** — merge duplicate classes/namespaces without conflicts
- **Skeleton generation** — empty method bodies with `NotImplementedException`
- **Built-in compilation testing**
- **CSharpier formatting** integration
- **Class hierarchy regions** — group derived classes under `#region`
- **Project-aware** — works directly with `.csproj` files via MSBuild

---

## 📋 Requirements

- **.NET 6.0+**
- **NuGet packages** (already referenced):
  - `Microsoft.CodeAnalysis.CSharp`
  - `Microsoft.CodeAnalysis.CSharp.Workspaces`
  - `Microsoft.Build.Locator`
  - `CSharpier.Core` (bundled in `_dependency\CSharpier`)

---

## 🚀 Installation

Just drop **`Evolver5.cs`** into your project (single-file library style).

---

## 📖 Sample Usage

### 1. Quick Reorder & Format (Most Common)

```csharp
var paths = ProjectPaths.Create("MyAwesomeLibrary", @"C:\Projects\MyAwesomeLibrary\");

paths.FlattenClassesOutOfNamespaces();
paths.GroupByModifierKindName();
paths.GroupStaticExtensionMethods();
paths.Format();


2. Create a Clean Skeleton

var tree = SemanticTree.DeserializeFile("MyBigClass.cs", fmt: true);
tree.EmptyMemberDeclarationsContents(paths);
tree.ToFormattedFile("MyBigClass.Skeleton.cs");


3. Merge Two Code Versions

var merged = baseTree.MergeFrom(newTree);
merged.ToFormattedFile("merged.cs");

