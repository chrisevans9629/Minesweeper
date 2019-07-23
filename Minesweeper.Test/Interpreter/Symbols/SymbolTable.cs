using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test.Symbols
{
    public class SymbolTable
    {
        IList<Symbol> symbols = new List<Symbol>();

        public SymbolTable()
        {
            Define(new BuiltInTypeSymbol(Pascal.Int));
            Define(new BuiltInTypeSymbol(Pascal.Real));
        }
        public void Define(Symbol symbol)
        {
            Console.WriteLine($"Define: {symbol}");
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
            Console.WriteLine($"Lookup: {name}");
            return symbols.FirstOrDefault(p => p.Name == name);
        }
    }
}