
# C# Evolver

**Advanced Roslyn-powered toolkit for semantic C# code transformation, intelligent refactoring, and automated code organization.**

[![GitHub stars](https://img.shields.io/github/stars/terrycode27/csharp-evolver?style=social)](https://github.com/terrycode27/csharp-evolver)
[![License](https://img.shields.io/github/license/terrycode27/csharp-evolver)](https://github.com/terrycode27/csharp-evolver/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Roslyn](https://img.shields.io/badge/Roslyn-powered-6A4EFF)](https://github.com/dotnet/roslyn)

## ✨ Features

- **Semantic Tree Model**: Rich, strongly-typed representation of C# code using a custom `TreeNode<SemanticNode>` hierarchy
- **Intelligent Merging**: Merge partial classes, namespaces, and code from multiple sources while preserving semantics
- **Interface Extraction**: Automatically generate interfaces from classes with proper attributes and documentation
- **Code Organization**: Group members by kind, access modifier, and name for consistent, readable structure
- **Extension Method Handling**: Seamlessly convert between static helper classes and proper C# extension methods
- **File Structure Tools**: Split types into one-file-per-type or consolidate multiple files into a single semantic model
- **Round-trip Safety**: Full serialization/deserialization with compilation verification and CSharpier formatting
- **Large Class Refactoring**: Split oversized classes into partial implementations based on interfaces
- **Self-hosting Tests**: Includes comprehensive self-testing on its own codebase

## 📖 Table of Contents

- [✨ Features](#-features)
- [🚀 Quick Start](#-quick-start)
- [📦 Installation](#-installation)
- [💡 Usage](#-usage)
- [🛠 Tech Stack](#-tech-stack)
- [🔧 Configuration](#-configuration)
- [🧪 Testing / Development](#-testing--development)
- [🚀 Deployment](#-deployment)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [🙏 Acknowledgments](#-acknowledgments)

## 🚀 Quick Start

```bash
git clone https://github.com/terrycode27/csharp-evolver.git
cd csharp-evolver

# Build the solution
dotnet build

# Run self-test (demonstrates all core capabilities)
dotnet run --project Evolver5
```

## 📦 Installation

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- Visual Studio 2022 (recommended for full Roslyn integration)

### As a Library

```bash
dotnet add package CSharpEvolver
```

### From Source

1. Clone the repository
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Build the project:
   ```bash
   dotnet build -c Release
   ```

### Environment Variables

No environment variables are required for core functionality. The tool automatically detects the solution root.

## 💡 Usage

### Basic Example

```csharp
using CSharpEvolver;

// Initialize entry point for a project
var entry = EntryPoint.Create("Evolver5");

// Extract interfaces from all classes
entry.ExtractInterfaces();

// Group code logically by kind, modifier, and name
entry.GroupByKindModifierName();

// Convert static helper methods into proper extension methods
entry.GroupStaticExtensionMethods();
```

### Common Workflows

**Consolidate multiple class files:**

```csharp
var entry = EntryPoint.Create("MyProject");
entry.MergeIntoOneFile("classes", "MyProject.cs");
```

**Split a monolithic file into one type per file:**

```csharp
var entry = EntryPoint.Create("MyProject");
entry.SplitIntoOneTypePerFile("MyProject.cs", "classes");
```

**Refactor a large class using interfaces:**

```csharp
var entry = EntryPoint.Create("MyProject");
entry.RefactorLargeClassIntoPartialsWithInterfaces("LargeService");
```

**Round-trip test (parse → transform → verify):**

```csharp
var path = KnownProjectPaths.Evolver5;
path.TestSemanticSerializer();
path.GroupByKindModifierName();
```

### Command Reference (via EntryPoint)

| Method | Purpose |
|-------|---------|
| `ExtractInterfaces()` | Generates `I*` interfaces from implementation classes |
| `GroupByKindModifierName()` | Organizes members consistently within types |
| `GroupStaticExtensionMethods()` | Converts static classes containing extensions |
| `SplitIntoOneTypePerFile()` | Creates clean one-type-per-file structure |
| `MergeIntoOneFile()` | Combines multiple files into semantic model |
| `ExtractFromNamespaces()` | Flattens namespaces for analysis |
| `MergeSelf()` / `MergeFrom()` | Smart merging of partial definitions |

## 🛠 Tech Stack

- **Language**: C# 12
- **Framework**: .NET 8
- **Compiler Platform**: Microsoft.CodeAnalysis (Roslyn)
- **Formatting**: CSharpier (isolated assembly loading)
- **JSON**: Newtonsoft.Json
- **Build**: MSBuild + Microsoft.Build.Locator
- **Architecture**: Custom immutable-style semantic tree with extensive extension methods

## 🔧 Configuration

The toolkit is primarily code-driven. Key paths are defined in `KnownProjectPaths`:

```csharp
public partial class KnownProjectPaths
{
    public static EntryPoint Evolver5 => new("Evolver5");
}
```

For custom projects:

```csharp
var entry = new EntryPoint("MyProjectName", @"C:\path\to\solution");
```

The project expects a standard structure:
```
MyProject/
├── MyProject.csproj
├── MyProject.cs              # consolidated file
├── classes/                  # one type per file
└── MyProject_test.cs
```

## 🧪 Testing / Development

```bash
# Run all self-tests
dotnet test

# Run specific semantic round-trip test
dotnet run --project Evolver5 --self-test

# Format all generated code
entry.Format();
```

**Testing Capabilities Built-in:**
- Round-trip verification (parse → serialize → compare)
- Compilation testing against original project
- Semantic equivalence checking
- CSharpier formatting validation

## 🚀 Deployment

As a library, simply reference the compiled DLL or NuGet package.

For standalone use:
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

The tool includes isolated loading of CSharpier to avoid Roslyn version conflicts, making it robust for deployment across different .NET environments.

## 🤝 Contributing

Contributions are welcome! This is a sophisticated code manipulation engine — please:

1. Discuss major changes via GitHub Issues first
2. Maintain the high standard of semantic correctness
3. Add or update self-tests when extending functionality
4. Ensure all transformations pass compilation tests

See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Roslyn Team** at Microsoft for the incredible compiler platform
- **CSharpier** for excellent code formatting
- The broader .NET community for inspiration in code generation tools
- All contributors who have helped evolve this codebase using itself

---


Need help or have a refactoring challenge? Open an issue on the [GitHub repository](https://github.com/terrycode27/csharp-evolver).
```

