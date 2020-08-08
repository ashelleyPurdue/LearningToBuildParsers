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
      Token classKeyword = tokens.First();
      tokens = tokens.Skip(1);
      if (classKeyword.Content != "class")
      {
        string msg = $"Expected class keyword, but got {classKeyword.Content}";
        throw new CompileErrorException(classKeyword.Position, msg);
      }
      yield return NextTokenResult.GoodSoFar(classDef);

      // Grab the name
      Token classNameWord = tokens.First();
      tokens = tokens.Skip(1);

      if (classNameWord.Type != TokenType.Word)
      {
        string msg = $"Expected a word token for the name, but got a {classNameWord.Type} {classNameWord.Content}";
        throw new CompileErrorException(classNameWord.Position, msg);
      }
      classDef.Name = classNameWord.Content;
      yield return NextTokenResult.GoodSoFar(classDef);

      // Expect an opening curly bracket
      Token openCurly = tokens.First();
      if (openCurly.Content != "{")
      {
        string msg = $"Expected an open curly, but got a {classNameWord.Type} {classNameWord.Content}";
        throw new CompileErrorException(classNameWord.Position, msg);
      }
      tokens = tokens.Skip(1);
      yield return NextTokenResult.GoodSoFar(classDef);

      // TODO: Parse the insides of the class.
      // For now, just expect it to be empty
      Token closeCurly = tokens.First();
      if (closeCurly.Content != "}")
      {
        string msg = $"Expected an open curly, but got a {classNameWord.Type} {classNameWord.Content}";
        throw new CompileErrorException(classNameWord.Position, msg);
      }
      yield return NextTokenResult.GoodSoFar(classDef);
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
