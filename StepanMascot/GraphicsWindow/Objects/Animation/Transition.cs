using System;
using System.Collections.Generic;
using System.Text;

namespace StepanMascot.GraphicsWindow.Objects
{

    public class AnimatorTransition
    {
        public string targetState;
        public List<TransitionCondition> conditions = new();
    }

    public class TransitionCondition
    {
        public string parameterName { get;  set; }
        public object expectedValue;
        public ConditionMode conditionMode;
    }
    public class AnimatorParameter
    {
        public string parameterName;
        public AnimatorParamaterType paramaterType;
        public object Value;
    }

    public enum AnimatorParamaterType
    {
        INT,
        BOOLEAN,
        FLOAT,
        TRIGGER,
    }

    public enum ConditionMode
    {
        EQUALS,
        NOT_EQUALS,
        GREATER,
        LESS,
        TRIGGERED
    }
}
