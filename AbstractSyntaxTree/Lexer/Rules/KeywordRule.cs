using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  internal class KeywordRule : ILexerRule
  {
    private readonly ISet<string> _keywords;
    public KeywordRule(ISet<string> keywords)
    {
      _keywords = keywords;
    }

    public bool IsStartOfToken(StringWalker w)
    {
      if (!char.IsLetter(w.Peek()))
        return false;

      string word = w.PeekWhile(char.IsLetterOrDigit);
      return _keywords.Contains(word);
    }

    public Token ConsumeToken(StringWalker w)
    {
      CodePos pos = w.Position;
      string content = w.ConsumeWhile(char.IsLetterOrDigit);

      return new Token(pos, TokenType.Keyword, content);
    }
  }
}
