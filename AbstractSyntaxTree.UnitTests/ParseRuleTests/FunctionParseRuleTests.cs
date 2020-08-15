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
      NextTokenResult result = ParseFunction("function DoThing(){}");

      Assert.Equal(RuleMatchState.Complete, result.state);
      var funcDef = Assert.IsAssignableFrom<FunctionDefinition>(result.node);
      Assert.Equal("DoThing", funcDef.Name);
      Assert.Empty(funcDef.Statements);
    }

    private NextTokenResult ParseFunction(string src)
    {
      // Get tokens from the src
      var keywords = new HashSet<string>
      {
        "function",
        "let"
      };
      var lexer = new Lexer(keywords);
      var tokens = lexer.ToTokens(src);

      // Feed all the tokens to the rule
      var rule = new FunctionParseRule();
      NextTokenResult lastResult = default;

      foreach (Token t in tokens)
        lastResult = rule.FeedToken(t);

      return lastResult;
    }
  }
}
