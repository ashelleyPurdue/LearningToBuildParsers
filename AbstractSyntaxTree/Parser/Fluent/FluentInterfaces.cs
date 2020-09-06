using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class Starts
  {
    public static IThen With => new Then();
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
    TNextSyntax Rule<TNode>(IRuleParser rule, Action<TNode> onMatched);
    TNextSyntax Token(Action<Token> onMatched);
    TNextSyntax Token(TokenType type, Action<string> onMatched);

    TNextSyntax Word(Action<string> onMatched) => Token(TokenType.Word, onMatched);
  }

  public interface ITheOptions<TNextSyntax>
  {
    TNextSyntax Token(TokenType type, string content);

    TNextSyntax Keyword(string content) => Token(TokenType.Keyword, content);
    TNextSyntax Symbol(string content) => Token(TokenType.Symbol, content);
  }

  public interface IThenA
  {
    IThen Then { get; }
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
    IThen Then { get; }
    IOr Or();
    IRuleParser Build();
  }
}
