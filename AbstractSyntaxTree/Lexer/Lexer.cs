using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
  public class Lexer
  {
    private readonly ISet<string> _keywords;

    public Lexer(ISet<string> keywords = null)
    {
      _keywords = keywords ?? new HashSet<string>();
    }

    public IEnumerable<Token> ToTokens(IEnumerable<char> src)
    {
      var walker = new StringWalker(src);
      while (!walker.IsEmpty())
      {
        char c = walker.Peek();

        // Skip all whitespace
        if (char.IsWhiteSpace(c))
        {
          walker.Consume(1);
          continue;
        }

        // Detect the start of multi-character tokens

        // WordTokens must start with a letter.
        // The rest of the chars can be either a letter or a digit.
        // If this word is on the list of keywords, then it will be
        // a KeywordToken.
        if (char.IsLetter(c))
        {
          CodePos p = walker.Position;
          string word = walker.ConsumeWhile(cc => char.IsLetterOrDigit(cc));

          if (_keywords.Contains(word))
            yield return new Token(p, TokenType.Keyword, word);
          else
            yield return new Token(p, TokenType.Word, word);

          continue;
        }

        // Strings begin with a double-quote, and end with the
        // next non-escaped quote
        if (c == '"')
        {
          CodePos p = walker.Position;
          string content = walker.ConsumeEscapedString();

          yield return new Token(p, TokenType.String, content);
          continue;
        }

        // TODO: number tokens

        // TODO: multi-character symbol tokens

        // This must be a single-character symbol token
        CodePos pos = walker.Position;
        walker.Consume(1);
        var allowedSymbols = new[]
        {
          '{',
          '}',
          '(',
          ')'
        };
        if (!allowedSymbols.Contains(c))
          throw new CompileErrorException(pos, $"Unexpected character '{c}'");

        yield return new Token(pos, TokenType.Symbol, "" + c);
      }
    }
  }
}
