﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper
{
    public class NoShowCell : BaseCell
    {
        public override void Show()
        {
        }

        public override void Highlight()
        {
        }

        public override void UnHighLight()
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
            mine.Setup(p => new NoShowCell(p.Row, p.Column,(int) p.Width), numOfBombs: 50);
            int clicks = 0;
            int score = 0;
            while (mine.GameEnd() != true && clicks < mine.MaxScore)
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
    public class MinesweeperBase
    {
        internal Grid Grid;
        internal int Columns;
        internal int Rows;
        internal float Width;
        internal float Height;
        public IEnumerable<BaseCell> Cells => Grid?.Cells;

        public virtual int MaxScore => Grid.Cells.Count;

        public virtual int Score => Grid.Cells.Count(p => p.Visible || p.Flag);

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
            if (Grid.Cells.Any(p => p.Bomb) != true)
            {
                SetupBombs(_numOfBombs, _seed, item);
            }
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
            return Grid.Cells.Any(p => p.Bomb && p.Flag != true) != true || Grid.Cells.Where(p=>p.Bomb != true).All(p=>p.Visible);
        }

        private int _numOfBombs;
        private int _seed;
        private Func<CellParams, BaseCell> _cellFunc;
        public void Setup(Func<CellParams, BaseCell> createCellFunc, float cellwidth = 40, 
            float width = 600, float height = 600, int numOfBombs = 20, int seed = 100 )
        {
            // var cellwidth = 40;
            _cellFunc = createCellFunc;
            Width = width;
            Height = height;
            Columns = (int)(Width / cellwidth);
            Rows = (int)(Height / cellwidth);
            _numOfBombs = numOfBombs;
            _seed = seed;
            Grid = new Grid(Rows, Columns, cellwidth,createCellFunc);

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