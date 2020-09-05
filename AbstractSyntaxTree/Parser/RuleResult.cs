using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public enum RuleStatus
  {
    NotStarted,
    GoodSoFar,
    Complete,
    Failed
  }
  public struct RuleResult
  {
    public RuleStatus status;

    /// <summary>
    /// If the status is Complete, then this contains the node 
    /// that was constructed.  You will need to cast it, obviously.
    /// </summary>
    public object node;

    /// <summary>
    /// If the status is Failed, then this contains the
    /// compile error that caused the rule to fail.
    /// </summary>
    public CompileErrorException error;

    public static RuleResult GoodSoFar() => new RuleResult
    {
      status = RuleStatus.GoodSoFar
    };

    public static RuleResult Complete(object node) => new RuleResult
    {
      status = RuleStatus.Complete,
      node = node
    };

    public static RuleResult Failed(CompileErrorException err) => new RuleResult
    {
      status = RuleStatus.Failed,
      error = err
    };

    public static RuleResult Failed(CodePos pos, string reason) => new RuleResult
    {
      status = RuleStatus.Failed,
      error = new CompileErrorException(pos, reason)
    };
  }
}
