using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public abstract class Node
    {

        public Node()
        {

        }

        public static string Aggregate(IEnumerable<Node> nodes)
        {
            return (nodes.Any() ? nodes.Select(p => p.Display()).Aggregate((f, s) => $"{f}, {s}") : "");
        }

        //public string Name => TokenItem.Token.Name;
        //public TokenItem TokenItem { get; set; }

        public abstract string Display();
    }
}