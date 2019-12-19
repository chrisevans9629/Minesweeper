﻿using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Minesweeper.Test
{
    [TestFixture]
    public class MineSweeperTests
    {
        private MinesweeperBase minesweeper;
        [SetUp]
        public void Setup()
        {
            minesweeper = new MinesweeperBase();

            minesweeper.Setup(new MinesweeperConfig());
        }

        [Test]
        public void ToggleFlag()
        {
            var item = minesweeper.Cells.First();
            minesweeper.ClickOnCell(item, true);
            item.Flag.Should().BeTrue();

            minesweeper.ClickOnCell(item, true);
            item.Flag.Should().BeFalse();
        }


        [Test]
        public void Start_GameEnd_False()
        {
            minesweeper.GameEnd.Should().BeFalse();
        }

        [Test]
        public void Start_PlayFirst_GameEnd_False()
        {
            minesweeper.ClickOnCell(minesweeper.Cells.First(), false);
            minesweeper.GameEnd.Should().BeFalse();
        }

        [Test]
        public void TapCell_FlagFalse_ShouldNotSetFlag()
        {
            PlaceFirst();

            var cell = minesweeper.Cells.First(p=>p.Visible != true);

            minesweeper.ClickOnCell(cell, false);

            cell.Flag.Should().BeFalse();
        }

        [Test]
        public void TapCell_FlagTrue_ShouldSetFlag()
        {
            PlaceFirst();

            var cell = minesweeper.Cells.First(p=>p.Visible != true);

            minesweeper.ClickOnCell(cell, true);

            cell.Flag.Should().BeTrue();
        }

        [Test]
        public void Bomb_Num()
        {
            PlaceFirst();
            minesweeper.Cells.Count(p => p.Bomb).Should().Be(20);
        }

        [Test]
        public void Setup_100Cells()
        {
            minesweeper.Setup(new MinesweeperConfig(){Columns = 10, Rows = 10});
            minesweeper.Cells.Count().Should().Be(100);
        }

        [Test]
        public void CellCount()
        {
            minesweeper.Cells.Count().Should().Be(225);
        }

        [Test]
        public void NoBombsUntilFirstClick()
        {
            minesweeper.Cells.Any(p => p.Bomb).Should().BeFalse();
        }

        [Test]
        public void Click_CreatesBombs()
        {
            var first = minesweeper.Cells.First();

            minesweeper.ClickOnCell(first, false);

            minesweeper.Cells.Any(p => p.Bomb).Should().BeTrue();
            first.Bomb.Should().BeFalse();
        }

        [Test]
        public void PlaceFlag_Flag_True()
        {
            var first = minesweeper.Cells.First();
            minesweeper.ClickOnCell(first, true);
            first.Flag.Should().BeTrue();
        }

        [Test]
        public void ClickOnCell_Bomb_GameEnd()
        {
            var t = minesweeper.Cells.First();
            minesweeper.ClickOnCell(t, false);
            var first = minesweeper.Cells.First(p => p.Bomb);
            minesweeper.ClickOnCell(first, false);
            minesweeper.GameEnd.Should().BeTrue();
            minesweeper.Lost.Should().BeTrue();
        }

        [Test]
        public void ClickOnCell_Bomb_Flag()
        {
            PlaceFlag();
            minesweeper.GameEnd.Should().BeFalse();
            minesweeper.Lost.Should().BeFalse();
        }

        private void PlaceFlag()
        {
            PlaceFirst();

            var first = minesweeper.Cells.First(p => p.Bomb);
            minesweeper.ClickOnCell(first, true);
        }

        private void PlaceFirst()
        {
            var t = minesweeper.Cells.First();
            minesweeper.ClickOnCell(t, false);
        }

        [Test]
        public void FlagAllBombs_Win()
        {
            minesweeper.ClickOnCell(minesweeper.Cells.FirstOrDefault(), false);
            var bombs = minesweeper.Cells.Where(p => p.Bomb);
            foreach (var baseCell in bombs)
            {
                minesweeper.ClickOnCell(baseCell, true);
            }


            minesweeper.Cells.Where(p => p.Bomb).All(p => p.Flag).Should().BeTrue();

            minesweeper.GameEnd.Should().BeTrue();
            minesweeper.Win.Should().BeTrue();
        }

        [Test]
        public void HalfOfEverythingNotBomb_GameEnd_False()
        {
            var bombs = minesweeper.Cells.Where(p => p.Bomb != true).Take(minesweeper.Cells.Count()/2);
            foreach (var baseCell in bombs)
            {
                minesweeper.ClickOnCell(baseCell, false);
            }

            minesweeper.GameEnd.Should().BeFalse();
            minesweeper.Win.Should().BeFalse();
        }

        [Test]
        public void MaxScore_Equals_CellCount()
        {
          
            minesweeper.MaxScore.Should().Be(minesweeper.Cells.Count());
        }

        [Test]
        public void Score_0()
        {
            minesweeper.Score.Should().Be(0);
        }

        [Test]
        public void Reset_No_Bombs()
        {
            PlaceFirst();
            minesweeper.Reset();

            minesweeper.Cells.Any(p => p.Bomb).Should().BeFalse();

        }

        [Test]
        public void PlaceFlag_Score1()
        {
            PlaceFlag();
            minesweeper.Score.Should().Be(minesweeper.Cells.Count(p=>p.Flag || p.Visible));
        }

        [Test]
        public void Click_Score1()
        {
            var notBomb = minesweeper.Cells.First(p => p.Bomb != true);
            minesweeper.ClickOnCell(notBomb, false);
            minesweeper.Score.Should().Be(minesweeper.Cells.Count(p=>p.Visible));

        }


        [Test]
        public void All_Not_Visible()
        {
            minesweeper.Cells.Any(p => p.Visible).Should().BeFalse();
        }

        [Test]
        public void Reset_All_NotVisible()
        {
            minesweeper.Reset();
            minesweeper.Cells.Any(p => p.Visible).Should().BeFalse();
        }

        [Test]
        public void ClickOnEverythingThatsNotABomb_Win()
        {
            var bombs = minesweeper.Cells.Where(p => p.Bomb != true);
            foreach (var baseCell in bombs)
            {
                minesweeper.ClickOnCell(baseCell, false);
            }

            minesweeper.GameEnd.Should().BeTrue();
            minesweeper.Win.Should().BeTrue();
        }

        [Test]
        public void Model_Is_Serializable()
        {
            var t = minesweeper.Cells.FirstOrDefault(p => p.Bomb != true);

            minesweeper.ClickOnCell(t, false);


            var gamestr = JsonConvert.SerializeObject(minesweeper);


            var newGame = JsonConvert.DeserializeObject<MinesweeperBase>(gamestr);


            newGame.Height.Should().Be(minesweeper.Height);
            newGame.Width.Should().Be(minesweeper.Width);

            newGame.Columns.Should().Be(minesweeper.Columns);
            newGame.Rows.Should().Be(minesweeper.Rows);
            newGame.Score.Should().Be(minesweeper.Score);
            newGame.MaxScore.Should().Be(minesweeper.MaxScore);
            newGame.Cells.Count().Should().Be(minesweeper.Cells.Count());



            

            newGame.Cells.Count(p => p.Visible).Should().BeGreaterOrEqualTo(1);
        }


        [Test]
        public void SimpleSerialization()
        {
            minesweeper.Setup(new MinesweeperConfig(){Rows = 2, Columns = 2, BombCount = 2});

            var t = minesweeper.Cells.FirstOrDefault(p => p.Bomb != true);

            minesweeper.ClickOnCell(t, false);


            var gamestr = JsonConvert.SerializeObject(minesweeper);


            var newGame = JsonConvert.DeserializeObject<MinesweeperBase>(gamestr);


            newGame.Height.Should().Be(minesweeper.Height);
            newGame.Width.Should().Be(minesweeper.Width);

            newGame.Columns.Should().Be(minesweeper.Columns);
            newGame.Rows.Should().Be(minesweeper.Rows);
            newGame.Score.Should().Be(minesweeper.Score);
            newGame.MaxScore.Should().Be(minesweeper.MaxScore);
            newGame.Cells.Count().Should().Be(minesweeper.Cells.Count());
        }
    }
}