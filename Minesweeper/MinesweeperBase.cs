using System;
using System.Linq;

namespace Minesweeper
{
    public struct CellParams
    {
        public int Row { get; }
        public int Column { get; }
        public int Width { get; }

        public CellParams(int row, int column, int width)
        {
            Row = row;
            Column = column;
            Width = width;
        }
    }
    public class MinesweeperBase
    {
        internal Grid Grid;
        internal int Columns;
        internal int Rows;
        internal int Width;
        internal int Height;

        public int MaxScore
        {
            get { return Grid.Cells.Count(p => p.Bomb != true); }
        }

        public int Score
        {
            get { return Grid.Cells.Count(p => p.Visible && p.Bomb != true); }
        }

        public bool GameEnd()
        {
            return Lost() || Win();
        }
        public bool Lost()
        {
            return Grid.Cells.Any(p => p.Visible && p.Bomb);
        }
        public bool ClickOnCell(BaseCell item, bool flag)
        {
            if (item.Visible == true)
            {
                return false;
            }
            if (flag != true)
            {
                item.Visible = true;
                if (item.Bomb)
                {
                    OnHasLost();
                    OnGameEnded(false);
                }
                
                if (item.Value == 0)
                {
                    ShowOthers(item);
                    foreach (var gridCell in Grid.Cells)
                    {
                        if (Grid.SquareCells(gridCell).Any(p => p.Value == 0 && p.Bomb != true && p.Visible))
                        {
                            gridCell.Visible = true;
                        }
                    }
                }
            }
            {
                item.Flag = true;
            }
            if (Win())
            {
                OnHasWon();
                OnGameEnded(true);
            }
            return true;
        }
        private void CalculateBombs(BaseCell cell)
        {
            int numOfBombs = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var column = cell.Column + i;
                    var row = cell.Row + j;
                    if (row >= 0 && row <= Rows && column >= 0 && column <= Columns)
                    {
                        if (Grid.Cells.FirstOrDefault(p => p.Row == row && p.Column == column)?.Bomb == true)
                        {
                            numOfBombs++;
                        }
                    }

                }
            }
            cell.Value = numOfBombs;
        }

        public void Reset()
        {
            Setup(_cellFunc);
            Show();
        }
        private void ShowOthers(BaseCell cell)
        {

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var column = cell.Column + i;
                    var row = cell.Row + j;
                    if (row >= 0 && row < Grid.Rows && column >= 0 && column < Grid.Columns)
                    {
                        var neighbor = Grid.Cells.FirstOrDefault(p => p.Row == row && p.Column == column);
                        if (neighbor?.Value == 0 && neighbor.Visible != true && neighbor.Bomb != true)
                        {
                            neighbor.Visible = true;
                            ShowOthers(neighbor);
                        }
                    }
                }
            }
            //cell.Show();
        }

        public event EventHandler HasLost;
        public event EventHandler HasWon;
        public event EventHandler<bool> GameEnded;
        internal void Show()
        {
            foreach (var item in Grid.Cells)
            {
                item.Show();
            }
            
        }
        public bool Win()
        {
            return Grid.Cells.Any(p => p.Bomb && p.Flag != true) != true;
        }

        private Func<CellParams, BaseCell> _cellFunc;
        public void Setup(Func<CellParams, BaseCell> createCellFunc, int cellwidth = 40, int width = 600, int height = 600, int numOfBombs = 20, int seed = 100 )
        {
            // var cellwidth = 40;
            _cellFunc = createCellFunc;
            Width = width;
            Height = height;
            Columns = Width / cellwidth;
            Rows = Height / cellwidth;

            Grid = new Grid(Rows, Columns, cellwidth,createCellFunc);

            //var numOfBombs = 20;
            var random = new Random(seed);

            //create bombs
            for (int i = 0; i < numOfBombs; i++)
            {
                var cells = Grid.Cells.Where(p => p.Bomb != true).ToList();
                cells[random.Next(0, cells.Count - 1)].Bomb = true;
            }

            //set values
            foreach (var item in Grid.Cells)
            {
                if (item.Bomb != true)
                {
                    CalculateBombs(item);

                }
            }

        }

        protected virtual void OnHasWon()
        {
            HasWon?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnHasLost()
        {
            HasLost?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnGameEnded(bool e)
        {
            GameEnded?.Invoke(this, e);
        }
    }
}