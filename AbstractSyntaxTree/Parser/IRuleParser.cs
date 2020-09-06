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

    /// <summary>
    /// Resets the parser to its initial state
    /// </summary>
    public void Reset();

    /// <summary>
    /// Feeds all of the tokens into the parser
    /// Returns the final result.
    /// Stops early and throws an error if any of the tokens yields a failed result.
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public RuleResult FeedAll(IEnumerable<Token> tokens)
    {
      RuleResult result = default;

      foreach (Token t in tokens)
      {
        result = FeedToken(t);

        if (result.status == RuleStatus.Failed)
          throw result.error;
      }

      return result;
    }
  }
}
