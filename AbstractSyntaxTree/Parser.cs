using System;
using System.Linq;
using System.Collections.Generic;

namespace AbstractSyntaxTree
{
  public class Parser
  {
    private readonly ISet<string> _keywords = new HashSet<string>
    {
      "class"
    };

    public AstRoot Parse(IEnumerable<char> src)
    {
      var lexer = new Lexer(_keywords);
      var tokens = new TokenWalker(lexer.ToTokens(src));

      var root = new AstRoot();

      // Parse all the classes
      root.Classes = new List<ClassDefinition>();

      while (!tokens.IsEmpty())
      {
        switch (tokens.Peek())
        {
          case KeywordToken k when k.Content == "class":
            root.Classes.Add(ParseClass(tokens, out tokens));
            break;

          default: throw new Exception("Unexpected token");
        }
      }
      
      return root;
    }

    private ClassDefinition ParseClass(TokenWalker tokens, out TokenWalker rest)
    {
      // Look for the class keyword
      tokens = tokens.ConsumeKeyword("class");
      var classDef = new ClassDefinition();

      // Grab the name
      classDef.Name = tokens
        .Expect<WordToken>()
        .Content;

      tokens = tokens.Consume();

      // TODO: Parse the functions.  For now, just enforce that
      // it's an empty class
      tokens = tokens
        .Consume<OpenCurlyToken>()
        .Consume<CloseCurlyToken>();

      classDef.Functions = new List<FunctionDefinition>();

      rest = tokens;
      return classDef;
    }
  }
}