using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TcpClientApp
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MessageDTO
    {
        private const int STRING_LEN = 100;

        public MessageDTO()
        {
            Message = new byte[STRING_LEN];
            Level = new byte[STRING_LEN];
            MethodName = new byte[STRING_LEN];
            ClassName = new byte[STRING_LEN];
            LineNumber = new byte[STRING_LEN];
            timeStamp = new byte[STRING_LEN];
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = STRING_LEN)]
        public byte[] Message;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = STRING_LEN)]
        public byte[] Level;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = STRING_LEN)]
        public byte[] MethodName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = STRING_LEN)]
        public byte[] ClassName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = STRING_LEN)]
        public byte[] LineNumber;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = STRING_LEN)]
        public byte[] timeStamp;
    }
}






