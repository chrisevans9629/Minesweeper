using System.Collections.Generic;

namespace Minesweeper
{
    public interface IOptimizationFunction
    {
       IFitnessVal FitnessVal { get; }
        int Generation { get; set; }
        IGeneration EvolveGeneration();
       // IEnumerable<INeuralNetwork> NeuralNetworks { get; }

        INeuralNetwork Evolve(double error);
    }
}