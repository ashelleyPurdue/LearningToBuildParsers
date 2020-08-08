using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class FunctionParseRule : IParseRule
  {
    public IEnumerable<NextTokenResult> TryParse(IEnumerable<Token> tokens)
    {
      throw new NotImplementedException();
    }
  }
}
