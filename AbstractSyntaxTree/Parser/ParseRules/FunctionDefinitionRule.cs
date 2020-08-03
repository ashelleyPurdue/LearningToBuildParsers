using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class FunctionDefinitionRule : IParseRule<FunctionDefinition>
  {
    public bool IsStartOfNode(TokenWalker w)
    {
      var t = w.Peek();
      return t.Type == TokenType.Keyword && t.Content == "function";
    }

    public (FunctionDefinition node, TokenWalker rest) ParseNode(TokenWalker tokens)
    {
      // Look for the function keyword
      tokens = tokens.ConsumeKeyword("function");
      var funcDef = new FunctionDefinition();

      // Grab the name
      funcDef.Name = tokens.Expect(TokenType.Word).Content;
      tokens = tokens.Consume();

      // TODO: Parse the parameters.  For now, just enforce
      // that there are none.
      tokens = tokens
        .ConsumeSymbol("(")
        .ConsumeSymbol(")");

      // Parse the statements.
      var rules = new RuleParser();
      rules.AddRule(new LetStatementRule(), (s, rest) =>
      {
        funcDef.Statements.Add(s);
        tokens = rest;
      });

      tokens = tokens.ConsumeSymbol("{");
      funcDef.Statements = new List<IStatement>();

      while (!tokens.IsEmpty())
      {
        var token = tokens.Peek();

        if (token.Type == TokenType.Symbol && token.Content == "}")
          break;

        bool anyMatches = rules.NextNode(tokens);

        if (!anyMatches)
        {
          throw new CompileErrorException(
            token.Position,
            $"Unexpected token {token.Content}"
          );
        }
      }

      tokens = tokens.ConsumeSymbol("}");
      return (funcDef, tokens);
    }

  }
}
