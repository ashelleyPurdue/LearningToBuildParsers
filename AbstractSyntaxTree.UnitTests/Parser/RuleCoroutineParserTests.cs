using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

using AbstractSyntaxTree.Parser;

namespace AbstractSyntaxTree.UnitTests.Parser
{
  public class RuleCoroutineParserTests
  {
    [Fact]
    public void It_Can_Expect_A_Sequence_Of_Tokens()
    {

      IEnumerable<RuleResult> SequenceRule(CurrentTokenCallback t)
      {
        yield return Expect.Symbol(t(), "(");
        yield return Expect.Symbol(t(), ")");
        yield return Expect.Symbol(t(), "{", true);
      }

      var parser = new RuleCoroutineParser(SequenceRule);
      var tokens = new[]
      {
        "(",
        ")",
        "{"
      }.Select(t => new Token(new CodePos(0, 0), TokenType.Symbol, t));

      // Feed all the tokens to the parser
      RuleResult result = default;
      foreach (Token t in tokens)
        result = parser.FeedToken(t);

      // Assert that it completed and returned the node.
      // In this test, the "node" is just a boolean.
      result.AssertComplete();
      Assert.Equal(true, result.node);
    }
  }
}
