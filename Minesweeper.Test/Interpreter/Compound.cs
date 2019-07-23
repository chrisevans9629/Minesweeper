using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class Compound : Node
    {
        public Compound()
        {
            Nodes = new List<Node>();
        }
        public IList<Node> Nodes { get; set; }
        public override string Display()
        {
            return $"Compound({Aggregate(Nodes)}";
        }
    }
}