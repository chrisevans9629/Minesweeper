using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public interface ICloneable<T> : ICloneable
    {
        T CloneObject();
    }
    public interface ICandidateSolution<T> : ICloneable<ICandidateSolution<T>>
    {
        T CandidateItem { get; set; }
        int Fitness { get; set; }

        void Repair();
        void SetGene(int i, bool hasGene);
        bool HasGene(int i);
    }
}