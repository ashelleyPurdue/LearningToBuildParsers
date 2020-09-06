using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class Starts
  {
    public static IThen With() => throw new NotImplementedException();
  }

  public interface IThen : IATokenOptions<IThenA>
  {
    IOneOf OneOf();
  }

  public interface IATokenOptions<TNextSyntax>
  {
    TNextSyntax A(IRuleParser rule);
    TNextSyntax AWord();
    TNextSyntax TheSymbol(string content);
    TNextSyntax TheKeyword(string content);
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

  public interface IOr : IATokenOptions<IOrA> { }

  public interface IOrA
  {
    IThen Then();
    IOr Or();
    IRuleParser Build();
  }
}
