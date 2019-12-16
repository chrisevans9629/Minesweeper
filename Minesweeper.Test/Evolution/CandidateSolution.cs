using System.Collections.Generic;

namespace Minesweeper.Test
{
    public abstract class CandidateSolution<T> : ICandidateSolution<T>
    {
        public virtual ICandidateSolution<T> CloneObject()
        {
            return this.MemberwiseClone() as ICandidateSolution<T>;
        }

        public object Clone()
        {
            return CloneObject();
        }

        public T CandidateItem { get; set; }
        public int Fitness { get; set; }
        public abstract void Repair();
        public List<bool> Genes { get; set; }

        public void SetGene(int i, bool hasGene)
        {
            Genes[i] = hasGene;
        }

        public bool HasGene(int i)
        {
            return Genes[i];
        }

    }
}