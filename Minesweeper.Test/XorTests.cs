using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{

    public class SingleChar : Component
    {
        private readonly char _matchValue;

        public SingleChar(char matchValue)
        {
            _matchValue = matchValue;
        }
        public override bool AddValueIfValid(char nextValue)
        {
            if (_matchValue == nextValue)
            {
                this.MatchingCharacters = nextValue.ToString();
                Value = nextValue;
                return true;
            }

            return false;
        }
    }
    public class GoUntil : Component
    {
        private readonly Component _from;
        private readonly Component _to;

        public GoUntil(Component from, Component to)
        {
            _from = @from;
            _to = to;
        }
        public override bool AddValueIfValid(char nextValue)
        {
            return _from.AddValueIfValid(nextValue) || _to.AddValueIfValid(nextValue);
        }
    }
    public class Number : Component
    {
        public override bool AddValueIfValid(char nextValue)
        {
            if (double.TryParse(MatchingCharacters + nextValue, out var t))
            {
                Value = t;
                MatchingCharacters += nextValue;
                return true;
            }

            return false;
        }
    }
    public abstract class Component
    {
        public object Value { get; set; }
        public string MatchingCharacters { get; set; }
        public string PreviousCharacters { get; set; }
        public abstract bool AddValueIfValid(char nextValue);
    }
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