using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WallEPixelproject.AST;
using WallEPixelproject.Interfaces;
using static System.Windows.Forms.LinkLabel;

namespace WallEPixelproject
{
    public partial class PixelWallE : Form
    {
        private const int DISPLAY_PHYSICAL_WIDTH = 500;
        private const int DISPLAY_PHYSICAL_HEIGHT = 500;


        private WallE _wallE;
        private Bitmap displayBitmap;
        private int pixeldivisor;
        private Image WallEImage;

        private ILogger _appLogger;

        private System.Windows.Forms.Panel lineNumberPanel;

        public PixelWallE()
        {
            InitializeComponent();

            _appLogger = new TextBoxLogger(this.logText);

            try
            {
                WallEImage = Properties.Resources.walle_icon;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo cargar la imagen de Wall-E desde recursos: {ex.Message}");
                WallEImage = null;
            }

            _wallE = new WallE(500, 500);
            pixeldivisor = 25;
            ReinitializeCanvasAndLogic();
        }


        // Este método se llama al inicio y cuando el _pixelDivisor cambia
        private void ReinitializeCanvasAndLogic()
        {
            _appLogger?.Info("UI", $"Reinicializando. Divisor actual: {pixeldivisor}");


            int logicalWidth = DISPLAY_PHYSICAL_WIDTH / pixeldivisor;
            int logicalHeight = DISPLAY_PHYSICAL_HEIGHT / pixeldivisor;
            logicalWidth = Math.Max(1, logicalWidth);
            logicalHeight = Math.Max(1, logicalHeight);


            if (_wallE == null)
            {
                _wallE = new WallE(logicalWidth, logicalHeight, _appLogger);
                _wallE.CanvasUpdated += OnWallELogicCanvasUpdated;
            }
            else
            {
                _wallE.ResizeCanvas(logicalWidth, logicalHeight);
            }


            if (displayBitmap == null || displayBitmap.Width != DISPLAY_PHYSICAL_WIDTH || displayBitmap.Height != DISPLAY_PHYSICAL_HEIGHT)
            {
                if (displayBitmap != null) displayBitmap.Dispose();
                displayBitmap = new Bitmap(DISPLAY_PHYSICAL_WIDTH, DISPLAY_PHYSICAL_HEIGHT);
                pbCanvas.Image = displayBitmap;
            }


            RenderLogicalContextToDisplayBitmap();
        }

        private void OnWallELogicCanvasUpdated(object sender, EventArgs e)
        {
            RenderLogicalContextToDisplayBitmap();
        }

