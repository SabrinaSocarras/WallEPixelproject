using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEPixelproject;
using WallEPixelproject.AST;
using WallEPixelproject.Interfaces;

namespace WallEPixelproject
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;
        private readonly ILogger _logger;
        private const string LOG_PREFIX = "Parser";

        private readonly List<ParseError> _errors = new List<ParseError>();

        private bool _hasSpawnBeenParsed = false;


        public Parser(List<Token> tokens, ILogger logger = null)
        {

            _tokens = tokens.Where(t => t.Type != TokenType.Unknown).ToList(); // Ignorar tokens desconocidos del lexer
            _logger = logger ?? new DummyLogger();
        }

        public List<Statement> ParseProgram(out List<ParseError> errors)
        {
            _logger.Info(LOG_PREFIX, "Iniciando fase de parseo...");
            _errors.Clear();
            _current = 0;
            _hasSpawnBeenParsed = false;
            List<Statement> statements = new List<Statement>();

            errors = new List<ParseError>();

            ValidateSpawnAsFirstRule();
            if (_errors.Any())
            {
                errors = new List<ParseError>(_errors);
                return null;
            }

            _current = 0; 

            while (!IsAtEnd())
            {
                SkipWhitespace();
                if (IsAtEnd()) break;

                try
                {
                    var statement = ParseStatement();
                    if (statement != null)
                    {
                        statements.Add(statement);
                        SkipWhitespace();
                       
                    }
                }
                catch (ParseError error)
                {
                    _errors.Add(error);
                    Synchronize();
                }
            }
            errors = new List<ParseError>(_errors);
            _logger.Info(LOG_PREFIX, $"Parseo completado. {statements.Count} statements generados, {errors.Count} errores encontrados.");

            return _errors.Any() ? null : statements;
        }


        private void SkipWhitespace()
        {
            while (!IsAtEnd() &&
                  (Peek().Type == TokenType.Whitespace ||
                   Peek().Type == TokenType.NewLine ||
                   Peek().Type == TokenType.Semicolon))
            {
                Advance(); 
            }
        }

        private void ValidateSpawnAsFirstRule()
        {
            var firstRealToken = _tokens.FirstOrDefault(t =>
                t.Type != TokenType.NewLine &&
                t.Type != TokenType.Whitespace &&
                t.Type != TokenType.EOF);

            if (firstRealToken == null)
            {
                Error(null, "El programa está vacío");
                return;
            }

            if (firstRealToken.Type != TokenType.Spawn)
            {
                Error(firstRealToken, "El primer comando del programa debe ser Spawn()");
            }

            bool spawnFound = false;
            foreach (var token in _tokens)
            {
                if (token.Type == TokenType.Spawn)
                {
                    if (spawnFound)
                    {
                        Error(token, "El comando Spawn solo puede aparecer una vez al inicio del programa");
                    }
                    spawnFound = true;
                }
            }
        }


        // métodos de ayuda del parser 
        private Token Peek() => _tokens[_current];
        private Token Previous() => _tokens[_current - 1];
        private bool IsAtEnd() => Peek().Type == TokenType.EOF;
        private Token Advance() { if (!IsAtEnd()) _current++; return Previous(); }
        private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;
        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types) { if (Check(type)) { Advance(); return true; } }
            return false;
        }
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }
        private ParseError Error(Token token, string message)
        {
            var error = new ParseError(token, message); _errors.Add(error);
            _logger.Error(LOG_PREFIX, message, token.Line); return error;
        }
        private void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.NewLine) return;
                switch (Peek().Type) { case TokenType.Spawn: return; }
                Advance();
            }
        }

        private Statement ParseStatement()
        {
            SkipWhitespace();

            if (Check(TokenType.Label))
            {
                Token labelToken = Advance();
                return new LabelStmt(labelToken); 
            }

            if (Check(TokenType.Identifier) && LookAhead(1)?.Type == TokenType.Assign)
            {
                return ParseAssignment();
            }

            if (IsAtEnd()) return null;

            switch (Peek().Type)
            {
                case TokenType.Spawn:
                    if (_hasSpawnBeenParsed)
                        throw Error(Peek(), "El comando Spawn solo puede usarse una vez.");
                    _hasSpawnBeenParsed = true;
                    return ParseSpawnStatement();

                case TokenType.GoTo:
                    return ParseGotoStatement(); 

                case TokenType.Color:
                    return ParseColorStatement();

                case TokenType.Size:
                    return ParseSizeStatement();

                case TokenType.DrawLine:
                    return ParseDrawLineStatement();

                case TokenType.DrawCircle:
                    return ParseDrawCircleStatement();

                case TokenType.DrawRectangle:
                    return ParseDrawRectangleStatement();

                case TokenType.Fill:
                    return ParseFillStatement();

                default:
                    throw Error(Peek(), "Se esperaba un comando conocido (Spawn, GoTo, Color, etc.).");
            }
        }

        private AssignmentStmt ParseAssignment()
        {
            Token variableName = Consume(TokenType.Identifier, "Se esperaba nombre de variable");
            ValidateVariableName(variableName); 
            Consume(TokenType.Assign, "Se esperaba '<-' después del nombre de variable");
            Expression expr = ParseExpression();
            return new AssignmentStmt(variableName, expr);
        }


        private SpawnStmt ParseSpawnStatement()
        {
            Token keyword = Consume(TokenType.Spawn, "Se esperaba 'Spawn'.");
            Consume(TokenType.LeftParen, "Se esperaba '(' después de 'Spawn'.");
            Expression x = ParseExpression();
            Consume(TokenType.Comma, "Se esperaba ',' entre argumentos de Spawn.");
            Expression y = ParseExpression();
            Consume(TokenType.RightParen, "Se esperaba ')' después de argumentos de Spawn.");
            return new SpawnStmt(keyword, x, y);
        }

        private ColorStmt ParseColorStatement()
        {
            Token keyword = Consume(TokenType.Color, "Se esperaba 'Color'.");
            Consume(TokenType.LeftParen, "Se esperaba '(' después de 'Color'.");
            Expression colorName = ParseExpression();
            Consume(TokenType.RightParen, "Se esperaba ')' después de argumento de Color.");
            return new ColorStmt(keyword, colorName);
        }

        private SizeStmt ParseSizeStatement()
        {
            Token keyword = Consume(TokenType.Size, "Se esperaba 'Size'.");
            Consume(TokenType.LeftParen, "Se esperaba '(' después de 'Size'.");
            Expression sizeValue = ParseExpression();  
            Consume(TokenType.RightParen, "Se esperaba ')' después de argumento de Size.");
            return new SizeStmt(keyword, sizeValue);
        }

        private DrawLineStmt ParseDrawLineStatement()
        {
            Token keyword = Consume(TokenType.DrawLine, "Se esperaba 'DrawLine'.");
            Consume(TokenType.LeftParen, "Se esperaba '('.");
            Expression dirX = ParseExpression(); Consume(TokenType.Comma, "Se esperaba ','.");
            Expression dirY = ParseExpression(); Consume(TokenType.Comma, "Se esperaba ','.");
            Expression distance = ParseExpression();
            Consume(TokenType.RightParen, "Se esperaba ')'.");
            return new DrawLineStmt(keyword, dirX, dirY, distance);
        }

        private DrawCircleStmt ParseDrawCircleStatement()
        {
            Token keyword = Consume(TokenType.DrawCircle, "Se esperaba 'DrawCircle'.");
            Consume(TokenType.LeftParen, "Se esperaba '('.");
            Expression dirX = ParseExpression(); Consume(TokenType.Comma, "Se esperaba ','.");
            Expression dirY = ParseExpression(); Consume(TokenType.Comma, "Se esperaba ','.");
            Expression radius = ParseExpression();
            Consume(TokenType.RightParen, "Se esperaba ')'.");
            return new DrawCircleStmt(keyword, dirX, dirY, radius);
        }

        private DrawRectangleStmt ParseDrawRectangleStatement()
        {
            Token keyword = Consume(TokenType.DrawRectangle, "Se esperaba 'DrawRectangle'.");
            Consume(TokenType.LeftParen, "Se esperaba '('.");
            Expression dirX = ParseExpression(); Consume(TokenType.Comma, "Se esperaba ','.");
            Expression dirY = ParseExpression(); Consume(TokenType.Comma, "Se esperaba ','.");
            Expression distance = ParseExpression(); Consume(TokenType.Comma, "Se esperaba ','.");
            Expression width = ParseExpression(); Consume(TokenType.Comma, "Se esperaba ','.");
            Expression height = ParseExpression();
            Consume(TokenType.RightParen, "Se esperaba ')'.");
            return new DrawRectangleStmt(keyword, dirX, dirY, distance, width, height);
        }

        private FillStmt ParseFillStatement()
        {
            Token keyword = Consume(TokenType.Fill, "Se esperaba 'Fill'.");
            Consume(TokenType.LeftParen, "Se esperaba '('.");
            Consume(TokenType.RightParen, "Se esperaba ')'.");
            return new FillStmt(keyword);
        }



        private Expression ParseExpression()
        {
            return ParseAnd();
        }

        private Expression ParseAnd()
        {
            Expression expr = ParseOr();
            while (Match(TokenType.And))
            {
                Token operatorToken = Previous();
                Expression right = ParseOr(); 
                expr = new BinaryExpr(expr, operatorToken, right);
            }
            return expr;
        }
        private Expression ParseOr()
        {
            Expression expr = ParseEquality();
            while (Match(TokenType.Or))
            {
                Token operatorToken = Previous();
                Expression right = ParseEquality(); 
                expr = new BinaryExpr(expr, operatorToken, right);
            }
            return expr;
        }
        private Expression ParseEquality()
        {
            Expression expr = ParseComparison();
            while (Match(TokenType.EqualsEquals))
            {
                Token operatorToken = Previous();
                Expression right = ParseComparison();
                expr = new BinaryExpr(expr, operatorToken, right);
            }
            return expr;
        }

        private Expression ParseComparison()
        {
            Expression expr = ParseTerm();
            while (Match(TokenType.GreaterThan, TokenType.GreaterEquals, TokenType.LessThan, TokenType.LessEquals))
            {
                Token operatorToken = Previous();
                Expression right = ParseTerm();
                expr = new BinaryExpr(expr, operatorToken, right);
            }
            return expr;
        }

        private Expression ParseTerm()
        {
            Expression expr = ParseFactor();
            while (Match(TokenType.Plus, TokenType.Minus))
            {
                Token operatorToken = Previous();
                Expression right = ParseFactor();
                expr = new BinaryExpr(expr, operatorToken, right);
            }
            return expr;
        }

        private Expression ParseFactor()
        {
            Expression expr = ParsePower();
            while (Match(TokenType.Multiply, TokenType.Split, TokenType.Module)) 
            {
                Token operatorToken = Previous();
                Expression right = ParsePower();
                expr = new BinaryExpr(expr, operatorToken, right);
            }
            return expr;
        }

       
        private Expression ParsePower()
        {
            Expression expr = ParseUnary();
            if (Match(TokenType.TwoStar)) 
            {
                Token operatorToken = Previous();
                Expression right = ParsePower();
                return new BinaryExpr(expr, operatorToken, right);
            }
            return expr;
        }

        private Expression ParseUnary()
        {
            if (Match(TokenType.Minus /*, TokenType.Not */)) // Si tuvieras Not para booleanos
            {
                Token operatorToken = Previous();
                Expression right = ParseUnary(); 
                return new UnaryExpr(operatorToken, right);
            }
            return ParseCallOrPrimary(); 
        }

        private Expression ParseCallOrPrimary()
        {
            if (IsFunctionKeyword(Peek().Type) && LookAhead(1)?.Type == TokenType.LeftParen)
            {
                return ParseCallExpression();
            }

            if (Peek().Type == TokenType.Identifier && LookAhead(1)?.Type == TokenType.LeftParen)
            {
                Token identifier = Advance(); 
                return ParseCallExpression(identifier); 
            }

            return ParsePrimary();
        }


        private bool IsFunctionKeyword(TokenType type)
        {
            switch (type)
            {
                case TokenType.GetActualX:
                case TokenType.GetActualY:
                case TokenType.GetCanvasSize:
                case TokenType.GetColorCount:
                case TokenType.IsBrushColor:
                case TokenType.IsBrushSize:
                case TokenType.IsCanvasColor:
                    return true;
                default:
                    return false;
            }
        }
        private Token LookAhead(int distance) 
        {
            if (_current + distance >= _tokens.Count) return _tokens.LastOrDefault(t => t.Type == TokenType.EOF);
            return _tokens[_current + distance];
        }

        private CallExpr ParseCallExpression(Token customFunctionToken = null)
        {
            // Usar el token de función predefinida o el proporcionado
            Token calleeNameToken = customFunctionToken ?? Advance();

            Consume(TokenType.LeftParen, "Se esperaba '(' después del nombre de función.");
            List<Expression> arguments = new List<Expression>();

            if (!Check(TokenType.RightParen))
            {
                do
                {
                    arguments.Add(ParseExpression());
                } while (Match(TokenType.Comma));
            }

            Token closingParen = Consume(TokenType.RightParen, "Se esperaba ')' después de los argumentos.");
            return new CallExpr(calleeNameToken, arguments, closingParen);
        }

        private Expression ParsePrimary()
        {
            if (Match(TokenType.IntegerLiteral)) return new LiteralExpr(Previous().Literal, Previous());
            if (Match(TokenType.StringLiteral)) return new LiteralExpr(Previous().Literal, Previous());

            // Si tienes literales booleanos true/false como keywords
            // if (Match(TokenType.True)) return new LiteralExpr(true, Previous());
            // if (Match(TokenType.False)) return new LiteralExpr(false, Previous());

            if (Match(TokenType.Identifier))
            {
                return new VariableExpr(Previous());
            }

            if (Match(TokenType.LeftParen))
            {
                Expression expr = ParseExpression();
                Consume(TokenType.RightParen, "Se esperaba ')' después de la expresión agrupada.");
                return new GroupingExpr(expr);
            }

            if (IsFunctionKeyword(Peek().Type))
            {
                throw Error(Peek(), $"Se esperaba '(' después del nombre de función '{Peek().Lexeme}'.");
            }


            throw Error(Peek(), "Se esperaba una expresión primaria (literal, variable, llamada a función o expresión agrupada).");
        }

        public class ParseError : Exception
        {
            public Token Token { get; } 

           
            public ParseError(Token token, string message) : base(message)
            {
                Token = token;
            }

         
            public ParseError(int line, string message) : base($"Línea {line}: {message}")
            {
                Token = null; 
            }
        }

        private void ValidateVariableName(Token nameToken)
        {
            string name = nameToken.Lexeme;
            if (char.IsDigit(name[0]) || name[0] == '_')
                throw Error(nameToken, "Nombre de variable inválido: no puede comenzar con dígito o '_'");

            if (!name.All(c => char.IsLetterOrDigit(c) || c == '_'))
                throw Error(nameToken, "Nombre de variable inválido: solo puede contener letras, números y _");
        }

        private GotoStmt ParseGotoStatement()
        {
            Token gotoKeyword = Advance(); 
            Consume(TokenType.LeftBracket, "Se esperaba '['");
            Token labelToken = Consume(TokenType.Identifier, "Se esperaba nombre de etiqueta");
            Consume(TokenType.RightBracket, "Se esperaba ']'");
            Consume(TokenType.LeftParen, "Se esperaba '('");
            Expression condition = ParseExpression();
            Consume(TokenType.RightParen, "Se esperaba ')'");

            return new GotoStmt(labelToken, condition); 
        }
    }
} 
