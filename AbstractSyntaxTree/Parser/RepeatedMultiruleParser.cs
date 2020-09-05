using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public class RepeatedMultiruleParser : IRuleParser
  {
    public bool IsFinished { get; private set; } = false;

    private Func<Token, bool> _shouldTerminate;
    private object _node;

    private readonly MultiRuleParser _rules = new MultiRuleParser();
    private readonly RuleCoroutineParser _impl;

    public RepeatedMultiruleParser()
    {
      _impl = new RuleCoroutineParser(ImplCoroutine);
    }

    public RepeatedMultiruleParser AddRule<TNode>(IRuleParser rule, Action<TNode> onCompleted)
    {
      _rules.AddRule<TNode>(rule, onCompleted);
      return this;
    }

    public RepeatedMultiruleParser AddRule<TNode>(RuleCoroutine rule, Action<TNode> onCompleted)
    {
      _rules.AddRule<TNode>(rule, onCompleted);
      return this;
    }

    public RepeatedMultiruleParser TerminatesWhen(Func<Token, bool> shouldTerminate)
    {
      _shouldTerminate = shouldTerminate;
      return this;
    }

    public RepeatedMultiruleParser YieldsWhenComplete(object node)
    {
      _node = node;
      return this;
    }

    public RuleResult FeedToken(Token t) => _impl.FeedToken(t);

    public void Reset()
    {
      _rules.Reset();
      _impl.Reset();
      IsFinished = false;
    }

    private IEnumerable<RuleResult> ImplCoroutine(CurrentTokenCallback t)
    {
      while (!_shouldTerminate(t()))
      {
        _rules.Reset();

        // Keep feeding tokens until one of the rules completes, or
        // until there's an error.
        RuleResult result = RuleResult.GoodSoFar();
        while (result.status == RuleStatus.GoodSoFar)
        {
          result = _rules.FeedToken(t());

          if (result.status == RuleStatus.GoodSoFar)
            yield return result;
        }

        switch (result.status)
        {
          case RuleStatus.Complete:
            yield return RuleResult.GoodSoFar();
            continue;
          case RuleStatus.Failed:
            IsFinished = true;
            yield return result;
            yield break;
          default:
            IsFinished = true;
            yield return RuleResult.Failed(t().Position, "Unexpected rule result " + result.status);
            yield break;
        }
      }

      IsFinished = true;
      yield return RuleResult.Complete(_node);
    }
  }
}
