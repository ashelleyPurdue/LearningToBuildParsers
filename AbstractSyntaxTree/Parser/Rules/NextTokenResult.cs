using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public enum RuleMatchState
  {
    GoodSoFar,
    Complete,
    Fail
  }
  public struct NextTokenResult
  {
    public RuleMatchState state;
    public object node;
    public CompileErrorException error;

    public static NextTokenResult Complete(object node)
    {
      return new NextTokenResult
      {
        state = RuleMatchState.Complete,
        node = node,
        error = null
      };
    }

    public static NextTokenResult GoodSoFar(object node)
    {
      return new NextTokenResult
      {
        state = RuleMatchState.GoodSoFar,
        node = node,
        error = null
      };
    }

    public static NextTokenResult Fail(object node, CompileErrorException error)
    {
      return new NextTokenResult
      {
        state = RuleMatchState.Fail,
        node = node,
        error = error
      };
    }

    public static NextTokenResult Fail(object node, CodePos pos, string errMessage)
    {
      var err = new CompileErrorException(pos, errMessage);
      return Fail(node, err);
    }
  }
}
