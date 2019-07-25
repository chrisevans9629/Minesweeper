namespace Minesweeper.Test
{
    public class PascalProgramNode : Node
    {
        public string ProgramName { get; set; }
        public Block Block { get; set; }
        public TokenItem TokenItem { get; set; }
        public PascalProgramNode(TokenItem name, Block block)
        {
            ProgramName = name.Value;
            Block = block;
            TokenItem = name;
        }
        public override string Display()
        {
            return $"Program({ProgramName}, {Block.Display()})";
        }
    }
}