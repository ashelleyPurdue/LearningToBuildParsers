using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class Starts
  {
    public static RuleParserBuilder With(params RuleCallbackPair[] rules)
    {
      return new RuleParserBuilder().Then(rules);
    }
  }

  public class RuleParserBuilder
  {
    private readonly ThenChainParser _thenChain = new ThenChainParser();

    public RuleParserBuilder Then(params RuleCallbackPair[] rules)
    {
      foreach (var pair in rules)
        _thenChain.AddRule(pair);

      return this;
    }

    public IRuleParser ReturnsNode(object node)
    {
      _thenChain.ReturnWhenComplete(node);
      return _thenChain;
    }
  }

  public struct RuleCallbackPair
  {
    public Func<IRuleParser> ruleFactory;
    public Action<object> onMatched;

    public RuleCallbackPair(Func<IRuleParser> ruleFactory, Action<object> onMatched)
    {
      this.ruleFactory = ruleFactory;
      this.onMatched = onMatched;
    }
  }
}
