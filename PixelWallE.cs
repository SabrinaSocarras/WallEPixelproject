using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WallEPixelproject
{
    public partial class PixelWallE : Form
    {

        private Bitmap canvasBitmap;
        private Graphics canvasGraphics;
        public PixelWallE()
        {
            InitializeComponent();
           // richTextBox1.Paint += richTextBox1_Paint;
            InicializarCanvas(500, 500);
        }

        private void InicializarCanvas(int width, int height)
        {
            if (pbCanvas.Image != null)
            {
                pbCanvas.Image.Dispose();
            }
            canvasBitmap = new Bitmap(width, height);
            canvasGraphics = Graphics.FromImage(canvasBitmap);
            canvasGraphics.Clear(Color.White);

            Pen gridPen = new Pen(Color.Black, 1);
            int cellSize = 50;

            // lineas verticales
            for (int x = 0; x < width; x += cellSize)
            {
                canvasGraphics.DrawLine(gridPen, x, 0, x, height);
            }

            //lineas horizontales
            for (int y = 0; y < height; y += cellSize)
            {
                canvasGraphics.DrawLine(gridPen, 0, y, width, y);
            }
            pbCanvas.Image = canvasBitmap;
            pbCanvas.Size = new Size(width, height);
        }




        private void richTextBox1_Paint(object sender, PaintEventArgs e)
        {
            // Dibuja números de línea en el RichTextBox
            RichTextBox rtb = (RichTextBox)sender;
            int lineHeight = (int)e.Graphics.MeasureString("X", rtb.Font).Height;
            int firstLine = rtb.GetCharIndexFromPosition(new Point(0, 0));
            int firstLineY = rtb.GetPositionFromCharIndex(firstLine).Y;

            for (int i = 1; i <= rtb.Lines.Length; i++)
            {
                int lineY = firstLineY + (i - 1) * lineHeight;
                e.Graphics.DrawString(i.ToString(), rtb.Font, Brushes.Gray, 5, lineY);
            }
        }


       

        private void btnEjecutar_Click(object sender, EventArgs e)
        {
        //    string codigo = richTextBox1.Text;
            MessageBox.Show("¡Código ejecutado!");
        }

        private void numTamanioCanvas_ValueChanged(object sender, EventArgs e)
        {
            int nuevoTamanio = (int)numTamanioCanvas.Value;
            InicializarCanvas(nuevoTamanio, nuevoTamanio);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Archivos PW (*.pw)|*.pw";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
               // richTextBox1.Text = File.ReadAllText(openFile.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos PW (*.pw)|*.pw";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
             //       File.WriteAllText(saveFileDialog.FileName, txtCodeEditor.Text);
                  //  lblStatus.Text = $"Archivo guardado como {Path.GetFileName(saveFileDialog.FileName)}.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            InicializarCanvas((int)numTamanioCanvas.Value, (int)numTamanioCanvas.Value);
        }

        private void pbCanvas_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
} 
