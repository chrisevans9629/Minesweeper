using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public interface IRandomizer
    {
        int IntInRange(IntegerRange range);
        int IntInRange(int from, int to);
        int IntLessThan(int upper);
        double GetDoubleFromZeroToOne();
    }

    public interface ICandidateSolutionGenerator<T, TInput>
    {
        ICandidateSolution<T> GenerateCandidate(TInput data);

        ICandidateSolution<T> CrossOver(ICandidateSolution<T> male, ICandidateSolution<T> female);

        ICandidateSolution<T> Mutate(ICandidateSolution<T> candidate);

    }




    public class GeneticAlgorithmEngine<T, TInput> where T : class,ICloneable
    {
        private readonly ICandidateSolutionGenerator<T, TInput> _generator;
        public int PopulationSize { get; set; }
        public double CrossOverRate { get; set; }
        public double MutationRate { get; set; }

        public GeneticAlgorithmEngine(int popsize, 
            int crossoverPercent, double mutationPercent, ICandidateSolutionGenerator<T, TInput> generator)
        {
            _generator = generator;
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

            throw new NotImplementedException();
        }
        public List<ICandidateSolution<T>> CurrentGeneration { get; private set; }

    }
}