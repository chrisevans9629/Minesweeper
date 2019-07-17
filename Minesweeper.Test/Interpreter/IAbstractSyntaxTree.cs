using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    
    public interface IAbstractSyntaxTree
    {
        void AddExpress(string expression, Func<IList<TokenItem>, double, double> eval);
        double Evaluate(IList<TokenItem> tokens);
    }
}