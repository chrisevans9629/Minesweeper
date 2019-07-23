using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class CityCollection
    {
        public List<City> Cities { get; set; } = new List<City>();  
        public CityCollection(IRandomizer randomizer, int count, IntegerRange distanceRange)
        {
            for (int i = 0; i < count; i++)
            {
                Cities.Add(new City(){Id = i});
            }

            foreach (var city in Cities)
            {
                foreach (var otherCities in Cities)
                {
                    city.Roads.Add(new Road(){City = otherCities, Distance = randomizer.IntInRange(distanceRange)});
                }
            }
        }
    }
}