# Evolver5

Evolver5 is a QD C# utility that builds a semantic tree from Roslyn syntax trees and provides operations for loading, merging, reorganizing, and refactoring C# source code while preserving compilation.

## Features

- Merge partial classes, namespaces, and code fragments from multiple sources
- Group static extension methods into dedicated `ExtensionsOfXxx` classes
- Reorder members by access modifier, declaration kind, and name
- Split large classes into interface-based partial classes
- Format output and perform round-trip compilation checks

## Purpose

It is used to organize, understand and merge code anticipating use to merge code output from LLMs.

