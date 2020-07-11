using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
  public class Lexer
  {
    private readonly ISet<string> _keywords;

    public Lexer(ISet<string> keywords = null)
    {
      _keywords = keywords ?? new HashSet<string>();
    }

    public IEnumerable<IToken> ToTokens(IEnumerable<char> src)
    {
      var walker = new StringWalker(src);
      while (!walker.IsEmpty())
      {
        char c = walker.Peek();

        // Skip all whitespace
        if (char.IsWhiteSpace(c))
        {
          walker.Consume(1);
          continue;
        }

        // Detect the start of multi-character tokens

        // WordTokens must start with a letter.
        // The rest of the chars can be either a letter or a digit.
        // If this word is on the list of keywords, then it will be
        // a KeywordToken.
        if (char.IsLetter(c))
        {
          CodePos p = walker.Position;
          string word = walker.ConsumeWhile(cc => char.IsLetterOrDigit(cc));

          if (_keywords.Contains(word))
            yield return new KeywordToken(p, word);
          else
            yield return new WordToken(p, word);

          continue;
        }

        // TODO: number tokens
        // TODO: string tokens

        // This must be a single-character token
        CodePos pos = walker.Position;
        walker.Consume(1);
        yield return c switch
        {
          '{' => new OpenCurlyToken(pos),
          '}' => new CloseCurlyToken(pos),
          _ => throw new CompileErrorException(pos, $"Unexpected character '{c}'")
        };
      }
    }
  }
}
