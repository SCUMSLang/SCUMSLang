﻿using System.Collections.Generic;
using System.Linq;

namespace SCUMSLang.AST
{
    public static class ConstantDefinitionExtensions
    {
        public static List<ParameterDefinition> ToParameterDefinitionList(this IEnumerable<ConstantDefinition> constants) =>
            constants.Select(x => new ParameterDefinition(x.ValueType.DeclaringType ?? x.ValueType)).ToList();
    }
}
