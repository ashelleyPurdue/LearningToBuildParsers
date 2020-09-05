using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public delegate Token CurrentTokenCallback();
  public delegate IEnumerable<RuleResult> RuleCoroutine(CurrentTokenCallback currentToken);
  public class RuleCoroutineParser : IRuleParser
  {
    private readonly IEnumerator<RuleResult> _state;
    private Token _currentToken;

    public RuleCoroutineParser(RuleCoroutine rule)
    {
      _state = rule(GetCurrentToken).GetEnumerator();
    }

    public RuleResult FeedToken(Token t)
    {
      _currentToken = t;
      _state.MoveNext();

      return _state.Current;
    }

    private Token GetCurrentToken() => _currentToken;
  }
}
