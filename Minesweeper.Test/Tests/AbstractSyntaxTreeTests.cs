﻿namespace Minesweeper.Test.Tests
{
    //[TestFixture(typeof(SimpleTree))]
    //[TestFixture(typeof(AbstractSyntaxTree))]
    //public class AbstractSyntaxTreeTests<T> where T : IAbstractSyntaxTree, new()
    //{
    //    private T tree;

    //    [SetUp]
    //    public void Setup()
    //    {
    //        tree = new T();
    //    }

    //    [TestCase("10 + 3", 13)]
    //    [TestCase("10+10", 20)]
    //    [TestCase("10-10", 0)]
    //    [TestCase("10*10", 100)]
    //    [TestCase("10/10", 1)]
    //    [TestCase("9 - 5 + 3 + 11", 18)]
    //    [TestCase("9 + 5 - 3 + 11", 22)]
    //    [TestCase("3", 3)]
    //    [TestCase("7*4/2*3", 42)]
    //    public void Evaluate_Test(string input, double output)
    //    {
    //        var lex = new Lexer();
    //        lex.Ignore(" ");
    //        lex.Add("NUM", @"\d+");
    //        lex.Add("ADD", @"\+");
    //        lex.Add("SUB", @"-");
    //        lex.Add("MUL", @"\*");
    //        lex.Add("DIV", @"/");

    //        tree.AddExpress("NUM", (list, d) => Double.Parse(list[0].Value));
    //        // tree.AddExpress("Num Add Num", (p,_) => (int.Parse(p[0].Value) + int.Parse(p[2].Value)));
    //        tree.AddExpress("ADD NUM", (p, prev) => (prev + double.Parse(p[1].Value)));
    //        tree.AddExpress("SUB NUM", (p, prev) => (prev - double.Parse(p[1].Value)));
    //        // tree.AddExpress("Num Sub Num", (p, _) => (int.Parse(p[0].Value) - int.Parse(p[2].Value)));
    //        tree.AddExpress("MUL NUM", (p, prev) => (prev * double.Parse(p[1].Value)));
    //        tree.AddExpress("DIV NUM", (p, prev) => (prev / double.Parse(p[1].Value)));
    //        var tokens = lex.Tokenize(input);
    //        var t = tree.Evaluate(tokens);

    //        t.Should().Be(output);
    //    }
    //}
}