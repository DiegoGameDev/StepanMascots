using StepanMascot.GraphicsWindow.Objects;
using StepanMascot.GraphicsWindow.Objects.Animation.Parser;
using System.Runtime.InteropServices;
using System.Text;

namespace StepanMascot.Animation.Parser
{
    internal class AnimControllerDTO
    {
        readonly Lexer _lexer;
        readonly Parser _parser;

        private readonly string clipsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanMascots", "Clips");

        public AnimControllerDTO(Lexer lexer, Parser parser)
        {
            _lexer = lexer;
            _parser = parser;
        }

        public Dictionary<string, AnimatorState> StatesMachineAnimator()
        {
            var statesDeclarations = _parser.ParserStatesBlock();

            Dictionary<string, AnimatorState> animatorStates = new();

            foreach (var state in statesDeclarations)
            {
                var aState = new AnimatorState()
                {
                    name = state.state,
                };

                aState.clip = GetAnimationClipByName(state.clip);

                animatorStates.Add(aState.name, aState);
            }

            return animatorStates;
        }

        public Dictionary<string, AnimatorTransition> TransitionsStateMachineAnimator()
        {
            var transitionsDeclarations = _parser.ParserTransitionsBlock();

            Dictionary<string, AnimatorTransition> animatorTransitions = new();

            foreach (var transition in transitionsDeclarations)
            {
                AnimatorTransition animatorTransition = new()
                {
                    targetState = transition.targetState,
                };

                if (transition.ConditionsDeclarations != null)
                {
                    IEnumerable<TransitionCondition> transitionConditions = transition.ConditionsDeclarations.Select(x => x.ToTransitionCondition());

                    animatorTransition.conditions.AddRange(transitionConditions);


                    // remover este debug
                    StringBuilder @string = new();

                    foreach (var i in transitionConditions)
                    {
                        @string.Append(i.parameterName).Append(i.expectedValue).Append(i.conditionMode.ToString()).Append(Environment.NewLine);
                    }

                    Console.Write(@string.ToString());
                }

                animatorTransitions.Add(transition.state, animatorTransition);

            }

            return animatorTransitions;
        }

        public Dictionary<string, AnimatorParameter> ParametersInAnimator()
        {
            Dictionary<string, AnimatorParameter> parameters = new();

            var declarations = _parser.ParseParamatersBlock();

            foreach (var declaration in declarations)
            {
                var param = new AnimatorParameter()
                {
                    parameterName = declaration.name,
                    Value = declaration.value,
                    paramaterType = declaration.ParamaterType
                };

                parameters.Add(param.parameterName, param);
            }

            return parameters;
        }

        public AnimationClip GetAnimationClipByName(string? name)
        {
            if (name == null) return null ;

            string file = string.Concat(name, ".STPCLIP");

            if (File.Exists(Path.Combine(clipsPath, file)))
            {
                return new AnimationClip()
                {
                    name = name
                };

                //read clip file content
            }


            throw new Exception($"Clip de animação não existe. Clip: {file}");
        }
    }
}
