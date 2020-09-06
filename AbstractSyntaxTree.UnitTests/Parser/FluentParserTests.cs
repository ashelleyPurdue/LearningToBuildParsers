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
      var classDef = new ClassDefinition();

      var classParser = Starts.With.The.Keyword("class")
        .Then.A.Word(name => classDef.Name = name)
        .Then.The.Symbol("{")
        .Then.The.Symbol("}")
        .ReturnsNode(classDef);

      // Generate some tokens to parse with the parser we just made
      string src = "class FooBar {}";
      var tokens = new Lexer().ToTokens(src);
      var result = classParser.FeedAll(tokens);

      // Assert that it correctly did stuff.
      result.AssertComplete();
      Assert.Equal(classDef, result.node);
      Assert.Equal("FooBar", classDef.Name);
    }

    [Fact]
    public void You_Can_Compose_Multiple_Rules()
    {
      var funcDef = new FunctionDefinition();
      var classDef = new ClassDefinition();
      classDef.Functions = new List<FunctionDefinition>();

      var funcParser = Starts.With.The.Keyword("function")
        .Then.A.Word(n => funcDef.Name = n)
        .Then.The.Symbol("(")
        .Then.The.Symbol(")")
        .Then.The.Symbol("{")
        .Then.The.Symbol("}")
        .ReturnsNode(funcDef);

      var classParser = Starts.With.The.Keyword("class")
        .Then.A.Word(n => classDef.Name = n)
        .Then.The.Symbol("{")
        .Then.A.Rule<FunctionDefinition>(funcParser, f => classDef.Functions.Add(f))
        .Then.The.Symbol("}")
        .ReturnsNode(classDef);

      // Generate some tokens to parse with the parser we just made
      string src = 
      @"
        class FooBar
        {
          function DoThing() {}
        }
      ";
      var tokens = new Lexer().ToTokens(src).ToArray();
      var result = classParser.FeedAll(tokens);

      // Assert that it correctly did stuff.
      result.AssertComplete();

      Assert.Equal(classDef, result.node);
      Assert.Equal("FooBar", classDef.Name);

      Assert.Contains(funcDef, classDef.Functions);
      Assert.Equal("DoThing", funcDef.Name);
    }
  }
}
