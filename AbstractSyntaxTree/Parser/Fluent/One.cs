using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class One
  {
    public static RuleCallbackPair Of(params RuleCallbackPair[] rules)
    {
      IRuleParser RuleFactory()
      {
        var orParser = new OrParser();
        foreach (var pair in rules)
          orParser.AddRule(pair);
        return orParser;
      }

      return new RuleCallbackPair(RuleFactory, _ => { });
    }
  }
}
