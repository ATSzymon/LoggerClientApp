using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TcpClientApp
{
    class Frame_Comm
    {
        public class FOKEventArg : EventArgs
        {
            public readonly string TheString;

            public FOKEventArg(string s)
            {
                TheString = s;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct t_ramka_odb
        {
            public t_ramka_odb(ushort size_of_frame)
                : this()
            {
                frame_data = new byte[size_of_frame];
            }
            public ushort data_cnt;
            public ushort frame_length;
            public bool ctrl;
            public bool in_progress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = LEN)]
            public byte[] frame_data;
        }


        private const byte LEN_BYTE_COUNT = 2;

        private const int LEN = 8192;

        private const byte BCHAR_BEGIN_FRAME = 0xcd;

        private const byte BCHAR_CTRL = 0xce;

        private const byte BCHAR_CTRL_BEGIN = (byte)'1';

        private const byte BCHAR_CTRL_CNTRL = (byte)'2';


        public delegate void Frame_OK_Handler(object o, FOKEventArg e);

        public event Frame_OK_Handler Frame_OK;

        private FOKEventArg my_event = new FOKEventArg("Odebrałem poprawną ramkę");


        private t_ramka_odb ramka;

        private byte[] Buffer_out;

        private int buffer_out_count;


        public Frame_Comm()
        {
            ramka = new t_ramka_odb(LEN);

            Buffer_out = new byte[LEN];
        }

        private void Reset_Frame()
        {
            ramka.frame_length = 0;
            ramka.data_cnt = 0;
            ramka.ctrl = false;
            ramka.frame_data.Initialize();
        }

        public byte[] Get_Input_Frame()
        {
            return ramka.frame_data;
        }

        private bool AddFrameChar(byte value)
        {
            if (value == BCHAR_BEGIN_FRAME) // poczatek
            {
                Reset_Frame();
                ramka.in_progress = true;
                return false;
            }

            if (ramka.in_progress == false) // poza ramka
            {
                return false;
            }

            if (ramka.ctrl == true)
            {
                if (value == BCHAR_CTRL_BEGIN)
                {
                    value = BCHAR_BEGIN_FRAME;
                    ramka.ctrl = false;
                }
                else if (value == BCHAR_CTRL_CNTRL)
                {
                    value = BCHAR_CTRL;
                    ramka.ctrl = false;
                }
                else
                {
                    ramka.in_progress = false;
                    return false;
                }
            }
            else if (value == BCHAR_CTRL)
            {
                ramka.ctrl = true;
                return false;
            }

            if(ramka.data_cnt == 0) // LSB length
            {
                ramka.frame_length = value;
            }
            else if (ramka.data_cnt == 1) // MSB length
            {
                ramka.frame_length += (ushort)(((ushort)value) << 8);
            }
            else if (ramka.data_cnt < LEN)
            {
                ramka.frame_data[ramka.data_cnt - LEN_BYTE_COUNT] = value;
            }
            else
            {
                ramka.in_progress = false;
                return false;
            }

            if (ramka.data_cnt == ramka.frame_length)
            {
                ramka.in_progress = false;
                return true;
            }

            ramka.data_cnt++;
            return false;
        }

        public void Decode_data(byte[] data, int count)
        {
            int licznik = 0;

            while(licznik != count)
            {
                try
                {
                    if (AddFrameChar(data[licznik++]) == true)
                    {
                        OnFrame_OK((object)this, my_event);
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message, "my rs Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
        }

        private void OnFrame_OK(object o, FOKEventArg e)
        {
            if (Frame_OK != null)
                Frame_OK(o, e);
        }

        private void PutPortData(byte znak)
        {
            Buffer_out[buffer_out_count++] = znak;
        }

        private void fr_sendBinChar(byte value)
        {
            if (value == BCHAR_BEGIN_FRAME)
            {
                PutPortData(BCHAR_CTRL);
                PutPortData(BCHAR_CTRL_BEGIN); // znaczek '1'
            }
            else if (value == BCHAR_CTRL)
            {
                PutPortData(BCHAR_CTRL);
                PutPortData(BCHAR_CTRL_CNTRL); // znaczek '2'
            }
            else
                PutPortData(value);
        }

        public byte[] Encode_data(object obj, ref int out_length)
        {
            byte[] ptr_data;

            int raw_size;

            raw_size = Marshal.SizeOf(obj);

            ptr_data = new byte[raw_size];

            IntPtr ptr = Marshal.AllocHGlobal(raw_size);

            Marshal.StructureToPtr(obj, ptr, false);

            Marshal.Copy(ptr, ptr_data, 0, raw_size);

            Marshal.FreeHGlobal(ptr);

            if (raw_size > LEN)
            {
                return null;
            }

            buffer_out_count = 0;

            PutPortData(BCHAR_BEGIN_FRAME);

            byte[] size = BitConverter.GetBytes((ushort)raw_size);

            fr_sendBinChar(size[0]);
            fr_sendBinChar(size[1]);

            for (int i = 0; i < raw_size; i++)
            {
                fr_sendBinChar(ptr_data[i]);
            }

            out_length = buffer_out_count;

            return Buffer_out;
        }
    }
}
