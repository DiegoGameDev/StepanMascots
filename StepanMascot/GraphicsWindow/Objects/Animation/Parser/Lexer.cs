using System;
using System.Collections.Generic;
using System.Text;

namespace StepanMascot.Animation.Parser
{
    internal class Lexer
    {
        private readonly string Source;
        private int _position = 0;

        public Lexer(string source)
        {
            Source = source;
        }

        public Token NextToken()
        {
            SkipWhiteSpace();

            if (_position >= Source.Length)
            {
                return new Token(TokenType.END_OF_FILLE,
                    "",
                    _position);
            }
                

            char current = Source[_position];

            if (char.IsLetter(current) || current == '_')
            {
                return ReadIdentifier();
            }

            _position++;

            return current switch
            {
                ':' => new Token(TokenType.COLON, ":", _position - 1),
                ';' => new Token(TokenType.SEMI_COLON, ",", _position - 1),
                '.' => new Token(TokenType.DOT, ".", _position - 1),
                '(' => new Token(TokenType.OPEN_PARENTHESIS, "(", _position - 1),
                ')' => new Token(TokenType.CLOSE_PARENTHESIS, ")", _position - 1),
                ',' => new Token(TokenType.COMMA, ",", _position - 1),
                '*' => new Token(TokenType.STAR, "*", _position - 1),
                '"' => new Token(TokenType.STAR, '"'.ToString(), _position - 1),

                _ => throw new Exception(
                    $"Caractere inesperado '{current}' " +
                    $"na posição {_position - 1}."
                )
            };
        }

        private Token ReadIdentifier()
        {
            int start = _position;

            while (_position < Source.Length && (char.IsLetterOrDigit(Source[_position]) || Source[_position] == '_'))
            {
                _position++;
            }

            return new Token(TokenType.IDENTIFIER, Source[start.._position], start);
        }

        private void SkipWhiteSpace()
        {
            while (_position < Source.Length && char.IsWhiteSpace(Source[_position]))
            {
                _position++;
            }
        }

        private void SkipComments()
        {
            int position = _position;

            if (Source[_position] == '<')
                while (_position < Source.Length && Source[_position - 1] != '>')
                {
                    _position++;
                }

            if (_position >= Source.Length)
            {
                throw new Exception($"Comentário na posição{position} não tem uma conclusão (>)");
            }
        }
    }
}
