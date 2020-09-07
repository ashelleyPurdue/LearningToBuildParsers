using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public class ThenChainParser : IRuleParser
  {
    private readonly List<RuleCallbackPair> _ruleSequence = new List<RuleCallbackPair>();

    private int _currentRuleIndex = 0;
    private IRuleParser _currentRule = null;
    private bool _isFinished = false;
    private object _node = null;

    public void AddRule(RuleCallbackPair rule)
    {
      _ruleSequence.Add(rule);
    }

    public void ReturnWhenComplete(object node)
    {
      _node = node;
    }

    public RuleResult FeedToken(Token t)
    {
      if (_isFinished)
        throw new Exception("Tried to feed a token to an already-finished ThenChainParser.");

      var currentCallabck = _ruleSequence[_currentRuleIndex].onMatched;

      // HACK: Create the first rule, if it hasn't been already.
      if (_currentRule == null)
        _currentRule = _ruleSequence[_currentRuleIndex].ruleFactory();

      // Feed the token to the current rule
      var result = _currentRule.FeedToken(t);

      // If it failed, cascade that failure upwards
      if (result.status == RuleStatus.Failed)
        return result;

      // If it succeeded, invoke the callback and move to the next rule
      if (result.status == RuleStatus.Complete)
      {
        currentCallabck(result.node);
        _currentRuleIndex++;

        // If the last rule just succeeded, the whole chain is complete.
        if (_currentRuleIndex >= _ruleSequence.Count)
        {
          _isFinished = true;
          return RuleResult.Complete(_node);
        }
        _currentRule = _ruleSequence[_currentRuleIndex].ruleFactory();
      }

      return RuleResult.GoodSoFar();
    }

    public void Reset()
    {
      _currentRuleIndex = 0;
      _currentRule = _ruleSequence[0].ruleFactory();
      _isFinished = false;
    }
  }
}
