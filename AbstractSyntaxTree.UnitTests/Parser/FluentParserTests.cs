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
      var classParser = Starts.With()
        .The.Keyword("class").Then()
        .A.Word().Then()
        .The.Symbol("{").Then()
        .The.Symbol("}").Build();

      string src = "class FooBar {}";
      var tokens = new Lexer().ToTokens(src);

      RuleResult result = RuleResult.GoodSoFar();
      foreach (var token in tokens)
        result = classParser.FeedToken(token);

      result.AssertComplete();
    }
  }
}
