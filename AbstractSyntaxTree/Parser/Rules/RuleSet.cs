using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AbstractSyntaxTree
{
  public class RuleSet
  {
    private readonly List<IParseRule> _rules = new List<IParseRule>();

    private Token _currentToken = null;
    private IEnumerator<NextTokenResult> _state = null;

    public RuleSet AddRule(IParseRule rule)
    {
      _rules.Add(rule);
      return this;
    }

    public NextTokenResult FeedToken(Token token)
    {
      // Start up the state machine, if it isn't already.
      if (_state == null)
        _state = TryParse().GetEnumerator();

      // Feed the token to the state machine
      _currentToken = token;

      if (!_state.MoveNext())
        throw new Exception("TODO: I dont' know what to do here yet");

      return _state.Current;
    }

    private IEnumerable<NextTokenResult> TryParse()
    {
      // We're going to start with a set of candidate rules that could
      // potentially be matched by the upcoming tokens.
      // Each token will be fed to every candidate rule.
      // The first candidate rule to match will be the "winner", and we'll
      // yield its node.
      // If a candidate rule fails, it is eliminated from the list of candidates.
      // If all of the candidates are eliminated, then this ruleset is fails.

      var candidateRules = _rules;

      while (true)
      {
        NextTokenResult? lastFailure = null;
        var rules = candidateRules;
        candidateRules = new List<IParseRule>();

        // Feed this token to every rule.
        foreach (var rule in rules)
        {
          NextTokenResult result = rule.FeedToken(_currentToken);

          // If this rule matched, then it wins.
          // The ruleset is complete, so yield its result and stop.
          if (result.state == RuleMatchState.Complete)
          {
            yield return result;
            yield break;
          }

          // If this rule failed, then it is eliminated.
          // We eliminate it by not adding it back into the
          // candidate rules.
          if (result.state == RuleMatchState.Fail)
          {
            lastFailure = result;
            continue;
          }

          // This rule is good so far, so don't eliminate it.
          candidateRules.Add(rule);
        }

        // If the last remaining candidate was eliminated, so the whole thing is a
        // bust.  Yield the error that caused the last rule to fail, and then
        // exit.
        if (candidateRules.Count <= 0)
        {
          yield return lastFailure.Value;
          yield break;
        }

        // There is at least one remaining candidate, so we're good so far.
        yield return NextTokenResult.GoodSoFar(null);
      }
    }
  }
}
