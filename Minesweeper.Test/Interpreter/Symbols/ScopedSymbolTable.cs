using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test.Tests;

namespace Minesweeper.Test.Symbols
{
    public class ScopedSymbolTable
    {
        public ScopedSymbolTable ParentScope { get; }
        private readonly ILogger _logger;
        public string ScopeName { get; }
        public int ScopeLevel { get; }
        IList<Symbol> symbols = new List<Symbol>();

        public ScopedSymbolTable(string scopeName, int scopeLevel, ScopedSymbolTable parentScope = null, ILogger logger = null)
        {
            ParentScope = parentScope;
            _logger = logger ?? new Logger();
            ScopeName = scopeName;
            ScopeLevel = scopeLevel;
            Define(new BuiltInTypeSymbol(Pascal.Int));
            Define(new BuiltInTypeSymbol(Pascal.Real));
            if (parentScope?.ScopeLevel >= scopeLevel)
            {
                throw new InvalidOperationException($"Parent scope must be greater:\nParent: {parentScope}, Current: {this}");
            }
        }

        public int Count => symbols.Count;

        public void Define(Symbol symbol)
        {
            _logger.Log($"Define: {symbol}");
            symbols.Add(symbol);
        }

        public override string ToString()
        {
            var str = "Symbols: ";
            foreach (var symbol in symbols)
            {
                str += symbol.Name + ", ";
            }

            return str;
        }

        public Symbol LookupSymbol(string name, bool searchParent)
        {
            _logger.Log($"Lookup: {name}");
            var current = symbols.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            if (current == null && ParentScope != null && searchParent)
            {
                return ParentScope.LookupSymbol(name, true);
            }
            return current;
        }
    }
}