using System.Collections.Generic;
using System.Linq;

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

    public class CandidateSolutionItem : CandidateSolution<ItemList>
    {
        private readonly IRandomizer _randomizer;

        public CandidateSolutionItem(int capacity, ItemList list, IRandomizer randomizer)
        {
            _randomizer = randomizer;
            CandidateItem = list;
            Capacity = capacity;

            Genes = new List<bool>(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                Genes.Add(randomizer.GetDoubleFromZeroToOne() >= 0.5);
            }
            Repair();
            CalcFitness();
        }

        public override void Repair()
        {
            var items = CandidateItem.Count;
            var selected = Genes.Count(p => p);
            while (CandidateItem.Zip(Genes, (item, b) => new {item,b}).Where(p=>p.b).Sum(p=>p.item.Size) > Capacity )
            {
                var deslect = _randomizer.IntLessThan(selected);
                for (int i = 0; i < items; i++)
                {
                    if (Genes[i])
                    {
                        if (deslect == 0)
                        {
                            Genes[i] = false;
                            selected--;
                            break;
                        }

                        deslect--;
                    }
                }
            }
        }

        
        public int Capacity { get; set; }

        void CalcFitness()
        {
            Fitness = CandidateItem.Zip(Genes, (item, b) => new {item, b}).Where(p => p.b).Sum(p => p.item.Value);
        }


        public override ICandidateSolution<ItemList> CloneObject()
        {
            return new CandidateSolutionItem(Capacity, CandidateItem.Clone() as ItemList, _randomizer);
        }

      
    }
}