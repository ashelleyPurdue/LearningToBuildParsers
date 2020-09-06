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

      var classParser = Starts
        .With
        (
          The.Keyword("class"),
          A.Word(name => classDef.Name = name),
          The.Symbol("{"),
          The.Symbol("}")
        )
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

      var funcParser = Starts
        .With
        (
          The.Keyword("function"),
          A.Word(n => funcDef.Name = n),
          The.Symbol("("),
          The.Symbol(")"),
          The.Symbol("{"),
          The.Symbol("}")
        )
        .ReturnsNode(funcDef);

      var classParser = Starts
        .With
        (
          The.Keyword("class"),
          A.Word(n => classDef.Name = n),
          The.Symbol("{"),
          A.Rule<FunctionDefinition>(() => funcParser, classDef.Functions.Add),
          The.Symbol("}")
        )
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
  
    [Fact]
    public void One_Of_Works()
    {
      IRuleParser AlphabetParser()
      {
        return Starts
          .With
          (
            The.Word("a"),
            The.Word("b"),
            The.Word("c"),
            The.Word("d")
          )
          .ReturnsNode("abcd");
      }

      IRuleParser AberahamParser()
      {
        return Starts
          .With
          (
            The.Word("a"),
            The.Word("b"),
            The.Word("e"),
            The.Word("raham") // laziness
          )
          .ReturnsNode("aberaham");
      }

      var node = new TestNode();
      var oneOfParser = Starts
        .With
        (
          One.Of
          (
            A.Rule<string>(AlphabetParser(), t => node.name = t),
            A.Rule<string>(AberahamParser(), t => node.name = t)
          )
        )
        .ReturnsNode(node);

      string src = "a b e raham";
      var tokens = new Lexer().ToTokens(src);
      var result = oneOfParser.FeedAll(tokens);

      var resultNode = result.AssertComplete<TestNode>();
      Assert.Equal(node, resultNode);
      Assert.Equal("aberaham", resultNode.name);
    }
    public class TestNode
    {
      public string type;
      public string name;
      public List<TestNode> children = new List<TestNode>();
    }
  }
}
