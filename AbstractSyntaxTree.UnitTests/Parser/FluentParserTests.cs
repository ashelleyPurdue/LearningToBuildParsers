using System;
using System.Collections.Generic;
using System.Text;
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
  }
}
