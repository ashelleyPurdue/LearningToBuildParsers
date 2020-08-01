using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  internal class StringRule : ILexerRule
  {
    public bool IsStartOfToken(StringWalker w) => w.Peek() == '"';

    public Token ConsumeToken(StringWalker w)
    {
      CodePos startPos = w.Position;

      // Skip the opening quote, after asserting that it is indeed
      // a quote.
      char openingQuote = w.Peek();
      if (openingQuote != '"')
        throw new CompileErrorException(w.Position, "StringRule.ConsumeToken() called, but it didn't start on a quote.");
      w.Consume(1);

      var strContent = new StringBuilder();
      while (w.Peek() != '"')
      {
        char c = w.Peek();

        // If it's a backslash, it must be an escape sequence.
        if (c == '\\')
        {
          // Skip the backslash, then use the next char
          // to determine which character this escape sequence
          // represents
          w.Consume(1);
          char codeChar = w.Consume(1)[0];
          char resultChar = codeChar switch
          {
            '\\' => '\\',
            '"' => '"',
            'n' => '\n',
            'r' => '\r',
            _ => throw new CompileErrorException(w.Position, $"Invalid escape sequence \\{codeChar}")
          };

          strContent.Append(resultChar);
          continue;
        }

        strContent.Append(w.Consume(1));
      }

      // Skip the ending quote
      w.Consume(1);
      return new Token(
        startPos,
        TokenType.String,
        strContent.ToString()
      );
    }
  }
}
