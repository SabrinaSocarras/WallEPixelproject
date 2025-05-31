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
        private CanvasManager canvasManager;

        public PixelWallE()
        {
            InitializeComponent();
            canvasManager = new CanvasManager();
            canvasManager.InitializeCanvas(500, 500);
            // richTextBox1.Paint += richTextBox1_Paint;
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

        private void numTamanioCanvas_ValueChanged_1(object sender, EventArgs e)
        {

        }

        private void pbCanvas_Click(object sender, EventArgs e)
        {

        }
    }
} 
