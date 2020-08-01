using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
  /// <summary>
  /// Unlike StringWalker, this class is IMMUTABLE
  /// </summary>
  public class TokenWalker
  {
    private readonly IEnumerable<Token> _stream;

    public TokenWalker(IEnumerable<Token> tokenStream)
    {
      _stream = tokenStream;
    }

    public bool IsEmpty() => !_stream.Any();

    public Token Peek() => _stream.First();

    public TokenWalker Consume()
    {
      var stream = _stream.Skip(1);
      return new TokenWalker(stream);
    }

    /// <summary>
    /// Expects the next token to be of a given type,
    /// and then consumes/returns it.
    /// </summary>
    /// <typeparam name="TToken"></typeparam>
    /// <returns></returns>
    public TokenWalker Consume(TokenType type)
    {
      Expect(type);
      return Consume();
    }

    /// <summary>
    /// Peeks at the next token and expects it to be of a given
    /// type.  
    /// Returns the token if it is one.
    /// Throws an error if it is not.
    /// </summary>
    /// <typeparam name="TToken"></typeparam>
    /// <returns></returns>
    public Token Expect(TokenType type)
    {
      if (IsEmpty())
        throw new Exception($@"Expected a {type}, but reached the end of the file.");

      var token = Peek();

      if (token.Type != type)
        throw new CompileErrorException(
          token.Position,
          $@"Expected a {type}, but got a {token.Type} instead."
        );

      return token;
    }

    /// <summary>
    /// Peeks at the next token and expects it to be a keyword.
    /// Returns the keyword if it is one.
    /// Throws an error if it is not.
    /// </summary>
    /// <returns></returns>
    public Token ExpectKeyword(string keyword)
    {
      if (IsEmpty())
        throw new Exception($@"Expected the keyword ""{keyword}"", but reached the end of the file.");

      var token = Peek();

      if (token.Type != TokenType.Keyword)
        throw new CompileErrorException(
          token.Position,
          $@"Expected the keyword ""{keyword}"", but got the {token.Type} ""{token.Content}"" instead."
        );

      if (token.Content != keyword)
        throw new CompileErrorException(
          token.Position,
          $@"Expected the keyword ""{keyword}"", but got the keyword ""{token.Content}"" instead."
        );

      return token;
    }

    public Token ExpectSymbol(string symbol)
    {
      if (IsEmpty())
        throw new Exception($@"Expected the symbol ""{symbol}"", but reached the end of file.");

      var token = Peek();

      if (token.Type != TokenType.Symbol)
        throw new CompileErrorException(
          token.Position,
          $@"Expected the symbol ""{symbol}"", but got the {token.Type} {token.Content}."
        );

      return token;
    }

    /// <summary>
    /// Expects the next token to be a specific keyword.
    /// Returns that token if it is.
    /// Throws an error if it is not.
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public TokenWalker ConsumeKeyword(string keyword)
    {
      ExpectKeyword(keyword);
      return Consume();
    }

    public TokenWalker ConsumeSymbol(string symbol)
    {
      ExpectSymbol(symbol);
      return Consume();
    }
  }
}
