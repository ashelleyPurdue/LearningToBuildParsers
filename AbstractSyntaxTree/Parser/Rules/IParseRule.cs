using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public interface IParseRule
  {
    IEnumerable<NextTokenResult> TryParse(IEnumerable<Token> tokens);
  }
}
