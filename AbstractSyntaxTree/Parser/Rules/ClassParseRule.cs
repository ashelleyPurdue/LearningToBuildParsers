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

      yield return ExpectKeyword("class", classDef);
      yield return ExtractWord(classDef, n => classDef.Name = n);
      yield return ExpectSymbol("{", classDef);

      // TODO: Parse the insides of the class
      var rules = new RuleSet()
        .AddRule(new FunctionParseRule());

      // For now, just expect it to be empty
      yield return FinishWithSymbol("}", classDef);
    }
  }
}
