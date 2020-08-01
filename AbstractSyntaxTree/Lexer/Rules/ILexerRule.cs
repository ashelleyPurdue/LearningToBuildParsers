using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  internal interface ILexerRule
  {
    bool IsStartOfToken(StringWalker walker);
    Token ConsumeToken(StringWalker walker);
  }
}
