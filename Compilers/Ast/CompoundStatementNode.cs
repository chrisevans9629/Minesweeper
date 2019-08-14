﻿using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class CompoundStatementNode : Node, IStatementNode
    {
        public CompoundStatementNode()
        {
            Nodes = new List<Node>();
        }
        public IList<Node> Nodes { get; set; }
        public override IEnumerable<Node> Children => Nodes;

        public override string Display()
        {
            return $"Compound({Aggregate(Nodes)}";
        }
    }
}