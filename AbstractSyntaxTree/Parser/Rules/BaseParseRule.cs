﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public abstract class BaseParseRule : IParseRule
  {
    protected Token _currentToken = null;
    private IEnumerator<NextTokenResult> _state = null;

    protected abstract IEnumerable<NextTokenResult> TryParse();

    public NextTokenResult FeedToken(Token token)
    {
      // Start up the state machine, if it isn't already.
      if (_state == null)
        _state = WrapCompileErrors(TryParse()).GetEnumerator();

      // Feed the token to the state machine
      _currentToken = token;

      if (!_state.MoveNext())
        throw new Exception("TODO: I dont' know what to do here yet");

      return _state.Current;
    }

    private IEnumerable<NextTokenResult> WrapCompileErrors(IEnumerable<NextTokenResult> results)
    {
      var resultsEnum = results.GetEnumerator();

      while (true)
      {
        bool hadNext = false;
        CompileErrorException error = null;

        // Attempt to get the next result
        try
        {
          hadNext = resultsEnum.MoveNext();
        }
        catch (CompileErrorException e)
        {
          error = e;
        }

        // If an error was thrown, convert it to a failed result.
        if (error != null)
        {
          // TODO: Somehow plumb in the node, instead of setting it to null.
          yield return NextTokenResult.Fail(null, error);
          break;
        }

        // If it was the end of the sequence, stop.
        if (!hadNext)
          break;

        // There is another result, so yield it.
        yield return resultsEnum.Current;
      }
    }

    protected NextTokenResult ExpectSpecificToken(
      TokenType type,
      string content,
      object node
    )
    {
      if (_currentToken.Type != TokenType.Symbol)
      {
        string msg = $@"Expected the {type} ""{content}"", but got the {_currentToken.Type} {_currentToken.Content}.";
        return NextTokenResult.Fail(node, _currentToken.Position, msg);
      }

      return NextTokenResult.GoodSoFar(node);
    }

    protected NextTokenResult ExpectSymbol(
      string symbol,
      object node
    ) => ExpectSpecificToken(TokenType.Symbol, symbol, node);

    protected NextTokenResult ExpectKeyword(
      string keyword,
      object node
    ) => ExpectSpecificToken(TokenType.Keyword, keyword, node);

    protected NextTokenResult ExtractToken(
      TokenType tokenType,
      object node,
      out string content
    )
    {
      // TODO: Return a failed NextTokenResult instead of throwing
      if (_currentToken.Type != tokenType)
      {
        string msg = $@"Expected a {tokenType}, but got the {_currentToken.Type} {_currentToken.Content}.";
        throw new CompileErrorException(_currentToken.Position, msg);
      }

      content = _currentToken.Content;
      return NextTokenResult.GoodSoFar(node);
    }
  }
}