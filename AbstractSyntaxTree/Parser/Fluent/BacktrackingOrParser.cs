using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public class BacktrackingOrParser : IRuleParser
  {
    private readonly List<RuleCallbackPair> _rules = new List<RuleCallbackPair>();

    private List<Token> _backtrackBuffer = new List<Token>();

    private bool _isFinished = false;
    private int _currentRuleIndex = 0;
    private IRuleParser _currentRule = null;

    public void AddRule(RuleCallbackPair rule)
    {
      _rules.Add(rule);
    }

    public RuleResult FeedToken(Token t)
    {
      if (_isFinished)
        throw new Exception("Tried to feed a token to an already-finished MultiRuleParser.");

      // HACK: Create the first rule if it hasn't been already
      if (_currentRule == null)
        _currentRule = _rules[_currentRuleIndex].ruleFactory();

      // Maintain a list of tokens so we can backtrack if necessary.
      _backtrackBuffer.Add(t);

      // Feed the token to the current rule
      var result = _currentRule.FeedToken(t);

      // If it completed, fire the callback.
      if (result.status == RuleStatus.Complete)
      {
        _rules[_currentRuleIndex].onMatched(result.node);
        _isFinished = true;
        return result;
      }

      // If it failed, backtrack
      if (result.status == RuleStatus.Failed)
        return Backtrack(result);

      // If it's still good, that's good
      return RuleResult.GoodSoFar();
    }

    private RuleResult Backtrack(RuleResult prevResult)
    {
      // Move to the next rule.
      // If we're all out of rules, the whole thing has failed.
      _currentRuleIndex++;
      if (_currentRuleIndex >= _rules.Count)
        return prevResult;
      _currentRule = _rules[_currentRuleIndex].ruleFactory();

      // Fast-forward through all the tokens we've received so far
      RuleResult result = prevResult;
      foreach (var token in _backtrackBuffer)
      {
        // HACK: If this one completed, but there's still tokens
        // left over, then those leftover tokens will vanish into
        // the aether.  That's because we have no way of making our
        // parent parser backtrack.  Need to redesign to make that
        // possible.
        // In the meantime, we can mitigate this problem by placing the
        // "shortest" rules higher in priority.
        if (result.status == RuleStatus.Complete)
          throw new Exception("Rule completed too early when backtracking; leftover tokens.");

        result = _currentRule.FeedToken(token);

        // If this one failed, backtrack AGAIN.
        if (result.status == RuleStatus.Failed)
          return Backtrack(result);
      }

      // We're all caught up!
      return result;
    }

    public void Reset()
    {
      _isFinished = false;
      _currentRuleIndex = 0;
      _currentRule = _rules[_currentRuleIndex].ruleFactory();
    }
  }
}
