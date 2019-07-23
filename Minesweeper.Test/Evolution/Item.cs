using System;

namespace Minesweeper.Test
{
    public class Item : ICloneable
    {
        public string Id { get; set; }
        public int Size { get; set; }
        public int Value { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}