        private void RenderLogicalContextToDisplayBitmap()
        {
            if (_wallE == null || displayBitmap == null) return;

            int visualCellSize = pixeldivisor; // El tamaño de cada "cuadradito" lógico en pantalla

            using (Graphics g = Graphics.FromImage(displayBitmap))
            {
                g.Clear(Color.White); // Limpiar el bitmap físico

                // Dibujar los píxeles lógicos
                for (int lx = 0; lx < _wallE.CanvasWidth; lx++)
                {
                    for (int ly = 0; ly < _wallE.CanvasHeight; ly++)
                    {
                        Color pixelColor = _wallE.GetPixelColorForUI(lx, ly);
                        if (pixelColor == Color.Transparent) continue;

                        using (SolidBrush brush = new SolidBrush(pixelColor))
                        {

                            int screenX = lx * visualCellSize;
                            int screenY = ly * visualCellSize;
                            g.FillRectangle(brush, screenX, screenY, visualCellSize, visualCellSize);
                        }
                    }
                }

                // Dibujar la cuadrícula
                if (visualCellSize > 2)
                {
                    using (Pen gridPen = new Pen(Color.LightGray))
                    {
                        // Líneas verticales
                        for (int i = 0; i <= _wallE.CanvasWidth; i++)
                        {
                            int screenX = i * visualCellSize;
                            // Ajuste para que la última línea no se pinte un píxel fuera si la división no es exacta
                            if (screenX >= DISPLAY_PHYSICAL_WIDTH) screenX = DISPLAY_PHYSICAL_WIDTH - 1;
                            g.DrawLine(gridPen, screenX, 0, screenX, DISPLAY_PHYSICAL_HEIGHT - 1);
                        }
                        // Líneas horizontales
                        for (int i = 0; i <= _wallE.CanvasHeight; i++)
                        {
                            int screenY = i * visualCellSize;
                            if (screenY >= DISPLAY_PHYSICAL_HEIGHT) screenY = DISPLAY_PHYSICAL_HEIGHT - 1;
                            g.DrawLine(gridPen, 0, screenY, DISPLAY_PHYSICAL_WIDTH - 1, screenY);
                        }
                    }
                }

                if (WallEImage != null) // Solo dibujar si la imagen se cargó correctamente
                {
                    // Coordenadas lógicas de Wall-E
                    int wallELogicalX = _wallE.X;
                    int wallELogicalY = _wallE.Y;

                    // Convertir a coordenadas de pantalla para la esquina superior izquierda del ícono
                    int wallEScreenX = wallELogicalX * visualCellSize;
                    int wallEScreenY = wallELogicalY * visualCellSize;

                    Rectangle destRect = new Rectangle(wallEScreenX, wallEScreenY, visualCellSize, visualCellSize);

                    if (wallEScreenX >= 0 && wallEScreenX < DISPLAY_PHYSICAL_WIDTH &&
                       wallEScreenY >= 0 && wallEScreenY < DISPLAY_PHYSICAL_HEIGHT)
                    {
                        g.DrawImage(WallEImage, destRect);
                        _appLogger?.Debug("Render", $"Imagen de Wall-E dibujada en ({wallEScreenX},{wallEScreenY}) tamaño ({visualCellSize}x{visualCellSize})");
                    }
                }
                pbCanvas.Invalidate();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            lineNumberPanel.Invalidate();
        }

        private void richTextBox1_VScroll(object sender, EventArgs e)
        {
            lineNumberPanel.Invalidate();
            this.Update();
        }

        private void lineNumberPanel_Paint(object sender, PaintEventArgs e)
        {
            Font font = richTextBox1.Font;
            int lineHeight = (int)e.Graphics.MeasureString("X", font).Height;

            int firstIndex = richTextBox1.GetCharIndexFromPosition(new Point(0, 0));
            int firstLine = richTextBox1.GetLineFromCharIndex(firstIndex);

            Point firstPos = richTextBox1.GetPositionFromCharIndex(firstIndex);

            e.Graphics.Clear(lineNumberPanel.BackColor);

            for (int i = 0; i < (richTextBox1.Height / lineHeight) + 1; i++)
            {
                int lineNum = firstLine + i;
                if (lineNum >= richTextBox1.Lines.Length) break;

                string lineNumber = (lineNum + 1).ToString();
                float yPos = firstPos.Y + (i * lineHeight);

                float textHeight = e.Graphics.MeasureString(lineNumber, font).Height;
                float y = yPos + (lineHeight - textHeight) / 2;

                e.Graphics.DrawString(
                    lineNumber,
                    font,
                    Brushes.Black,
                    new PointF(lineNumberPanel.Width - e.Graphics.MeasureString(lineNumber, font).Width - 5, y)
                );
            }
        }

        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            if (_wallE == null)
            {
                _appLogger?.Error("Execution", "WallE no está inicializado", null);
                MessageBox.Show("Error: WallE no está inicializado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string source = richTextBox1.Text;

            // 1. Validar si hay código para procesar
            if (string.IsNullOrWhiteSpace(source))
            {
                _appLogger?.Info("Execution", "No hay código para ejecutar");
                MessageBox.Show("No hay código en el editor para ejecutar", "Entrada Vacía",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _appLogger?.Info("Execution", "--- Iniciando ejecución ---");

                // 2. Limpiar el canvas y resetear estado
                _wallE.ClearCanvasToWhite();

                // 3. Fase de Lexer (Tokenización)
                _appLogger?.Info("Lexer", "Iniciando tokenización...");
                Lexer lexer = new Lexer(source, _appLogger);
                List<Token> tokens = lexer.ScanTokens();
                _appLogger?.Debug("LEXER_OUTPUT_DETAILED", "--- Tokens del Lexer ---");
                foreach (Token tkn in tokens)
                {
                    _appLogger?.Debug("LEXER_OUTPUT_DETAILED", tkn.ToString());
                }
                _appLogger?.Debug("LEXER_OUTPUT_DETAILED", "--- Fin Tokens del Lexer ---");
                

                // 4. Fase de Parser
                _appLogger?.Info("Parser", "Iniciando análisis sintáctico...");
                Parser parser = new Parser(tokens, _appLogger);
                List<Statement> statements = parser.ParseProgram(out List<Parser.ParseError> errors);

                if (errors?.Count > 0)
                {
                    _appLogger?.Error("Execution", $"Errores de análisis: {errors.Count}");
                    ShowParserErrors(errors);
                    return;
                }

                // 5. Crear el ejecutor AST
                var executor = new ASTExecutor(_wallE, _appLogger);

                // 6. Ejecutar cada statement
                _appLogger?.Info("Execution", $"Ejecutando {statements.Count} comandos...");
                foreach (var stmt in statements)
                {
                    try
                    {
                        stmt.Accept(executor);
                    }
                    catch (Exception ex)
                    {
                        _appLogger?.Error("Execution", $"Error ejecutando comando: {ex.Message}", null, ex);
                        MessageBox.Show($"Error al ejecutar comando: {ex.Message}",
                                      "Error de Ejecución", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                _appLogger?.Info("Execution", "Ejecución completada exitosamente");
                MessageBox.Show("Programa ejecutado correctamente", "Éxito",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _appLogger?.Error("Execution", "Error fatal durante la ejecución", null, ex);
                MessageBox.Show($"Error fatal: {ex.Message}", "Error Crítico",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Forzar actualización visual
                RenderLogicalContextToDisplayBitmap();
                pbCanvas.Refresh();
            }
        }

        private void ShowParserErrors(List<Parser.ParseError> errors)
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendLine($"Se encontraron {errors.Count} errores de sintaxis:");

            foreach (var error in errors)
            {
                errorMsg.AppendLine($"- Línea {error.Token?.Line}: {error.Message}");
            }

            MessageBox.Show(errorMsg.ToString(), "Errores de Sintaxis",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos Wall-E (*.pw)|*.pw|Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            openFileDialog.Title = "Seleccionar archivo para cargar";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string contenido = File.ReadAllText(openFileDialog.FileName);
                    richTextBox1.Text = contenido;
                    _appLogger?.Info("IO", $"Archivo cargado correctamente: {openFileDialog.FileName}");
                }
                catch (Exception ex)
                {
                    _appLogger?.Error("IO", $"Error al cargar el archivo: {ex.Message}", null, ex);
                    MessageBox.Show($"Error al cargar el archivo: {ex.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos Wall-E (*.pw)|*.pw|Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            saveFileDialog.Title = "Guardar archivo";
            saveFileDialog.DefaultExt = "pw";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, richTextBox1.Text);
                    _appLogger?.Info("IO", $"Archivo guardado correctamente: {saveFileDialog.FileName}");
                    MessageBox.Show("Archivo guardado correctamente.", "Éxito",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    _appLogger?.Error("IO", $"Error al guardar el archivo: {ex.Message}", null, ex);
                    MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            _appLogger?.Info("UI", "Editor de código limpiado");
        }

        private void btnRedimensionar_Click(object sender, EventArgs e)
        {
            try
            {
                int newSize = int.Parse(textCanvasSize.Text);
                if (newSize < 10 || newSize > 100)
                {
                    MessageBox.Show("El tamaño debe estar entre 10 y 100", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _wallE.ResizeCanvas(newSize, newSize);
                RenderLogicalContextToDisplayBitmap();
                _appLogger?.Info("UI", $"Canvas redimensionado a {newSize}x{newSize}");
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor ingrese un número válido", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al redimensionar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _appLogger?.Error("UI", $"Error al redimensionar canvas: {ex.Message}");
            }
        }

    }
} 
 


