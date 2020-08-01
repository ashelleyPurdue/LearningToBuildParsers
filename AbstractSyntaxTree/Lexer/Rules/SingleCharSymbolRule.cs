using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AbstractSyntaxTree
{
  internal class SingleCharSymbolRule : ILexerRule
  {
    private readonly char[] _allowedSymbols = new[]
    {
      '{',
      '}',
      '(',
      ')',
      '-'
    };

    public bool IsStartOfToken(StringWalker w)
    {
      return _allowedSymbols.Contains(w.Peek());
    }

    public Token ConsumeToken(StringWalker w)
    {
      return new Token(
        w.Position,
        TokenType.Symbol,
        w.Consume(1)
      );
    }
  }
}
