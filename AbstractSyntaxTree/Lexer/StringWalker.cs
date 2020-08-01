using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
  /// <summary>
  /// Unlike TokenWalker, this class is MUTABLE.
  /// </summary>
  public class StringWalker
  {
    public CodePos Position => _position;
    private CodePos _position = new CodePos(0, 0);

    private IEnumerable<char> _content;

    // We allow for 1 character of lookbehind, mainly so we can
    // tell the difference between negative numbers and subtraction.
    // HACK: If this is the first character in the file, we treat it
    // as if the previous character were whitespace.
    private char _prevChar = ' ';

    public StringWalker(IEnumerable<char> content)
    {
      _content = content;
    }

    public bool IsEmpty() => !_content.Any();

    public char Peek() => _content.First();

    public char PeekBehind() => _prevChar;

    /// <summary>
    /// Returns the next n characters without
    /// advancing the line/column counters.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public string Peek(int count) => _content
      .Take(count)
      .AsString();

    public string PeekWhile(Func<char, bool> predicate) => _content
      .TakeWhile(predicate)
      .AsString();

    /// <summary>
    /// Returns the next n characters and advances the
    /// line/column counters
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public string Consume(int count)
    {
      if (count == 0)
        return "";

      string result = Peek(count);
      IncrementCounters(result);

      _content = _content.Skip(count);
      _prevChar = result[result.Length - 1];

      return result;
    }

    public string ConsumeWhile(Func<char, bool> predicate)
    {
      string result = PeekWhile(predicate);
      IncrementCounters(result);
      _content = _content.Skip(result.Length);

      // Logically, the previous char doesn't change
      // if nothing is consumed.
      if (result.Length > 0)
        _prevChar = result[result.Length - 1];

      return result;
    }

    public string ConsumeEscapedString()
    {
      // Skip the opening quote, after asserting that it is indeed
      // a quote.
      char openingQuote = Peek();
      if (openingQuote != '"')
        throw new CompileErrorException(Position, "ConsumeEscapedString() called, but it didn't start on a quote.");
      Consume(1);

      var strContent = new StringBuilder();
      while (Peek() != '"')
      {
        char c = Peek();

        // If it's a backslash, it must be an escape sequence.
        if (c == '\\')
        {
          // Skip the backslash, then use the next char
          // to determine which character this escape sequence
          // represents
          Consume(1);
          char codeChar = Consume(1)[0];
          char resultChar = codeChar switch
          {
            '\\' => '\\',
            '"' => '"',
            'n' => '\n',
            'r' => '\r',
            _ => throw new CompileErrorException(Position, $"Invalid escape sequence \\{codeChar}")
          };

          strContent.Append(resultChar);
          continue;
        }

        strContent.Append(Consume(1));
      }

      // Skip the ending quote
      Consume(1);
      return strContent.ToString();
    }

    public string ConsumeNumber()
    {
      string sign = ConsumeSign();
      string wholePart = ConsumeWholePart();
      string decimalPart = ConsumeDecimalPart();
      return $"{sign}{wholePart}{decimalPart}";

      string ConsumeSign() => Peek() == '-'
        ? Consume(1)
        : "";

      string ConsumeWholePart() => ConsumeWhile(char.IsDigit);

      string ConsumeDecimalPart()
      {
        if (IsEmpty() || Peek() != '.')
          return "";

        string decimalPoint = Consume(1);
        string digits = ConsumeWhile(char.IsDigit);

        // If there is nothing after the decimal point, that's an error
        if (digits.Length == 0)
          throw new CompileErrorException(Position, "There must be digits after the decimal point");

        // There can only be one decimal point in the number.  If there's
        // another, that's an error.
        if (!IsEmpty() && Peek() == '.')
          throw new CompileErrorException(Position, "A number may only have one decimal point");

        return $".{digits}";
      }
    }

    private void IncrementCounters(string consumedText)
    {
      foreach (char c in consumedText)
      {
        _position.CharNumber++;

        if (c == '\n')
        {
          _position.CharNumber = 0;
          _position.LineNumber++;
        }
      }
    }
  }
}
