using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEPixelproject.Interfaces;

namespace WallEPixelproject
{
    public class WallE
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public Color BrushColor { get; set; } = Color.Black;
        public int BrushPixelSize { get; set; } = 1;

        ILogger DummyLogger; 
        public int CanvasWidth { get; private set; }
        public int CanvasHeight {  get; private set; }


        private Color[,] logicalPixelGrid;

        public event EventHandler CanvasUpdated;

        public WallE(int initialCanvasWidth, int initialCanvasHeight, ILogger logger = null)
        {

            X = 0;
            Y = 0;
           
            BrushPixelSize = 1;

            int validInitialWidth = Math.Max(1, initialCanvasWidth);
            int validInitialHeight = Math.Max(1, initialCanvasHeight);


            ResizeCanvas(initialCanvasWidth, initialCanvasHeight);
        }
        public void ResizeCanvas(int newWidth, int newHeight)
        {
            if (newWidth <= 0 || newHeight <= 0)
                throw new ArgumentException("El tamaño del canvas debe ser positivo");

            CanvasWidth = newWidth;
            CanvasHeight = newHeight;
            logicalPixelGrid = new Color[CanvasWidth, CanvasHeight];
            ClearCanvasToWhite();

            X = Math.Min(X, CanvasWidth - 1);
            Y = Math.Min(Y, CanvasHeight - 1);

            OnCanvasUpdated();
        }


        public void ClearCanvasToWhite()
        {
            
            for (int x = 0; x < CanvasWidth; x++)
            {
                for (int y = 0; y < CanvasHeight; y++)
                {
                    logicalPixelGrid[x, y] = Color.White;
                }
            }
            OnCanvasUpdated();
        }

        public void Spawn(int x, int y)
        {
            if (x < 0 || x >= CanvasWidth || y < 0 || y >= CanvasHeight)
            {
             
            }
            X = x;
            Y = y;
           
        }


        public void SetBrushColor(Color colorToPaint)
        {
            this.BrushColor = colorToPaint;
          
        }

        public void SetBrushSize(int newPixelSize)
        {
            if (newPixelSize <= 0) newPixelSize = 1;
            BrushPixelSize = (newPixelSize % 2 == 0) ? newPixelSize - 1 : newPixelSize;
            if (BrushPixelSize <= 0) BrushPixelSize = 1;
             
        }


        private void PaintPixel(int logicalX, int logicalY, Color colorToPaint)
        {
            if (colorToPaint == Color.Transparent)
            {
                return;
            }

            int halfBrush = BrushPixelSize / 2;

            for (int dx = -halfBrush; dx <= halfBrush; dx++)
            {
                for (int dy = -halfBrush; dy <= halfBrush; dy++)
                {
                    int currentPaintX = logicalX + dx;
                    int currentPaintY = logicalY + dy;

                    
                    if (currentPaintX >= 0 && currentPaintX < CanvasWidth &&
                        currentPaintY >= 0 && currentPaintY < CanvasHeight)
                    {

                        logicalPixelGrid[currentPaintX, currentPaintY] = colorToPaint;
                      
                    }
                    else
                    {
                        //  _logger.Warn($"  PaintLogicalPixelWithBrush: Intento de pintar píxel lógico ({currentPaintX},{currentPaintY}) fuera del canvas. Ignorado.");
                    }
                }

            }

        }
        public List<Point> GenerateLinePoints(int startX, int startY, int dirX, int dirY, int distance)
        {
          
            List<Point> pointsToPaint = new List<Point>();
            if (distance <= 0) return pointsToPaint;

            int currentX = startX;
            int currentY = startY;

            for (int i = 0; i < distance; i++)
            {
                if (i > 0)
                {
                    currentX += dirX;
                    currentY += dirY;
                }

                if (currentX < 0 || currentX >= CanvasWidth || currentY < 0 || currentY >= CanvasHeight)
                {
                  
                    break; 
                }
                pointsToPaint.Add(new Point(currentX, currentY));
            }
            return pointsToPaint;
        }

        public List<Point> DrawLine(int dirX, int dirY, int distance)
        {
            List<Point> points = GenerateLinePoints( X, Y, dirX, dirY, distance);
            int finalX = X;
            int finalY = Y;

            foreach (Point p in points)
            {
                PaintPixel(p.X, p.Y, BrushColor);
                finalX = p.X; 
                finalY = p.Y;
            }

            if (points.Any())
            {
                X = finalX;
                Y = finalY;
            }
            OnCanvasUpdated(); 
            return points; 
        }


