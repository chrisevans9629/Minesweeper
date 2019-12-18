using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper
{
    public class NoShowCell : BaseCell
    {
        public NoShowCell()
        {

        }
        public NoShowCell(int row, int column, int width)
        {
            X = row * width;
            Y = column * width;
            Row = row;
            Column = column;
            Width = width;
            Visible = false;

        }
    }
    public class MinesweeperFitnessTest : IFitnessVal
    {
        public bool Maximize
        {
            get { return true; }
        }

        public double EvaluateFitness(INeuralNetwork network)
        {
            var mine = new MinesweeperBase();
            mine.Setup(new MinesweeperConfig() { BombCount = 50 });
            int clicks = 0;
            int score = 0;
            while (mine.GameEnd != true && clicks < mine.MaxScore)
            {
                var result = network.FeedForward(mine.Grid.Cells.Select(p => p.Value).ToArray().ToDoubleArray());
                double x = 0, y = 0;
                x = result[0] * mine.Width;
                y = result[1] * mine.Width;
                var cell = mine.Grid.Cells.FirstOrDefault(p => p.Hit((int)x, (int)y));
                if (cell != null)
                {
                    var c = mine.ClickOnCell(cell, false);
                    if (c != true)
                    {
                        score--;
                    }
                }
                clicks++;

            }

            return mine.Score + score;

        }
    }

    public class MinesweeperConfig
    {
        //public Func<BaseCell> CreateCellFunc { get; }
        //public MinesweeperConfig(Func<BaseCell> createCell)
        //{
        //    CreateCellFunc = createCell;
        //}
        public int? Rows { get; set; }
        public int? Columns { get; set; }
        public float CellWidth { get; set; } = 40;
        public float Width { get; set; } = 600;
        public float Height { get; set; } = 600;
        public int Seed { get; set; } = 100;
        public int? BlockCount { get; set; }
        public int BombCount { get; set; } = 20;

    }
    public struct CellParams
    {
        public int Row { get; }
        public int Column { get; }
        public float Width { get; }

        public CellParams(int row, int column, float width)
        {
            Row = row;
            Column = column;
            Width = width;
        }
    }

    public interface IMinesweeperBase
    {

        bool ClickOnCell(BaseCell item, bool placeAsFlag);
    }
    public class MinesweeperBase : IMinesweeperBase
    {
        public MinesweeperBase()
        {
            
        }
        public MinesweeperConfig Config { get; set; }

        public MinesweeperGrid Grid { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }


        [JsonIgnore]
        public IEnumerable<BaseCell> Cells => Grid?.Cells;
        [JsonIgnore]
        public virtual int MaxScore => Grid.Cells.Count;
        [JsonIgnore]
        public virtual int Score => Grid.Cells.Count(p => p.Visible || p.Flag);


        [JsonIgnore]
        public bool GameEnd => (Win || Lost) && Cells.Count(p=>p.Bomb) > 0;

        public bool Win => Grid.Cells.Where(p=>p.Bomb).All(p => p.Flag) || Grid.Cells.Where(p => !p.Bomb).All(p => p.Visible);
        public bool Lost => Grid.Cells.Any(p => p.Visible && p.Bomb);

        public bool ClickOnCell(BaseCell item, bool placeAsFlag)
        {
            if (Grid.Cells.Any(p => p.Bomb) != true)
            {
                SetupBombs(Config.BombCount, Config.Seed, item);
            }
            if (item.Visible == true)
            {
                return false;
            }
            if (placeAsFlag != true)
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
            else
            {
                item.Flag = true;
            }
            if (Win)
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
            Setup(Config);
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

        public void Setup(MinesweeperConfig config)
        {
            Config = config;
            var cellwidth = config.CellWidth;
            Width = config.Width;
            Height = config.Height;
            if (config.Columns != null)
            {
                Columns = config.Columns ?? 0;
            }
            else
                Columns = (int)(Width / cellwidth);

            if (config.Rows != null) Rows = config.Rows ?? 0;
            else
                Rows = (int)(Height / cellwidth);
            if (config.BlockCount != null)
            {
                throw new NotImplementedException();
            }
            Grid = new MinesweeperGrid(Rows, Columns, cellwidth);

        }

        private void SetupBombs(int numOfBombs, int seed, BaseCell firstCell)
        {
            //var numOfBombs = 20;
            var random = new Random(seed);

            //create bombs
            for (int i = 0; i < numOfBombs; i++)
            {
                var cells = Grid.Cells.Where(p => p.Bomb != true && p != firstCell).ToList();
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