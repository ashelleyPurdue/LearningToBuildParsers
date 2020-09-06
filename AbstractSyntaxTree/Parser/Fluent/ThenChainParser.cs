using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public class ThenChainParser : IRuleParser
  {
    private List<(IRuleParser rule, Action<object> callback)> _ruleSequence
      = new List<(IRuleParser rule, Action<object> callback)>();

    private int _currentRuleIndex = 0;

    public void AddRule(IRuleParser rule, Action<object> callback)
    {
      _ruleSequence.Add((rule, callback));
    }

    public RuleResult FeedToken(Token t)
    {
      var currentRule = _ruleSequence[_currentRuleIndex].rule;
      var currentCallabck = _ruleSequence[_currentRuleIndex].callback;

      // Feed the token to the current rule
      var result = currentRule.FeedToken(t);

      // If it failed, cascade that failure upwards
      if (result.status == RuleStatus.Failed)
        return result;

      // If it succeeded, invoke the callback and move to the next rule
      if (result.status == RuleStatus.Complete)
      {
        currentCallabck(result.node);
        _currentRuleIndex++;
      }

      // If the last rule just succeeded, the whole chain is complete.
      if (_currentRuleIndex >= _ruleSequence.Count)
        return RuleResult.Complete(null); // TODO: Shit, where does node come from?

      return RuleResult.GoodSoFar();
    }

    public void Reset()
    {
      _currentRuleIndex = 0;

      foreach (var rulePair in _ruleSequence)
        rulePair.rule.Reset();
    }
  }
}
