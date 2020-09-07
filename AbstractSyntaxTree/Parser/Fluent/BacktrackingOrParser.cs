using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public class BacktrackingOrParser : IRuleParser
  {
    private readonly List<RuleCallbackPair> _rules = new List<RuleCallbackPair>();
    private int _currentRule = 0;

    public void AddRule(RuleCallbackPair rule)
    {
      _rules.Add(rule);
    }

    public RuleResult FeedToken(Token t)
    {
      throw new NotImplementedException();
    }

    public void Reset()
    {
      throw new NotImplementedException();
    }
  }
}
