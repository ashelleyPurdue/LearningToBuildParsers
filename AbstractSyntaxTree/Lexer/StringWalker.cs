using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
  public class StringWalker
  {
    private IEnumerable<char> _content;
    private int _charCount = 0;

    public StringWalker(IEnumerable<char> content)
    {
      _content = content;
    }

    public bool IsEmpty() => !_content.Any();

    public char Peek() => _content.First();

    public string Peek(int count) => _content
      .Take(count)
      .AsString();

    public string PeekWhile(Func<char, bool> predicate) => _content
      .TakeWhile(predicate)
      .AsString();

    public string Consume(int count)
    {
      string result = Peek(count);
      _charCount += count;
      _content = _content.Skip(count);

      return result;
    }

    public string ConsumeWhile(Func<char, bool> predicate)
    {
      string result = PeekWhile(predicate);
      _charCount += result.Length;
      _content = _content.Skip(result.Length);

      return result;
    }
  }
}
