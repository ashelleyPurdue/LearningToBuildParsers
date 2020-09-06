using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class The
  {
    public static RuleCallbackPair Token(TokenType type, string content)
    {
      var rule = new SingleTokenParser(t =>
      {
        if (t.Type == type && t.Content == content)
          return RuleResult.Complete(t);

        string errMsg = $"Expected the {type} \"{content}\", but got the {t.Type} \"{t.Content}\"";
        return RuleResult.Failed(t.Position, errMsg);
      });
      return new RuleCallbackPair(() => rule, BlankCallback);

      void BlankCallback(object node) { }
    }

    public static RuleCallbackPair Keyword(string content) => Token(TokenType.Keyword, content);
    public static RuleCallbackPair Symbol(string content) => Token(TokenType.Symbol, content);
    public static RuleCallbackPair Word(string content) => Token(TokenType.Word, content);
  }
}
