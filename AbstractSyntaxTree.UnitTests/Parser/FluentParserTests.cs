using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

using AbstractSyntaxTree.Parser;
using AbstractSyntaxTree.Parser.Fluent;

namespace AbstractSyntaxTree.UnitTests.Parser
{
  public class FluentParserTests
  {
    [Fact]
    public void You_Can_Express_An_Empty_Class()
    {
      string className = null;

      var classParser = Starts.With()
        .The.Keyword("class").Then()
        .A.Word(name => className = name).Then()
        .The.Symbol("{").Then()
        .The.Symbol("}").Build();

      // Generate some tokens to parse with the parser we just made
      string src = "class FooBar {}";
      var tokens = new Lexer().ToTokens(src);

      // Parse them
      RuleResult result = RuleResult.GoodSoFar();
      foreach (var token in tokens)
        result = classParser.FeedToken(token);

      // Assert that it correctly did stuff.
      result.AssertComplete();
      Assert.Equal("FooBar", className);
    }

    [Fact]
    public void You_Can_Compose_Multiple_Rules()
    {
      string funcName = null;
      string className = null;

      var funcParser = Starts.With().The.Keyword("function")
        .Then().A.Word(n => funcName = n)
        .Then().The.Symbol("(")
        .Then().The.Symbol(")")
        .Then().The.Symbol("{")
        .Then().The.Symbol("}")
        .Build();

      var classParser = Starts.With().The.Keyword("class")
        .Then().A.Word(n => className = n)
        .Then().The.Symbol("{")
        .Then().A.Rule<object>(funcParser, node => { }) // TODO: test that it returns some kind of function object
        .Then().The.Symbol("}")
        .Build();

      // Generate some tokens to parse with the parser we just made
      string src = 
      @"
        class FooBar
        {
          function DoThing() {}
        }
      ";
      var tokens = new Lexer().ToTokens(src).ToArray();

      // Parse them
      RuleResult result = RuleResult.GoodSoFar();
      foreach (var token in tokens)
        result = classParser.FeedToken(token);

      // Assert that it correctly did stuff.
      result.AssertComplete();
      Assert.Equal("FooBar", className);
      Assert.Equal("DoThing", funcName);
    }
  }
}
