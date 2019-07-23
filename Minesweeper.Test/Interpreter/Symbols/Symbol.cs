namespace Minesweeper.Test.Symbols
{
    public class Symbol
    {
        public string Name { get; protected set; }
        public string Type { get; protected set; }

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
        public Symbol TypeSymbol { get; }

        public VariableSymbol(string name, Symbol type) : base(name)
        {
            TypeSymbol = type;
            Type = type.Name;
        }

        public override string ToString()
        {
            return $"<{Name}:{Type}>";
        }
    }
}