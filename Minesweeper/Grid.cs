﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper
{
    public class Grid
    {
        private readonly Func<BaseCell> _createCellFunc;

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
        public Grid(int rows, int columns, float width, Func<BaseCell> createCellFunc)
        {
            _createCellFunc = createCellFunc;
            Rows = rows;
            Columns = columns;
            Width = width;
            Setup();
        }
        public List<BaseCell> Cells { get; set; }
        private void Setup()
        {
            Cells = new List<BaseCell>(Rows * Columns);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var cell = _createCellFunc();

                    cell.Row = i;
                    cell.Column = j;
                    cell.Width = Width;

                    Cells.Add(cell);
                }
            }
        }

        public int Rows { get; }
        public int Columns { get; }
        public float Width { get; }
    }
}