using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WallEPixelproject.Interfaces;

namespace WallEPixelproject
{
    public class TextBoxLogger : ILogger
    {
        private readonly TextBox _logTextBox;

        public TextBoxLogger(TextBox textBoxControl)
        {
            _logTextBox = textBoxControl ?? throw new ArgumentNullException(nameof(textBoxControl));
        }

        private string FormatMessage(string level, string prefix, string message, int? line)
        {
            string lineInfo = line.HasValue ? $" (L:{line.Value})" : "";
            return $"[{level}] [{prefix}]{lineInfo}: {message}";
        }

      /*  private void AppendToTextBox(string formattedMessageWithLevelAndPrefix)
        {
            string textToAppend = $"{DateTime.Now:HH:mm:ss.fff} {formattedMessageWithLevelAndPrefix}{Environment.NewLine}";
            if (_logTextBox.InvokeRequired)
            {
                // Si estamos en un hilo diferente al de la UI, invocamos la actualización en el hilo UI
                _logTextBox.BeginInvoke(new Action(() => {
                    _logTextBox.AppendText(textToAppend);
                    _logTextBox.ScrollToCaret();
                }));
            }
            else
            {
                _logTextBox.AppendText(textToAppend);
                _logTextBox.ScrollToCaret();
            }
        }
      */

        public void Info(string prefix, string message, int? line = null)
        {
     //       AppendToTextBox(FormatMessage("INFO", prefix, message, line));
        }

        public void Debug(string prefix, string message, int? line = null)
        {
#if DEBUG
           // AppendToTextBox(FormatMessage("DEBUG", prefix, message, line));
#endif
        }

        public void Warn(string prefix, string message, int? line = null)
        {
          //  AppendToTextBox(FormatMessage("WARN", prefix, message, line));
        }

        public void Error(string prefix, string message, int? line = null, Exception ex = null)
        {
            string baseMsg = FormatMessage("ERROR", prefix, message, line);
            if (ex != null)
            {
                baseMsg += $"{Environment.NewLine}    Exception: {ex.GetType().Name} - {ex.Message}";
#if DEBUG
                if (ex.StackTrace != null)
                    baseMsg += $"{Environment.NewLine}    StackTrace: {ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None).FirstOrDefault()}";
#endif
            }
          //  AppendToTextBox(baseMsg);
        }
    }
}
