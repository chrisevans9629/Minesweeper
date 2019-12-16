using System.Linq;
using FluentAssertions;
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

            minesweeper.Setup(new MinesweeperConfig(NoShow));
        }

        BaseCell NoShow(CellParams p)
        {
            return new NoShowCell(p.Row, p.Column, (int) p.Width);
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
            minesweeper.Setup(new MinesweeperConfig(NoShow){Columns = 10, Rows = 10});
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
            minesweeper.GameEnd().Should().BeTrue();
            minesweeper.Lost().Should().BeTrue();
        }

        [Test]
        public void ClickOnCell_Bomb_Flag()
        {
            PlaceFlag();
            minesweeper.GameEnd().Should().BeFalse();
            minesweeper.Lost().Should().BeFalse();
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
            var bombs = minesweeper.Cells.Where(p => p.Bomb);
            foreach (var baseCell in bombs)
            {
                minesweeper.ClickOnCell(baseCell, true);
            }

            minesweeper.GameEnd().Should().BeTrue();
            minesweeper.Win().Should().BeTrue();
        }

        [Test]
        public void HalfOfEverythingNotBomb_GameEnd_False()
        {
            var bombs = minesweeper.Cells.Where(p => p.Bomb != true).Take(minesweeper.Cells.Count()/2);
            foreach (var baseCell in bombs)
            {
                minesweeper.ClickOnCell(baseCell, false);
            }

            minesweeper.GameEnd().Should().BeFalse();
            minesweeper.Win().Should().BeFalse();
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

            minesweeper.GameEnd().Should().BeTrue();
            minesweeper.Win().Should().BeTrue();
        }
    }
}