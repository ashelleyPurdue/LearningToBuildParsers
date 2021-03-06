﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree.Parser
{
  public static partial class RuleCoroutines
  {
    public static IEnumerable<RuleResult> ParseClass(CurrentTokenCallback t)
    {
      var node = new ClassDefinition();
      node.Functions = new List<FunctionDefinition>();

      yield return Expect.Keyword(t(), "class");
      yield return Extract.Word(t(), name => node.Name = name);
      yield return Expect.Symbol(t(), "{");

      var rules = new WhileParser(t => !t.IsSymbol("}"))
        .Or<FunctionDefinition>(ParseFunction, f => node.Functions.Add(f))
        .YieldsNode(node);

      while (!rules.IsFinished)
        yield return rules.FeedToken(t());
    }
  }
}
