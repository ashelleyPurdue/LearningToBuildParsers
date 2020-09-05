using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public interface IRuleParser
  {
    /// <summary>
    /// Feeds the next token to the parser, updating its
    /// internal state machine.
    /// Returns the current status of the parser.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public RuleResult FeedToken(Token t);
  }
}
