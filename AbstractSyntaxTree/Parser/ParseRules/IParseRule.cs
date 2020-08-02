using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public interface IParseRule<TNode>
  {
    bool IsStartOfNode(TokenWalker walker);
    (TNode node, TokenWalker rest) ParseNode(TokenWalker walker);
  }
}
