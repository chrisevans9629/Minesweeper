namespace Minesweeper.Test
{
    public class Variable : Node
    {
        public string VariableName { get; set; }
        public Variable(TokenItem token)
        {
            TokenItem = token;
            VariableName = token.Value;
        }

        public override string Display()
        {
            return $"Variable({VariableName})";
        }
    }
}