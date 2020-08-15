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
      yield return ExtractWord(node, n => node.Name = n);

      yield return ExpectSymbol("(", node);
      yield return ExpectSymbol(")", node);
      yield return ExpectSymbol("{", node);

      // Keep parsing let statements until we encounter a closing bracket.
      // TODO: Replace this with a RuleParser

      while (_currentToken.Content != "}")
      {
        var rules = new RuleSet()
          .AddRule<IStatement>(new LetStatementParseRule(), node.Statements.Add);

        // Feed all tokens into the ruleset until one of the rules completes,
        // or until there is an error.
        NextTokenResult result = rules.FeedToken(_currentToken);
        while (result.state == RuleMatchState.GoodSoFar)
        {
          yield return NextTokenResult.GoodSoFar(node);
          result = rules.FeedToken(_currentToken);
        }

        // If there was an error, bubble it up
        if (result.state == RuleMatchState.Fail)
          yield return result;
      }

      yield return FinishWithSymbol("}", node);
    }
  }

  public class LetStatementParseRule : BaseParseRule
  {
    protected override IEnumerable<NextTokenResult> TryParse()
    {
      var node = new LetStatement();

      yield return ExpectKeyword("let", node);
      yield return ExtractWord(node, n => node.Name = n);
      yield return FinishWithSymbol(";", node);
    }
  }
}
