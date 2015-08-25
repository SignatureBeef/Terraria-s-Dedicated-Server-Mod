using Microsoft.Xna.Framework;
using System;
using System.Text;
using tdsm.api;
using tdsm.core.Logging;
using tdsm.core.ServerCore;

namespace tdsm.core.Messages
{
    public abstract class MessageHandler
    {
        public MessageHandler()
        {
            ValidStates = SlotState.PLAYING;
            if (utf8Encoding == null)
            {
                utf8Encoding = Encoding.GetEncoding("utf-8", new EncoderExceptionFallback(), new DecoderExceptionFallback());
            }
        }

        public SlotState ValidStates { get; protected set; }
        public SlotState IgnoredStates { get; protected set; }

        private static Encoding utf8Encoding;

        protected bool ParseString(byte[] strBuffer, int offset, int count, out string str)
        {
            try
            {
                str = utf8Encoding.GetString(strBuffer, offset, count).Trim();
                _readOffset += count;
            }
            catch (DecoderFallbackException)
            {
                str = null;
                return false;
            }
            return true;
        }

        public abstract Packet GetPacket();

        public abstract void Process(ClientConnection conn, byte[] readBuffer, int length, int pos);

        [ThreadStatic]
        private int _readOffset;
        public void Reset(int offset = 0)
        {
            _readOffset = offset;
        }

        public string ReadString(byte[] readBuffer)
        {
            var str = String.Empty;
            ReadString(readBuffer, out str);
            return str;
            //int pos = 0;
            //int length = 0;
            //while (true)
            //{
            //    length += readBuffer[pos + _readOffset] & 0x7F;
            //    if (readBuffer[pos++ + _readOffset] > 127)
            //        length <<= 7;
            //    else break;
            //}
            //var str = String.Empty;
            //ParseString(readBuffer, _readOffset + 1, length, out str);

            //_readOffset += 1;
            ////_readOffset += length + 1;

            //return str;
        }

        public bool ReadString(byte[] readBuffer, out string str)
        {
            str = null;

            int count = 0;
            int shift = 0;
            byte b;
            while (shift != 35)
            {
                b = readBuffer[_readOffset++];
                count |= (int)(b & 127) << shift;
                shift += 7;
                if ((b & 128) == 0)
                    return ParseString(readBuffer, _readOffset, count, out str);
            }

            return false;
        }

        public byte ReadByte(byte[] readBuffer)
        {
            return readBuffer[_readOffset++];
        }

        public bool ReadBoolean(byte[] readBuffer)
        {
            return readBuffer[_readOffset++] == 1;
        }

        public short ReadInt16(byte[] readBuffer)
        {
            var val = BitConverter.ToInt16(readBuffer, _readOffset);
            _readOffset += 2;
            return val;
        }

        public ushort ReadUInt16(byte[] readBuffer)
        {
            var val = BitConverter.ToUInt16(readBuffer, _readOffset);
            _readOffset += 2;
            return val;
        }

        public int ReadInt32(byte[] readBuffer)
        {
            var val = BitConverter.ToInt32(readBuffer, _readOffset);
            _readOffset += 4;
            return val;
        }

        public float ReadSingle(byte[] readBuffer)
        {
            var val = BitConverter.ToSingle(readBuffer, _readOffset);
            _readOffset += 4;
            return val;
        }

        public Vector2 ReadVector2(byte[] readBuffer)
        {
            return new Vector2(ReadSingle(readBuffer), ReadSingle(readBuffer));
        }

        public void Skip(int count)
        {
            _readOffset += count;
        }

        public Color ReadRGB(byte[] readBuffer)
        {
            return new Color(ReadByte(readBuffer), ReadByte(readBuffer), ReadByte(readBuffer));
        }
    }

    public abstract class SlotMessageHandler : MessageHandler
    {
        public abstract void Process(int whoAmI, byte[] readBuffer, int length, int num);

        public override void Process(ClientConnection conn, byte[] readBuffer, int length, int pos)
        {
            var slot = conn.SlotIndex;
            if (slot >= 0)
                Process(slot, readBuffer, length, pos);
            else
                ProgramLog.Log("Attempt to process packet {0} before slot assignment.", GetPacket());
        }
    }
}
