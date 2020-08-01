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

    private IEnumerable<char> _content;
    private CodePos _position = new CodePos(0, 0);

    public StringWalker(IEnumerable<char> content)
    {
      _content = content;
    }

    public bool IsEmpty() => !_content.Any();

    public char Peek() => _content.First();

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
      string result = Peek(count);
      IncrementCounters(result);
      _content = _content.Skip(count);

      return result;
    }

    public string ConsumeWhile(Func<char, bool> predicate)
    {
      string result = PeekWhile(predicate);
      IncrementCounters(result);
      _content = _content.Skip(result.Length);

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
