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
      node.Statements = new List<IStatement>();

      yield return ExpectKeyword("function", node);

      string funcName;
      yield return ExtractToken(TokenType.Word, node, out funcName);
      node.Name = funcName;

      yield return ExpectSymbol("(", node);
      yield return ExpectSymbol(")", node);
      yield return ExpectSymbol("{", node);

      // Keep parsing let statements until we encounter a closing bracket.
      // TODO: Replace this with a RuleParser
      while (_currentToken.Content != "}")
      {
        if (_currentToken.Content == "let")
        {
          // Parse the entirety of this let-statement, converting its
          // NextTokenResults into ones for a FunctionDefinition.
          foreach (var result in ParseLetStatement())
          {
            switch (result.state)
            {
              case RuleMatchState.GoodSoFar:
                yield return NextTokenResult.GoodSoFar(node);
                break;
              case RuleMatchState.Fail:
                yield return result;
                break;
              case RuleMatchState.Complete:
                node.Statements.Add((LetStatement)result.node);
                yield return NextTokenResult.GoodSoFar(node);
                break;
            }
          }
        }
      }

      yield return FinishWithSymbol("}", node);
    }

    private IEnumerable<NextTokenResult> ParseLetStatement()
    {
      var node = new LetStatement();

      yield return ExpectKeyword("let", node);

      string varName;
      yield return ExtractToken(TokenType.Word, node, out varName);
      node.Name = varName;

      yield return FinishWithSymbol(";", node);
    }
  }
}
