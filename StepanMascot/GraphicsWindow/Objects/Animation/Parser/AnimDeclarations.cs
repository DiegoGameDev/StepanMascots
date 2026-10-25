using StepanMascot.GraphicsWindow.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepanMascot.Animation.Parser
{
    internal record StateDeclaration(string state, string? clip);

    internal record TransitionDeclaration(string state, string targetState, TransitionConditionsDeclarations[]? ConditionsDeclarations);
    internal record TransitionConditionsDeclarations(string parameterName, object expectedValue, ConditionMode ConditionMode);

    internal record ParametersDeclaration(string name, AnimatorParamaterType ParamaterType, object value);
}