        public List<Point> DrawRectangle(int dirXToCenter, int dirYToCenter, int distanceToCenter, int width, int height)
        {
           
            List<Point> rectangleEdgePoints = new List<Point>();
            if (width <= 0 || height <= 0)
            {
          
                OnCanvasUpdated();
                return rectangleEdgePoints;
            }

            int centerX = X + dirXToCenter * distanceToCenter;
            int centerY = Y + dirYToCenter * distanceToCenter;

            int halfWidth = width / 2;
            int halfHeight = height / 2;
            int x1 = centerX - halfWidth;
            int y1 = centerY - halfHeight;
            int x2 = centerX + halfWidth - (width % 2 == 0 ? 1 : 0); 
            int y2 = centerY + halfHeight - (height % 2 == 0 ? 1 : 0);


            // Dibujar los 4 lados usando GenerateLinePoints
            // Lado superior
            rectangleEdgePoints.AddRange(GenerateLinePoints(x1, y1, 1, 0, width));
            // Lado inferior
            rectangleEdgePoints.AddRange(GenerateLinePoints(x1, y2, 1, 0, width));
            // Lado izquierdo (sin contar esquinas ya pintadas)
            rectangleEdgePoints.AddRange(GenerateLinePoints(x1, y1 + 1, 0, 1, height - 2));
            // Lado derecho (sin contar esquinas ya pintadas)
            rectangleEdgePoints.AddRange(GenerateLinePoints(x2, y1 + 1, 0, 1, height - 2));

            // Pintar los puntos
            foreach (Point p in rectangleEdgePoints.Distinct()) // Distinct para no pintar esquinas dos veces con el mismo pincel
            {
                PaintPixel(p.X, p.Y, BrushColor);
            }

            // Mover Wall-E al centro del rectángulo
            int finalWallEX = Math.Max(0, Math.Min(CanvasWidth - 1, centerX));
            int finalWallEY = Math.Max(0, Math.Min(CanvasHeight - 1, centerY));

            if (X != finalWallEX || Y != finalWallEY)
            {
                X = finalWallEX;
                Y = finalWallEY;
            }

            OnCanvasUpdated();
            return rectangleEdgePoints;
        }

        public void Fill()
        {
            if (X < 0 || X >= CanvasWidth || Y < 0 || Y >= CanvasHeight)
                return;

            Color targetColor = logicalPixelGrid[X, Y];
            if (targetColor == BrushColor)
                return; 

            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(X, Y));

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                int x = p.X;
                int y = p.Y;

                if (x < 0 || x >= CanvasWidth || y < 0 || y >= CanvasHeight)
                    continue;

                if (logicalPixelGrid[x, y] != targetColor)
                    continue;

                PaintPixel(x, y, BrushColor);

             
                queue.Enqueue(new Point(x + 1, y));
                queue.Enqueue(new Point(x - 1, y));
                queue.Enqueue(new Point(x, y + 1));
                queue.Enqueue(new Point(x, y - 1));
            }

            OnCanvasUpdated();
        }

        public List<Point> DrawCircle(int dirX, int dirY, int radius)
        {
            List<Point> circlePoints = new List<Point>();

            if (radius <= 0)
            {
                OnCanvasUpdated();
                return circlePoints;
            }

            // Calcular el centro del círculo
            int centerX = X + dirX * radius;
            int centerY = Y + dirY * radius;

            // Algoritmo del círculo de Bresenham
            int x = 0;
            int y = radius;
            int d = 3 - 2 * radius;

            // Pintar los 8 octantes del círculo
            while (x <= y)
            {
                AddCirclePoints(centerX, centerY, x, y, circlePoints);

                if (d < 0)
                {
                    d = d + 4 * x + 6;
                }
                else
                {
                    d = d + 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }

            foreach (Point p in circlePoints.Distinct())
            {
                PaintPixel(p.X, p.Y, BrushColor);
            }

            // Mover Wall-E al centro del círculo
            X = Math.Max(0, Math.Min(CanvasWidth - 1, centerX));
            Y = Math.Max(0, Math.Min(CanvasHeight - 1, centerY));

            OnCanvasUpdated();
            return circlePoints;
        }

        private void AddCirclePoints(int centerX, int centerY, int x, int y, List<Point> points)
        {
            points.Add(new Point(centerX + x, centerY + y));
            points.Add(new Point(centerX - x, centerY + y));
            points.Add(new Point(centerX + x, centerY - y));
            points.Add(new Point(centerX - x, centerY - y));
            points.Add(new Point(centerX + y, centerY + x));
            points.Add(new Point(centerX - y, centerY + x));
            points.Add(new Point(centerX + y, centerY - x));
            points.Add(new Point(centerX - y, centerY - x));
        }

        protected virtual void OnCanvasUpdated()
        {
            CanvasUpdated?.Invoke(this, EventArgs.Empty);
        }

       
        public Color GetPixelColorForUI(int logicalX, int logicalY)
        {
            if (logicalX < 0 || logicalX >= CanvasWidth || logicalY < 0 || logicalY >= CanvasHeight)
            {
                return Color.Magenta; 
            }
            return logicalPixelGrid[logicalX, logicalY];
        } 
    }
}
