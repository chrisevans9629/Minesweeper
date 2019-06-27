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

        public T Run(TInput data)
        {
            CurrentGeneration = new List<ICandidateSolution<T>>(PopulationSize);
            for (int i = 0; i < PopulationSize; i++)
            {
                CurrentGeneration.Add(_generator.GenerateCandidate(data));
            }

            while (true)
            {
                var totalFitness = 0;
                foreach (var candidateSolution in CurrentGeneration)
                {
                    candidateSolution.Repair();
                    totalFitness += candidateSolution.Fitness;
                }
                var nextGen = new List<ICandidateSolution<T>>();
                while (nextGen.Count < PopulationSize)
                {
                    var parent1 = RouletteWheelSelectCandidate(totalFitness).CloneObject();
                    var parent2 = RouletteWheelSelectCandidate(totalFitness).CloneObject();
                    if (_randomizer.GetDoubleFromZeroToOne() < CrossOverRate)
                    {
                        var (child1, child2) = _generator.CrossOver(parent1, parent2);

                    }
                }
            }
            throw new NotImplementedException();
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