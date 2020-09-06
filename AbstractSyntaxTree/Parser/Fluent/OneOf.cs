using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser.Fluent
{
  public class OneOf : IOneOf
  {
    public ITheOptions<IOneOfA> The => throw new NotImplementedException();

    public IAOptions<IOneOfA> A => throw new NotImplementedException();
  }
}
