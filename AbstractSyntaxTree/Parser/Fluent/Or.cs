using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public class OrA : IOrA
  {
    public readonly OrParser Parser = new OrParser();

    public IThen Then { get; private set; }
    public IOr Or { get; private set; }

    private readonly ThenA _thenA;

    public OrA(ThenA thenA, Then then, Or or)
    {
      _thenA = thenA;
      Then = then;
      Or = or;
    }

    public IRuleParser ReturnsNode(object node) 
      => _thenA.ReturnsNode(node);
  }

  public class Or : IOr
  {
    public IAOptions<IOrA> A { get; private set; }
    public ITheOptions<IOrA> The { get; private set; }

    public Or(ThenA thenA, Then then)
    {
      var orA = new OrA(thenA, then, this);
      A = new OrAOptions(orA);
      The = new OrTheOptions(orA);
    }
  }

  public class OrAOptions : IAOptions<IOrA>
  {
    private readonly OrA _orA;

    public OrAOptions(OrA orA)
    {
      _orA = orA;
    }

    public IOrA Rule<TNode>(IRuleParser rule, Action<TNode> onMatched)
    {
      _orA.Parser.Or(rule, onMatched);
      return _orA;
    }

    public IOrA Token(Action<Token> onMatched)
    {
      var rule = new SingleTokenParser(t => RuleResult.Complete(t));
      _orA.Parser.Or(rule, onMatched);
      return _orA;
    }

    public IOrA Token(TokenType type, Action<string> onMatched)
    {
      var rule = new SingleTokenParser(t =>
      {
        if (t.Type == type)
          return RuleResult.Complete(t);

        string errMsg = $"Expected a {type}, but got the {t.Type} \"{t.Content}\"";
        return RuleResult.Failed(t.Position, errMsg);
      });
      _orA.Parser.Or(rule, onMatched);
      return _orA;
    }
  }

  public class OrTheOptions : ITheOptions<IOrA>
  {
    private readonly OrA _orA;

    public OrTheOptions(OrA orA)
    {
      _orA = orA;
    }

    public IOrA Token(TokenType type, string content)
    {
      var rule = new SingleTokenParser(t =>
      {
        if (t.Type == type && t.Content == content)
          return RuleResult.Complete(t);

        string errMsg = $"Expected the {type} \"{content}\", but got the {t.Type} \"{t.Content}\"";
        return RuleResult.Failed(t.Position, errMsg);
      });
      _orA.Parser.Or<object>(rule, BlankCallback);
      return _orA;

      void BlankCallback(object node) { }
    }
  }
}
