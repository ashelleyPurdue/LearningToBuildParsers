using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AbstractSyntaxTree.UnitTests
{
  public static class Utils
  {
    public static void AssertEqualIgnoringOrder<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
      Assert.Equal(expected.Count(), actual.Count());

      foreach (T item in expected)
        Assert.Contains(item, actual);
    }
  }
}
