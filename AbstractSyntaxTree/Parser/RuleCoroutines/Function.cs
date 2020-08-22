using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public static partial class RuleCoroutines
  {
    public static IEnumerable<RuleResult> ParseFunction(CurrentTokenCallback t)
    {
      var node = new FunctionDefinition();
      node.Statements = new List<IStatement>();

      yield return Expect.Keyword(t(), "function");
      yield return Extract.Word(t(), name => node.Name = name);

      yield return Expect.Symbol(t(), "(");
      // TODO: Parse the parameters
      yield return Expect.Symbol(t(), ")");

      yield return Expect.Symbol(t(), "{");
      // TODO: Parse the statements
      yield return Expect.Symbol(t(), "}", node);

    }
  }
}
