using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    public class SecondAdd : Operator
    {
        public Add First { get; set; }
        public double? Second { get; set; }
        private bool calculated = false;
        public override double Calculate()
        {
            if (!calculated)
            {
                calculated = true;
                return First.Calculate() + (Second ?? 0);
            }

            return 0;
        }
    }

    public abstract class Operator
    {
        public abstract double Calculate();
    }
    public class Add : Operator
    {
        public double? First { get; set; }
        public double? Second { get; set; }
        private bool calculated = false;

        public override double Calculate()
        {
            if (!calculated)
            {
                calculated = true;
                return (First ?? 0) + (Second ?? 0);
            }

            return 0;
        }
    }
    public class MathParser
    {

        public double Calculate(string value)
        {
            var operators = new List<Operator>();
            var previousValue = 0.0;
            for (var i = 0; i < value.Length; i++)
            {
                var currentValue = value[i].ToString();
                if (double.TryParse(currentValue, out var t))
                {
                    if (value.Length == 1)
                        return t;
                    previousValue = t;
                }
                var lastOp = operators.OfType<Add>().LastOrDefault();

                if (value[i] == '+')
                {
                    if (lastOp != null)
                    {
                        operators.Add(new SecondAdd() { First = lastOp });
                    }
                    else
                    {
                        operators.Add(new Add() { First = previousValue });

                    }
                    continue;
                }

                var secAdd = operators.OfType<SecondAdd>().LastOrDefault();
                if (secAdd != null && secAdd.Second == null)
                {
                    secAdd.Second = previousValue;
                    continue;
                }
                if (lastOp != null && lastOp.Second == null)
                {
                    lastOp.Second = previousValue;
                    continue;
                }
            }

            return operators.Sum(p => p.Calculate());
        }
    }

    [TestFixture]
    public class ParserTests
    {
        private MathParser parser;

        [SetUp]
        public void Setup()
        {
            parser = new MathParser();
        }

        [Test]
        public void Two()
        {
            parser.Calculate("2").Should().Be(2);
        }

        [Test]
        public void TwoAddTwo()
        {
            parser.Calculate("2+2").Should().Be(4);
        }

        [TestCase("2+2+2", 6)]
        [TestCase("2+2+2+2", 8)]
        [TestCase("2+4", 6)]
        public void Tests(string math, double result)
        {
            parser.Calculate(math).Should().Be(result);
        }
    }
}