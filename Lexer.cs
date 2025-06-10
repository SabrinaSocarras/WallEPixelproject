using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WallEPixelproject.Interfaces;

namespace WallEPixelproject
{
    public class Lexer
    {

        public readonly string source;

        public readonly List<Token> tokens = new List<Token>();

       private readonly ILogger logger;

        public int start = 0;
        public int current = 0;
        public int line = 1;


        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>()
            {
            {"Spawn", TokenType.Spawn },
            { "Color", TokenType.Color },
            { "Size", TokenType.Size },
            { "DrawLine", TokenType.DrawLine },
            { "DrawCircle", TokenType.DrawCircle },
            { "DrawRectangle", TokenType.DrawRectangle },
            { "Fill", TokenType.Fill },
            { "GoTo", TokenType.GoTo },
            { "GetActualX", TokenType.GetActualX },
            { "GetActualY", TokenType.GetActualY },
            { "GetCanvasSize", TokenType.GetCanvasSize },
            { "GetColorCount", TokenType.GetColorCount },
            { "IsBrushColor", TokenType.IsBrushColor },
            { "IsBrushSize", TokenType.IsBrushSize },
            { "IsCanvasColor", TokenType.IsCanvasColor },
            };

      //  public Lexer(string source, ILogger logger = null)
       // {
        //    this.logger = logger;
         //   this.source = source;

        //}

        public bool IsAtEnd()
        {
            return (current >= source.Length);
        }

        public List<Token> ScanTokens()
        {
            logger.Debug("Scanner","Se inicio el escaneo de tokens",line );
            tokens.Clear();
            start = 0;
            current = 0;
            line = 1;

            while (!IsAtEnd())
            {
                start = current; 
                ScanNextToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            logger.Info("Scanner", $"Escaneo completado. Total de tokens: {tokens.Count}");
            return tokens;

        }

        private void ScanNextToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LeftParen); break;
                case ')': AddToken(TokenType.RightParen); break;
                case '[': AddToken(TokenType.LeftBracket); break;
                case ']': AddToken(TokenType.RightBracket); break;
                case ',': AddToken(TokenType.Comma); break;
                case '+': AddToken(TokenType.Plus); break;
                case '%': AddToken(TokenType.Modulo); break;
                case '-': AddToken(TokenType.Minus); break; // Parser decide unario/binario
                case '<':
                    if (Match('-')) AddToken(TokenType.Assign);
                    else if (Match('=')) AddToken(TokenType.LessEquals);
                    else AddToken(TokenType.LessThan);
                    break;
                case '=':
                    if (Match('=')) AddToken(TokenType.EqualsEquals);
                    else ReportLexerError($"Se esperaba '=' para formar '=='.", start, 1);
                    break;
                case '>':
                    if (Match('=')) AddToken(TokenType.GreaterEquals);
                    else AddToken(TokenType.GreaterThan);
                    break;
                case '*':
                    if (Match('*')) AddToken(TokenType.Power);
                    else AddToken(TokenType.Multiply);
                    break;
                case '/':
                    if (Match('/')) { while (Peek() != '\n' && !IsAtEnd()) Advance(); }
                    else AddToken(TokenType.Divide);
                    break;
                case '&':
                    if (Match('&')) AddToken(TokenType.And);
                    else ReportLexerError($"Se esperaba '&' para formar '&&'.", start, 1);
                    break;
                case '|':
                    if (Match('|')) AddToken(TokenType.Or);
                    else ReportLexerError($"Se esperaba '|' para formar '||'.", start, 1);
                    break;
                case '"': HandleStringLiteral(); break;
                case ' ': case '\r': case '\t': break; // Ignorar whitespace
                case '\n':
                  //  AddToken(TokenType.NewLine);
                    line++;
                   // _currentColumn = 1; // Resetear columna para la nueva línea
                    break;
                default:
                    if (IsDigit(c)) HandleNumberLiteral();
                    else if (IsAlphaForIdentifierStart(c)) HandleIdentifierOrKeyword();
                    else ReportLexerError($"Carácter inesperado '{c}'.", current- 1, 1); // -1 porque ya avanzó
                    break;
            }

        }
        private char Advance()
        {
            char currentChar = source[current];
            current++;

            return currentChar;
        }

        private void AddToken(TokenType type, object literal = null)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
        private char Peek()
        {
            return IsAtEnd() ? '\0' : source[current];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd() || source[current] != expected) return false;
            current++;

            return true;
        }

        private void HandleStringLiteral()
        {
            int stringStartLine = line;


            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {

                    ReportLexerError("String literal multilínea no permitido o no terminado.", start, current - start, stringStartLine);

                    return;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                ReportLexerError("String literal no terminado.", start, current - start, stringStartLine);
                return;
            }

            Advance(); // Consumir la comilla de cierre '"'
            string value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.StringLiteral, value);
        }

        private void HandleNumberLiteral()
        {
            // _startOfLexeme está en el primer dígito.
            while (IsDigit(Peek())) Advance();

            string numberText = source.Substring(start, current - start);
            if (int.TryParse(numberText, out int value))
            {
                AddToken(TokenType.IntegerLiteral, value);
            }
            else
            {
                ReportLexerError($"Número inválido o demasiado grande: '{numberText}'.", start, numberText.Length);
            }
        }

        private void HandleIdentifierOrKeyword()
        {
            // _startOfLexeme está en el primer carácter válido.
            while (IsAlphaNumericDashOrUnderscoreForIdentifier(Peek())) Advance();

            string text = source.Substring(start, current - start);
            if (Keywords.TryGetValue(text, out TokenType keywordType))
            {
                AddToken(keywordType);
            }
            else
            {
                AddToken(TokenType.Identifier);
            }
        }
        private bool IsDigit(char c) => c >= '0' && c <= '9';

        private bool IsAlphaForIdentifierStart(char c) =>
         (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == 'ñ' || c == 'Ñ';

        private bool IsAlphaNumericDashOrUnderscoreForIdentifier(char c) =>
           IsAlphaForIdentifierStart(c) || IsDigit(c) || c == '-' || c == '_';


        private void ReportLexerError(string message, int errorStartIndex, int errorLength, int? errorLine = null, int? errorColumn = null)
        {
            int lineToReport = errorLine ?? line;
            // Si no se da errorColumn, tratar de calcularlo basado en _startOfLexeme
          //  int columnToReport = errorColumn ?? (column - (current - errorStartIndex));
         //   if (columnToReport < 1) columnToReport = 1;


            string offendingText = source.Substring(errorStartIndex, Math.Min(errorLength, source.Length - errorStartIndex));
            //logger.Error(LOG_PREFIX, $"{message} (Texto: '{offendingText}')", lineToReport);
            //tokens.Add(new Token(TokenType.Unknown, offendingText, message, lineToReport, columnToReport));
            // No actualizamos _currentColumn aquí para el token Unknown, ya que AddToken lo hará
            // o el siguiente ScanNextToken lo recalculará.
        }

        // Clase DummyLogger interna para evitar NullReferenceException si no se pasa logger
        private class DummyLogger : ILogger
        {
            public void Info(string prefix, string message, int? line = null) { }
            public void Debug(string prefix, string message, int? line = null) { }
            public void Warn(string prefix, string message, int? line = null) { }
            public void Error(string prefix, string message, int? line = null, Exception ex = null) { }
        }


    }
}