namespace Minesweeper.Test
{
    public class PascalProgram : Node
    {
        public string ProgramName { get; set; }
        public Block Block { get; set; }
        public TokenItem TokenItem { get; set; }
        public PascalProgram(TokenItem name, Block block)
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