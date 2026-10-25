using StepanMascot.GraphicsWindow.Objects;
using System.Runtime.InteropServices;

namespace StepanMascot.Animation.Parser
{
    internal class Parser
    {
        private readonly Lexer _lexer;
        private Token _current;

        private readonly Dictionary<string, Type> getTypeByName = new()
        {
            ["float"] = typeof(float),
            ["int"] = typeof(int),
            ["boolean"] = typeof(bool),
        };

        private readonly Dictionary<string, ConditionMode> getConditionByName = new()
        {
            ["greater"] = ConditionMode.GREATER,
            ["equals"] = ConditionMode.EQUALS,
        };

        public Parser(Lexer lexer)
        {
            _lexer = lexer;

            Advance();
        }

        private void Advance()
        {
            _current = _lexer.NextToken();
        }

        private Token Expect(TokenType type)
        {
            if (_current.TokenType != type)
            {
                throw new Exception($"Esperado {type}, " +
                    $"mas encontrado{_current.TokenType}, " +
                    $"na posição {_current.position}");
            }

            Token consumed = _current;

            Advance();

            return consumed;
        }

        private Token ExpectIdentifier(string expectedText)
        {
            Token token = Expect(TokenType.IDENTIFIER);

            if (token.text != expectedText)
            {
                throw new Exception($"Esperado {expectedText}, " +
                    $"mas encontrado{token.text}, " +
                    $"na posição {token.position}");
            }

            return token;
        }

        public List<StateDeclaration> ParserStatesBlock()
        {
            ExpectIdentifier("States");
            Expect(TokenType.COLON);

            List<StateDeclaration> stateDeclarations = new();

            while (_current.TokenType != TokenType.STAR)
            {
                StateDeclaration state = ParseStateDeclaration();
                stateDeclarations.Add(state);

                if (_current.TokenType == TokenType.COMMA)
                {
                    Advance();
                }
            }

            Expect(TokenType.STAR);

            return stateDeclarations;
        }

        public List<TransitionDeclaration> ParserTransitionsBlock()
        {
            ExpectIdentifier("Transitions");
            Expect(TokenType.COLON);

            List<TransitionDeclaration> transitionDeclarations = new();

            while (_current.TokenType != TokenType.STAR)
            {
                TransitionDeclaration transition = ParseTransitionDeclaration();
                transitionDeclarations.Add(transition);

                Expect(TokenType.SEMI_COLON);
            }

            Expect(TokenType.STAR);

            return transitionDeclarations;
        }

        public List<ParametersDeclaration> ParseParamatersBlock()
        {
            List<ParametersDeclaration> parametersDeclarations = new();

            while (_current.TokenType != TokenType.STAR)
            {
                ParametersDeclaration parametersDeclaration = ParseParameterDeclaration();
                parametersDeclarations.Add(parametersDeclaration);

                if (_current.TokenType == TokenType.COMMA)
                {
                    Advance();
                }
            }

            Expect(TokenType.STAR);

            return parametersDeclarations;
        }

        private StateDeclaration ParseStateDeclaration()
        {
            var identifier = Expect(TokenType.IDENTIFIER);

            string? clipName = null;

            if (_current.TokenType == TokenType.DOT)
            {
                Advance();

                ExpectIdentifier("has");
                Expect(TokenType.OPEN_PARENTHESIS);

                Token? clip = Expect(TokenType.IDENTIFIER);
                clipName = clip.text;

                Expect(TokenType.CLOSE_PARENTHESIS);
            }

            return new StateDeclaration(identifier.text, clipName);
        }

