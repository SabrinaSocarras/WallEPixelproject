using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallEPixelproject
{
    public enum TokenType
    {
       //comandos
        Spawn,
        Color,
        Size,
        DrawLine,
        DrawCircle,
        DrawRectangle,
        Fill,


        //funciones
        GetActualX,
        GetActualY,
        GetCanvasSize, 
        IsBrushSize,
        IsBrushColor,
        GetColorCount,
        IsCanvasColor,

        //asignacion 
        Assign,

        //Saltos y etiquetas
        GoTo,
        Identifier,

        //Literales 
        IntegerLiteral,
        StringLiteral,

        //Operadores
        Plus,
        Minus,
        Multiply,
        Divide,
        Power,
        Modulo,

        //comparadores logicos 
        EqualsEquals,   // ==
        GreaterThan,    // >
        LessThan,       // <
        GreaterEquals,   // >=
        LessEquals,      // <=
        NotEquals,      // !=
        And,   // &&
        Or,    // ||

       // puntuacion 
        LeftParen,   // (
        RightParen,  // )
        LeftBracket, // [
        RightBracket,// ]
        Comma,       // ,
        Semicolon,   // ;
        Dot,         // .
     
        // final de la linea 
        EOF,

        Unknown, 






    }
}
