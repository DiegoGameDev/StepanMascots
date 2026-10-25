using Newtonsoft.Json;
using StepanMascot.Animation.Parser;

namespace StepanMascot.GraphicsWindow.Objects
{
    public class Animator
    {
        public readonly Dictionary<string, AnimatorState> states = new();
        public readonly Dictionary<string, AnimatorParameter> parameters = new();
        public AnimatorState currentState { get; internal set; }

        private readonly Lexer lexer;
        private readonly Parser parser;

        private readonly AnimControllerDTO animControllerDTO;

        public Animator()
        {
            string source = """
                States:
                init,
                any_state,
                sleeping.has(sleep),
                breathing.has(breathe),
                attackMonitor.has(attack),
                exit *

                Parameters:
                idle.type(boolean).value(true),
                sleepFactor.type(float).value(f#0),
                inactiveTime.type(float).value(f#0),
                isTired.type(boolean).value(false) *

                Transitions:
                init.addTransition(breathing);
                any_state.addTransition(sleep).animParameter(inactiveTime, f#99, greater)(isTired, bool, true, equals); *

                """;

            lexer = new Lexer(source);
            parser = new Parser(lexer);
            animControllerDTO = new AnimControllerDTO(lexer, parser);

            states = animControllerDTO.StatesMachineAnimator();

            parameters = animControllerDTO.ParametersInAnimator();

            INIT();
        }

        private void INIT()
        {
            var transInStates = animControllerDTO.TransitionsStateMachineAnimator();

            foreach (var i in transInStates) // 
            {
                states[i.Key].transitions.Add(i.Value);
            }

            foreach (var state in states)
            {
                string clip = state.Value.clip != null ? state.Value.clip.name : "nenhum";
                Console.WriteLine($"state: {state.Key}. Clip: {clip}");
            }
        }

        public void Update(float deltaTime, Mascot mascot)
        {
            //foreach (var transition in currentState.transitions)
            //{
            //    if (CheckTransitionState(transition))
            //    {
            //        Console.Write(153);
            //        break;
            //    }
            //}
        }

        private bool CheckTransitionState(AnimatorTransition transition)
        {
            int okValues = 0; 
            foreach (var condition in transition.conditions)
            {
                //bool
                if (condition.expectedValue.GetType() == typeof(bool))
                {
                    var parameter = parameters[condition.parameterName];

                    switch (condition.conditionMode)  //bool only equals or not_equals condition mode
                    {
                        case ConditionMode.EQUALS:
                            if (parameter.Value == condition.expectedValue)
                            {
                                okValues++;
                            }
                            break;
                        case ConditionMode.NOT_EQUALS:
                            if (parameter.Value != condition.expectedValue)
                            {
                                okValues++;
                            }
                            break;
                    }

                    continue;
                }
                //int
                if (condition.expectedValue.GetType() == typeof(int))
                {
                    var parameter = parameters[condition.parameterName];
                    switch (condition.conditionMode)
                    {
                        
                        case ConditionMode.EQUALS:
                            okValues = (int)condition.expectedValue == (int)parameter.Value ? okValues + 1 : okValues;
                            break;
                        case ConditionMode.NOT_EQUALS:
                            okValues = (int)condition.expectedValue != (int)parameter.Value ? okValues + 1 : okValues;
                            break;
                        case ConditionMode.GREATER:
                            okValues = (int)condition.expectedValue < (int)parameter.Value ? okValues + 1 : okValues;
                            break;
                        case ConditionMode.LESS:
                            okValues = (int)condition.expectedValue > (int)parameter.Value ? okValues + 1 : okValues;
                            break;
                    }
                    continue;
                }
            }

            if (okValues == transition.conditions.Count)
            {
                return true;
            }
            return false;
        }


        #region API
        public void SetBool(string name, bool value)
        {
            parameters[name].Value = value;
        }

        public void SetFloat(string name, float value)
        {
            parameters[name].Value = value;
        }

        public void SetTrigger(string name)
        {
            parameters[name].Value = true;
        }
        #endregion
    }

    public class AnimatorState
    {
        public string name;
        public AnimationClip? clip;
        public List<AnimatorTransition> transitions = new();
    }
}
