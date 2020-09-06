using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public class SingleTokenParser : IRuleParser
  {
    private Func<Token, RuleResult> _feedToken;

    public SingleTokenParser(Func<Token, RuleResult> feedToken)
    {
      _feedToken = feedToken;
    }

    public RuleResult FeedToken(Token t) => _feedToken(t);

    public void Reset() { }
  }
}
