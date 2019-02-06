using System;
using System.Linq;
using System.Threading.Tasks;
using Bridge.QUnit;

namespace Minesweeper
{
    

    public class NeuralNetworkTest
    {
        public NeuralNetworkTest()
        {
            QUnit.Test("inputTest", InputTest);
            QUnit.Test("hiddenLayerTest", HiddenLayerTest);
            QUnit.Test("OutputTest", OutputTest);
            QUnit.Test("WeightTest", WeightTest);
            QUnit.Test("NeuroEvolutionTest", NeuroEvolution);
            QUnit.Test("EvolutionTest", EvolutionTest);
        }

        private void EvolutionTest(Assert assert)
        {
            var random = new Random(100);
            var neuro = new NeuroEvolution(new BaseFitnessTest(), 100, () => new NeuralNetwork(random, 1, new[] { 2, 4, 2 }, 2, 0.1), random);

            for (int i = 0; i < 10; i++)
            {
                ListCountAssert(assert, neuro);
            }

        }

        private static void ListCountAssert(Assert assert, NeuroEvolution neuro)
        {
            neuro.EvolveGeneration();

            assert.Ok(neuro.NeuralNetworks.ToList().Count == 100, neuro.NeuralNetworks.ToList().Count.ToString());
        }

        private void NeuroEvolution(Assert assert)
        {
            var random = new Random(100);
            var neuro = new NeuroEvolution(new BaseFitnessTest(), 1000, () => new NeuralNetwork(random, 1, new[] { 2, 4, 2 }, 2, 0.1), random);
            IGeneration firstGen = null;
            INeuralNetwork bestnet = null;

            //neuro.GenerationFinished += (sender, generation) => { Console.WriteLine(generation.Best.Error); };
            firstGen = neuro.EvolveGeneration();
            // bestnet = neuro.Evolve(1);
            for (int i = 0; i < 10; i++)
            {
                bestnet = neuro.EvolveGeneration().Best;
            }

            assert.Ok(firstGen.Best.Error < bestnet?.Error);



        }

        private void WeightTest(Assert assert)
        {
            var network = new NeuralNetwork(new Random(123), 3, new int[] { 1, 2, 3 }, 3, .1);
            assert.Ok(network.Weights.Length == 4, "weights equals " + network.Weights.Length + " and network size is " + (network.HiddenLayers.Length + 2));

        }

        private void OutputTest(Assert assert)
        {
            var network = new NeuralNetwork(new Random(123), 3, new int[] { 1, 2, 3 }, 3, .1);
            assert.Ok(network.FeedForward(new[] { 1.0, 2, 3 })[0] != 0, "Output: " + network.FeedForward(new[] { 1.0, 2, 3 }).ArrayContentString());
        }

        private void HiddenLayerTest(Assert assert)
        {
            var network = new NeuralNetwork(new Random(123), 3, new int[] { 1, 2, 3 }, 3, .1);
            assert.Equal(network.HiddenLayers.Length, 3);
        }

        private void InputTest(Assert assert)
        {
            var network = new NeuralNetwork(new Random(123), 3, new int[] { 1 }, 3, .1);
            assert.Equal(network.Inputs.Length, 3);
        }
    }
}