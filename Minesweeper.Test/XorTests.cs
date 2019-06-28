using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    public class CanidateSolution : ICandidateSolution<bool[]>
    {
        public ICandidateSolution<bool[]> CloneObject()
        {
            return new CanidateSolution(){CandidateItem = CandidateItem.ToArray()};
        }

        public object Clone()
        {
            return CloneObject();
        }

        public bool[] CandidateItem { get; set; }
        public int Fitness { get; set; }

        public bool[] Genes { get; set; }
        public void Repair()
        {
            throw new NotImplementedException();
        }

        public void SetGene(int i, bool hasGene)
        {
            throw new NotImplementedException();
        }

        public bool HasGene(int i)
        {
            throw new NotImplementedException();
        }
    }

    public class CandidateSolutionGenerator : ICandidateSolutionGenerator<bool[], bool[]>
    {
        public ICandidateSolution<bool[]> GenerateCandidate(bool[] data)
        {
            throw new System.NotImplementedException();
        }

        public (ICandidateSolution<bool[]> child1, ICandidateSolution<bool[]> child2) CrossOver(ICandidateSolution<bool[]> male, ICandidateSolution<bool[]> female,
            bool[] input = default(bool[]))
        {
            throw new System.NotImplementedException();
        }

        public ICandidateSolution<bool[]> Mutate(ICandidateSolution<bool[]> candidate, bool[] input, double mutationRate)
        {
            throw new System.NotImplementedException();
        }
    }

    [TestFixture]
    public class XorTests
    {
        [Test]
        public void XorAiTest()
        {
            var ai = new GeneticAlgorithmEngine<bool[],bool[]>(100,10,10, new CandidateSolutionGenerator(), new Randomizer(new Random(100)));

            var t = ai.Run(new[] {true, false, true});

            t[0].Should().BeFalse();
            t.Should().HaveCount(3);
        }
    }
}