using System;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test
{
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
            lexer.Add("Add", @"\+");
            lexer.Add("Subtract", @"\-");
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
        public void Subtraction()
        {
            lexer.Add("Sub", "-");
            var token = lexer.Tokenize("-");
            token.Should().HaveCount(1);
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
}