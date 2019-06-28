using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class CandidateSolution : ICandidateSolution<ItemList>
    {
        private readonly IRandomizer _randomizer;

        public List<bool> Genes { get; set; }
        public CandidateSolution(int capacity, ItemList list, IRandomizer randomizer)
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

        public void Repair()
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

        public void SetGene(int i, bool hasGene)
        {
            Genes[i] = hasGene;
        }

        public bool HasGene(int i)
        {
            return Genes[i];
        }

        public int Capacity { get; set; }

        void CalcFitness()
        {
            Fitness = CandidateItem.Zip(Genes, (item, b) => new {item, b}).Where(p => p.b).Sum(p => p.item.Value);
        }

       

        public ItemList CandidateItem { get; set; }
        public int Fitness { get; set; }
        public ICandidateSolution<ItemList> CloneObject()
        {
            return new CandidateSolution(Capacity, CandidateItem.Clone() as ItemList, _randomizer);
        }

        public object Clone()
        {
            return CloneObject();
        }
    }
}