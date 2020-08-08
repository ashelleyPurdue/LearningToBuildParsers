using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AbstractSyntaxTree
{
  public class ClassParseRule
  {
    public IEnumerable<NextTokenResult> TryParse(IEnumerable<Token> tokens)
    {
      return WrapCompileErrors(TryParseImpl(tokens));
    }

    private IEnumerable<NextTokenResult> TryParseImpl(IEnumerable<Token> tokens)
    {
      var classDef = new ClassDefinition();

      // Expect the class keyword
      yield return tokens.ExpectKeyword("class", classDef);
      tokens = tokens.Skip(1);

      // Grab the name
      string className;
      yield return tokens.ExtractToken(TokenType.Word, classDef, out className);
      tokens = tokens.Skip(1);

      classDef.Name = className;

      // Expect an opening curly bracket
      yield return tokens.ExpectSymbol("{", classDef);
      tokens = tokens.Skip(1);

      // TODO: Parse the insides of the class.
      // For now, just expect it to be empty
      yield return tokens.ExpectSymbol("}", classDef);
      tokens = tokens.Skip(1);
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
