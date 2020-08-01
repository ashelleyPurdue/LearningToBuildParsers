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

    public IEnumerable<Token> ToTokens(IEnumerable<char> src)
    {
      var walker = new StringWalker(src);
      var rules = new ILexerRule[]
      {
        new NumberRule(),
        new KeywordRule(_keywords),
        new WordRule(),
        new StringRule(),
        new SingleCharSymbolRule()
      };

      while (!walker.IsEmpty())
      {
        char c = walker.Peek();

        // Skip all whitespace
        if (char.IsWhiteSpace(c))
        {
          walker.Consume(1);
          continue;
        }

        // Get the standard rules out of the way first.
        bool handledByStandardRule = false;
        foreach (var rule in rules)
        {
          if (rule.IsStartOfToken(walker))
          {
            handledByStandardRule = true;
            yield return rule.ConsumeToken(walker);
            break;
          }
        }

        if (handledByStandardRule)
          continue;

        // TODO: multi-character symbol tokens
        // TODO: Special cases go here

        // We didn't find any rule that matches what we're seeing,
        // so throw an error.
        CodePos pos = walker.Position;
        throw new CompileErrorException(pos, $"Unexpected character '{c}'");
      }
    }

  }
}
