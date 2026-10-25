using System;
using System.Collections.Generic;
using System.Text;

namespace StepanMascot.Animation.Parser
{
    internal record Token(TokenType TokenType, string text, int position);

    internal enum TokenType
    {
        IDENTIFIER,
        DOT,
        QUOTES,
        OPEN_PARENTHESIS,
        CLOSE_PARENTHESIS,
        COMMA,
        COLON,
        SEMI_COLON,
        STAR,
        END_OF_FILLE
    }
}
