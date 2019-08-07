namespace Minesweeper.Test
{
    public class ParameterNode : Node
    {
        public VarDeclarationNode Declaration { get; }

        public ParameterNode(VarDeclarationNode declaration)
        {
            Declaration = declaration;
        }
        public override string Display()
        {
            return $"Param({Declaration.Display()})";
        }
    }
}