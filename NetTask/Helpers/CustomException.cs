using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTask.Helpers
{
    public class CustomException : Exception
    {
        public CustomException(string message, Exception inEx)
        {
            CustomMessage = message;
            SourceException = inEx;
        }
        public string CustomMessage { get;  private set; }
        public Exception SourceException { get; private set; }
    }
}
