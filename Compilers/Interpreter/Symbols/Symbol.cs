namespace Minesweeper.Test.Symbols
{
    public class Symbol
    {
        public string Name { get; protected set; }
        public Symbol Type { get; protected set; }

        public Symbol(string name, Symbol type = null)
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
        public bool Initialized { get; set; }
        public VariableSymbol(string name, Symbol type) : base(name, type)
        {
        }

        public override string ToString()
        {
            return $"<{Name}:{Type}>";
        }
    }
}