using System.Collections.Generic;

namespace Minesweeper.Test.Symbols
{
    public class ProcedureSymbol : Symbol
    {
        public IList<ProcedureParameter> Parameters { get; }

        public ProcedureSymbol(string name, IList<ProcedureParameter> parameters) : base(name)
        {
            Parameters = parameters;
        }

        public override string ToString()
        {
            return $"<{GetType()}(name={Name}, parameters={Parameters})>";
        }
    }
}