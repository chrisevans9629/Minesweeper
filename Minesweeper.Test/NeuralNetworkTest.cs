using System;
using System.Diagnostics;
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

            minesweeper.Setup(p=> new NoShowCell(p.Row,p.Column,p.Width));
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
            var first = minesweeper.Cells.First(p => p.Bomb);
            minesweeper.ClickOnCell(first, true);
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
        public void PlaceFlag_Score1()
        {
            PlaceFlag();
            minesweeper.Score.Should().Be(1);
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


    [TestFixture]
    public class NeuralNetworkTest
    {



        //[Test]
        //public void ActualMinSweepTest()
        //{
        //    var random = new Random(100);
        //    var evol = new NeuroEvolution(new MinesweeperFitnessTest(), 1000, () => new NeuralNetwork(random, 15*15, new[] { 225,450,225 }, 2, 0.50), random);
        //    evol.GenerationFinished += (sender, generation) =>
        //    {
        //        Debug.WriteLine(generation.Best.Error);
        //    };

        //    var first = evol.EvolveGeneration();
        //    var final = evol.Evolve(100);
        //    Assert.IsTrue(first.Best.Error < final.Error);

        //}
        //[Test]
        //public void OptimizationTest()
        //{
        //    var random = new Random(100);
        //    var evol = new NeuroEvolution(new BaseFitnessTest(), 1000, () => new NeuralNetwork(random, 1, new[] { 2 }, 1, 0.30), random);
        //    var first = evol.EvolveGeneration();
        //  //  IGeneration final = null;
        //    var final = evol.Evolve(1);
        //    Assert.IsTrue(first.Best.Error < final.Error);
        //}
        //[Test]
        //public void SpeciesSizeTest()
        //{
        //    var random = new Random(100);
        //    var evol = new NeuroEvolution(new BaseFitnessTest(), 100, () => new NeuralNetwork(random,1, new []{2},1,0), random );
        //    Assert.AreEqual(100,evol.NeuralNetworks.Count());
        //    evol.EvolveGeneration();
        //    Assert.AreEqual(100, evol.NeuralNetworks.Count());
        //    evol.EvolveGeneration();
        //    Assert.AreEqual(100, evol.NeuralNetworks.Count());
        //    evol.EvolveGeneration();
        //    Assert.AreEqual(100, evol.NeuralNetworks.Count());
        //    Assert.AreEqual(4, evol.Generation);
        //}
    }
}
