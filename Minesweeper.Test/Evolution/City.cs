using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class City
    {
        public List<Road> Roads { get; set; } = new List<Road>();
        public int Id { get; set; }
    }
}