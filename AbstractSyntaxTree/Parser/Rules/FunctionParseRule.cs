using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class FunctionParseRule : IParseRule
  {
    public NextTokenResult FeedToken(Token token)
    {
      throw new NotImplementedException();
    }
  }
}
