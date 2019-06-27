using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public interface ICandidateSolution<T> : ICloneable
    {
        T CandidateItem { get; set; }
        int Fitness { get; set; }
        
    }
}