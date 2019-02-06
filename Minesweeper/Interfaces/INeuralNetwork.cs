using System;

namespace Minesweeper
{
    public interface INeuralNetwork : ICloneable
    {
        double Sigmoid(double val);
        double Derivative(double val);
        double[] FeedForward(double[] input);
        double Error { get; set; }
        double[][] Weights { get; }

        double[] Inputs { get; }
        double[] Outputs { get; }
        double[][] HiddenLayers { get; }
    }
}