        private TransitionDeclaration ParseTransitionDeclaration()
        {
            Token stateIdentifier = Expect(TokenType.IDENTIFIER);
            Token targetState;

            if (_current.TokenType == TokenType.DOT) //get target state
            {
                Advance();
                Token token = ExpectIdentifier("addTransition");

                Expect(TokenType.OPEN_PARENTHESIS);
                targetState = Expect(TokenType.IDENTIFIER);
                Expect(TokenType.CLOSE_PARENTHESIS);
            }
            else
                throw new Exception($"Esperado um (.addTransition) após {stateIdentifier.text}");

            if (_current.TokenType == TokenType.DOT) //if has a operation
            {
                Advance();

                string[] operations = { "animParameter"};

                Token operationIdentifier = Expect(TokenType.IDENTIFIER);

                //(string state, string targetState, string? parameterName, object? expectedValue, ConditionMode? ConditionMode);

                List<TransitionConditionsDeclarations> conditionsList = new();
                switch (operationIdentifier.text)
                {
                    case "animParameter": //(parameter name, type,value, conditionMode)
                        while (ContainParameters(out TransitionConditionsDeclarations conditionDeclaration))
                        {
                            conditionsList.Add(conditionDeclaration);
                        }

                        return new TransitionDeclaration(stateIdentifier.text, targetState.text, conditionsList.ToArray());
                }
            }

            return new TransitionDeclaration (stateIdentifier.text, targetState.text, null);
        }
        private bool ContainParameters(out TransitionConditionsDeclarations conditionsDeclarations)
        {
            if (_current.TokenType == TokenType.OPEN_PARENTHESIS)
            {
                Expect(TokenType.OPEN_PARENTHESIS);
                Token paramater = Expect(TokenType.IDENTIFIER);
                Expect(TokenType.COMMA);
                Token value = Expect(TokenType.IDENTIFIER);
                Expect(TokenType.COMMA);
                Token conditionMode = Expect(TokenType.IDENTIFIER);
                Expect(TokenType.CLOSE_PARENTHESIS);

                conditionsDeclarations = new TransitionConditionsDeclarations(paramater.text, value.text, getConditionByName[conditionMode.text]);

                return true;
            }

            conditionsDeclarations = null;
            return false;
        }

        private ParametersDeclaration ParseParameterDeclaration()
        {
            if (_current.TokenType != TokenType.IDENTIFIER || _current.text != "Parameters")
            {
                Advance();
                if (_current.TokenType == TokenType.END_OF_FILLE)
                    throw new Exception("Parameters está invalido");
            }

            ExpectIdentifier("Parameters");
            Expect(TokenType.COLON);

            Token parameterName = Expect(TokenType.IDENTIFIER);
            Expect(TokenType.DOT);
            ExpectIdentifier("value");

            Expect(TokenType.OPEN_PARENTHESIS);
            Token typeToken = Expect(TokenType.IDENTIFIER);
            Expect(TokenType.CLOSE_PARENTHESIS);
            Expect(TokenType.DOT);

            AnimatorParamaterType type = typeToken.text == "boolean" ? AnimatorParamaterType.BOOLEAN : typeToken.text == "float" ? AnimatorParamaterType.FLOAT :
                typeToken.text == "int" ? AnimatorParamaterType.INT : AnimatorParamaterType.TRIGGER;

            Expect(TokenType.OPEN_PARENTHESIS);
            Token value = Expect(TokenType.IDENTIFIER);
            Expect(TokenType.CLOSE_PARENTHESIS);

            return new ParametersDeclaration(parameterName.text, type, TextToCObject(value.text));
        }


        //OBJECTS TYPE SOLUTIONER

        private object TextToCObject(string text)
        {
            if (text == "false" || text == "true")
            {
                return Convert.ToBoolean(text);
            }

            if (text[0] == 'f' && text[1] == '#') //float
            {
                int length = text.Length - 1;
                string numbers = text[2..length];
                return Convert.ToSingle(numbers);
            }
            if (text[0] == 'i' && text[1] == '#') //int
            {
                int length = text.Length - 1;
                string numbers = text[2..length];
                return Convert.ToInt32(numbers);
            }

            throw new Exception("Valor utilizado é invalido. Não se encaixa nos objetos padrão (bool, float, int, trigger)");
        }
    }
}
