using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minesweeper
{
    public class MinesweeperGrid
    {
        public string GetStringRepresentation()
        {
            var str = new StringBuilder();
            foreach (var baseCells in Cells.GroupBy(p=>p.Row).OrderBy(p=>p.Key))
            {
                foreach (var baseCell in baseCells)
                {
                    str.Append(baseCell.DisplayValue());
                }
                str.AppendLine();
            }
            return str.ToString();
        }
        public BaseCell[] SquareCells(BaseCell cell)
        {
            var cells = new List<BaseCell>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var column = cell.Column + i;
                    var row = cell.Row + j;
                    if (row >= 0 && row < Rows && column >= 0 && column < Columns)
                    {
                        var neighbor = Cells.FirstOrDefault(p => p.Row == row && p.Column == column);

                        cells.Add(neighbor);
                    }
                }
            }
            return cells.ToArray();
        }

        public MinesweeperGrid()
        {
            
        }
        public MinesweeperGrid(int rows, int columns, float width)
        {
            Rows = rows;
            Columns = columns;
            Width = width;
            Setup();
        }
        public List<BaseCell> Cells { get; set; }

        public void SetDimensions(float width, float xOffset, float yOffset)
        {
            Width = width;
            foreach (var baseCell in Cells)
            {
                baseCell.Width = width;
                baseCell.XOffset = xOffset;
                baseCell.YOffset = yOffset;
            }
        }

        private void Setup()
        {
            Cells = new List<BaseCell>(Rows * Columns);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var cell = new BaseCell();

                    cell.Row = i;
                    cell.Column = j;
                    cell.Width = Width;
                    Cells.Add(cell);
                }
            }
        }

        public int Rows { get; }
        public int Columns { get; }
        public float Width { get; private set; }
    }
}