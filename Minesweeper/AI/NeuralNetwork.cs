using System;
using System.Linq;

namespace Minesweeper
{
   
    public class BaseFitnessTest : IFitnessVal
    {
        public bool Maximize
        {
            get { return true; }
        }

        public double EvaluateFitness(INeuralNetwork network)
        {
            return network.FeedForward(new[] { network.Weights[0][0] })[0];
        }
    }
    public class NeuralNetwork : INeuralNetwork
    {
        public override string ToString()
        {
            return $"Error: {Error}, InputSize: {Inputs.Length}";
        }

        public NeuralNetwork(NeuralNetwork network)
        {
            _bias = network._bias;
            _random = network._random;
            Inputs = (double[])network.Inputs.Clone();
            Outputs = (double[])network.Outputs.Clone();
            HiddenLayers = (double[][])network.HiddenLayers.Clone();
            this.Weights = (double[][])network.Weights.Clone();
            this.Error = network.Error;
            // InitWeights();
            //  SetWeights();
        }
        private readonly Random _random;
        public double[] Inputs { get; private set; }
        public double[][] HiddenLayers { get; }
        public double[] Outputs { get; private set; }
        public double[][] Weights { get; private set; }
        private double _bias;
        void InitWeights()
        {
            Weights = new double[1 + HiddenLayers.Length][];
            for (var i = 0; i < Weights.Length; i++)
            {
                if (i == 0)
                {
                    Weights[i] = new double[Inputs.Length * HiddenLayers[0].Length];
                }
                else if (i == Weights.Length - 1)
                {
                    Weights[i] = new double[Outputs.Length * HiddenLayers[HiddenLayers.Length - 1].Length];
                }
                else
                {
                    if (HiddenLayers.Length > 1)
                    {
                        Weights[i] = new double[HiddenLayers[i - 1].Length * HiddenLayers[i].Length];

                    }
                }
            }



        }
        public double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }
        public double Derivative(double x)
        {
            double s = Sigmoid(x);
            return s * (1 - s);
        }

        public void ResetHiddenLayers()
        {
            for (var i = 0; i < HiddenLayers.Length; i++)
            {
                for (var i1 = 0; i1 < HiddenLayers[i].Length; i1++)
                {
                    HiddenLayers[i][i1] = 0;
                }
            }
        }
        public double[] FeedForward(double[] input)
        {
            if (input.Length != Inputs.Length)
            {
                throw new Exception("input size must be same size as input layer");
            }
            ResetHiddenLayers();
            Inputs = input;
            // CalculateFirstHiddenLayer();
            CalculateLayer(Inputs, HiddenLayers[0], Weights[0]);
            for (var i = 0; i < HiddenLayers[0].Length; i++)
            {
                HiddenLayers[0][i] += _bias;
            }
            for (int i = 1; i < HiddenLayers.Length; i++)
            {
                CalculateLayer(HiddenLayers[i - 1], HiddenLayers[i], Weights[i]);
            }

            CalculateLayer(HiddenLayers[HiddenLayers.Length - 1], Outputs, Weights[Weights.Length - 1]);
            //for (var i = 0; i < _outputs.Length; i++)
            //{
            //    var value = _outputs[i];
            //    _outputs[i] = Sigmoid(value);
            //}
            return Outputs;
        }

        public double Error { get; set; }


        void CalculateLayer(double[] firstLayer, double[] secondLayer, double[] weights)
        {
            var weightindex = 0;
            for (var i = 0; i < firstLayer.Length; i++)
            {
                for (var i1 = 0; i1 < secondLayer.Length; i1++)
                {
                    var weight = weights[weightindex];
                    var value = firstLayer[i] * weight;
                    secondLayer[i1] += value;
                    weightindex++;
                }
            }
            for (int i = 0; i < secondLayer.Length; i++)
            {
                secondLayer[i] = Sigmoid(secondLayer[i]);
            }
        }


        void SetWeights()
        {
            for (var i = 0; i < Weights.Length; i++)
            {
                for (var i1 = 0; i1 < Weights[i].Length; i1++)
                {
                    //between -1 and 1
                    Weights[i][i1] = _random.NextDouble() * 2 - 1;
                }
            }
        }
        public NeuralNetwork(Random random, int inputs, int[] hiddenlayer, int output, double bias)
        {
            _bias = bias;
            _random = random;
            Inputs = new double[inputs];
            Outputs = new double[output];
            HiddenLayers = new double[hiddenlayer.Length][];
            for (var index = 0; index < hiddenlayer.Length; index++)
            {
                var hi = hiddenlayer[index];
                HiddenLayers[index] = new double[hi];
            }
            InitWeights();
            SetWeights();
        }

        public object Clone()
        {
            return new NeuralNetwork(this);
        }
    }
}