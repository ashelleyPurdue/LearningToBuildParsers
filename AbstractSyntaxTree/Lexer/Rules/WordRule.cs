using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  internal class WordRule : ILexerRule
  {
    public bool IsStartOfToken(StringWalker w)
    {
      return char.IsLetter(w.Peek());
    }

    public Token ConsumeToken(StringWalker w)
    {
      CodePos pos = w.Position;
      string content = w.ConsumeWhile(char.IsLetterOrDigit);

      return new Token(pos, TokenType.Word, content);
    }
  }
}