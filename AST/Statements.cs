using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallEPixelproject.AST
{
    public struct VoidType { public static readonly VoidType Instance = new VoidType(); }

    public interface IStatementVisitor<T> // T podría ser VoidType o object
    {
        T VisitSpawnStmt(SpawnStmt stmt);
        T VisitColorStmt(ColorStmt stmt);
        T VisitSizeStmt(SizeStmt stmt);
        T VisitDrawLineStmt(DrawLineStmt stmt);
        T VisitDrawCircleStmt(DrawCircleStmt stmt);
        T VisitDrawRectangleStmt(DrawRectangleStmt stmt);
        T VisitFillStmt(FillStmt stmt);
        T VisitAssignmentStmt(AssignmentStmt stmt);
        T VisitLabelStmt(LabelStmt stmt);
        T VisitGotoStmt(GotoStmt stmt);


    }
    public abstract class Statement
    {
        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
    public class SpawnStmt : Statement
    {
        public Token KeywordToken { get; } // El token "Spawn"
        public Expression XExpr { get; }
        public Expression YExpr { get; }

        public SpawnStmt(Token keywordToken, Expression xExpr, Expression yExpr)
        {
            KeywordToken = keywordToken;
            XExpr = xExpr;
            YExpr = yExpr;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitSpawnStmt(this);
    }
    public class ColorStmt : Statement
    {
        public Token KeywordToken { get; } // El token "Color"
        public Expression ColorNameExpr { get; } // Debería evaluarse a un string

        public ColorStmt(Token keywordToken, Expression colorNameExpr)
        {
            KeywordToken = keywordToken;
            ColorNameExpr = colorNameExpr;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitColorStmt(this);
    }
    public class SizeStmt : Statement
    {
        public Token KeywordToken { get; } // El token "Size"
        public Expression SizeValueExpr { get; } // Debería evaluarse a un entero

        public SizeStmt(Token keywordToken, Expression sizeValueExpr)
        {
            KeywordToken = keywordToken;
            SizeValueExpr = sizeValueExpr;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitSizeStmt(this);
    }

    public class DrawLineStmt : Statement
    {
        public Token KeywordToken { get; } // "DrawLine"
        public Expression DirXExpr { get; }
        public Expression DirYExpr { get; }
        public Expression DistanceExpr { get; }

        public DrawLineStmt(Token keyword, Expression dirX, Expression dirY, Expression distance)
        {
            KeywordToken = keyword;
            DirXExpr = dirX;
            DirYExpr = dirY;
            DistanceExpr = distance;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitDrawLineStmt(this);
    }

    public class DrawCircleStmt : Statement
    {
        public Token KeywordToken { get; } // "DrawCircle"
        public Expression DirXExpr { get; }
        public Expression DirYExpr { get; }
        public Expression RadiusExpr { get; }

        public DrawCircleStmt(Token keyword, Expression dirX, Expression dirY, Expression radius)
        {
            KeywordToken = keyword;
            DirXExpr = dirX;
            DirYExpr = dirY;
            RadiusExpr = radius;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitDrawCircleStmt(this);
    }

    public class DrawRectangleStmt : Statement
    {
        public Token KeywordToken { get; } // "DrawRectangle"
        public Expression DirXExpr { get; }
        public Expression DirYExpr { get; }
        public Expression DistanceExpr { get; }
        public Expression WidthExpr { get; }
        public Expression HeightExpr { get; }

        public DrawRectangleStmt(Token kw, Expression dX, Expression dY, Expression dist, Expression w, Expression h)
        {
            KeywordToken = kw;
            DirXExpr = dX;
            DirYExpr = dY;
            DistanceExpr = dist;
            WidthExpr = w;
            HeightExpr = h;
        }
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitDrawRectangleStmt(this);
    }

    public class FillStmt : Statement
    {
        public Token KeywordToken { get; } // "Fill"
        public FillStmt(Token keyword) { KeywordToken = keyword; }
        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitFillStmt(this);
    }
    public class AssignmentStmt : Statement
    {
        public Token VariableName { get; }
        public Expression ValueExpr { get; }

        public AssignmentStmt(Token variableName, Expression valueExpr)
        {
            VariableName = variableName;
            ValueExpr = valueExpr;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitAssignmentStmt(this);
    }
    public class LabelStmt : Statement
    {
        public Token LabelToken { get; }

        public LabelStmt(Token labelToken)
        {
            LabelToken = labelToken;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitLabelStmt(this);
    }

    public class GotoStmt : Statement
    {
        public Token LabelToken { get; }
        public Expression Condition { get; }

        public GotoStmt(Token labelToken, Expression condition)
        {
            LabelToken = labelToken;
            Condition = condition;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitGotoStmt(this);

    }
} 