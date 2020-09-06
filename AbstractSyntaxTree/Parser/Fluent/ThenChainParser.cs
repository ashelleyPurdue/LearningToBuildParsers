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
    private bool _isFinished = false;
    private object _node = null;

    public void AddRule(IRuleParser rule, Action<object> callback)
    {
      _ruleSequence.Add((rule, callback));
    }

    public void ReturnWhenComplete(object node)
    {
      _node = node;
    }

    public RuleResult FeedToken(Token t)
    {
      if (_isFinished)
        throw new Exception("Tried to feed a token to an already-finished ThenChainParser.");

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
      {
        _isFinished = true;
        return RuleResult.Complete(_node);
      }

      return RuleResult.GoodSoFar();
    }

    public void Reset()
    {
      _currentRuleIndex = 0;
      _isFinished = false;

      foreach (var rulePair in _ruleSequence)
        rulePair.rule.Reset();
    }
  }
}
