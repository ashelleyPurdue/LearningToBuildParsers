using System;
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
  }
}
