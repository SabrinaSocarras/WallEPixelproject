using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WallEPixelproject.Interfaces;

namespace WallEPixelproject.AST
{
    public class ASTExecutor : IExpressionVisitor<object>, IStatementVisitor<VoidType>
    {
        private Dictionary<string, int> _labelPositions = new Dictionary<string, int>();
        private Dictionary<string, object> _variables = new Dictionary<string, object>();
        private int _currentStatementIndex = 0;
        private List<Statement> _currentProgram;

        private Dictionary<string, object> variables = new Dictionary<string, object>();
        private readonly WallE _wallE;
        private readonly ILogger _logger;
        private (int x, int y) wallEPosition;
        private string currentColor = "Transparent";
        private int brushSize = 1;

        public ASTExecutor(WallE wallE, ILogger logger)
        {
            _wallE = wallE ?? throw new ArgumentNullException(nameof(wallE));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public object VisitBinaryExpr(BinaryExpr expr)
        {
            object left = expr.Left.Accept(this);
            object right = expr.Right.Accept(this);

           
            if (left is int leftInt && right is int rightInt)
            {
                switch (expr.OperatorToken.Type)
                {
                    case TokenType.Plus: return leftInt + rightInt;
                    case TokenType.Minus: return leftInt - rightInt;
                    case TokenType.Multiply: return leftInt * rightInt;
                    case TokenType.Divide:
                        if (rightInt == 0) throw new Exception("División por cero");
                        return leftInt / rightInt;
                    case TokenType.Module: return leftInt % rightInt;
                    case TokenType.TwoStar: return (int)Math.Pow(leftInt, rightInt);
                    case TokenType.EqualsEquals: return leftInt == rightInt ? 1 : 0;
                    case TokenType.GreaterThan: return leftInt > rightInt ? 1 : 0;
                    case TokenType.GreaterEquals: return leftInt >= rightInt ? 1 : 0;
                    case TokenType.LessThan: return leftInt < rightInt ? 1 : 0;
                    case TokenType.LessEquals: return leftInt <= rightInt ? 1 : 0;
                    case TokenType.And: return (leftInt != 0 && rightInt != 0) ? 1 : 0;
                    case TokenType.Or: return (leftInt != 0 || rightInt != 0) ? 1 : 0;
                }
            }
            else if (left is string leftStr && right is string rightStr && expr.OperatorToken.Type == TokenType.EqualsEquals)
            {
                return leftStr.Equals(rightStr) ? 1 : 0;
            }

            throw new Exception($"Tipos incompatibles para operación {expr.OperatorToken.Lexeme} " +
                              $"entre {left?.GetType().Name} y {right?.GetType().Name}");
        }

        public object VisitUnaryExpr(UnaryExpr expr)
        {
            object right = expr.Right.Accept(this);

            switch (expr.OperatorToken.Type)
            {
                case TokenType.Minus:
                    return -(int)right;
               // case TokenType.Not:
                //    return (int)right == 0 ? 1 : 0;
                default:
                    throw new Exception($"Operador unario no soportado: {expr.OperatorToken.Lexeme}");
            }
        }

        public object VisitLiteralExpr(LiteralExpr expr)
        {
            return expr.Value; 
        }

        public object VisitVariableExpr(VariableExpr expr)
        {
            if (variables.TryGetValue(expr.NameToken.Lexeme, out var value))
                return value;

            throw new Exception($"Variable no definida: '{expr.NameToken.Lexeme}'");
        }
        public object VisitCallExpr(CallExpr expr)
        {
            try
            {
                switch (expr.CalleeNameToken.Lexeme)
                {
                    case "GetActualX":
                        return _wallE.X;

                    case "GetActualY":
                        return _wallE.Y;

                    case "GetCanvasSize":
                        return _wallE.CanvasWidth;

                    case "GetColorCount":
                        return HandleGetColorCount(expr);

                    case "IsBrushColor":
                        return HandleIsBrushColor(expr);

                    case "IsBrushSize":
                        return HandleIsBrushSize(expr);

                    case "IsCanvasColor":
                        return HandleIsCanvasColor(expr);

                    default:
                        throw new Exception($"Función no definida: {expr.CalleeNameToken.Lexeme}");
                }
            }
            catch (Exception ex)
            {
                _logger?.Error("EXEC", $"Error en función {expr.CalleeNameToken.Lexeme}: {ex.Message}");
                throw;
            }
        }

        private int HandleGetColorCount(CallExpr expr)
        {
           
            if (expr.Arguments.Count != 5)
                throw new Exception("GetColorCount requiere exactamente 5 argumentos");

           
            string colorName = (string)expr.Arguments[0].Accept(this);
            int x1 = Convert.ToInt32(expr.Arguments[1].Accept(this));
            int y1 = Convert.ToInt32(expr.Arguments[2].Accept(this));
            int x2 = Convert.ToInt32(expr.Arguments[3].Accept(this));
            int y2 = Convert.ToInt32(expr.Arguments[4].Accept(this));

            if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 ||
                x1 >= _wallE.CanvasWidth || x2 >= _wallE.CanvasWidth ||
                y1 >= _wallE.CanvasHeight || y2 >= _wallE.CanvasHeight)
            {
                return 0; 
            }

            
            int startX = Math.Min(x1, x2);
            int endX = Math.Max(x1, x2);
            int startY = Math.Min(y1, y2);
            int endY = Math.Max(y1, y2);

            Color targetColor;
            try
            {
                targetColor = Color.FromName(colorName);
                if (!targetColor.IsKnownColor && colorName != "Transparent")
                    throw new Exception($"Color '{colorName}' no reconocido");
            }
            catch
            {
                return 0; 
            }

            int count = 0;
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Color pixelColor = _wallE.GetPixelColorForUI(x, y);

                    
                    if (string.Equals(pixelColor.Name, targetColor.Name, StringComparison.OrdinalIgnoreCase) ||
                        (colorName == "Transparent" && pixelColor == Color.Transparent))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private int HandleIsBrushColor(CallExpr expr)
        {
            if (expr.Arguments.Count != 1)
                throw new Exception("IsBrushColor requiere exactamente 1 argumento");

            string colorName = (string)expr.Arguments[0].Accept(this);

            return string.Equals(_wallE.BrushColor.Name, colorName, StringComparison.OrdinalIgnoreCase) ||
                   (colorName == "Transparent" && _wallE.BrushColor == Color.Transparent)
                   ? 1 : 0;
        }

        private int HandleIsBrushSize(CallExpr expr)
        {
            if (expr.Arguments.Count != 1)
                throw new Exception("IsBrushSize requiere exactamente 1 argumento");

            int size = Convert.ToInt32(expr.Arguments[0].Accept(this));
            return _wallE.BrushPixelSize == size ? 1 : 0;
        }

        private int HandleIsCanvasColor(CallExpr expr)
        {
            if (expr.Arguments.Count != 3)
                throw new Exception("IsCanvasColor requiere exactamente 3 argumentos");

            string colorName = (string)expr.Arguments[0].Accept(this);
            int vertical = Convert.ToInt32(expr.Arguments[1].Accept(this));
            int horizontal = Convert.ToInt32(expr.Arguments[2].Accept(this));

           
            int checkX = _wallE.X + horizontal;
            int checkY = _wallE.Y + vertical;

            if (checkX < 0 || checkX >= _wallE.CanvasWidth ||
                checkY < 0 || checkY >= _wallE.CanvasHeight)
            {
                return 0; 
            }

            Color targetColor;
            try
            {
                targetColor = Color.FromName(colorName);
                if (!targetColor.IsKnownColor && colorName != "Transparent")
                    return 0;
            }
            catch
            {
                return 0; 
            }

            Color pixelColor = _wallE.GetPixelColorForUI(checkX, checkY);

            return string.Equals(pixelColor.Name, targetColor.Name, StringComparison.OrdinalIgnoreCase) ||
                   (colorName == "Transparent" && pixelColor == Color.Transparent)
                   ? 1 : 0;
        }

        public object VisitGroupingExpr(GroupingExpr expr)
        {
            return expr.InnerExpression.Accept(this);
        }
        public VoidType VisitSpawnStmt(SpawnStmt stmt)
        {
            try
            {
                int x = Convert.ToInt32(stmt.XExpr.Accept(this));
                int y = Convert.ToInt32(stmt.YExpr.Accept(this));

                if (x < 0 || x >= _wallE.CanvasWidth || y < 0 || y >= _wallE.CanvasHeight)
                {
                    string errorMsg = $"Coordenadas de Spawn ({x}, {y}) fuera del canvas (tamaño: {_wallE.CanvasWidth}x{_wallE.CanvasHeight})";
                    _logger?.Error("EXEC", errorMsg);
                    throw new Exception(errorMsg);
                }

                _wallE.Spawn(x, y);
                _logger?.Info("EXEC", $"Wall-E posicionado en ({x}, {y})");

                return VoidType.Instance;
            }
            catch (Exception ex)
            {
                _logger?.Error("EXEC", $"Error en Spawn: {ex.Message}");
                throw new Exception($"Error en Spawn: {ex.Message}");
            }
        }

        public VoidType VisitColorStmt(ColorStmt stmt)
        {
            string colorName = (string)stmt.ColorNameExpr.Accept(this);
            Color color = Color.FromName(colorName);

            if (color.IsKnownColor)
            {
                _wallE.SetBrushColor(color);
                _logger?.Info("EXEC", $"Color cambiado a {colorName}");
            }
            else
            {
                throw new Exception($"Color desconocido: {colorName}");
            }

            return VoidType.Instance;
        }

        public VoidType VisitSizeStmt(SizeStmt stmt)
        {
            int size = Convert.ToInt32(stmt.SizeValueExpr.Accept(this));
            _wallE.SetBrushSize(size);
            _logger?.Info("EXEC", $"Tamaño de pincel cambiado a {size}");
            return VoidType.Instance;
        }

        public VoidType VisitDrawLineStmt(DrawLineStmt stmt)
        {
            try
            {
                int dirX = Convert.ToInt32(stmt.DirXExpr.Accept(this));
                int dirY = Convert.ToInt32(stmt.DirYExpr.Accept(this));
                int distance = Convert.ToInt32(stmt.DistanceExpr.Accept(this));

                _logger?.Debug("EXEC", $"DrawLine(dirX={dirX}, dirY={dirY}, distance={distance})");

                if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
                    throw new Exception("Las direcciones deben ser -1, 0 o 1");

                if (distance <= 0)
                    throw new Exception("La distancia debe ser positiva");

                _wallE.DrawLine(dirX, dirY, distance);

                _logger?.Info("EXEC", $"Línea dibujada desde ({_wallE.X}, {_wallE.Y})");

                return VoidType.Instance;
            }
            catch (Exception ex)
            {
                _logger?.Error("EXEC", $"Error en DrawLine: {ex.Message}");
                throw;
            }
        }
        public VoidType VisitDrawCircleStmt(DrawCircleStmt stmt)
        {
            try
            {
                int dirX = Convert.ToInt32(stmt.DirXExpr.Accept(this));
                int dirY = Convert.ToInt32(stmt.DirYExpr.Accept(this));
                int radius = Convert.ToInt32(stmt.RadiusExpr.Accept(this));

                _logger?.Debug("EXEC", $"DrawCircle(dirX={dirX}, dirY={dirY}, radius={radius})");

                if (radius <= 0)
                {
                    throw new Exception("El radio debe ser un valor positivo");
                }

                if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
                {
                    throw new Exception("Las direcciones deben ser -1, 0 o 1");
                }

                _wallE.DrawCircle(dirX, dirY, radius);

                _logger?.Info("EXEC", $"Círculo dibujado. Wall-E ahora en ({_wallE.X}, {_wallE.Y})");

                return VoidType.Instance;
            }
            catch (Exception ex)
            {
                _logger?.Error("EXEC", $"Error en DrawCircle: {ex.Message}");
                throw;
            }
        }

        public VoidType VisitDrawRectangleStmt(DrawRectangleStmt stmt)
        {
            try
            {
                
                int dirX = Convert.ToInt32(stmt.DirXExpr.Accept(this));
                int dirY = Convert.ToInt32(stmt.DirYExpr.Accept(this));
                int distance = Convert.ToInt32(stmt.DistanceExpr.Accept(this));
                int width = Convert.ToInt32(stmt.WidthExpr.Accept(this));
                int height = Convert.ToInt32(stmt.HeightExpr.Accept(this));

                _logger?.Debug("EXEC", $"DrawRectangle(dirX={dirX}, dirY={dirY}, distance={distance}, width={width}, height={height})");

               
                if (width <= 0 || height <= 0)
                {
                    throw new Exception("Ancho y alto deben ser valores positivos");
                }

                if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
                {
                    throw new Exception("Las direcciones deben ser -1, 0 o 1");
                }

                
                _wallE.DrawRectangle(dirX, dirY, distance, width, height);

                _logger?.Info("EXEC", $"Rectángulo dibujado. Wall-E ahora en ({_wallE.X}, {_wallE.Y})");

                return VoidType.Instance;
            }
            catch (Exception ex)
            {
                _logger?.Error("EXEC", $"Error en DrawRectangle: {ex.Message}");
                throw;
            }
        }

        public VoidType VisitFillStmt(FillStmt stmt)
        {
            try
            {
                _logger?.Debug("EXEC", "Ejecutando comando Fill");
 
                if (_wallE.X < 0 || _wallE.X >= _wallE.CanvasWidth ||
                    _wallE.Y < 0 || _wallE.Y >= _wallE.CanvasHeight)
                {
                    throw new Exception("Wall-E está fuera del canvas, no se puede realizar Fill");
                }

                Color targetColor = _wallE.GetPixelColorForUI(_wallE.X, _wallE.Y);

                if (targetColor == _wallE.BrushColor)
                {
                    _logger?.Info("EXEC", "Fill no necesario: el área ya tiene el color del pincel");
                    return VoidType.Instance;
                }

                _wallE.Fill();

                _logger?.Info("EXEC", $"Área rellenada con color {_wallE.BrushColor}");

                return VoidType.Instance;
            }
            catch (Exception ex)
            {
                _logger?.Error("EXEC", $"Error en Fill: {ex.Message}");
                throw;
            }
        }
        public void Execute(List<Statement> statements)
        {
            if (statements.Count == 0 || !(statements[0] is SpawnStmt))
            {
                _logger?.Error("EXEC", "Error: El programa debe comenzar con un comando Spawn");
                throw new Exception("El programa debe comenzar con un comando Spawn");
            }

            _labelPositions.Clear();
            _variables.Clear();
            _currentProgram = statements;
            _currentStatementIndex = 0;

            for (int i = 0; i < statements.Count; i++)
            {
                if (statements[i] is LabelStmt label)
                {
                    if (_labelPositions.ContainsKey(label.LabelToken.Lexeme))
                    {
                        _logger?.Error("EXEC", $"Etiqueta duplicada: {label.LabelToken.Lexeme}");
                        throw new Exception($"Etiqueta duplicada: {label.LabelToken.Lexeme}");
                    }
                    _labelPositions[label.LabelToken.Lexeme] = i;
                    _logger?.Debug("EXEC", $"Registrada etiqueta {label.LabelToken.Lexeme} en posición {i}");
                }
            }

            for (_currentStatementIndex = 0; _currentStatementIndex < statements.Count; _currentStatementIndex++)
            {
                try
                {
                    statements[_currentStatementIndex].Accept(this);
                }
                catch (Exception ex)
                {
                    _logger?.Error("EXEC", $"Error ejecutando statement {_currentStatementIndex}: {ex.Message}");
                    throw;
                }
            }
        }

        public VoidType VisitAssignmentStmt(AssignmentStmt stmt)
        {
            try
            {
                object value = stmt.ValueExpr.Accept(this);

                string varName = stmt.VariableName.Lexeme;
                if (char.IsDigit(varName[0]))
                    throw new Exception($"Nombre de variable inválido: '{varName}' no puede comenzar con dígito"); 

                variables[varName] = value;
                _logger?.Info("EXEC", $"Variable '{varName}' asignada con valor: {value}");

                return VoidType.Instance;
            }
            catch (Exception ex)
            {
                _logger?.Error("EXEC", $"Error en asignación: {ex.Message}");
                throw;
            }
        }

        public VoidType VisitLabelStmt(LabelStmt stmt)
        {
            if (_labelPositions.ContainsKey(stmt.LabelToken.Lexeme))
                throw new Exception($"Etiqueta duplicada: {stmt.LabelToken.Lexeme}");

            _labelPositions[stmt.LabelToken.Lexeme] = _currentStatementIndex;
            return VoidType.Instance;
        }
        private int _jumpCount = 0;
        private const int MAX_JUMPS = 1000;
        public VoidType VisitGotoStmt(GotoStmt stmt)
        {
            if (_jumpCount++ > MAX_JUMPS)
            {
                throw new Exception("Límite de saltos excedido. Posible bucle infinito.");
            }
            try
            {
                object conditionResult = stmt.Condition.Accept(this);

                bool shouldJump = Convert.ToInt32(conditionResult) != 0;

                if (shouldJump)
                {
                    if (!_labelPositions.TryGetValue(stmt.LabelToken.Lexeme, out int targetIndex))
                    {
                        _logger?.Error("EXEC", $"Etiqueta no definida: {stmt.LabelToken.Lexeme}");
                        throw new Exception($"Etiqueta no definida: {stmt.LabelToken.Lexeme}");
                    }

                    _currentStatementIndex = targetIndex - 1;
                    _logger?.Debug("EXEC", $"Saltando a etiqueta {stmt.LabelToken.Lexeme} en posición {targetIndex}");
                }
                else
                {
                    _logger?.Debug("EXEC", $"Condición falsa, no se salta a {stmt.LabelToken.Lexeme}");
                }

                return VoidType.Instance;
            }
            catch (Exception ex)
            {
                _logger?.Error("EXEC", $"Error en Goto: {ex.Message}");
                throw new Exception($"Error en Goto: {ex.Message}");
            }
        }

    }

}

