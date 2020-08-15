using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AbstractSyntaxTree.UnitTests
{
  public class FunctionParseRuleTests
  {
    private static readonly CodePos DEFAULT_POS = new CodePos(0, 0);

    [Fact]
    public void It_Can_Parse_Empty_Functions()
    {
      var tokens = new Token[]
      {
        Keyword("function"),
        Word("DoThing"),
        Symbol("("),
        Symbol(")"),
        Symbol("{"),
        Symbol("}")
      };

      var rule = new FunctionParseRule();
      NextTokenResult lastResult = default;

      foreach (Token t in tokens)
        lastResult = rule.FeedToken(t);

      Assert.Equal(RuleMatchState.Complete, lastResult.state);
      var funcDef = Assert.IsAssignableFrom<FunctionDefinition>(lastResult.node);
      Assert.Equal("DoThing", funcDef.Name);
      Assert.Empty(funcDef.Statements);
    }

    private Token Word(string content) => new Token(DEFAULT_POS, TokenType.Word, content);
    private Token Keyword(string content) => new Token(DEFAULT_POS, TokenType.Keyword, content);
    private Token Symbol(string content) => new Token(DEFAULT_POS, TokenType.Symbol, content);
  }
}
