using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
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