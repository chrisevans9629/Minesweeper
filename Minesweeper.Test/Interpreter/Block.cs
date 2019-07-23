using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class Block : Node
    {
        public IList<VarDeclaration> Declarations { get; }
        public Node CompoundStatement { get; }

        public Block(IList<VarDeclaration> declarations, Node compoundStatement)
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