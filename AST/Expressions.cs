using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEPixelproject;

namespace WallEPixelproject.AST
{
    public interface IExpressionVisitor<T>
    {
        T VisitBinaryExpr(BinaryExpr expr);
        T VisitUnaryExpr(UnaryExpr expr);
        T VisitLiteralExpr(LiteralExpr expr);
        T VisitVariableExpr(VariableExpr expr);
        T VisitCallExpr(CallExpr expr);
        T VisitGroupingExpr(GroupingExpr expr);
    }
    public abstract class Expression
    {
        public abstract T Accept<T>(IExpressionVisitor<T> visitor);
    }

    public class BinaryExpr : Expression
    {
        public Expression Left { get; }
        public Token OperatorToken { get; } // El token del operador (+, -, *, ==, &&, etc.)
        public Expression Right { get; }

        public BinaryExpr(Expression left, Token operatorToken, Expression right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }
        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
    }

    public class UnaryExpr : Expression
    {
        public Token OperatorToken { get; } // El token del operador (ej. - para negación)
        public Expression Right { get; }

        public UnaryExpr(Token operatorToken, Expression right)
        {
            OperatorToken = operatorToken;
            Right = right;
        }
        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
    }
    public class LiteralExpr : Expression
    {
        public object Value { get; } // El valor literal (int, string, bool - aunque bool no es literal en tu spec)
        public Token TokenInfo { get; } // Guardar el token original para info de línea/columna y el lexema

        public LiteralExpr(object value, Token tokenInfo)
        {
            Value = value;
            TokenInfo = tokenInfo;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
    }

    public class VariableExpr : Expression
    {
        public Token NameToken { get; } // El token del nombre de la variable (identificador)

        public VariableExpr(Token nameToken)
        {
            NameToken = nameToken;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitVariableExpr(this);
    }

    public class CallExpr : Expression // Para llamadas a funciones como GetActualX(), IsBrushColor("Red")
    {
        public Token CalleeNameToken { get; }      // El token del nombre de la función (GetActualX, IsBrushColor)
        public List<Expression> Arguments { get; } // Lista de expresiones para los argumentos
        public Token ClosingParenToken { get; } // El token ')' para info de línea/columna del final de la llamada

        public CallExpr(Token calleeNameToken, List<Expression> arguments, Token closingParenToken)
        {
            CalleeNameToken = calleeNameToken;
            Arguments = arguments;
            ClosingParenToken = closingParenToken;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitCallExpr(this);
    }

    public class GroupingExpr : Expression // Para (expresion)
    {
        public Expression InnerExpression { get; }

        public GroupingExpr(Expression expression)
        {
            InnerExpression = expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
    }
}

