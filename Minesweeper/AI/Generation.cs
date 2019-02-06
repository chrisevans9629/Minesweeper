using System.Collections.Generic;

namespace Minesweeper
{
    public class Generation : IGeneration
    {
        public IEnumerable<INeuralNetwork> NeuralNetworks { get; }
        public INeuralNetwork Best { get; }
        public INeuralNetwork Worst { get; }
        public INeuralNetwork Average { get; }
        public int GenerationIndex { get; }

        public Generation(IEnumerable<INeuralNetwork> neuralNetworks, INeuralNetwork best, INeuralNetwork worst, INeuralNetwork average, int generationIndex)
        {
            NeuralNetworks = neuralNetworks;
            Best = best;
            Worst = worst;
            Average = average;
            GenerationIndex = generationIndex;
        }
    }
}