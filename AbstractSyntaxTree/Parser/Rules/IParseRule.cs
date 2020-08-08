using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public interface IParseRule
  {
    NextTokenResult FeedToken(Token token);
  }
}
