using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public class GeneticAlgorithmEngine<T, TInput> where T : class,ICloneable
    {
        private readonly ICandidateSolutionGenerator<T, TInput> _generator;
        private readonly IRandomizer _randomizer;
        public int PopulationSize { get; set; }
        public double CrossOverRate { get; set; }
        public double MutationRate { get; set; }
        
        public GeneticAlgorithmEngine(int popsize, 
            int crossoverPercent, double mutationPercent, 
            ICandidateSolutionGenerator<T, TInput> generator, IRandomizer randomizer)
        {
            _generator = generator;
            _randomizer = randomizer;
            PopulationSize = popsize;
            CrossOverRate = crossoverPercent / 100D;
            MutationRate = mutationPercent / 100D;
        }

        public int MaxGenerationsWithNoChanges { get; set; }
        public T Run(TInput data)
        {
            CurrentGeneration = new List<ICandidateSolution<T>>(PopulationSize);
            for (int i = 0; i < PopulationSize; i++)
            {
                CurrentGeneration.Add(_generator.GenerateCandidate(data));
            }
            ICandidateSolution<T> bestSolution = null;
            int bestFitness = int.MinValue;

            var bestGen = 0;
            var gen = 1;
            while (true)
            {
                var bestGenFitness = 0;
                var totalFitness = 0;
                ICandidateSolution<T> bestSolutionThisGen = null;
                foreach (var candidateSolution in CurrentGeneration)
                {
                    candidateSolution.Repair();
                    totalFitness += candidateSolution.Fitness;
                    if (candidateSolution.Fitness > bestGenFitness)
                    {
                        bestGenFitness = candidateSolution.Fitness;
                        bestSolutionThisGen = candidateSolution;
                    }
                }

                if (totalFitness > bestFitness)
                {
                    bestFitness = totalFitness;
                    bestSolution = bestSolutionThisGen;
                    bestGen = gen;
                }
                else
                {
                    if ((gen - bestGen) > MaxGenerationsWithNoChanges)
                    {
                        break;
                    }
                }
                var nextGen = new List<ICandidateSolution<T>>();
                while (nextGen.Count < PopulationSize)
                {
                    var parent1 = RouletteWheelSelectCandidate(totalFitness).CloneObject();
                    var parent2 = RouletteWheelSelectCandidate(totalFitness).CloneObject();
                    if (_randomizer.GetDoubleFromZeroToOne() < CrossOverRate)
                    {
                        var (child1, child2) = _generator.CrossOver(parent1, parent2, data);
                        parent1 = child1;
                        parent2 = child2;
                    }

                    if (_randomizer.GetDoubleFromZeroToOne() < MutationRate)
                    {
                        parent1 = _generator.Mutate(parent1, data, MutationRate);
                    }

                    if (_randomizer.GetDoubleFromZeroToOne() < MutationRate)
                    {
                        parent2 = _generator.Mutate(parent2, data, MutationRate);
                    }

                    nextGen.Add(parent1);
                    nextGen.Add(parent2);

                }

                gen++;
            }

            return bestSolution?.CandidateItem;
        }
        


        ICandidateSolution<T> RouletteWheelSelectCandidate(int totalFitness)
        {
            var value = _randomizer.IntLessThan(totalFitness);

            for (int i = 0; i < PopulationSize; i++)
            {
                value -= CurrentGeneration[i].Fitness;
                if (value <= 0)
                {
                    return CurrentGeneration[i];
                }
            }

            return CurrentGeneration[PopulationSize - 1];
        }

        public List<ICandidateSolution<T>> CurrentGeneration { get; private set; }

    }
}