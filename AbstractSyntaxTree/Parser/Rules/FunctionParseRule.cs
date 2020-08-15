using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class FunctionParseRule : BaseParseRule
  {
    protected override IEnumerable<NextTokenResult> TryParse()
    {
      var node = new FunctionDefinition();

      yield return ExpectKeyword("function", node);

      string funcName;
      yield return ExtractToken(TokenType.Word, node, out funcName);
      node.Name = funcName;

      yield return ExpectSymbol("(", node);
      yield return ExpectSymbol(")", node);
      yield return ExpectSymbol("{", node);

      var end = ExpectSymbol("}", node);
      end.state = RuleMatchState.Complete;
      yield return end;
    }
  }
}
