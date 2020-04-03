using System;

namespace ATDiagnosticsLogger
{
    [Serializable]
    public class Log
    {
        public string? Message;
        public Levels Level;
        public string MethodName;
        public string ClassName;
        public uint LineNumber;
        public DateTime timeStamp;

    }
}
