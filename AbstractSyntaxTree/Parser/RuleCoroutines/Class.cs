using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public static partial class RuleCoroutines
  {
    public static IEnumerable<RuleResult> ParseClass(CurrentTokenCallback t)
    {
      var node = new ClassDefinition();
      node.Functions = new List<FunctionDefinition>();

      yield return Expect.Keyword(t(), "class");
      yield return Extract.Word(t(), name => node.Name = name);
      yield return Expect.Symbol(t(), "{");

      while (!t().IsSymbol("}"))
      {
        var classMemberParser = new MultiRuleParser()
          .AddRule<FunctionDefinition>(ParseFunction, f => node.Functions.Add(f));

        // Otherwise, keep feeding tokens until one of the rules completes, or
        // until there's an error.
        RuleResult result = RuleResult.GoodSoFar();
        while (result.status == RuleStatus.GoodSoFar)
        {
          result = classMemberParser.FeedToken(t());

          if (result.status == RuleStatus.GoodSoFar)
            yield return result;
        }
        
        switch (result.status)
        {
          case RuleStatus.Complete:
            yield return RuleResult.GoodSoFar();
            continue;
          case RuleStatus.Failed:
            yield return result;
            yield break;
          default:
            yield return RuleResult.Failed(t().Position, "Unexpected rule result " + result.status);
            yield break;
        }
      }

      yield return RuleResult.Complete(node);
    }
  }
}
