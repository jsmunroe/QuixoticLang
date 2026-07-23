using Quixotic.Common.Contracts;
using Quixotic.Common.Exceptions.Interop;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Namespaces;
using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Environment
{
    public class TypeRegistry : ITypeRegistry
    {
        private readonly AssemblyCatalog _assemblyCatalog = AssemblyCatalog.For(new InstalledLibraryLocator(), new DevelopmentLibraryLocator());

        private readonly Dictionary<TypeName, TypeSymbol> _types = []; // case-insensitivity is handles by TypeName class

        private readonly ImportSet _importSet;

        public TypeRegistry()
        {
            _importSet = new(this);
        }

        public List<TypeSymbol> AllTypes => [.. _types.Values];

        public void Add(TypeRegistry other)
        {
            foreach (var type in other.AllTypes)
                _types[type.Name] = new(type);
        }

        public TypeRegistry Capture(ClosureCapture closureCapture)
        {
            var registry = new TypeRegistry();

            foreach (var type in AllTypes.Where(closureCapture.IsCaptured))
                registry.Register(type);

            return registry;
        }

        public void Register(string name, QxType type)
        {
            _types[name] = new TypeSymbol(name, type);
        }

        private void Register(TypeSymbol symbol)
        {
            _types[symbol.Name] = symbol;
        }

        public bool Contains(string name)
        {
            return _types.ContainsKey(name);
        }

        public QxType? Resolve(string name)
        {
            return TryResolve(name, out var type) ? type : null;
        }

        public bool TryResolve(string name, [NotNullWhen(returnValue: true)] out QxType? type)
        {
            type = null;

            // TODO: This may be the wrong place for this code. Consider moving it elsewhere.
            if (name.StartsWith("deferred:", CaseRule.Current.StringComparison) && Enum.TryParse<ContextDependency>(name.Substring(9), out var contextDependency))
            {
                type = new DeferredType($"Type is stated as deferred type in the AST and is awaiting: {contextDependency}", contextDependency);
                return true;
            }

            // TODO: This may be the wrong place for this code. Consider moving it elsewhere.
            if (name.StartsWith("function", CaseRule.Current.StringComparison))
            {
                if (!FunctionDefinition.Default.TryParseTypeName(name, this, out var functionType))
                    throw new RuntimeException($"Cannot parse function type name '{name}'.");

                type = functionType;
                return true;
            }

            if (_types.TryGetValue(name, out var typeSymbol))
            {
                type = typeSymbol.Type;

                if (!type.HasGenerics)
                    return true;
            }

            var match = _types.FirstOrDefault(kvp => kvp.Key.IsMatch(name));

            if (match.Key is not null)
            {
                var typeName = match.Key;

                var bindings = typeName.GetGenericBindings(name, this);
                type = match.Value.Type.Substitute(bindings);

                return true;
            }

            return false;
        }

        public bool TryImport(Namespace ns)
        {
            if (!_assemblyCatalog.TryGetLocation(ns, out var location))
                return false;

            return _importSet.TryLoad(location);
        }

        public void Import(Namespace ns)
        {
            if (!_assemblyCatalog.TryGetLocation(ns, out var location))
                throw new AssemblyNotFoundException(ns);

            _importSet.Load(location);
        }
    }
}
