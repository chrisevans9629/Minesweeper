﻿namespace Minesweeper.Test
{
    public class TokenItem
    {
        public Token Token { get; set; }
        public int Position { get; set; }
        public int Line { get; set; }
        public string Value { get; set; }
        public override string ToString()
        {
            return $"{Token.Name}: {Value}";
        }
    }
}