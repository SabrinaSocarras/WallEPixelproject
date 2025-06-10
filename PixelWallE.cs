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

        private ILogger _appLogger;


        public PixelWallE()
        {
            InitializeComponent();

            _appLogger = new TextBoxLogger(this.logText);
           
            _wallE = new WallE(500, 500);
            pixeldivisor = 50;
            ReinitializeCanvasAndLogic();
        }
               
         
        // Este método se llama al inicio y cuando el _pixelDivisor cambia
        private void ReinitializeCanvasAndLogic()
        {
            _appLogger?.Info("UI", $"Reinicializando. Divisor actual: {pixeldivisor}");

            // 1. Calcular dimensiones lógicas
            int logicalWidth = DISPLAY_PHYSICAL_WIDTH / pixeldivisor;
            int logicalHeight = DISPLAY_PHYSICAL_HEIGHT / pixeldivisor;
            logicalWidth = Math.Max(1, logicalWidth); // Evitar cero
            logicalHeight = Math.Max(1, logicalHeight);

            // 2. Crear/Redimensionar el contexto lógico de WallE
            if (_wallE== null)
            {
                _wallE= new WallE(logicalWidth, logicalHeight /*, _appLogger */); // Pasar logger a WallE
                _wallE.CanvasUpdated += OnWallELogicCanvasUpdated;
            }
            else
            {
                _wallE.ResizeCanvas(logicalWidth, logicalHeight); // El ResizeCanvas de WallE debe limpiar y llamar a su OnCanvasUpdated
            }

            // 3. Crear/Asegurar el displayBitmap con tamaño físico fijo
            if (displayBitmap == null || displayBitmap.Width != DISPLAY_PHYSICAL_WIDTH || displayBitmap.Height != DISPLAY_PHYSICAL_HEIGHT)
            {
                if (displayBitmap != null) displayBitmap.Dispose();
                displayBitmap = new Bitmap(DISPLAY_PHYSICAL_WIDTH, DISPLAY_PHYSICAL_HEIGHT);
                pbCanvas.Image = displayBitmap;
            }

            // 4. Renderizar (ResizeCanvas en WallE debería haber llamado a OnCanvasUpdated, que llama a Render)
            // Pero una llamada explícita aquí asegura el renderizado inicial si OnCanvasUpdated no se disparó aún.
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
                for (int lx = 0; lx < _wallE.CanvasWidth; lx++) // Usa LogicalCanvasWidth de WallE
                {
                    for (int ly = 0; ly < _wallE.CanvasHeight; ly++) // Usa LogicalCanvasHeight de WallE
                    {
                        Color pixelColor = _wallE.GetPixelColorForUI(lx, ly);
                        if (pixelColor == Color.Transparent) continue; // No pintar transparente

                        using (SolidBrush brush = new SolidBrush(pixelColor))
                        {
                            // Coordenadas en el bitmap físico
                            int screenX = lx * visualCellSize;
                            int screenY = ly * visualCellSize;
                            g.FillRectangle(brush, screenX, screenY, visualCellSize, visualCellSize);
                        }
                    }
                }

                // Dibujar la cuadrícula
                if (visualCellSize > 2) // Solo si las celdas son visibles
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
            }
            pbCanvas.Invalidate(); // Actualizar el PictureBox
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
            if (_wallE == null)
            {
                _appLogger?.Error("TestExecution", "WallE (Contexto Lógico) no inicializado.", null);
                MessageBox.Show("Error: El contexto lógico de WallE no está inicializado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _appLogger?.Info("TestExecution", "--- Iniciando Prueba Manual de DrawLine ---");

            _wallE.ClearCanvasToWhite(); // Limpia y redibuja

            try
            {
                int spawnX = 0;
                int spawnY = 0;
                _wallE.Spawn(spawnX, spawnY); // Usa CmdSpawn
                _appLogger?.Info("TestExecution", $"Spawn en ({spawnX},{spawnY})");
            }
            catch (Exception exSpawn)
            {
                _appLogger?.Error("TestExecution", "Error durante CmdSpawn de prueba.", null, exSpawn);
                MessageBox.Show($"Error en Spawn de prueba: {exSpawn.Message}");
                return;
            }

            _wallE.SetBrushColor(Color.Red);    // Usa CmdSetBrushColor
            _wallE.SetBrushSize(1);         // Usa CmdSetBrushSize
            _appLogger?.Info("TestExecution", "Pincel configurado a Rojo, tamaño 1.");

            try
            {
                _appLogger?.Info("TestExecution", "Probando DrawLine horizontal...");
                List<Point> puntosPintados = _wallE.DrawLine(1, 0, 5); // Usa CmdDrawLine
                _appLogger?.Info("TestExecution", $"DrawLine horizontal completado. Puntos afectados: {puntosPintados.Count}. Wall-E ahora en: ({_wallE.X}, {_wallE.Y})");
            }
            catch (Exception exDraw)
            {
                _appLogger?.Error("TestExecution", "Error durante CmdDrawLine de prueba.", null, exDraw);
                MessageBox.Show($"Error en DrawLine de prueba: {exDraw.Message}");
            }
            MessageBox.Show("Prueba de DrawLine ejecutada. Revisa el canvas y los logs.", "Prueba Manual", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /*  string source = richTextBox1.Text;


          // 1. Validar si hay código para procesar
          if (string.IsNullOrWhiteSpace(source))
          {
              _appLogger?.Info("Interpreter", "No hay código para ejecutar.");
              MessageBox.Show("No hay código en el editor para ejecutar.",
                              "Entrada Vacía",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
              return;
          }

          // Loguear el inicio del proceso y el código fuente
          _appLogger?.Info("ExecutionEngine", "--- Iniciando Procesamiento de Código ---");
          _appLogger?.Debug("ExecutionEngine", "Código Fuente Recibido:\n" + source);

          // 2. Fase de Lexer (Tokenización)
          _appLogger?.Info("Lexer", "Iniciando fase de tokenización...");
          Lexer lexer = new Lexer(source, _appLogger); // Pasa el código y el logger
          List<Token> tokens; // Usar tu clase Token (o Tokens si la llamaste así)

          try
          {
              tokens = lexer.ScanTokens(); // El método ScanTokens ya loguea su progreso interno
          }
          catch (Exception ex)
          {
              _appLogger?.Error("Lexer", "Excepción crítica durante el escaneo de tokens.", null, ex);
              MessageBox.Show($"Error fatal en el Lexer: {ex.Message}\n\nConsulte los logs para más detalles.",
                              "Error Crítico del Lexer",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
              return; // Detener la ejecución si el lexer falla catastróficamente
          }

          _appLogger?.Info("Lexer", $"Fase de tokenización completada. Se generaron {tokens.Count} tokens (incluyendo EOF).");

          // (Opcional pero MUY RECOMENDADO para depuración) Imprimir todos los tokens generados
          if (tokens != null && tokens.Count > 0)
          {
              StringBuilder tokenDetails = new StringBuilder();
              tokenDetails.AppendLine("Tokens Generados:");
              foreach (Token token in tokens)
              {
                  tokenDetails.AppendLine(token.ToString()); // Asumiendo que Token.ToString() está bien formateado
              }
              _appLogger?.Debug("Lexer_Output", tokenDetails.ToString());
          }

          // 3. Verificar Errores del Lexer (Tokens Desconocidos)
          bool hasLexerErrors = false;
          if (tokens != null)
          {
              foreach (Token token in tokens)
              {
                  if (token.Type == TokenType.Unknown)
                  {
                      hasLexerErrors = true;

                      _appLogger?.Error("Lexer", $"Token desconocido encontrado: '{token.Lexeme}' en línea {token.Line}. Detalle: {token.Literal}", token.Line);
                  }
              }
          }
          if (hasLexerErrors)
          {
              _appLogger?.Warn("ExecutionEngine", "Se encontraron errores durante la fase de Lexer. La ejecución podría no continuar o ser incorrecta.");
              MessageBox.Show("Se encontraron errores durante el análisis léxico (tokenización).\n" +
                              "Revise los logs para más detalles (Ventana Resultados -> Depurar, o su TextBox de logs).",
                              "Errores del Lexer",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
          }*/



        //cargar 
        private void button2_Click(object sender, EventArgs e)
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "Archivos PW (*.pw)|*.pw";
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    // richTextBox1.Text = File.ReadAllText(openFile.FileName);
                }
            }

            // guardar 
            private void button3_Click(object sender, EventArgs e)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Archivos PW (*.pw)|*.pw";
                saveFileDialog.FilterIndex = 0; // Establece el filtro predeterminado   
                saveFileDialog.RestoreDirectory = true;  // Restablece el directorio al predeterminado después de guardar
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Verifica si el archivo ya existe y pregunta al usuario si desea sobrescribirlo
                        if (File.Exists(saveFileDialog.FileName))
                        {
                            DialogResult result = MessageBox.Show("El archivo ya existe. ¿Deseas sobrescribirlo?", "Confirmar Sobrescritura", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.No)
                            {
                                return; // No sobrescribir, salir del método
                            }
                        }
                        // Guarda el contenido del RichTextBox en el archivo seleccionado
                        File.WriteAllText(saveFileDialog.FileName, richTextBox1.Text);
                        MessageBox.Show("Archivo guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            private void button4_Click(object sender, EventArgs e)
            {
                //  InicializarCanvas((int)numTamanioCanvas.Value, (int)numTamanioCanvas.Value);
            }

            private void numTamanioCanvas_ValueChanged_1(object sender, EventArgs e)
            {

            }

            private void pbCanvas_Click(object sender, EventArgs e)
            {

            }

    }
    
    } 
