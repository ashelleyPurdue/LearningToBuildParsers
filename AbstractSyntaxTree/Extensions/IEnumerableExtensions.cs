using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractSyntaxTree
{
  public static class IEnumerableExtensions
  {
    public static string AsString(this IEnumerable<char> charSeq)
      => new string(charSeq.ToArray());
  }
}