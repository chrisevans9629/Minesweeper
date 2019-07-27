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
            var str = "";
            foreach (var procedureParameter in Parameters)
            {
                str += procedureParameter.Display() + ", ";
            }
            return $"<{GetType()}(name={Name}, parameters={str})>";
        }
    }
}