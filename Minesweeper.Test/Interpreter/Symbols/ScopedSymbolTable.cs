using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test.Tests;

namespace Minesweeper.Test.Symbols
{
    public class ScopedSymbolTable
    {
        private readonly ILogger _logger;
        public string ScopeName { get; }
        public int ScopeLevel { get; }
        IList<Symbol> symbols = new List<Symbol>();

        public ScopedSymbolTable(string scopeName, int scopeLevel, ILogger logger = null)
        {
            _logger = logger ?? new Logger();
            ScopeName = scopeName;
            ScopeLevel = scopeLevel;
            Define(new BuiltInTypeSymbol(Pascal.Int));
            Define(new BuiltInTypeSymbol(Pascal.Real));
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

        public Symbol LookupSymbol(string name)
        {
            _logger.Log($"Lookup: {name}");
            return symbols.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        }
    }
}