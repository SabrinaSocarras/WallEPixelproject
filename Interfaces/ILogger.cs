using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallEPixelproject.Interfaces
{
    public interface ILogger
    {
        void Info(string prefix, string message, int? line = null);
        void Debug(string prefix, string message, int? line = null);
        void Warn(string prefix, string message, int? line = null);
        void Error(string prefix, string message, int? line = null, Exception ex = null);
    }

    public class DummyLogger : ILogger
    {
        public void Info(string prefix, string message, int? line = null) { }


        public void Debug(string prefix, string message, int? line = null) { }


        public void Warn(string prefix, string message, int? line = null) { }


        public void Error(string prefix, string message, int? line = null, Exception ex = null) { } 
      
    }
}
