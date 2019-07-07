using System;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{

    public class MathValue
    {

    }
    public class NumberValue : MathValue
    {
        public string StringValue { get; set; }
        public double? Value { get; set; }
     
    }

    public class SecondAdd : DoubleOperator
    {
      
        public override double Calculate(double first, double second)
        {
            return first + second;
        }
    }

    public class SecondSubtract : DoubleOperator
    {

        public override double Calculate(double first, double second)
        {
                return first - second;
        }
    }
    public class Subtract : NumberOperator
    {
        public override double Calculate(double first, double second)
        {
            return first - second;
        }
    }
    public abstract class Operator : MathValue
    {
        public abstract double Calculate(double first, double second);
        public abstract double Calculate();
    }

    public abstract class DoubleOperator : Operator
    {
        private bool calculated = false;

        public override double Calculate()
        {
            if (!calculated)
            {
                calculated = true;
                return Calculate(First.Calculate(),(Second.Value ?? 0));
            }
            return 0;
        }
        public NumberOperator First { get; set; }
        public NumberValue Second { get; set; }
    }
    public abstract class NumberOperator : Operator
    {
        private bool calculated = false;

        public NumberValue First { get; set; }
        public NumberValue Second { get; set; }
        public override double Calculate()
        {
            if (!calculated)
            {
                calculated = true;
                return Calculate(First.Value ?? 0, Second.Value ?? 0);
            }
            return 0;

        }

    }

    public class Add : NumberOperator
    {
        public override double Calculate(double first, double second)
        {
            return first + second;
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
        [TestCase("20+1", 21)]
        [TestCase("20+15", 35)]
        [TestCase("20", 20)]
        [TestCase("200+10", 210)]
        [TestCase("200-10", 190)]
        [TestCase("200*10", 2000)]
        [TestCase("1.5+2.5", 4)]
        [TestCase(".5+2.5", 3)]

        public void Tests(string math, double result)
        {
            parser.Calculate(math).Should().Be(result);
        }
    }
}