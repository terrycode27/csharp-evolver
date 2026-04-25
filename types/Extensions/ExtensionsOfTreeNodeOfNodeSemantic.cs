public static partial class ExtensionsOfTreeNodeOfNodeSemantic
{
    public static void AddChildTextNode(this TreeNode<NodeSemantic> node, SyntaxKind kind, string text)
    {
        node.AddChild(StaticHelpers.NewTextNode(kind, text));
    }
    public static void AddNewLine(this TreeNode<NodeSemantic> node)
    {
        node.AddChildTextNode(SyntaxKind.EndOfLineTrivia, "\n");
    }
    public static void AddToFile(this TreeNode<NodeSemantic> node, string filePath)
    {
        node.SerializeFile(filePath);
    }
    public static bool AlreadyHasPartialKeyword(this TreeNode<NodeSemantic> classObj) => classObj.FindWhere(t => t.Value.Kind == SyntaxKind.PartialKeyword).Any();
    public static IEnumerable<TreeNode<NodeSemantic>> Ancestors(this TreeNode<NodeSemantic> node)
    {
        var current = node.Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }
    public static void AppendAttributeTo(this TreeNode<NodeSemantic> tree, string appendTo, string newAttribute)
    {
        foreach (
            NodeAttribute rt in tree.GetTypedList<NodeAttribute>()
                .Where(t => t.TypeName == appendTo)
        )
        {
            rt.NamedMemberNodeParent.InsertNodeAttribute(newAttribute);
        }
    }
    public static TreeNode<NodeSemantic> BuildRegionTree(this TreeNode<NodeSemantic> NodeClass, Dictionary<string, List<TreeNode<NodeSemantic>>> derivedMap, Dictionary<string, TreeNode<NodeSemantic>> byName)
    {
        var className = ((NodeClass)NodeClass.Value).TypeName;
        var region = NodeRegionStart.Factory(className);
        region.AddChild(NodeClass);
        if (derivedMap.TryGetValue(className, out var children))
        {
            children
                .OrderBy(c => ((NodeClass)c.Value).TypeName)
                .Select(c => BuildRegionTree(c, derivedMap, byName))
                .ToList()
                .ForEach(region.AddChild);
        }
        region.AddChild(NodeRegionEnd.Factory());
        return region;
    }
    public static void BulkRenameDerivedClassesOf(this TreeNode<NodeSemantic> tree, string className,Func<string,string> rename)
    {
        var all = tree.GetAllDerivedClassesDictionary();
        var referencedKindDictionary = tree.GetNameReferenceKindDictionary();
        var derivedClasses = all[className];
        foreach (var derivedClass in derivedClasses)
        {
            string derivedClassName = derivedClass.Value.TypeName;
            var newName =rename(derivedClassName);
            var referencedKinds = referencedKindDictionary[derivedClassName];
            foreach (var referencedKind in referencedKinds)
            {
                referencedKind.Text = newName;
            }
        }
    }
    public static bool CheckTree(this TreeNode<NodeSemantic> tree)
    {
        var noPar = tree.Flatten().Where(t => t.Parent == null).ToList();
        if (noPar.Count > 1)
        {
            throw new Exception();
        }
        return true;
    }
    public static TreeNode<NodeSemantic> CleanCopy(this TreeNode<NodeSemantic> node)
    {
        var code = node.ToCode();
        var ret = SerializerSemanticTree.DeserializeAndGroupCode(code);
        return ret.Children.First();
    }
    public static void CleanWhitespaceTypeArgumentList(this TreeNode<NodeSemantic> tree)
    {
        /* foreach (var t in tree.Flatten().Where(t => SyntaxKindGroups.TypeArgumentListKinds.Contains(t.Value.Kind)).ToList())
            {
                t.DeleteKind(SyntaxKind.EndOfLineTrivia);
            }*/
    }
    public static List<SyntaxKind> ComputeKindPath(this TreeNode<NodeSemantic> tree,TreeNode<NodeSemantic> topLevel=null)
    {
        var parent = tree.Parent;
        var kinds = new List<SyntaxKind>();
        kinds.Add(tree.Value.Kind);
        while (parent != topLevel)
        {
            if (!SyntaxKindGroups.WhitespaceTrivia.Contains(parent.Value.Kind) && parent.Value.Kind != SyntaxKind.CompilationUnit)
            {
                kinds.Add(parent.Value.Kind);
            }
            parent = parent.Parent;
        }
        kinds.Reverse();
        return kinds;
    }
    public static TreeNode<NodeSemantic> ConsolidateMultiFileCode(this TreeNode<NodeSemantic> tree)
    {
        var newCode = NodeCompilationUnit.Factory();
        var usings = tree.GetUsings();
        foreach (var u in usings)
            u.tree.DeleteNewlinesAndAdd();
        foreach (var u in usings.GroupBy(t => t.tree.ToCode()).Select(t => t.First()).OrderBy(t => t.tree.ToCode()).ToList())
        {
            newCode.AddChild(u.tree);
        }
        var tdh = tree.GetTypeDeclarations().Where(t => !(t is NodeNamespace) && t.ContainingClassName == null).ToList();
        foreach (var t in tdh)
        {
            if (t is NodeDelegate)
                throw new Exception();
            newCode.AddChild(t.tree);
        }
        newCode.DeleteKinds(SyntaxKindGroups.RegionTrivia);
        return newCode;
    }
    public static void DeleteByNameType<T>(this TreeNode<NodeSemantic> tree, List<string> toRemove)
    {
        var hs = new HashSet<string>(toRemove);
        tree.DeleteRecursive(
            t => hs.Contains(t.Value.TypeName) && t.Value is T,
            DeleteType.NodeAndSubTree
        );
    }
    public static IEnumerable<TreeNode<NodeSemantic>> DeleteKind(this TreeNode<NodeSemantic> tree, SyntaxKind kind, DeleteType deleteType = DeleteType.SingleNode)
    {
        return tree.DeleteKindsSeek(new HashSet<SyntaxKind>() { kind }, deleteType).ToList();
    }
    public static List<TreeNode<NodeSemantic>> DeleteKinds(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds, DeleteType deleteType = DeleteType.SingleNode)
    {
        var ret = tree.DeleteKindsSeek(kinds, deleteType).ToList();
        return ret;
    }
    public static IEnumerable<TreeNode<NodeSemantic>> DeleteKindsSeek(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds, DeleteType deleteType = DeleteType.SingleNode)
    {
        foreach (var x in tree.Flatten().ToList())
        {
            if (kinds.Contains(x.Value.Kind))
            {
                yield return x;
                x.Delete(deleteType);
            }
        }
    }
    public static void DeleteNewlinesAndAdd(this TreeNode<NodeSemantic> node)
    {
        var x = node.DeleteKinds(SyntaxKind.EndOfLineTrivia.ItemToHashSet()).ToList();
        node.AddNewLine();
    }
    public static List<TreeNode<NodeSemantic>> DeleteStartingWithKinds(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds)
    {
        return tree.DeleteStartingWithKindsSeek(kinds).ToList();
    }
    public static IEnumerable<TreeNode<NodeSemantic>> DeleteStartingWithKindsSeek(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds)
    {
        var deleted = new List<TreeNode<NodeSemantic>>();
        bool delete = false;
        foreach (TreeNode<NodeSemantic> x in tree.Flatten().ToList())
        {
            if (kinds.Contains(x.Value.Kind))
                delete = true;
            if (delete)
            {
                yield return x;
                x.RemoveSelf();
            }
        }
    }
    public static TreeNode<NodeSemantic> DeleteUsings(this TreeNode<NodeSemantic> tree)
    {
        foreach(var u in tree.GetUsings())
        {
            u.tree.RemoveSelf();
        }
        return tree;
    }
    public static void DivideIntoPartialClassByInterface(this TreeNode<NodeSemantic> tree, TreeNode<NodeSemantic> classObj, TreeNode<NodeSemantic> interfaceDecl)
    {
        var className = classObj.Value.TypeName;
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
    public static void DivideIntoPartialClassesByInterfaces(this TreeNode<NodeSemantic> tree, string className, string interfaceName = null)
    {
        var tnsn = tree.FindWhere(t =>
                t.Value.Kind == SyntaxKind.ClassDeclaration && t.Value.TypeName == className
            )
            .ToTypedList<NodeTypeDeclaration>()
            .First();
        if (interfaceName == null)
            interfaceName = tnsn.BaseType;
        var baseInterface = tree.FindWhere(t =>
                t.Value.Kind == SyntaxKind.InterfaceDeclaration && t.Value.TypeName == interfaceName
            )
            .First();
        var ia = (NodeInterface)baseInterface.Value;
        foreach (var i in ia.BaseTypes)
        {
            var iDecl = tree.FindWhere(t =>
                    t.Value.Kind == SyntaxKind.InterfaceDeclaration && t.Value.TypeName == i
                )
                .First();
            DivideIntoPartialClassByInterface(tree, tnsn.tree, iDecl);
        }
    }
    public static TreeNode<NodeSemantic> DocCleanWhitespaceAll(this TreeNode<NodeSemantic> tree)
    {
        var methods = tree.GetTypedList<NodeNamedMember>();
        foreach (var x in methods)
        {
            x.CleanWhitespace();
        }
        return tree;
    }
    public static TreeNode<NodeSemantic> DocDeleteDuplicateEOL(this TreeNode<NodeSemantic> tree)
    {
        var en = tree.Flatten().ToList().GetEnumerator();
        List<TreeNode<NodeSemantic>> vals = new();
        while (en.MoveNext())
        {
            if (en.Current.Value.IsWhitespace)
            {
                vals.Add(en.Current);
            }
            else
            {
                vals.Where(t => t.Value.Kind == SyntaxKind.EndOfLineTrivia).Skip(1).ForEach(t => t.Delete());
                vals = new();
            }
        }
        var x = tree.FindKind(SyntaxKind.EndOfLineTrivia).ToList();
        return tree;
    }
    public static TreeNode<NodeSemantic> DocDoAll(this TreeNode<NodeSemantic> tree)
    {
        return tree.DocGroupExtensionsSelfMergeOrderAndCleanWhitespace();
    }
    public static TreeNode<NodeSemantic> DocGroupByKindModifierName(this TreeNode<NodeSemantic> tree)
    {
        return tree.GroupByKindModifierName().Tree;
    }
    public static TreeNode<NodeSemantic> DocGroupExtensionsSelfMergeOrderAndCleanWhitespace(this TreeNode<NodeSemantic> tree)
    {
        tree.DocGroupStaticExtensionMethods();
        tree = tree.Reload();
        tree = tree.DocMergeSelf();
        tree.GroupByKindModifierName();
        tree.DocDeleteDuplicateEOL();
        tree.DocCleanWhitespaceAll();
        return tree;
    }
    public static TreeNode<NodeSemantic> DocGroupStaticExtensionMethods(this TreeNode<NodeSemantic> tree)
    {
        var ret = new List<TreeNode<NodeSemantic>>();
        tree.MoveNonExtensionStaticMembersInStaticClassesIntoSingleClass();
        var extensionMethods = tree.FindStaticExtensionMethods();
        extensionMethods.SetStaticExtensionMethodsPublic();
        var classes = tree.GetClasses().Where(t => t.IsStatic).ToList();
        foreach (var c in classes)
        {
            c.tree.RemoveSelf();
        }
        var dict = extensionMethods
            .GroupBy(t => ((NodeMethod)t.Value).GetStaticExtensionClassName())
            .ToDictionary(t => t.Key, t => t.ToList());
        foreach (var kv in dict)
        {
            var declaration = new StringWriter();
            declaration.WriteLine($"public static partial class {kv.Key}");
            declaration.WriteLine("{");
            declaration.WriteLine("}");
            var newNode = kv.Value.WrapInDeclaration(declaration.ToString());
            tree.Children.Add(newNode);
        }
        return tree;
    }
    public static TreeNode<NodeSemantic> DocMergeFrom(this TreeNode<NodeSemantic> dest, TreeNode<NodeSemantic> source)
    {
        dest.MergeFromAdditive(source);
        dest.MergeFromEdit(source);
        return dest;
    }
    public static TreeNode<NodeSemantic> DocMergeSelf(this TreeNode<NodeSemantic> tree)
    {
        var mergeTypes = tree.GetTypeKinds<NodeTypeDeclaration>(SyntaxKindGroups.SelfMergeKinds);
        var compareResult = mergeTypes.CompareWithKeyFrom(mergeTypes, t => t.FullNameWithTypeParameters);
        var newNodes = new List<TreeNode<NodeSemantic>>();
        var names = compareResult.DuplicatesInSource.Select(t => t.Value.First().FullNameWithTypeParameters).ToList();
        foreach (var k in compareResult.DuplicatesInSource)
        {
            var toAdd = k.Value.SupersetTypeDeclarations();
            k.Value.ForEach(t => t.tree.RemoveSelf());
            if (toAdd.Children.First().Value.FullName.Contains("extensions"))
            {
                int bp = 0;
            }
            newNodes.Add(toAdd.Children.First());
        }
        foreach (var n in newNodes)
            tree.AddChild(n);
        return tree;
    }
    public static TreeNode<NodeSemantic> DocOrderCollectionValues(this TreeNode<NodeSemantic> originalTree)
    {
        foreach (
            var x in originalTree.FindWhere(t =>
                SyntaxKindGroups.CollectionInitializerKinds.Contains(t.Value.Kind)
            )
        )
        {
            x.Children = x.Children.Interleave(t => t.ToCode(), t => t.Value.Kind == SyntaxKind.SimpleMemberAccessExpression);
        }
        return originalTree;
    }
       public static void DocSplitIntoFilesFlat(this TreeNode<NodeSemantic> tree, string dir)
    {
        var tdn = tree.Children.Where(t => t.Value.Kind != SyntaxKind.UsingDirective).ToList();
        dir.ToDI().DeleteAndCreate();
        foreach (var t in tdn)
        {
            var cu = NodeCompilationUnit.Factory();
            //cu.Children.AddRange(usings);
            cu.Children.Add(t);
            string file = Path.Combine(dir, t.Value.TypeName + ".cs");
            StreamWriter sw = new StreamWriter(file, true);
            sw.WriteLine(cu.ToCode());
            sw.Close();
        }
    }
    public static TreeNode<NodeSemantic> DocSplitIntoMethodFile(this TreeNode<NodeSemantic> tree)
    {
        var methodTree = NodeCompilationUnit.Factory();
        foreach (var c in tree.GetClasses())
        {
            c.MarkPartial();
            var newClass = (NodeClass)c.CopyDeclaration().Value;
            newClass.PruneDeclarationCopy();
            foreach (var m in c.GetMethods())
            {
                newClass.MembersNode.AddChild(m.tree);
            }
            if (newClass.MembersNode.Children.Count > 0)
                methodTree.AddChild(newClass.tree);
        }
        return methodTree;
    }
    public static void EnsureClassIsMarkedPartial(this TreeNode<NodeSemantic> classObj)
    {
        if (classObj.AlreadyHasPartialKeyword())
            return;
        var classKeyword = classObj.FindWhere(t => t.Value.Kind == SyntaxKind.ClassKeyword).First();
        classObj.InsertPartialKeywordBefore(classKeyword);
    }
    public static TreeNode<NodeSemantic> ExtractFromNamespace(this TreeNode<NodeSemantic> tree)
    {
        var cu = NodeCompilationUnit.Factory();
          foreach(var c in tree.GetTypedList<NodeTypeDeclaration>().Where(t=>!SyntaxKindGroups.NamespaceLevelExclude.Contains(t.Kind)))
          {
             cu.AddChild(c.tree);
          }
        return cu;
    }
    public static TreeNode<NodeSemantic> ExtractInterfaces(this TreeNode<NodeSemantic> root, Func<string, string> rename)
    {
        root.DeleteKinds(SyntaxKindGroups.AttributeKinds, DeleteType.NodeAndSubTree);
        var cu = NodeCompilationUnit.Factory();
        foreach (var c in root.GetTypedList<NodeClass>())
        {
            var ift = c.ExtractInterface(rename);
            cu.Children.Add(ift.tree);
        }
        return cu;
    }
    public static List<string> FilterTypesOnly(this TreeNode<NodeSemantic> tree, List<string> identifiers)
    {
        var typeNames = tree.GetTypeDeclarations().Select(t => t.TypeName).ToList();
        var ret = (from t in typeNames join i in identifiers on t equals i select t).ToList();
        return ret;
    }
    public static IEnumerable<T> FindByName<T>(this TreeNode<NodeSemantic> tree, string name) where T : NodeSemantic
    {
        return tree.FindByNames<T>(name.AsList());
    }
    public static IEnumerable<T> FindByNames<T>(this TreeNode<NodeSemantic> tree, List<string> find) where T : NodeSemantic
    {
        var hs = new HashSet<string>(find);
        return tree.FindWhere(t => hs.Contains(t.Value.TypeName) && t.Value is T)
            .Select(t => (T)t.Value);
    }
    public static TreeNode<NodeSemantic> FindDeadCode(this TreeNode<NodeSemantic> tree)
    {
        var nameReferenceDictionary = tree.GetNameReferenceKindDictionary();
        var nameMemberDictionary = tree.GetTypedList<NodeParameterizedMemberWithBody>().ToGroupedDictionary(t => t.TypeName);
        var deadCodeDictionary = nameReferenceDictionary.Where(t => t.Value.Count == 1).ToDictionary(t => t.Key, t => t);
        var combinedDead = deadCodeDictionary.CombineDictionaries(nameMemberDictionary);
        var deadCodeCleanTuple = combinedDead.Select(t => (t.Key, t.Value.Second)).OrderBy(t => t.Key).ToList();
        return tree;
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindDelimitedText(this TreeNode<NodeSemantic> root, HashSet<SyntaxKind> delimeters)
    {
        if (root == null)
            return Enumerable.Empty<TreeNode<NodeSemantic>>();
        return root.AsList().FindDelimitedText(delimeters);
    }
    public static List<TreeNode<NodeSemantic>> FindImplementingMethodsInClass(this TreeNode<NodeSemantic> classObj, HashSet<string> interfaceMethodNames) => classObj.FindWhere(t => t.Value.Kind == SyntaxKind.MethodDeclaration).Where(m => interfaceMethodNames.Contains(((NodeMethod)m.Value).TypeName)).ToList();
    public static List<TreeNode<NodeSemantic>> FindKind(this TreeNode<NodeSemantic> tree, SyntaxKind kind)
    {
        return tree.FindWhere(t => t.Value.Kind == kind).ToList();
    }
    public static List<TreeNode<NodeSemantic>> FindKinds(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds)
    {
        return tree.FindWhere(t => kinds.Contains(t.Value.Kind)).ToList();
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindKindsAfter(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kindsToSkip, HashSet<SyntaxKind> kindsToFind)
    {
        return tree.FindKindsAfter(kindsToSkip).Where(t => kindsToFind.Contains(t.Value.Kind));
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindKindsAfter(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kindsToSkip)
    {
        var en = tree.Flatten().GetEnumerator();
        while (en.MoveNext())
        {
            if (kindsToSkip.Contains(en.Current.Value.Kind))
            {
                en.MoveNext();
                break;
            }
        }
        while (true)
        {
            yield return en.Current;
            if (!en.MoveNext())
                break;
        }
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindKindsAt(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds)
    {
        var en = tree.Flatten().GetEnumerator();
        while (en.MoveNext())
        {
            if (kinds.Contains(en.Current.Value.Kind))
                break;
        }
        while (true)
        {
            yield return en.Current;
            if (!en.MoveNext())
                break;
        }
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindKindsStop(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds, HashSet<SyntaxKind> stopKinds)
    {
        foreach (var f in tree.Flatten())
        {
            if (stopKinds.Contains(f.Value.Kind))
                yield break;
            if (kinds.Contains(f.Value.Kind))
                yield return f;
        }
    }
    public static TreeNode<NodeSemantic> FindNameAfterType(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> stopKinds)
    {
        return tree.FindSkipStop(
                t => t.Value.Kind == SyntaxKind.IdentifierToken,
                t =>
                    SyntaxKindGroups.MemberModifierKeywords.Contains(t.Value.Kind)
                    || SyntaxKindGroups.KindsContainingParameterType.Contains(t.Value.Kind)
                    || t.Value.Kind == SyntaxKind.AttributeList,
                t => stopKinds.Contains(t.Value.Kind)
            )
            .FirstOrDefault();
    }
    public static TreeNode<NodeSemantic> FindNameAfterTypeBeforeSemicolon(this TreeNode<NodeSemantic> tree)
    {
        var stopKinds = new HashSet<SyntaxKind> { SyntaxKind.SemicolonToken };
        return tree.FindNameAfterType(stopKinds);
    }
    public static List<TreeNode<NodeSemantic>> FindNotKinds(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds)
    {
        return tree.FindWhere(t => !kinds.Contains(t.Value.Kind)).ToList();
    }
    public static TreeNode<NodeSemantic> FindParentByFullName(this TreeNode<NodeSemantic> root, string parentFullName)
    {
        if (string.IsNullOrEmpty(parentFullName))
            return root;
        return root.FindWhere(t =>
                t.Value.FullName == parentFullName && t.Value is not NodeUsingDirective
            )
            .First();
    }
    public static List<TreeNode<NodeSemantic>> FindStaticExtensionMethods(this TreeNode<NodeSemantic> tree)
    {
        var ret = tree.GetMethods()
            .Where(t => t.IsStatic && t.IsExtension)
            .Select(t => t.tree)
            .ToList();
        return ret;
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindStopAt(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds)
    {
        return tree.FindWhereStop(t => true, t => kinds.Contains(t.Value.Kind));
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindSyntaxKinds(this TreeNode<NodeSemantic> node, HashSet<SyntaxKind> kinds)
    {
        var ret = node.FindWhere(t => kinds.Contains(t.Value.Kind));
        return ret;
    }
    public static List<NodeTypeDeclaration> FindTypeNodes(this TreeNode<NodeSemantic> tree, HashSet<string> typesToInclude)
    {
        var ret = tree.FindWhere(t =>
                typesToInclude.Contains(t.Value.TypeName) && t.Value is NodeTypeDeclaration
            )
            .Select(t => (NodeTypeDeclaration)t.Value)
            .ToList();
        return ret;
    }
    public static IEnumerable<TreeNode<NodeSemantic>> FindWhereIgnoreConsecutive(this TreeNode<NodeSemantic> tree, Func<TreeNode<NodeSemantic>, bool> predicate)
    {
        if (tree == null || predicate == null)
            yield break;
        bool lastMatched = false;
        foreach (var node in tree.Flatten())
        {
            bool currentMatches = predicate(node);
            if (currentMatches && !lastMatched)
            {
                yield return node;
            }
            lastMatched = currentMatches;
        }
    }
    public static Dictionary<string, List<TreeNode<NodeSemantic>>> GetAllDerivedClassesDictionary(this TreeNode<NodeSemantic> tree)
    {
        var classes = tree.FindWhere(t => t.Value.Kind == SyntaxKind.ClassDeclaration)
            .ToDictionary(t => ((NodeClass)t.Value).TypeName, t => t);
        var directChildren = new Dictionary<string, List<TreeNode<NodeSemantic>>>();
        foreach (var c in classes.Values)
        {
            var baseName = ((NodeClass)c.Value).BaseType;
            if (baseName != null && classes.ContainsKey(baseName))
            {
                if (!directChildren.ContainsKey(baseName))
                    directChildren[baseName] = new();
                directChildren[baseName].Add(c);
            }
        }
        var result = new Dictionary<string, List<TreeNode<NodeSemantic>>>();
        void CollectAllDerived(string name, List<TreeNode<NodeSemantic>> list)
        {
            if (directChildren.TryGetValue(name, out var children))
                foreach (var child in children)
                {
                    list.Add(child);
                    CollectAllDerived(((NodeClass)child.Value).TypeName, list);
                }
        }
        foreach (var name in classes.Keys)
        {
            var list = new List<TreeNode<NodeSemantic>>();
            CollectAllDerived(name, list);
            list.Add(classes[name]);
            result[name] = list;
        }
        foreach (var key in result.Keys)
        {
            result[key] = result[key].OrderBy(t => t.Value.TypeName).ToList();
        }
        return result;
    }
    public static List<NodeClass> GetClasses(this TreeNode<NodeSemantic> tree)
    {
        return tree.GetTypedList<NodeClass>();
    }
    public static List<string> GetDependencies(this TreeNode<NodeSemantic> root, NodeTypeDeclaration type)
    {
        var ret = new List<string>();
        var identifiers = type.tree.GetIdentifiers();
        var identifierTypes = root.FilterTypesOnly(identifiers);
        identifierTypes.Remove(type.TypeName);
        return identifierTypes;
    }
    public static List<NodeTypeDeclaration> GetDependents(this TreeNode<NodeSemantic> tree, string typeName)
    {
        var dependents = new HashSet<NodeTypeDeclaration>();
        var allTypes = tree.GetTypeDeclarations();
        foreach (var dependentType in allTypes.Where(t => t.TypeName != typeName))
        {
            var dependentIdentifiers = dependentType.tree.GetIdentifiers();
            if (dependentIdentifiers.Contains(typeName))
                dependents.Add(dependentType);
        }
        return dependents.ToList();
    }
    public static List<string> GetDerivedClasses(this TreeNode<NodeSemantic> tree, string typeName)
    {
        var allTypes = tree.GetTypeDeclarations().Where(t => t.TypeName != typeName);
        List<string> ret = new List<string>();
        foreach (var type in allTypes)
        {
            if (type.BaseTypes.Contains(typeName))
                ret.Add(type.TypeName);
        }
        return ret;
    }
    public static List<string> GetIdentifiers(this TreeNode<NodeSemantic> tree)
    {
        var ret = tree.FindKinds(SyntaxKindGroups.TypeNameLeafKinds)
            .Select(t => ((NodeSemantic)t.Value).Text)
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .ToList();
        return ret;
    }
    public static string GetInterfaceName(this TreeNode<NodeSemantic> interfaceDecl) => ((NodeInterface)interfaceDecl.Value).TypeName ?? string.Empty;
    public static List<TreeNode<NodeSemantic>> GetLowestLevelTypes(this TreeNode<NodeSemantic> root) => root.FindWhere(t => t.Value.HasName && t.Value.ChildOnly).ToList();
    public static int GetMaxParentCount(this TreeNode<NodeSemantic> root) => root.GetNodeNameds().Max(t => t.Value.Parents.Count);
    public static HashSet<string> GetMethodNamesDeclaredInInterface(this TreeNode<NodeSemantic> interfaceDecl) => interfaceDecl.FindWhere(t => t.Value.Kind == SyntaxKind.MethodDeclaration).Select(t => ((NodeMethod)t.Value).TypeName).Where(n => !string.IsNullOrEmpty(n)).ToHashSet(StringComparer.Ordinal);
    public static List<NodeMethod> GetMethods(this TreeNode<NodeSemantic> tree)
    {
        return tree.FindWhere(t => t.Value.Kind == SyntaxKind.MethodDeclaration)
            .Select(t => (NodeMethod)t.Value)
            .ToList();
    }
    public static Dictionary<string, List<NodeSemantic>> GetNameReferenceKindDictionary(this TreeNode<NodeSemantic> tree)
    {
        var ret = tree.GetTypeKinds<NodeSemantic>(SyntaxKindGroups.NameReferenceKinds).Where(t => !t.IsWhitespace).ToList().ToGroupedDictionary(t => t.Text);
        return ret;
    }
    public static List<NodeNamed> GetNodeNamedList(this TreeNode<NodeSemantic> tree)
    {
        return tree.GetNodeNameds().Select(t => (NodeNamed)t.Value).ToList();
    }
    public static List<NodeNamed> GetNodeNameds(this TreeNode<NodeSemantic> tree, int i)
    {
        return tree.FindWhere(t => t.Value.Parents.Count == i).ToTypedList<NodeNamed>();
    }
    public static IEnumerable<TreeNode<NodeSemantic>> GetNodeNameds(this TreeNode<NodeSemantic> root) => root.FindWhere(t => t.Value.HasName);
    public static IEnumerable<TreeNode<NodeSemantic>> GetNodeNamedsAtDepth(this TreeNode<NodeSemantic> root, int depth) => root.GetNodeNameds().Where(t => t.Value.Parents.Count == depth);
    public static List<TreeNode<NodeSemantic>> GetNonExtensionStaticNamedMembers(this TreeNode<NodeSemantic> root)
    {
       var ret=root.GetClasses()
                   .Where(c => c.IsStatic)
                   .SelectMany(c => c.MembersNode.Children).Select(t=>(NodeNamedMember)t.Value)
                   .Where(member =>
                       member.IsStaticMember() &&
                       !member.IsExtensionMethod())
                   .Select(member => member.tree)
                   .ToList();
        return ret;
    }
    public static List<NodeTypeDeclaration> GetSelfMergeTypeKinds(this TreeNode<NodeSemantic> tree, int i)
    {
        return tree.FindWhere(t =>
                SyntaxKindGroups.SelfMergeKinds.Contains(t.Value.Kind)
                && t.Value.Parents.Count == i
            )
            .ToTypedList<NodeTypeDeclaration>();
    }
    public static List<NodeUsingDirective> GetSystemUsings(this TreeNode<NodeSemantic> tree)
    {
        return tree.GetUsings()
            .Where(t => t.TypeName.StartsWith("Microsoft") || t.TypeName.StartsWith("System"))
            .ToList();
    }
    public static T GetTypeByName<T>(this TreeNode<NodeSemantic> tree, string name) where T : NodeSemantic
    {
        return tree.GetTypeByNamesList<T>(name.AsList()).FirstOrDefault();
    }
    public static List<T> GetTypeByNamesList<T>(this TreeNode<NodeSemantic> tree, List<string> names) where T : NodeSemantic
    {
        return tree.FindWhere(t => t.Value is T && names.ToHashSet().Contains(t.Value.TypeName))
            .Select(t => (T)t.Value)
            .ToList();
    }
    public static Dictionary<string, List<T>> GetTypedDictionary<T>(this TreeNode<NodeSemantic> tree) where T : NodeNamed
    {
        var ret = tree.GetTypedList<T>().GroupBy(t => t.TypeName).ToDictionary(t => t.Key, t => t.ToList());
        return ret;
    }
    public static List<NodeTypeDeclaration> GetTypeDeclarations(this TreeNode<NodeSemantic> tree)
    {
        return tree.GetTypedList<NodeTypeDeclaration>();
    }
    public static List<T> GetTypedList<T>(this TreeNode<NodeSemantic> tree)
    {
        return tree.FindWhere(t => t.Value is T).ToTypedList<T>();
    }
    public static List<T> GetTypedListKinds<T>(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds)
    {
        return tree.FindWhere(t => kinds.Contains(t.Value.Kind) && t.Value is T).ToTypedList<T>();
    }
    public static List<T> GetTypedListSkip<T>(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> skipKinds)
    {
        return tree.FindWhere(t => t.Value is T && !skipKinds.Contains(t.Value.Kind)).ToTypedList<T>();
    }
    public static List<T> GetTypeKinds<T>(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds) where T : NodeSemantic
    {
        return tree.GetTypedListKinds<T>(kinds);
    }
    public static List<TreeNode<NodeSemantic>> GetTypes<T>(this TreeNode<NodeSemantic> tree, List<string> typeNames) where T : NodeSemantic
    {
        var hs = typeNames.ToHashSet();
        return tree.FindWhere(t => hs.Contains(t.Value.TypeName) && t.Value is T).ToList();
    }
    public static List<NodeNamedMember> GetTypesAndMembers<T>(this TreeNode<NodeSemantic> tree) where T : NodeSemantic
    {
        return tree.GetTypedList<T>()
            .SelectMany(t => t.tree.GetTypedList<NodeNamedMember>())
            .ToList();
    }
    public static T GetTypeSingle<T>(this TreeNode<NodeSemantic> tree, string name) where T : NodeSemantic
    {
        return tree.GetTypeByNamesList<T>(name.AsList()).Single();
    }
    public static List<NodeUsingDirective> GetUsings(this TreeNode<NodeSemantic> tree)
    {
        return tree.FindWhere(t => t.Value.Kind == SyntaxKind.UsingDirective)
            .Select(t => (NodeUsingDirective)t.Value)
            .ToList();
    }
    public static (TreeNode<NodeSemantic> Tree, List<List<TreeNode<NodeSemantic>>> Groups) GroupByKindModifierName(this TreeNode<NodeSemantic> tree)
    {
        var groups = new List<List<TreeNode<NodeSemantic>>>();
        foreach (var x in tree.FindWhere(t => t.Value.HasDirectNamedChildren))
        {
            var sorted = x.Children.SortedByKindModifierName();
            groups.AddRange(sorted.GroupByKindModifier());
            x.Children = sorted;
        }
        return (tree, groups);
    }
    public static TreeNode<NodeSemantic> GroupConsecutiveChildren(this TreeNode<NodeSemantic> node, Func<TreeNode<NodeSemantic>, bool> predicate)
    {
        if (node.Children.Count == 0)
            return new TreeNode<NodeSemantic>(node.Value);
        var groups = node.Children.GroupConsecutive(predicate);
        var copy = new TreeNode<NodeSemantic>(node.Value);
        if (groups.Count > 1)
        {
            groups.ForEach(group =>
            {
                var cu = NodeCompilationUnit.Factory();
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
    public static TreeNode<NodeSemantic> GroupNodes(this TreeNode<NodeSemantic> tree)
    {
        tree = tree.GroupConsecutiveChildren(t => t.Value.HasName);
        var groupKinds = tree.GetTypeKinds<NodeTypeDeclaration>(SyntaxKindGroups.GroupKinds).Where(t => t.tree.Children.Count != 3).ToList();
        foreach (var c in groupKinds)
        {
            c.ExtractMembersNode();
        }
        var nf = groupKinds.Where(t => t.tree.Children.Count != 3).ToList();
        if (nf.Any())
            throw new Exception();
        foreach (var t in tree.GetTypedList<NodeNamedMember>())
        {
            t.ExtractSignatureNode();
            t.NodeSignatureAndAttribute.SplitByAttribute();
        }
        return tree;
    }
    public static TreeNode<NodeSemantic> HydrateObjectInitializer(this TreeNode<NodeSemantic> tree, string className)
    {
        foreach (var x in tree.FindKind(SyntaxKind.CollectionInitializerExpression))
        {
            if (x.FindKind(SyntaxKind.EndOfLineTrivia).Any() || x.Value.TypeDeclarationParent.TypeName != className)
                continue;
            foreach (var child in x.Children)
            {
                if (child.Value.Kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    child.PreviousSibling().AddNewLine();
                }
                if (child.Value.Kind == SyntaxKind.CloseBraceToken || child.Value.Kind == SyntaxKind.OpenBraceToken)
                    child.PreviousSibling().AddNewLine();
            }
        }
        return tree;
    }
    public static void InitializeAllNodes(this TreeNode<NodeSemantic> node)
    {
        foreach (var child in node.Children)
        {
            child.InitializeAllNodes();
        }
        node.Value.SetTreeNode(node);
    }
    public static void InsertPartialKeywordBefore(this TreeNode<NodeSemantic> classObj, TreeNode<NodeSemantic> classKeyword)
    {
        var partialElement = new SyntaxElementNode(SyntaxKind.PartialKeyword, "partial ");
        var partialSemantic = new NodeUnknown(partialElement);
        var partialNode = new TreeNode<NodeSemantic>(partialSemantic);
        partialSemantic.SetTreeNode(partialNode);
        classKeyword.InsertBefore(partialNode);
    }
    public static TreeNode<NodeSemantic> MakeIntoAbstractClasses(this TreeNode<NodeSemantic> root, Func<string, string> rename)
    {
        var cu = NodeCompilationUnit.Factory();
        foreach (var c in root.GetTypedList<NodeClass>())
        {
            var res = c.BuildAbstractClass(rename);
            cu.Children.Add(res);
        }
        return cu;
    }
    public static void MarkMethodAccessByUsage(this TreeNode<NodeSemantic> tree)
    {
        var singleClassTokens = tree.GetTypedList<NodeIdentifierToken>()
            .GroupBy(t => t.IdentifierText)
            .Where(t => t.GroupBy(x => x.ContainingClassName).Count() == 1)
            .Select(t => t.Key)
            .Distinct()
            .OrderBy(t => t)
            .ToList();
        var singleClassTokenHS = new HashSet<string>(singleClassTokens);
        var singleClassMethods = tree.GetTypedList<NodeMethod>()
            .Where(t => singleClassTokenHS.Contains(t.TypeName))
            .ToList();
        foreach (var m in singleClassMethods)
        {
            if (m.TypeName == "Main")
                continue;
            if (m.IsVirtual)
                m.Modifier = AccessModifier.Protected;
            else
                m.Modifier = AccessModifier.Private;
        }
    }
    public static TreeNode<NodeSemantic> MergeFromAdditive(this TreeNode<NodeSemantic> dest, TreeNode<NodeSemantic> source)
    {
        var nnDest = dest.GetTypedList<NodeNamed>();
        var dParentDictionary = nnDest
            .GroupBy(t => t.ParentFullName)
            .ToDictionary(t => t.Key, t => t.ToList());
        var nnSource = source.GetTypedList<NodeNamed>();
        var compareResult = nnDest.CompareWithKeyFrom(nnSource, t => t.FullName);
        var sParentDictionary = compareResult
            .OnlyInSource.GroupBy(t => t.ParentFullName)
            .ToDictionary(t => t.Key, t => t.ToList());
        var toMerge = (
            from d in dParentDictionary
            join s in sParentDictionary on d.Key equals s.Key
            select new { d, s }
        ).ToList();
        foreach (var m in toMerge)
        {
            m.d.Value.FirstParentChildren().AddRange(m.s.Value.Select(t => t.tree));
        }
        return dest;
    }
    public static TreeNode<NodeSemantic> MergeFromEdit(this TreeNode<NodeSemantic> dest, TreeNode<NodeSemantic> source)
    {
        var nnDest = dest.GetTypedList<NodeNamed>();
        var nnSource = source.GetTypedList<NodeNamed>();
        var compareResult = nnDest.CompareWithKeyFrom(nnSource, t => t.FullName);
        var typeEditMerge = compareResult
            .InBothPairs.Where(t => t.FromSource is NodeTypeDeclaration)
            .ToList();
        var memberEditMerge = compareResult
            .InBothPairs.Where(t => !(t.FromSource is NodeTypeDeclaration))
            .ToList();
        foreach (var m in memberEditMerge)
        {
            m.FromDestination.tree.ReplaceSelf(m.FromSource.tree);
        }
        foreach (var t in typeEditMerge)
        {
            var st = (NodeTypeDeclaration)t.FromSource;
            var dt = (NodeTypeDeclaration)t.FromDestination;
            var newSig = st.ReadSignature().Combine(dt.ReadSignature());
            st.WriteSignature(newSig);
        }
        return dest;
    }
    public static void MoveNonExtensionStaticMembersInStaticClassesIntoSingleClass(this TreeNode<NodeSemantic> root)
    {
        var membersToMove = root.GetNonExtensionStaticNamedMembers();
        foreach (var c in membersToMove)
        {
            c.Value.Modifier = AccessModifier.Public;
            c.RemoveSelf();
        }
        var declaration = new StringWriter();
        declaration.WriteLine($"public partial class StaticHelpers");
        declaration.WriteLine("{");
        declaration.WriteLine("}");
        var existingClass = root.GetTypeByName<NodeClass>("StaticHelpers");
                if (existingClass != null)
        {
            existingClass.tree.RemoveSelf();
            membersToMove.AddRange(existingClass.MembersNode.Children);
        }
        var newClassTree = membersToMove.WrapInDeclaration(declaration.ToString());
        root.Children.Add(newClassTree);
    }
    public static void PullChildrenOutOfNamespaces(this TreeNode<NodeSemantic> root)
    {
        var namespaces = root.FindWhere(t => t.Value.Kind == SyntaxKind.NamespaceDeclaration)
            .ToList();
        var usings = root.FindWhere(t => t.Value.Kind == SyntaxKind.UsingDirective).ToList();
        foreach (var ns in namespaces)
        {
            root.PullChildrenOutOfNamespacesHelper(usings, ns);
        }
    }
    public static void PullChildrenOutOfNamespacesHelper(this TreeNode<NodeSemantic> root, List<TreeNode<NodeSemantic>> usings, TreeNode<NodeSemantic> ns)
    {
        var types = ns.FindSyntaxKinds(SyntaxKindGroups.NamespaceTypeChildren).ToList();
        ns.RemoveSelf();
        var nsUsing = usings.Where(t => t.Value.TypeName == ns.Value.TypeName).FirstOrDefault();
        nsUsing.RemoveSelf();
        root.Children.AddRange(types);
    }
    public static TreeNode<NodeSemantic> Reload(this TreeNode<NodeSemantic> node)
    {
        var code = node.ToCode();
        var ret = SerializerSemanticTree.DeserializeAndGroupCode(code);
        return ret;
    }
    public static void ReloadClassFormatted(this TreeNode<NodeSemantic> tree, string className)
    {
        var node = tree.Flatten().Where(t => t.Value.TypeName == className && t.Value.Kind == SyntaxKind.ClassDeclaration).First();
        var code = node.ToCode();
        code = CSharpierFormatter.Format(code);
        var newNode = SerializerSemanticTree.DeserializeAndGroupCode(code);
        node.ReplaceSelf(newNode);
    }
    public static List<TreeNode<NodeSemantic>> ReplaceExistingLowestLevelTypesFrom(this TreeNode<NodeSemantic> destination, TreeNode<NodeSemantic> source, bool replace = true)
    {
        var destByName = destination.GetLowestLevelTypes().ToDictionaryByFullName();
        var newLowLevelTypes = new List<TreeNode<NodeSemantic>>();
        var sourceTypes = source.GetLowestLevelTypes();
        foreach (var sourceNode in sourceTypes.ToDictionaryByFullName())
        {
            if (destByName.TryGetValue(sourceNode.Key, out var destNode))
            {
                if (replace)
                    destNode.ReplaceSelf(sourceNode.Value);
            }
            else
                newLowLevelTypes.Add(sourceNode.Value);
        }
        return newLowLevelTypes;
    }
    public static void SaveFile(this TreeNode<NodeSemantic> node, string filePath)
    {
        node.SerializeFile(filePath);
    }
    public static TreeNode<NodeSemantic> SelfMergeOrderAndCleanWhitespace(this TreeNode<NodeSemantic> tree)
    {
        tree = tree.DocMergeSelf();
        tree.GroupByKindModifierName();
        tree.DocCleanWhitespaceAll();
        return tree;
    }
    public static TreeNode<NodeSemantic> SemanticCopyDeep(this TreeNode<NodeSemantic> tree)
    {
        string code = tree.ToCode();
        var freshTree = SerializerSemanticTree.DeserializeAndGroupCode(code);
        return freshTree;
    }
    public static TreeNode<NodeSemantic> SemanticCopyNode(this TreeNode<NodeSemantic> node)
    {
        if (node.Value == null)
            return new TreeNode<NodeSemantic>(null);
        var freshValue = new SyntaxElementNode(node.Value.Kind, node.Value.Text);
        var sn = freshValue.CreateSemantic();
        return new TreeNode<NodeSemantic>(sn);
    }
    public static string Serialize(this TreeNode<NodeSemantic>? root)
    {
        if (root == null)
            return string.Empty;
        var sb = new StringBuilder(8192 * 8);
        void Visit(TreeNode<NodeSemantic> node)
        {
            if (node.Value?.Text != null)
                sb.Append(node.Value.Text);
            foreach (var child in node.Children)
                Visit(child);
        }
        Visit(root);
        return sb.ToString();
    }
    public static void SerializeFile(this TreeNode<NodeSemantic> node, string filePath)
    {
        node.ToFile(filePath);
    }
    public static List<TreeNode<NodeSemantic>> SkipKinds(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds)
    {
        return tree.FindWhere(t => !kinds.Contains(t.Value.Kind)).ToList();
    }
    public static TreeNode<NodeSemantic> SpliceSubset(this TreeNode<NodeSemantic> tree, List<string> typeSearch)
    {
        var NodeNameds = tree.GetTypedDictionary<NodeNamedMember>();
        var keysToMove = (from n in NodeNameds join t in typeSearch on 1 equals 1 where n.Key.Contains(t) select n.Key).ToList();
        var typesToMove = keysToMove.SelectMany(t => NodeNameds[t]).ToList();
        return typesToMove.MoveTypes();
    }
    public static bool SplitAfter(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> boundaryKinds)
    {
        var boundaryNode = tree.FindKinds(boundaryKinds).FirstOrDefault();
        if (boundaryNode == null)
            return false;
        var topLevel = tree.Children.Where(t => t.FindKinds(boundaryKinds).Any()).First();
        var en = tree.Children.ToList().GetEnumerator();
        var cuHalf1 = NodeCompilationUnit.Factory();
        while (en.MoveNext())
        {
            cuHalf1.AddChild(en.Current);
            if (en.Current == topLevel)
                break;
        }
        var cuHalf2 = NodeCompilationUnit.Factory();
        while (true)
        {
            cuHalf2.AddChild(en.Current);
            if (!en.MoveNext())
                break;
        }
        tree.Children.Clear();
        tree.AddChild(cuHalf1);
        tree.AddChild(cuHalf2);
        tree.InitializeAllNodes();
        return true;
    }
    public static bool SplitBefore(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> boundaryKinds)
    {
        var boundaryNode = tree.FindKinds(boundaryKinds).FirstOrDefault();
        if (boundaryNode == null)
            return false;
        var topLevel = tree.Children.Where(t => t.FindKinds(boundaryKinds).Any()).First();
        var en = tree.Children.ToList().GetEnumerator();
        var cuHalf1 = NodeCompilationUnit.Factory();
        while (en.MoveNext())
        {
            if (en.Current == topLevel)
                break;
            cuHalf1.AddChild(en.Current);
        }
        var cuHalf2 = NodeCompilationUnit.Factory();
        while (true)
        {
            cuHalf2.AddChild(en.Current);
            if (!en.MoveNext())
                break;
        }
        tree.Children.Clear();
        tree.AddChild(cuHalf1);
        tree.AddChild(cuHalf2);
        tree.InitializeAllNodes();
        return true;
    }
    public static void SplitByAttribute(this TreeNode<NodeSemantic> tree)
    {
        tree.SplitContains(SyntaxKindGroups.AttributeKinds);
    }
    public static void SplitContains(this TreeNode<NodeSemantic> tree, HashSet<SyntaxKind> kinds)
    {
        if (tree == null)
            return;
        var splitKinds = NodeCompilationUnit.Factory();
        var rest = NodeCompilationUnit.Factory();
        foreach (var x in tree.Children.ToList())
        {
            if (kinds.Contains(x.Value.Kind))
            {
                splitKinds.AddChild(x);
            }
            else
                rest.AddChild(x);
        }
        tree.AddChild(splitKinds);
        tree.AddChild(rest);
        tree.InitializeAllNodes();
    }
    public static void SplitTypeDeclarationIntoFile(this TreeNode<NodeSemantic> tree, string type)
    {
        var list = tree.FindByName<NodeTypeDeclaration>(type).ToList();
        var cu = NodeCompilationUnit.Factory();
        foreach (var l in list)
            cu.AddChild(l.tree);
        cu.ToFile(KPP.Evolver5.WithRootDir($"{type}.cs"));
    }
    public static bool TestCompile(this TreeNode<NodeSemantic> node, string projectPath, string savePath)
    {
        var code = node.ToCode();
        File.WriteAllText(savePath, code);
        var ret = CompilationUtil.CompileProject(projectPath);
        if (!ret.Success)
            throw new Exception("FAILURE");
        return ret.Success;
    }
    public static string ToCode(this TreeNode<NodeSemantic> node)
    {
        var code = node.Serialize();
        return code;
    }
    public static Dictionary<string, ModuleInfo> ToDependencyDictionary(this TreeNode<NodeSemantic> tree)
    {
        return tree.ToModuleDictionary();
    }
    public static void ToFile(this TreeNode<NodeSemantic> node, string filePath)
    {
        var code = node.ToCode();
        code = code.NormalizeLineEndings();
        File.WriteAllText(filePath, code, Encoding.UTF8);
    }
    public static Dictionary<string,List<string>> ToKindDictionary(this TreeNode<NodeSemantic> tree)
    {
        var ret=tree.Flatten().Where(t => t.Value.Text != null && t.Value.Text.Trim() != string.Empty).GroupBy(t => t.Value.ComputeKindFullName(tree)).ToDictionary(t => t.Key, t => t.Select(x=>x.Value.Text).ToList());
        return ret;
    }
    public static Dictionary<string, ModuleInfo> ToModuleDictionary(this TreeNode<NodeSemantic> tree)
    {
        var types = tree.GetTypeDeclarations();
        var moduleDictionary = new Dictionary<string, ModuleInfo>();
        foreach (var type in types)
        {
            var dependencies = tree.GetDependencies(type);
            var dependents = tree.GetDependents(type.TypeName).Select(t => t.TypeName).ToList();
            moduleDictionary[type.TypeName] = new ModuleInfo
            {
                Dependencies = dependencies,
                Dependents = dependents,
                Bases = type.BaseTypes.ToList(),
                TypeName = type.TypeName,
            };
        }
        return moduleDictionary;
    }
    public static Dictionary<string, string> ToTypeCodeDictionary(this TreeNode<NodeSemantic> tree)
    {
        var ret = new Dictionary<string, string>();
        foreach (var c in tree.GetTypeDeclarations())
        {
            ret.Add(c.TypeName, c.tree.ToCode());
        }
        return ret;
    }
    public static Dictionary<string, Dictionary<string, List<string>>> ToTypeMethodCodeDictionary(this TreeNode<NodeSemantic> tree)
    {
        var ret = new Dictionary<string, Dictionary<string, List<string>>>();
        foreach (var c in tree.GetTypeDeclarations())
        {
            var m = c.GetMethods()
                .GroupBy(t => t.TypeName)
                .ToDictionary(t => t.Key, t => t.Select(x => x.tree.ToCode()).ToList());
            ret.Add(c.TypeName, m);
        }
        return ret;
    }
    public static Dictionary<string, List<string>> ToTypeMethodDictionary(this TreeNode<NodeSemantic> tree)
    {
        var ret = new Dictionary<string, List<string>>();
        foreach (var c in tree.GetTypeDeclarations())
        {
            var m = c.GetMethods();
            ret.Add(c.TypeName, m.Select(t => t.TypeName).ToList());
        }
        return ret;
    }
    public static void WrapChildren(this TreeNode<NodeSemantic> node)
    {
        var cu = NodeCompilationUnit.Factory();
        foreach (var c in node.Children.ToList())
            cu.AddChild(c);
        node.AddChild(cu);
        node.InitializeAllNodes();
    }
    public static TreeNode<NodeSemantic> WrapInDeclaration(this TreeNode<NodeSemantic> method, string declaration)
    {
        return method.AsList().WrapInDeclaration(declaration);
    }
    public static void WrapInNewLines(this TreeNode<NodeSemantic> node)
    {
        var cu = NodeCompilationUnit.Factory();
        cu.AddNewLine();
        node.ReplaceSelf(cu);
        cu.AddChild(node);
        cu.AddNewLine();
    }
    public static void WriteTypeDeclarationCodeFile(this TreeNode<NodeSemantic> tree, string file)
    {
        var sw = new StreamWriter(file);
        var list = tree.GetTypedList<NodeTypeDeclaration>();
        foreach (var x in list)
        {
            sw.WriteLine(x.CopyDeclaration().ToCode());
        }
        sw.Close();
    }
    public static void WriteTypeDeclarationListFile(this TreeNode<NodeSemantic> tree, string file)
    {
        var sw = new StreamWriter(file);
        var list = tree.GetTypedList<NodeTypeDeclaration>();
        foreach (var x in list)
        {
            sw.WriteLine(x.TypeName);
        }
        sw.Close();
    }
}

