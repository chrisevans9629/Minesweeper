using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
    public class TokenItem
    {
        public Token Token { get; set; }
        public int Position { get; set; }
        public int Line { get; set; }
        public string Value { get; set; }
    }
    public class Token
    {
        public string Expression { get; set; }
        public string Name { get; set; }
    }
    public class Lexer
    {
        IList<Token> tokens = new List<Token>();
        public void Add(string name, string expression)
        {
            tokens.Add(new Token(){Expression = expression, Name = name});
        }
        IList<string> ignoreables = new List<string>();
        public void Ignore(string expression)
        {
            ignoreables.Add(expression);
        }
        public IList<TokenItem> Tokenize(string str)
        {
            var items = new List<TokenItem>();

            var matches = tokens.Select(p => new {match = Regex.Matches(str, p.Expression), p});

            foreach (var match in matches)
            {
                foreach (Match o in match.match)
                {
                    if (o.Success)
                    {
                        items.Add(new TokenItem(){Token = match.p, Position = o.Index, Value = o.Value});
                    }
                }
            }

            //Validation
            {
                var rStr = str;
                foreach (var token in tokens)
                {
                    rStr = Regex.Replace(rStr, token.Expression, "");
                }

                foreach (var ignoreable in ignoreables)
                {
                    rStr = Regex.Replace(rStr, ignoreable, "");
                }

                if (string.IsNullOrEmpty(rStr) != true)
                {
                    var index = str.IndexOf(rStr, StringComparison.InvariantCulture);
                    throw new Exception($"Token '{rStr}' unrecognized at position {index} line 0");
                }
            }

            var resultTokens = items.OrderBy(p => p.Position).ToList();

            return resultTokens;
        }
    }
    [TestFixture]
    public class LexerTests
    {
        private Lexer lexer;

        [SetUp]
        public void Setup()
        {
            lexer = new Lexer();
        }

        [Test]
        public void Tokenize_App()
        {
            lexer.Ignore(" ");
            lexer.Add("Name", "[a-zA-Z]+");
            lexer.Add("Equal", "=");
            lexer.Add("Number", @"\d+");


            var tokens = lexer.Tokenize("var test = 10");

            tokens.Should().HaveCount(4);
        }

        [Test]
        public void Tokenize_FullOperations()
        {
            lexer.Ignore(" ");
            lexer.Add("Number", @"\d+");
            lexer.Add("Add",@"\+");
            lexer.Add("Subtract",@"\-");
            lexer.Add("ParLeft", @"\(");
            lexer.Add("ParRight", @"\)");

            var tokens = lexer.Tokenize("10 + (10 - 100)");

            tokens.Should().HaveCount(7);
            tokens[5].Value.Should().Be("100");

        }

        [Test]
        public void Tokenize_Number()
        {
            lexer.Add("Number", @"\d+");

            var tokens = lexer.Tokenize("10");

            tokens.Should().HaveCount(1);
            tokens[0].Token.Name.Should().Be("Number");
        }

        [Test]
        public void Tokenize_Ignored()
        {
            lexer.Ignore(" ");
            var tokens = lexer.Tokenize(" ");
            tokens.Should().BeEmpty();
        }

        [Test]
        public void Tokenize_TokenUnrecognized()
        {
            var ex = Assert.Throws<Exception>(() => lexer.Tokenize(" "));
            ex.Message.Should().Be("Token ' ' unrecognized at position 0 line 0");
        }

        [Test]
        public void Tokenize_Addition()
        {
            lexer.Add("Number", @"\d+");
            lexer.Add("Add", @"\+");
            var tokens = lexer.Tokenize("10+15");
            tokens.Should().HaveCount(3);

            tokens[1].Token.Name.Should().Be("Add");
            tokens[1].Line.Should().Be(0);
            tokens[1].Position.Should().Be(2);
        }
    }

    public class AbstractSyntaxTree
    {

    }
    
   
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