namespace Minesweeper.Test
{
    public class VarDeclarationNode : Node
    {
        public Variable VarNode { get; set; }
        public TypeNode TypeNode { get; set; }

        public VarDeclarationNode(Variable varNode, TypeNode typeNode)
        {
            VarNode = varNode;
            TypeNode = typeNode;
        }

        public override string Display()
        {
            return $"VarDec({VarNode.Display()}, {TypeNode.Display()})";
        }
    }
}