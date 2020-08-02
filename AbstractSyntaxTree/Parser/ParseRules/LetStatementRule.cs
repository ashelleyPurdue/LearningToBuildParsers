using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class LetStatementRule : IParseRule<LetStatement>
  {
    public bool IsStartOfNode(TokenWalker walker)
    {
      var t = walker.Peek();
      return t.Type == TokenType.Keyword && t.Content == "let";
    }

    public (LetStatement node, TokenWalker rest) ParseNode(TokenWalker tokens)
    {
      tokens = tokens.ConsumeKeyword("let");

      var letStatement = new LetStatement();

      letStatement.Name = tokens.Expect(TokenType.Word).Content;
      tokens = tokens.Consume();

      // TODO: Optionally expect an equals sign and an expression
      // for the initial value

      tokens = tokens.ConsumeSymbol(";");
      return (letStatement, tokens);
    }
  }
}
