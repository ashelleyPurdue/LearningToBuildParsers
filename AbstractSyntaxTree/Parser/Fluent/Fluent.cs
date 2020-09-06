using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class Starts
  {
    public static IThen With() => throw new NotImplementedException();
  }

  public interface IThen : ITheAndA<IThenA>
  {
    IOneOf OneOf();
  }

  public interface ITheAndA<TNextSyntax>
  {
    ITheOptions<TNextSyntax> The { get; }
    IAOptions<TNextSyntax> A { get; }
  }

  public interface IAOptions<TNextSyntax>
  {
    TNextSyntax Rule(IRuleParser rule);
    TNextSyntax Token();
    TNextSyntax Token(TokenType type);

    TNextSyntax Word() => Token(TokenType.Word);
  }

  public interface ITheOptions<TNextSyntax>
  {
    TNextSyntax Token(TokenType type, string content);

    TNextSyntax Keyword(string content) => Token(TokenType.Keyword, content);
    TNextSyntax Symbol(string content) => Token(TokenType.Symbol, content);
  }

  public interface IThenA
  {
    IThen Then();
    IRuleParser Build();
  }

  public interface IOneOf
  {
    IOneOfA A(IRuleParser rule);
  }

  public interface IOneOfA
  {
    IOr Or();
  }

  public interface IOr : ITheAndA<IOrA> { }

  public interface IOrA
  {
    IThen Then();
    IOr Or();
    IRuleParser Build();
  }
}
