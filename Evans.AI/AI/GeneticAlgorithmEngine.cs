using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public interface ICandidateSolution<T> : ICloneable
    {
        T CandidateItem { get; set; }
        int Fitness { get; set; }
    }
    public interface IRandomizer
    {

    }
    public class GeneticAlgorithmEngine<T> where T : class,ICloneable
    {
        public int PopulationSize { get; set; }
        public double CrossOverRate { get; set; }
        public double MutationRate { get; set; }

        public GeneticAlgorithmEngine(int popsize, int crossoverPercent, double mutationPercent)
        {
            PopulationSize = popsize;
            CrossOverRate = crossoverPercent / 100D;
            MutationRate = mutationPercent / 100D;
        }

        public void Run(T data)
        {
            CurrentGeneration = new List<ICandidateSolution<T>>(PopulationSize);
            for (int i = 0; i < PopulationSize; i++)
            {
                //CurrentGeneration.Add();
            }
        }
        public List<ICandidateSolution<T>> CurrentGeneration { get; private set; }

    }
}