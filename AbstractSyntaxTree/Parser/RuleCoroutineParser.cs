using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public delegate Token CurrentTokenCallback();
  public delegate IEnumerable<RuleResult> RuleCoroutine(CurrentTokenCallback currentToken);
  public class RuleCoroutineParser : IRuleParser
  {
    private readonly RuleCoroutine _coroutine;

    private IEnumerator<RuleResult> _state;
    private Token _currentToken;

    public RuleCoroutineParser(RuleCoroutine rule)
    {
      _coroutine = rule;
      Reset();
    }

    public RuleResult FeedToken(Token t)
    {
      _currentToken = t;
      _state.MoveNext();

      return _state.Current;
    }

    public void Reset()
    {
      _currentToken = null;
      _state = _coroutine(GetCurrentToken).GetEnumerator();
    }

    private Token GetCurrentToken() => _currentToken;
  }
}
