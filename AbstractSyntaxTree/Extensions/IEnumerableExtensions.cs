using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractSyntaxTree
{
  public static class IEnumerableExtensions
  {
    /// <summary>
    /// Like TakeWhile, except it puts the remaining items
    /// in an out parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="stuffThatWasChopped"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<T> ChopWhile<T>(this IEnumerable<T> sequence, out IEnumerable<T> rest, Func<T, bool> predicate)
    {
      rest = sequence.SkipWhile(predicate);
      return sequence.TakeWhile(predicate);
    }

    public static string AsString(this IEnumerable<char> charSeq)
      => new string(charSeq.ToArray());

    public static IEnumerable<char> ChopWord(this IEnumerable<char> str, out IEnumerable<char> rest)
      => str.ChopWhile(out rest, c => char.IsLetterOrDigit(c));

    public static IEnumerable<char> TakeWord(this IEnumerable<char> str)
      => str.TakeWhile(c => char.IsLetterOrDigit(c));

    /// <summary>
    /// Expects the next characters in the sequence to match a string
    /// Throws an exception if they do not.
    /// Returns the rest of the sequence after the expected string
    /// </summary>
    /// <param name="src"></param>
    /// <param name="expectedSequence"></param>
    /// <returns></returns>
    public static IEnumerable<char> ExpectNext(this IEnumerable<char> src, string expectedSequence)
    {
      var actualSequence = src
        .Take(expectedSequence.Length)
        .AsString();

      if (actualSequence != expectedSequence)
        throw new Exception($@"Expected the next bit of code to be ""{expectedSequence}"", but it was ""{actualSequence}""");

      return src.Skip(expectedSequence.Length);
    }

    public static IEnumerable<char> SkipWhitespace(this IEnumerable<char> str)
      => str.SkipWhile(c => char.IsWhiteSpace(c));
  }
}