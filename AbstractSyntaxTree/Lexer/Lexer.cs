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

        // Numbers are tricky to identify the start of, but
        // that's what helper functions are for!
        if (IsStartOfNumber(walker))
        {
          CodePos p = walker.Position;
          string number = walker.ConsumeNumber();

          yield return new Token(p, TokenType.Number, number);
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

        // TODO: multi-character symbol tokens

        // This must be a single-character symbol token
        CodePos pos = walker.Position;
        walker.Consume(1);
        var allowedSymbols = new[]
        {
          '{',
          '}',
          '(',
          ')',
          '-'
        };
        if (!allowedSymbols.Contains(c))
          throw new CompileErrorException(pos, $"Unexpected character '{c}'");

        yield return new Token(pos, TokenType.Symbol, "" + c);
      }
    }
  
    private bool IsStartOfNumber(StringWalker walker)
    {
      char c = walker.Peek();

      if (char.IsDigit(c))
        return true;

      if (c != '-')
        return false;

      // It's a dash.  That means this COULD be the start of a negative number.
      // Or...it might just be a standalone minus sign.
      // We need to look at the surrounding characters to disambiguate.

      // If the next char is not a digit, then it can't possibly be a negative 
      // number.
      string group = walker.Peek(2);
      if (group.Length < 2)
        return false;
      if (!char.IsDigit(group[1]))
        return false;

      // OK, it's a dash followed by a digit.
      // But that's still not enough information!
      // The string "32-31" should be interpreted as subtracting 32 and 31, while
      // the string "32 -31" should be interpreted as 32 and -31.
      // To tell the difference, we need to look back 1 character and see if it
      // was whitespace.
      char prevChar = walker.PeekBehind();
      if (char.IsWhiteSpace(prevChar))
        return true;

      return false;
    }
  }
}
