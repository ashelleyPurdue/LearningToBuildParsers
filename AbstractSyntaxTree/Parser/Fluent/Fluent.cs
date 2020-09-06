using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public static class Starts
  {
    public static IWith With() => throw new NotImplementedException();
  }

  public interface IWith
  {
    IWithA A(IRuleParser rule);
  }

  public interface IWithA
  {
    IThen Then();
    IRuleParser Build();
  }

  public interface IThen
  {
    IThenA A(IRuleParser rule);
    IOneOf OneOf();
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

  public interface IOr
  {
    IOrA A(IRuleParser rule);
  }

  public interface IOrA
  {
    IThen Then();
    IOr Or();
    IRuleParser Build();
  }
}
