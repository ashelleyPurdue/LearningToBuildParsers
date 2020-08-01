using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  internal class NumberRule : ILexerRule
  {
    public bool IsStartOfToken(StringWalker walker)
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
  
    public Token ConsumeToken(StringWalker w)
    {
      CodePos pos = w.Position;
      string sign = ConsumeSign();
      string wholePart = ConsumeWholePart();
      string decimalPart = ConsumeDecimalPart();
      string content = $"{sign}{wholePart}{decimalPart}";

      return new Token(pos, TokenType.Number, content);

      string ConsumeSign() => w.Peek() == '-'
        ? w.Consume(1)
        : "";

      string ConsumeWholePart() => w.ConsumeWhile(char.IsDigit);

      string ConsumeDecimalPart()
      {
        if (w.IsEmpty() || w.Peek() != '.')
          return "";

        string decimalPoint = w.Consume(1);
        string digits = w.ConsumeWhile(char.IsDigit);

        // If there is nothing after the decimal point, that's an error
        if (digits.Length == 0)
          throw new CompileErrorException(w.Position, "There must be digits after the decimal point");

        // There can only be one decimal point in the number.  If there's
        // another, that's an error.
        if (!w.IsEmpty() && w.Peek() == '.')
          throw new CompileErrorException(w.Position, "A number may only have one decimal point");

        return $".{digits}";
      }
    }
  }
}
