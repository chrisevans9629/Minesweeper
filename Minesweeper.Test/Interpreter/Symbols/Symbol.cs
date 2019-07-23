namespace Minesweeper.Test.Symbols
{
    public class Symbol
    {
        public string Name { get; }
        public string Type { get; }

        public Symbol(string name, string type = null)
        {
            Name = name;
            Type = type;
        }
    }

    public class BuiltInTypeSymbol : Symbol
    {
        public BuiltInTypeSymbol(string name) : base(name)
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class VariableSymbol : Symbol
    {
        public VariableSymbol(string name, string type = null) : base(name, type)
        {
        }

        public override string ToString()
        {
            return $"<{Name}:{Type}>";
        }
    }
}