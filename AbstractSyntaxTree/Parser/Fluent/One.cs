using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class One
  {
    public static RuleCallbackPair Of(params RuleCallbackPair[] rules)
    {
      var orParser = new BacktrackingOrParser();
      foreach (var pair in rules)
        orParser.AddRule(pair);

      return new RuleCallbackPair(() => orParser, _ => { });
    }
  }
}
