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
