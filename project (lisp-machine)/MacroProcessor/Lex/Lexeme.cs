using System;
using System.Collections.Generic;

namespace LispMachine
{
    public class Lexeme
    {
        public LexemeType Type { get; }
        public string Text { get; }

        public Lexeme(LexemeType type)
        {
            Type = type;
        }

        public Lexeme(LexemeType type, String text)
        {
            Type = type;
            Text = text;
        }

    }
}
