using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

using AbstractSyntaxTree.Parser;

namespace AbstractSyntaxTree.UnitTests
{
  public static class RuleResultExtensions
  {
    public static object AssertComplete(this RuleResult result)
    {
      Assert.Equal(RuleStatus.Complete, result.status);
      return result.node;
    }
    public static TNode AssertComplete<TNode>(this RuleResult result)
    {
      var node = result.AssertComplete();
      return Assert.IsAssignableFrom<TNode>(node);
    }

    public static CompileErrorException AssertFailed(this RuleResult result)
    {
      Assert.Equal(RuleStatus.Failed, result.status);
      return result.error;
    }

    public static void AssertGoodSoFar(this RuleResult result)
    {
      Assert.Equal(RuleStatus.GoodSoFar, result.status);
    }
  }
}
