using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace AbstractSyntaxTree.UnitTests
{
  public class TokenExpecter
  {
    private IEnumerable<Token> _tokens;

    public TokenExpecter(IEnumerable<Token> tokens)
    {
      _tokens = tokens;
    }

    public void AndNoOthers()
    {
      Assert.Empty(_tokens);
    }

    public TokenExpecter FollowedBy(TokenType type)
    {
      Assert.Equal(type, Current().Type);
      Next();

      return this;
    }

    public TokenExpecter FollowedBy(TokenType type, string content)
    {
      var token = Current();
      Assert.Equal(type, token.Type);
      Assert.Equal(content, token.Content);
      Next();

      return this;
    }

    private Token Current() => _tokens.First();
    private void Next()
    {
      _tokens = _tokens.Skip(1);
    }
  }
}
