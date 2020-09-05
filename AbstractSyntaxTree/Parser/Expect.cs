using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public static class Expect
  {
    public static RuleResult Symbol(Token t, string content, object node = null)
      => SpecificToken(t, TokenType.Symbol, content, node);

    public static RuleResult Word(Token t, string content, object node = null)
      => SpecificToken(t, TokenType.Word, content, node);

    public static RuleResult Keyword(Token t, string content, object node = null)
      => SpecificToken(t, TokenType.Keyword, content, node);

    private static RuleResult SpecificToken(
      Token t, 
      TokenType type, 
      string content, 
      object node = null
    )
    {
      if (t.Type != type || t.Content != content)
      {
        string msg = $@"Expected the {type} ""{content}"", but got the {t.Type} ""{t.Content}"" instead.";
        return RuleResult.Failed(t.Position, msg);
      }

      // If a node was provided, return a Completed result instead of GoodSoFar
      if (node != null)
        return RuleResult.Complete(node);

      return RuleResult.GoodSoFar();
    }
  }

  public static class Extract
  {
    public static RuleResult Word(Token t, Action<string> setter)
      => WithType(t, TokenType.Word, setter);

    private static RuleResult WithType(Token t, TokenType type, Action<string> setter)
    {
      if (t.Type != type)
      {
        string msg = $@"Expected a {type}, but got the {t.Type} ""{t.Content}"" instead.";
        return RuleResult.Failed(t.Position, msg);
      }

      // It was the correct type, so extract its content using the setter.
      setter(t.Content);
      return RuleResult.GoodSoFar();

      // TODO: Consider adding an overload that returns RuleResult.Complete(), just like Expect.
    }
  }
}
