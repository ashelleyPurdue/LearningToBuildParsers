using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public class ThenA : IThenA
  {
    public readonly ThenChainParser Parser = new ThenChainParser();
    public IThen Then { get; private set; }

    public ThenA(Then then)
    {
      Then = then;
    }

    public IRuleParser ReturnsNode(object node)
    {
      Parser.ReturnWhenComplete(node);
      return Parser;
    }
  }

  public class Then : IThen
  {
    public IAOptions<IThenA> A { get; private set; }
    public ITheOptions<IThenA> The { get; private set; }

    private readonly ThenA _thenA;

    public Then()
    {
      _thenA = new ThenA(this);
      A = new ThenAOptions(_thenA);
      The = new ThenTheOptions(_thenA);
    }

    public IOr OneOf()
    {
      return new Or(_thenA, this);
    }
  }

  public class ThenAOptions : IAOptions<IThenA>
  {
    private readonly ThenA _thenA;

    public ThenAOptions(ThenA thenA)
    {
      _thenA = thenA;
    }

    public IThenA Rule<TNode>(IRuleParser rule, Action<TNode> onMatched)
    {
      _thenA.Parser.AddRule(rule, WrapCallback);
      return _thenA;

      void WrapCallback(object node)
      {
        onMatched((TNode)node);
      }
    }

    public IThenA Token(Action<Token> onMatched)
    {
      var rule = new SingleTokenParser(t => RuleResult.Complete(t));
      _thenA.Parser.AddRule(rule, WrapCallback);
      return _thenA;

      void WrapCallback(object node)
      {
        onMatched((Token)node);
      }
    }

    public IThenA Token(TokenType type, Action<string> onMatched)
    {
      var rule = new SingleTokenParser(t =>
      {
        if (t.Type == type)
          return RuleResult.Complete(t);

        string errMsg = $"Expected a {type}, but got the {t.Type} \"{t.Content}\"";
        return RuleResult.Failed(t.Position, errMsg);
      });
      _thenA.Parser.AddRule(rule, WrapCallback);
      return _thenA;

      void WrapCallback(object node)
      {
        var token = (Token)node;
        onMatched(token.Content);
      }
    }
  }

  public class ThenTheOptions : ITheOptions<IThenA>
  {
    private readonly ThenA _thenA;

    public ThenTheOptions(ThenA thenA)
    {
      _thenA = thenA;
    }

    public IThenA Token(TokenType type, string content)
    {
      var rule = new SingleTokenParser(t =>
      {
        if (t.Type == type && t.Content == content)
          return RuleResult.Complete(t);

        string errMsg = $"Expected the {type} \"{content}\", but got the {t.Type} \"{t.Content}\"";
        return RuleResult.Failed(t.Position, errMsg);
      });
      _thenA.Parser.AddRule(rule, BlankCallback);
      return _thenA;

      void BlankCallback(object node) { }
    }
  }
}
