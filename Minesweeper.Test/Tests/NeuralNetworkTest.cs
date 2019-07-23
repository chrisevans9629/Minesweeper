using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Minesweeper.Test
{
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
