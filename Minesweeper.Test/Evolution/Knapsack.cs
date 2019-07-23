namespace Minesweeper.Test
{
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
            var genetics = new GeneticAlgorithmEngine<ItemList, Knapsack>(100,10,10, new SolutionGenerator(), Randomizer);
            
            ResultContents = genetics.Run(this);
        }
    }
}