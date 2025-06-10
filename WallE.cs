using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallEPixelproject
{
    public class WallE
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public Color BrushColor { get; set; } = Color.Black;
        public int BrushPixelSize { get; set; } = 1;


        public int CanvasWidth { get; private set; }
        public int CanvasHeight {  get; private set; }


        private Color[,] logicalPixelGrid;

        public event EventHandler CanvasUpdated;

        public WallE(int initialCanvasWidth, int initialCanvasHeight)
        {
            X = 0;
            Y = 0;
            BrushColor = Color.White;
            BrushPixelSize = 1;

            // Establecer dimensiones mínimas si las iniciales son inválidas
            int validInitialWidth = Math.Max(1, initialCanvasWidth);
            int validInitialHeight = Math.Max(1, initialCanvasHeight);


            ResizeCanvas(initialCanvasWidth, initialCanvasHeight);
        }
        public void ResizeCanvas(int newWidth, int newHeight)
        {
            if (newWidth <= 0 || newHeight <= 0)
            {


                return;
            }

            CanvasWidth = newWidth;
            CanvasHeight = newHeight;

            logicalPixelGrid = new Color[CanvasWidth, CanvasHeight];

            ClearCanvasToWhite();


            OnCanvasUpdated();

        }


        public void ClearCanvasToWhite()
        {
            // _logger.Debug("Limpiando canvas a blanco.");
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
                // _logger.Error($"CmdSpawn: Coordenadas ({x},{y}) fuera del canvas ({CanvasWidth}x{CanvasHeight}).");
                //     throw new RuntimeError(null, $"Spawn: Coordenadas ({x},{y}) fuera de los límites del canvas [{CanvasWidth - 1},{CanvasHeight - 1}].");
            }
            X = x;
            Y = y;
            // _logger.Info($"CmdSpawn: Wall-E posicionado en ({WallEX},{WallEY}).");
        }


        public void SetBrushColor(Color newColor)
        {
            this.BrushColor = newColor;
            // _logger.Info($"CmdSetBrushColor: Color del pincel cambiado a {BrushColor}.");
        }

        public void SetBrushSize(int newPixelSize)
        {
            if (newPixelSize <= 0) newPixelSize = 1;
            BrushPixelSize = (newPixelSize % 2 == 0) ? newPixelSize - 1 : newPixelSize;
            if (BrushPixelSize <= 0) BrushPixelSize = 1;
            // _logger.Info($"CmdSetBrushSize: Tamaño del pincel cambiado a {BrushPixelSize}.");
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

                    // Verificar si el píxel a pintar está dentro de los límites del canvas
                    if (currentPaintX >= 0 && currentPaintX < CanvasWidth &&
                        currentPaintY >= 0 && currentPaintY < CanvasHeight)
                    {

                        logicalPixelGrid[currentPaintX, currentPaintY] = colorToPaint;
                        //  _logger.Debug($"  Píxel lógico ({currentPaintX},{currentPaintY}) en _logicalPixelGrid establecido a {colorToPaint}.");
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
            //_logger.Debug($"GenerateLinePoints: desde ({startX},{startY}), dir({dirX},{dirY}), dist({distance}).");
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
                  //  _logger.Warn($"  GenerateLinePoints: Punto ({currentX},{currentY}) fuera del canvas. Línea truncada.");
                    break; // Detener si se sale del canvas
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
                finalX = p.X; // El último punto pintado es donde termina Wall-E
                finalY = p.Y;
            }

            if (points.Any()) // Solo mover si se pintó algo
            {
                X = finalX;
                Y = finalY;
              //  _logger.Info($"CmdDrawLine: Wall-E movido a ({WallEX},{WallEY}).");
            }
            OnCanvasUpdated(); // Notificar a la UI después de que la línea completa se "pinta" en la matriz
            return points; // Retornar los puntos para posible animación en el intérprete
        }


        public List<Point> DrawRectangle(int dirXToCenter, int dirYToCenter, int distanceToCenter, int width, int height)
        {
           // _logger.Info($"DrawRectangle: dir({dirXToCenter},{dirYToCenter}), dist({distanceToCenter}), w({width}), h({height}). Desde ({WallEX}, {WallEY})");
            List<Point> rectangleEdgePoints = new List<Point>();
            if (width <= 0 || height <= 0)
            {
             //   _logger.Warn("DrawRectangle: Ancho o alto inválido, no se dibuja nada.");
                OnCanvasUpdated();
                return rectangleEdgePoints;
            }

            // Calcular el centro del rectángulo
            int centerX = X + dirXToCenter * distanceToCenter;
            int centerY = Y + dirYToCenter * distanceToCenter;

            // Calcular coordenadas de las esquinas
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            int x1 = centerX - halfWidth;
            int y1 = centerY - halfHeight;
            int x2 = centerX + halfWidth - (width % 2 == 0 ? 1 : 0); // Ajustar si es par para que el ancho sea exacto
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
                //_logger.Info($"CmdDrawRectangle: Wall-E movido al centro ({WallEX},{WallEY}).");
            }

            OnCanvasUpdated();
            return rectangleEdgePoints;
        }
        protected virtual void OnCanvasUpdated()
        {
           // _logger.Debug("OnCanvasUpdated invocado.");
            CanvasUpdated?.Invoke(this, EventArgs.Empty);
        }

        // --- Acceso a la Matriz para la UI (solo lectura) ---
        public Color GetPixelColorForUI(int logicalX, int logicalY)
        {
            if (logicalX < 0 || logicalX >= CanvasWidth || logicalY < 0 || logicalY >= CanvasHeight)
            {
                return Color.Magenta; // Color para fuera de los límites visibles en UI
            }
            return logicalPixelGrid[logicalX, logicalY];
        }

       
    }
}
