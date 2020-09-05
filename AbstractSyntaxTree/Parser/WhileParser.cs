using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public class WhileParser : IRuleParser
  {
    public bool IsFinished { get; private set; } = false;

    private object _node;

    private readonly Func<Token, bool> _shouldKeepLooping;
    private readonly OrParser _rules = new OrParser();
    private readonly RuleCoroutineParser _impl;

    public WhileParser(Func<Token, bool> shouldKeepLooping)
    {
      _impl = new RuleCoroutineParser(ImplCoroutine);
      _shouldKeepLooping = shouldKeepLooping;
    }

    public WhileParser Or<TNode>(IRuleParser rule, Action<TNode> onCompleted)
    {
      _rules.Or<TNode>(rule, onCompleted);
      return this;
    }

    public WhileParser Or<TNode>(RuleCoroutine rule, Action<TNode> onCompleted)
    {
      _rules.Or<TNode>(rule, onCompleted);
      return this;
    }

    public WhileParser YieldsNode(object node)
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
      while (_shouldKeepLooping(t()))
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
