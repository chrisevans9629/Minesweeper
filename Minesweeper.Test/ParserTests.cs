using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{

    public interface IMathValue : IMathNode
    {

    }
    public class NumberValue : IMathValue
    {
        public string StringValue { get; set; }
        public double? Value { get; set; }
        public IEnumerable<IMathNode> GetMathNodes()
        {
            return Enumerable.Empty<IMathNode>();
        }
    }
    public class OperatorValue : IMathValue
    {
        private readonly Operation _op;

        public OperatorValue(Operation op)
        {
            _op = op;
        }

        public double? Value
        {
            get => _op(First.Value ?? 0, Second.Value ?? 0);
            set => throw new NotImplementedException();
        }

        public IMathNode First { get; set; }
        public IMathNode Second { get; set; }

    }

    public class MathTree
    {
        public MathTree()
        {

        }
        public IMathNode ParentNode { get; set; }

        public double? Value => ParentNode.Value;
    }
    public interface IMathNode
    {
        double? Value { get; set; }
    }


    public abstract class Operator : IMathValue
    {
        public abstract double Calculate(double first, double second);
        public abstract double Calculate();
        public double? Value { get => Calculate(); set => new NotImplementedException(); }

    }

    //public abstract class DoubleOperator : Operator
    //{
    //    private bool _calculated = false;

    //    public override double Calculate()
    //    {
    //        if (!_calculated)
    //        {
    //            _calculated = true;
    //            return Calculate((First.Value ?? 0), (Second.Value ?? 0));
    //        }
    //        return 0;
    //    }

    //    public IMathNode First { get; set; }
    //    public IMathNode Second { get; set; }
    //}
    public abstract class NumberOperator : Operator
    {
        private bool calculated = false;

        public IMathNode First { get; set; }
        public IMathNode Second { get; set; }
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
        [TestCase("10+10-10", 10)]
       // [TestCase("10-10*10", 90)]
        //[TestCase("10-10-10*10", 80)]

        public void Tests(string math, double result)
        {
            parser.Calculate(math).Should().Be(result);
        }
    }
}