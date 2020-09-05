using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public class MultiRuleParser : IRuleParser
  {
    private List<IRuleParser> _rules = new List<IRuleParser>();
    private List<IRuleParser> _remainingRules = new List<IRuleParser>();

    private Dictionary<IRuleParser, Action<object>> _ruleCallbacks
      = new Dictionary<IRuleParser, Action<object>>();

    private bool _isFinished = false;

    public MultiRuleParser AddRule<TNode>(IRuleParser rule, Action<TNode> onCompleted)
    {
      _rules.Add(rule);
      _remainingRules.Add(rule);
      _ruleCallbacks.Add(rule, CallbackWrapper);

      void CallbackWrapper(object uncastedNode)
      {
        onCompleted((TNode)uncastedNode);
      }

      return this;
    }

    public MultiRuleParser AddRule<TNode>(RuleCoroutine coroutine, Action<TNode> onCompleted)
    {
      var rule = new RuleCoroutineParser(coroutine);
      return AddRule(rule, onCompleted);
    }

    public RuleResult FeedToken(Token t)
    {
      // TODO: Decide what the behavior should be when feeding a token
      // to an already-finished rule.
      if (_isFinished)
        throw new Exception("Tried to feed a token to an already-finished MultiRuleParser.");

      if (_remainingRules.Count == 0)
        throw new Exception("Tried to feed a token to an unfinished yet empty MultiRuleParser.");

      // Feed the token to each rule
      var ruleResults = _remainingRules
        .Select(r => (rule: r, result: r.FeedToken(t)))
        .ToArray();

      // Eliminate all of the rules that failed
      foreach (var p in ruleResults)
      {
        if (p.result.status == RuleStatus.Failed)
        {
          _remainingRules.Remove(p.rule);

          // If that was the last subrule, then the whole
          // ruleset fails.
          if (_remainingRules.Count == 0)
            return p.result;
        }
      }

      // If one of the rules succeeds, crown it the winner.
      // Its result will be returned and its callback will be
      // invoked.
      // If more than one rule succeeds, the first rule added
      // to the list gets priority.
      var completedRules = ruleResults
        .Where(p => p.result.status == RuleStatus.Complete);

      if (completedRules.Any())
      {
        _isFinished = true;

        var winner = completedRules.First();
        var node = winner.result.node;
        
        _ruleCallbacks[winner.rule](node);
        return winner.result;
      }

      // None of them succeeded, but not all of them failed.
      // That's the definition of being "good so far".
      return RuleResult.GoodSoFar();
    }
  
    public IEnumerable<RuleResult> ToCoroutine(CurrentTokenCallback t)
    {
      RuleResult result;
      do
      {
        result = this.FeedToken(t());
        yield return result;
      }
      while (result.status == RuleStatus.GoodSoFar);
    }

    public void Reset()
    {
      _isFinished = false;
      _remainingRules.Clear();

      foreach (var rule in _rules)
      {
        rule.Reset();
        _remainingRules.Add(rule);
      }
    }
  }
}
