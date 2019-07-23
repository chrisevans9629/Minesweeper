using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class Block : Node
    {
        public IList<VarDeclaration> Declarations { get; }
        public CompoundStatement CompoundStatement { get; }

        public Block(IList<VarDeclaration> declarations, CompoundStatement compoundStatement)
        {
            Declarations = declarations;
            CompoundStatement = compoundStatement;
        }
        public override string Display()
        {
            return $"Block({Aggregate(Declarations)}, {CompoundStatement.Display()})";
        }
    }
}