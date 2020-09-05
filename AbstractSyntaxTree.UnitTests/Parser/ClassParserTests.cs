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

    [Fact]
    private void It_Can_Parse_Classes_With_Empty_Functions()
    {
      string src =
      @"
        class ThingDoer
        {
          function DoThing() {}
          function DoAnotherThing() {}
          function DoOneLastThing() {}
        }
      ";

      var result = ParseClass(src).Last();
      result.AssertComplete();

      var classDef = Assert.IsAssignableFrom<ClassDefinition>(result.node);
      var functionNames = classDef
        .Functions
        .Select(f => f.Name);

      Assert.Contains("DoThing", functionNames);
      Assert.Contains("DoAnotherThing", functionNames);
      Assert.Contains("DoOneLastThing", functionNames);
    }

    private IEnumerable<RuleResult> ParseClass(string src)
    {
      var lexer = new Lexer();
      var tokens = lexer.ToTokens(src);
      var rule = new RuleCoroutineParser(RuleCoroutines.ParseClass);

      foreach (Token t in tokens)
      {
        var result = rule.FeedToken(t);

        if (result.status == RuleStatus.Failed)
          throw result.error;

        yield return result;
      }
    }
  }
}
