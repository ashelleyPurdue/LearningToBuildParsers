using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

using AbstractSyntaxTree.Parser;

namespace AbstractSyntaxTree.UnitTests.Parser
{
  public class ClassParserTests
  {
    [Fact]
    private void It_Can_Parse_An_Empty_Class()
    {
      var result = ParseClass("class ThingDoer {}").Last();
      result.AssertComplete();

      var classDef = Assert.IsAssignableFrom<ClassDefinition>(result.node);
      Assert.Equal("ThingDoer", classDef.Name);
    }

    private IEnumerable<RuleResult> ParseClass(string src)
    {
      var lexer = new Lexer();
      var tokens = lexer.ToTokens(src);
      var rule = new RuleCoroutineParser(RuleCoroutines.ParseClass);

      foreach (Token t in tokens)
        yield return rule.FeedToken(t);
    }
  }
}
