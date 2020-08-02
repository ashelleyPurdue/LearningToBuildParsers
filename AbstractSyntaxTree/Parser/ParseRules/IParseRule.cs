using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public interface IParseRule
  {
    bool IsStartOfNode(TokenWalker walker);
  }
  public interface IParseRule<TNode> : IParseRule
  {
    (TNode node, TokenWalker rest) ParseNode(TokenWalker walker);
  }
}
