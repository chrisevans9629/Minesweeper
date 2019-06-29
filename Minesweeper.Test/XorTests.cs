using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    public class PathChosen
    {
        public PathChosen(PathChosen pathChosen)
        {
            
        }
    }

    public class Road
    {
        public int Distance { get; set; }
        public City City { get; set; }
    }
    public class City
    {
        public List<Road> Roads { get; set; } = new List<Road>();
        public int Id { get; set; }
    }
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

    public class CandiateSolutionPath : CandidateSolution<PathChosen>
    {
       
        public override void Repair()
        {
            throw new NotImplementedException();
        }
        
    }
}