using System.Collections.Generic;

namespace Minesweeper
{
    public interface IGeneration
    {
        IEnumerable<INeuralNetwork> NeuralNetworks { get; }
        INeuralNetwork Best { get; }
        INeuralNetwork Worst { get; }
        INeuralNetwork Average { get; }

        int GenerationIndex { get; }


    }
}