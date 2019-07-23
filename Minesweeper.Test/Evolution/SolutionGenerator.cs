namespace Minesweeper.Test
{
    public class SolutionGenerator : ICandidateSolutionGenerator<ItemList, Knapsack>
    {
        public ICandidateSolution<ItemList> GenerateCandidate(Knapsack data)
        {
            return new CandidateSolutionItem(data.Capacity, data.Candidates, data.Randomizer);
        }

        public (ICandidateSolution<ItemList> child1, ICandidateSolution<ItemList> child2) CrossOver(ICandidateSolution<ItemList> male, ICandidateSolution<ItemList> female, Knapsack input)
        {
            var child1 = male.Clone() as ICandidateSolution<ItemList>;
            var child2 = female.Clone() as ICandidateSolution<ItemList>;

            var items = male.CandidateItem.Count;
            var crossPoint = input.Randomizer.IntLessThan(items);

            for (int i = 0; i < crossPoint; i++)
            {
                child1.SetGene(i, male.HasGene(i));
                child2.SetGene(i, female.HasGene(i));
            }

            for (int i = crossPoint; i < items; i++)
            {
                child1.SetGene(i, female.HasGene(i));
                child2.SetGene(i,male.HasGene(i));
            }

            return (child1, child2);
        }

        public ICandidateSolution<ItemList> Mutate(ICandidateSolution<ItemList> candidate, Knapsack input, double mutationRate)
        {
            for (int i = 0; i < candidate.CandidateItem.Count; i++)
            {
                if (input.Randomizer.GetDoubleFromZeroToOne() < mutationRate)
                {
                    candidate.SetGene(i,!candidate.HasGene(i));
                }
            }

            return candidate;
        }
    }
}