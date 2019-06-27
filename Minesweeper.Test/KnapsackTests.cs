using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    public class Item : ICloneable
    {
        public string Id { get; set; }
        public int Size { get; set; }
        public int Value { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class CandidateSolution : ICandidateSolution<ItemList>
    {
        private readonly IRandomizer _randomizer;

        public object Clone()
        {
            throw new NotImplementedException();
        }

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
        public int Capacity { get; set; }

        void CalcFitness()
        {
            Fitness = CandidateItem.Zip(Genes, (item, b) => new {item, b}).Where(p => p.b).Sum(p => p.item.Value);
        }
        public ItemList CandidateItem { get; set; }
        public int Fitness { get; set; }
    }


    public class ItemList : List<Item>, ICloneable
    {
        public ItemList(IList<Item> items) : base(items)
        {
            
        }
        public ItemList()
        {
            
        }
        public ItemList GetItemsBaseOnvalue(int containerSize)
        {
            var sorted = this.ToList().OrderByDescending(p => p.Value).ThenBy(p => p.Size).ToList();
            return GetItemsThatFit(containerSize, sorted);
        }

        ItemList GetItemsThatFit(int container, IList<Item> items)
        {
            var list = new ItemList();
            int size = 0;
            foreach (var item in items)
            {
                if (size + item.Size <= container)
                {
                    list.Add(item);
                    size += item.Size;
                }
            }

            return list;
        }

        public object Clone()
        {
            return new ItemList(this.Select(p=>p.Clone() as Item).ToList());
        }
    }

  
    public class Knapsack
    {
        public readonly IRandomizer Randomizer;
        public int Capacity { get; set; }
        public IntegerRange ItemSizes { get; set; }
        public IntegerRange ItemValues { get; set; }
        public int ItemCount { get; set; }

        public Knapsack(IRandomizer randomizer)
        {
            Randomizer = randomizer;
        }
        public void GenerateItems()
        {
            Candidates = new ItemList();
            for (int i = 0; i < ItemCount; i++)
            {
                Candidates.Add(new Item()
                {
                    Id = i.ToString(),
                    Value = Randomizer.IntInRange(this.ItemValues),
                    Size = Randomizer.IntInRange(this.ItemSizes),
                });
            }
        }

        public void Solve()
        {
            ResultContents = Candidates.GetItemsBaseOnvalue(Capacity);
        }

        public ItemList Candidates { get; private set; }
        public ItemList ResultContents { get; private set; }

        public void SolveWithGenetics()
        {
            var genetics = new GeneticAlgorithmEngine<ItemList, Knapsack>(1000,10,10, new SolutionGenerator());

            ResultContents = genetics.Run(this);
        }
    }

    public class SolutionGenerator : ICandidateSolutionGenerator<ItemList, Knapsack>
    {
        public ICandidateSolution<ItemList> GenerateCandidate(Knapsack data)
        {
            return new CandidateSolution(data.Capacity, data.Candidates, data.Randomizer);
        }

        public ICandidateSolution<ItemList> CrossOver(ICandidateSolution<ItemList> male, ICandidateSolution<ItemList> female)
        {
            throw new NotImplementedException();
        }

        public ICandidateSolution<ItemList> Mutate(ICandidateSolution<ItemList> candidate)
        {
            throw new NotImplementedException();
        }
    }


    [TestFixture]
    public class KnapsackTests
    {
        private Fixture fixture;
        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
            fixture.Inject<IRandomizer>(new Randomizer(new Random(100)));
        }
        Knapsack createKnapsack()
        {
            return fixture.Build<Knapsack>().With(p=>p.Capacity,300).With(p=>p.ItemCount,500).Create<Knapsack>();
        }

        [Test]
        public void KnapSack_GetBasedOnValue()
        {
            var sack = createKnapsack();
            sack.GenerateItems();
            sack.Solve();

            sack.ResultContents.Sum(p => p.Size).Should().BeLessOrEqualTo(sack.Capacity);
        }

        [Test]
        public void ItemList_BasedOnValue()
        {
            var list = new ItemList();
            list.Add(new Item(){Value = 20, Size = 15});
            list.Add(new Item(){Value = 40, Size = 10});
            list.Add(new Item(){Value = 50, Size = 30});
            list.Add(new Item(){Value = 45, Size = 20 });

            var result = list.GetItemsBaseOnvalue(50);

            result.Count.Should().Be(2);
            result.First().Value.Should().Be(50);
            result.Last().Value.Should().Be(45);
        }


        [Test]
        public void KnapSack_GetBasedOnGenetics()
        {
            var sack = createKnapsack();
            sack.GenerateItems();
            sack.SolveWithGenetics();

            var result = sack.ResultContents.Sum(p=>p.Value);

            sack.Solve();

            var regResult = sack.ResultContents.Sum(p=>p.Value);

            result.Should().BeGreaterOrEqualTo(regResult);
        }


        [Test]
        public void IntegerRangeTest()
        {
            var range = new IntegerRange(10,20);
            range.IsInRange(11).Should().BeTrue();
        }

        [Test]
        public void IntegerRange_OutsideRange()
        {
            var range = new IntegerRange(10,20);
            range.IsInRange(0).Should().BeFalse();
        }

        [Test]
        public void IntegerRange_FromMustBeSmallerThanTo()
        {
            var range = new IntegerRange(20,10);

            range.From.Should().Be(10);
            range.To.Should().Be(20);
        }
    }
}