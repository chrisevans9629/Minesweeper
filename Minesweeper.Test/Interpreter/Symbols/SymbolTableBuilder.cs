namespace Minesweeper.Test.Symbols
{
    public class SymbolTableBuilder
    {
        SymbolTable table = new SymbolTable();

        public SymbolTable CreateTable(Node rootNode)
        {
            VisitNode(rootNode);
            return table;
        }

        private void VisitNode(Node rootNode)
        {
            throw new System.NotImplementedException();
        }
    }
}