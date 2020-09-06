using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class One
  {
    public static RuleCallbackPair Of(params RuleCallbackPair[] rules)
    {
      var orParser = new OrParser();
      foreach (var pair in rules)
      {
        // TODO: Make this happen lazily
        var rule = pair.ruleFactory();
        orParser.Or<object>(rule, pair.onMatched);
      }

      return new RuleCallbackPair(() => orParser, _ => { });
    }
  }
}
