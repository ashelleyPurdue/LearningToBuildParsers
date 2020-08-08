﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AbstractSyntaxTree
{
  public static class NextTokenResultEnumerableExtensionscs
  {
    public static NextTokenResult ExpectSpecificToken(
      this IEnumerable<Token> tokens,
      TokenType type,
      string content,
      object node
    )
    {
      if (!tokens.Any())
        throw new Exception($@"Expected the {type} ""{content}"", but reached the end of file.");

      var token = tokens.First();

      if (token.Type != TokenType.Symbol)
      {
        string msg = $@"Expected the {type} ""{content}"", but got the {token.Type} {token.Content}.";
        return NextTokenResult.Fail(node, token.Position, msg);
      }

      return NextTokenResult.GoodSoFar(node);
    }

    public static NextTokenResult ExpectSymbol(
      this IEnumerable<Token> tokens,
      string symbol,
      object node
    ) => tokens.ExpectSpecificToken(TokenType.Symbol, symbol, node);

    public static NextTokenResult ExpectKeyword(
      this IEnumerable<Token> tokens,
      string keyword,
      object node
    ) => tokens.ExpectSpecificToken(TokenType.Keyword, keyword, node);

    public static NextTokenResult ExtractToken(
      this IEnumerable<Token> tokens,
      TokenType tokenType,
      object node,
      out string content
    )
    {
      if (!tokens.Any())
        throw new Exception($"Expected a {tokenType} token, but reached the end of the file.");

      var token = tokens.First();
      
      // TODO: Return a failed NextTokenResult instead of throwing
      if (token.Type != tokenType)
      {
        string msg = $@"Expected a {tokenType}, but got the {token.Type} {token.Content}.";
        throw new CompileErrorException(token.Position, msg);
      }

      content = token.Content;
      return NextTokenResult.GoodSoFar(node);
    }
  }
}