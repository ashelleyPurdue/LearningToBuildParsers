using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
  public class TokenWalker
  {
    private IEnumerable<IToken> _stream;

    public TokenWalker(IEnumerable<IToken> tokenStream)
    {
      _stream = tokenStream;
    }

    public bool IsEmpty() => !_stream.Any();

    public IToken Peek() => _stream.First();

    public IToken Consume()
    {
      var result = Peek();
      _stream = _stream.Skip(1);
      return result;
    }

    /// <summary>
    /// Expects the next token to be of a given type,
    /// and then consumes/returns it.
    /// </summary>
    /// <typeparam name="TToken"></typeparam>
    /// <returns></returns>
    public TToken Consume<TToken>() where TToken : IToken
    {
      Expect<TToken>();
      return (TToken)Consume();
    }

    /// <summary>
    /// Peeks at the next token and expects it to be of a given
    /// type.  
    /// Returns the token if it is one.
    /// Throws an error if it is not.
    /// </summary>
    /// <typeparam name="TToken"></typeparam>
    /// <returns></returns>
    public TToken Expect<TToken>() where TToken : IToken
    {
      string expectedName = typeof(TToken).Name;

      if (IsEmpty())
        throw new Exception($@"Expected a {expectedName}, but reached the end of the file.");

      var token = Peek();

      if (!(token is TToken correctToken))
        throw new Exception($@"Expected a {expectedName}, but got a {token.GetType().Name} instead.");

      return correctToken;
    }

    /// <summary>
    /// Peeks at the next token and expects it to be a keyword.
    /// Returns the keyword if it is one.
    /// Throws an error if it is not.
    /// </summary>
    /// <returns></returns>
    public KeywordToken ExpectKeyword(string keyword)
    {
      if (IsEmpty())
        throw new Exception($@"Expected the keyword ""{keyword}"", but reached the end of the file.");

      var token = Peek();

      if (!(token is KeywordToken keywordToken))
        throw new Exception($@"Expected the keyword ""{keyword}"", but got the token ""{token.ToString()}"" instead.");

      if (keywordToken.Content != keyword)
        throw new Exception($@"Expected the keyword ""{keyword}"", but got the keyword ""{keywordToken.Content}"" instead.");

      return keywordToken;
    }

    /// <summary>
    /// Expects the next token to be a specific keyword.
    /// Returns that token if it is.
    /// Throws an error if it is not.
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public KeywordToken ConsumeKeyword(string keyword)
    {
      ExpectKeyword(keyword);
      return (KeywordToken)Consume();
    }
  }
}
