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
            root.Classes.Add(ParseClass(tokens));
            break;

          default: throw new Exception("Unexpected token");
        }
      }
      
      return root;
    }

    private ClassDefinition ParseClass(TokenWalker tokens)
    {
      // Look for the class keyword
      tokens.ConsumeKeyword("class");

      // Grab the name
      var classDef = new ClassDefinition();
      classDef.Name = tokens
        .Consume<WordToken>()
        .Content;

      // TODO: Parse the functions.  For now, just enforce that
      // it's an empty class
      tokens.Consume<OpenCurlyToken>();
      tokens.Consume<CloseCurlyToken>();

      classDef.Functions = new List<FunctionDefinition>();

      return classDef;
    }
  }
}