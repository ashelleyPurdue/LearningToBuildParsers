using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AbstractSyntaxTree
{
  public class ClassParseRule : IParseRule
  {
    private Token _currentToken = null;
    private IEnumerator<NextTokenResult> _state = null;

    public NextTokenResult FeedToken(Token token)
    {
      // Start up the state machine, if it isn't already.
      if (_state == null)
        _state = TryParse().GetEnumerator();

      // Feed the token to the state machine
      _currentToken = token;

      if (!_state.MoveNext())
        throw new Exception("TODO: I dont' know what to do here yet");

      return _state.Current;
    }

    private IEnumerable<NextTokenResult> TryParse()
    {
      return WrapCompileErrors(TryParseImpl());
    }

    private IEnumerable<NextTokenResult> TryParseImpl()
    {
      var classDef = new ClassDefinition();

      // Expect the class keyword
      yield return (new Token[] {_currentToken}).ExpectKeyword("class", classDef);

      // Grab the name
      string className;
      yield return (new Token[] { _currentToken }).ExtractToken(TokenType.Word, classDef, out className);
      classDef.Name = className;

      // Expect an opening curly bracket
      yield return (new Token[] { _currentToken }).ExpectSymbol("{", classDef);

      // TODO: Parse the insides of the class
      var rules = new RuleSet()
        .AddRule(new FunctionParseRule());

      // For now, just expect it to be empty
      yield return (new Token[] { _currentToken }).ExpectSymbol("}", classDef);
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
