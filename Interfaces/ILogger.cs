using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallEPixelproject.Interfaces
{
    internal interface ILogger
    {
        void Info(string prefix, string message, int? line = null);
        void Debug(string prefix, string message, int? line = null);
        void Warn(string prefix, string message, int? line = null);
        void Error(string prefix, string message, int? line = null, Exception ex = null);
    }
}
