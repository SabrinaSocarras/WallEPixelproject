using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WallEPixelproject
{
    public class CanvasManager
    {
        public Bitmap CanvasBitmap { get; private set; }
        public Graphics CanvasGraphics { get; private set; }
        public int CellSize { get; set; } = 50;
        public Color GridColor { get; set; } = Color.Black;
        public Color BackgroundColor { get; set; } = Color.White;

        public event EventHandler CanvasUpdated;

        public void InitializeCanvas(int width, int height)
        {
            if (CanvasBitmap != null)
            {
                CanvasBitmap.Dispose();
                CanvasGraphics.Dispose();
            }

            CanvasBitmap = new Bitmap(width, height);
            CanvasGraphics = Graphics.FromImage(CanvasBitmap);
            ClearCanvas();
            DrawGrid();
            OnCanvasUpdated();
        }

        public void ClearCanvas()
        {
            CanvasGraphics.Clear(Color.White);
            DrawGrid();
            OnCanvasUpdated();
        }

        private void DrawGrid()
        {
            using (Pen gridPen = new Pen(GridColor, 1))
            {
                // lineas verticales
                for (int x = 0; x < CanvasBitmap.Width; x += CellSize)
                {
                    CanvasGraphics.DrawLine(gridPen, x, 0, x, CanvasBitmap.Height);
                }

                // lineas horizontales
                for (int y = 0; y < CanvasBitmap.Height; y += CellSize)
                {
                    CanvasGraphics.DrawLine(gridPen, 0, y, CanvasBitmap.Width, y);
                }
            }
        }

        protected virtual void OnCanvasUpdated()
        {
            CanvasUpdated?.Invoke(this, EventArgs.Empty);
        }

    }
}
