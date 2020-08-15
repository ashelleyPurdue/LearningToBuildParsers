using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AbstractSyntaxTree
{
  public class ClassParseRule : BaseParseRule
  {
    protected override IEnumerable<NextTokenResult> TryParse()
    {
      var classDef = new ClassDefinition();

      // Expect the class keyword
      yield return ExpectKeyword("class", classDef);

      // Grab the name
      string className;
      yield return ExtractToken(TokenType.Word, classDef, out className);
      classDef.Name = className;

      // Expect an opening curly bracket
      yield return ExpectSymbol("{", classDef);

      // TODO: Parse the insides of the class
      var rules = new RuleSet()
        .AddRule(new FunctionParseRule());

      // For now, just expect it to be empty
      var end = ExpectSymbol("}", classDef);
      end.state = RuleMatchState.Complete;
      yield return end;
    }
  }
}
