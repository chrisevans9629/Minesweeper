using System.Collections.Generic;

namespace Minesweeper.Test.Symbols
{

    public class DeclarationSymbol : Symbol
    {
        public IList<Parameter> Parameters { get; }

        public DeclarationSymbol(string name, IList<Parameter> parameters) : base(name)
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

    public class FunctionDeclarationSymbol : DeclarationSymbol
    {
        public FunctionDeclarationSymbol(string name, IList<Parameter> parameters) : base(name, parameters)
        {
        }
    }

    public class ProcedureDeclarationSymbol : DeclarationSymbol
    {
        public ProcedureDeclarationSymbol(string name, IList<Parameter> parameters) : base(name, parameters)
        {
        }
    }
}