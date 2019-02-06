namespace Minesweeper
{
    public interface IFitnessVal
    {
        bool Maximize { get;  }

        double EvaluateFitness(INeuralNetwork network);
    }
}