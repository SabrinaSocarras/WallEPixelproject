﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallEPixelproject
{
    public  class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public object Literal { get; }
        public int Line { get; }
       

        public Token (TokenType type, string lexeme, object literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;

        }

        public override string ToString()
        {
            return $"Type: {Type}, Lexeme: '{Lexeme}', Literal: {Literal}, Line: {Line} ";
        }










    }
}
