using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public abstract class Node
    {

        public Node()
        {

        }

        public override string ToString()
        {
            return Display();
        }

        public static string Aggregate(IEnumerable<Node> nodes)
        {
            if (nodes == null) return "";
            return (nodes.Any() ? nodes.Select(p => p.Display()).Aggregate((f, s) => $"{f}, {s}") : "");
        }

        //public string Name => TokenItem.Token.Name;
        //public TokenItem TokenItem { get; set; }

        public abstract string Display();
    }
}