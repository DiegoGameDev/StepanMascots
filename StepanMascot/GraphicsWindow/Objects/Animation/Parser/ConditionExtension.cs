using StepanMascot.Animation.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepanMascot.GraphicsWindow.Objects.Animation.Parser
{
    public static class ConditionExtension
    {
        internal static TransitionCondition ToTransitionCondition(this TransitionConditionsDeclarations conditionsDeclarations)
        {
            return new TransitionCondition()
            {
                parameterName = conditionsDeclarations.parameterName,
                expectedValue = conditionsDeclarations.expectedValue,
                conditionMode = conditionsDeclarations.ConditionMode
            };
        }
    }
}
