using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class A
  {
    public static RuleCallbackPair Rule<TNode>(IRuleParser rule, Action<TNode> onMatched)
    {
      return new RuleCallbackPair(rule, Wrapper);

      void Wrapper(object node)
      {
        onMatched((TNode)node);
      }
    }

    public static RuleCallbackPair Token(Action<Token> onMatched)
    {
      var rule = new SingleTokenParser(t => RuleResult.Complete(t));
      return new RuleCallbackPair(rule, Wrapper);

      void Wrapper(object node)
      {
        onMatched((Token)node);
      }
    }

    public static RuleCallbackPair Token(TokenType type, Action<string> onMatched)
    {
      var rule = new SingleTokenParser(t =>
      {
        if (t.Type == type)
          return RuleResult.Complete(t);

        string errMsg = $"Expected a {type}, but got the {t.Type} \"{t.Content}\"";
        return RuleResult.Failed(t.Position, errMsg);
      });
      return new RuleCallbackPair(rule, Wrapper);

      void Wrapper(object node)
      {
        var token = (Token)node;
        onMatched(token.Content);
      }
    }

    public static RuleCallbackPair Word(Action<string> onMatched) => Token(TokenType.Word, onMatched);
  }
}
