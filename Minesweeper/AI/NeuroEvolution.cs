using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if Roslyn
using NUnit.Framework;
#endif

namespace Minesweeper
{
#if Roslyn
    [TestFixture]
     class NeuroBackTest
    {
        [Test]
        public void ErrorTest()
        {
            var back = new NeuroBackPropagation(new NeuralNetwork(new Random(0), 3, new []{0}, 2, 2), 1 );
            var result = 0.75136507;
            var goal = 0.01;
            Assert.AreEqual(0.2748.ToString("n4"),back.Error(result,goal).ToString("n4"));
            Assert.AreEqual(0.7414.ToString("n4"), back.PartialDerivativeOutput(goal, result).ToString("n4"));
        }
    }
#endif

    public class NeuroBackPropagation : IOptimizationFunction
    {
        private readonly INeuralNetwork _network;
        private readonly double _steps;
        public IFitnessVal FitnessVal { get; }
        public int Generation { get; set; }

        public NeuroBackPropagation(INeuralNetwork network, double steps)
        {
            _network = network;
            _steps = steps;
        }

        public double PartialDerivativeOutput(double output, double target)
        {
            return (target - output);
        }
        public double Error(double output, double target)
        {
            return (1.0 / 2)*Math.Pow(target - output,2);
        }
        public void FeedForward(double[] input,double[] prediction)
        {
            var result = _network.FeedForward(input);
            var totalError = 0.0;
            for (var i = 0; i < result.Length; i++)
            {
                totalError += Error(result[i], prediction[i]);
            }
        }
        public IGeneration EvolveGeneration()
        {
            throw new NotImplementedException();
        }

        // public IEnumerable<INeuralNetwork> NeuralNetworks { get; }
        public INeuralNetwork Evolve(double error)
        {
            throw new NotImplementedException();
        }
    }

    public class NeuroEvolution : IOptimizationFunction
    {
        private readonly int _speciesCount;
        private readonly Random _random;
        public event EventHandler<IGeneration> GenerationFinished;
        public NeuroEvolution(IFitnessVal fitnessVal, int speciesCount, Func<INeuralNetwork> neuralFunc, Random random)
        {
            Generation = 1;
            _speciesCount = speciesCount;
            _random = random;
            FitnessVal = fitnessVal;
            var list = new List<INeuralNetwork>();
            for (int i = 0; i < speciesCount; i++)
            {
                list.Add(neuralFunc());
            }

            NeuralNetworks = list;
        }
        public IFitnessVal FitnessVal { get; }
        public int Generation { get; set; }

        private void Mutate(INeuralNetwork network)
        {
            for (var i = 0; i < network.Weights.Length; i++)
            {
                for (var i1 = 0; i1 < network.Weights[i].Length; i1++)
                {
                    var weights = network.Weights;
                    var option = _random.Next(0, 100);
                    //crossover
                    if (option >= 10 && option < 50)
                    {
                        weights[i][i1] *= _random.NextDouble() * 2 - 1;
                    }
                    //mutate
                    else if (option < 10)
                    {
                        weights[i][i1] = _random.NextDouble() * 2 - 1;
                    }
                    //invert
                    else if (option >= 80)
                    {
                        weights[i][i1] = -weights[i][i1];
                    }
                }
            }
        }
        public IGeneration EvolveGeneration()
        {
#if Roslyn
            Parallel.ForEach(NeuralNetworks, p =>
            {
                var fitness = FitnessVal.EvaluateFitness(p);
                p.Error = fitness;
            });
#endif
#if Bridge
            foreach (var neuralNetwork in NeuralNetworks)
            {
                var fitness = FitnessVal.EvaluateFitness(neuralNetwork);
                neuralNetwork.Error = fitness;
            }
#endif
            var orderedNetworks = FitnessVal.Maximize ? NeuralNetworks.OrderByDescending(p => p.Error).ToArray() : NeuralNetworks.OrderBy(p => p.Error).ToArray();
            //keep top 50
            //todo: copy top 50 networks and evolve the new networks

            var gen = new Generation(orderedNetworks, orderedNetworks[0], orderedNetworks[orderedNetworks.Length - 1], orderedNetworks[(orderedNetworks.Length - 1) / 2], Generation);
            Breed(ref orderedNetworks);
            var r = orderedNetworks.ToList();
            r.Reverse();
            var reverse = r.Take((int)(orderedNetworks.Length - orderedNetworks.Length * 0.1));
#if Roslyn
            Parallel.ForEach(reverse, Mutate);
#endif
#if Bridge
            foreach (var neuralNetwork in reverse)
            {
                Mutate(neuralNetwork);
            }
#endif

            OnGenerationFinished(gen);

            Generation++;
            return gen;
        }

        private void Breed(ref INeuralNetwork[] orderedNetworks)
        {
            var top = orderedNetworks.Take((orderedNetworks.Length - 1) / 2).ToArray();
            var newnetwork = new List<INeuralNetwork>();
            for (int i = 0; i < _speciesCount; i++)
            {
                var index = i % (top.Length - 1);
                if (index < top.Length)
                {
                    newnetwork.Add((INeuralNetwork)top[index].Clone());
                }
            }
            NeuralNetworks = newnetwork;
            orderedNetworks = FitnessVal.Maximize ? NeuralNetworks.OrderByDescending(p => p.Error).ToArray() : NeuralNetworks.OrderBy(p => p.Error).ToArray();
        }

        public IEnumerable<INeuralNetwork> NeuralNetworks { get; private set; }

        public INeuralNetwork Evolve(double error)
        {
            var currentBest = 0.0;
            if (FitnessVal.Maximize)
            {
                while (true)
                {
                    var gen = EvolveGeneration();
                    currentBest = gen.Best.Error;
                    if (error <= currentBest)
                    {
                        return gen.Best;
                    }
                }

            }
            else
            {
                while (true)
                {
                    var gen = EvolveGeneration();
                    currentBest = gen.Best.Error;

                    if (error >= currentBest)
                    {
                        return gen.Best;
                    }
                }
            }
        }

        protected virtual void OnGenerationFinished(IGeneration e)
        {
            GenerationFinished?.Invoke(this, e);
        }
    }
}