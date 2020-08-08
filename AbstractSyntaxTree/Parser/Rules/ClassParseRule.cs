using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class ClassParseRule
  {
    public IEnumerable<NextTokenResult> TryParse(IEnumerable<Token> tokens)
    {
      return WrapCompileErrors(TryParseImpl(tokens));
    }

    private IEnumerable<NextTokenResult> TryParseImpl(IEnumerable<Token> tokensFoo)
    {
      var tokens = new TokenWalker(tokensFoo);
      var classDef = new ClassDefinition();

      // Look for the class keyword
      tokens = tokens.ConsumeKeyword("class");
      yield return NextTokenResult.GoodSoFar(classDef);

      // Grab the name
      classDef.Name = tokens
        .Expect(TokenType.Word)
        .Content;

      tokens = tokens.Consume();
      yield return NextTokenResult.GoodSoFar(classDef);

      // Parse the insides of the class
      classDef.Functions = new List<FunctionDefinition>();
      tokens = tokens.ConsumeSymbol("{");

      while (!tokens.IsEmpty())
      {
        Token token = tokens.Peek();


        yield return NextTokenResult.Fail(classDef, token.Position, $"Unexpected token {token.Content}");
        yield break;
      }
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
