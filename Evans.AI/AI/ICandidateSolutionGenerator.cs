﻿namespace Minesweeper
{
    public interface ICandidateSolutionGenerator<T, TInput>
    {
        ICandidateSolution<T> GenerateCandidate(TInput data);

        (ICandidateSolution<T> child1, ICandidateSolution<T> child2) CrossOver(ICandidateSolution<T> male, ICandidateSolution<T> female, TInput input = default(TInput));

        ICandidateSolution<T> Mutate(ICandidateSolution<T> candidate);

    }
}