using System;
using System.Collections.Generic;
using System.Linq;

namespace ReliableConnectionLib
{
    public class Packet
    {
        public byte[] FromAddress { get; set; }
        public byte[] ToAddress { get; set; }
        public byte[] Data { get; set; }
        public byte Type1 { get; set; }
        public byte Type2 { get; set; }
        public int Index { get; set; }

        public static Packet Parse(byte[] data)
        {
            if (data.Length < 10)
            {
                return null;
            }

            Packet packet = new Packet();
            packet.FromAddress = data.Skip(0).Take(4).ToArray();
            packet.ToAddress = data.Skip(4).Take(4).ToArray();
            packet.Type1 = data.Skip(8).Take(1).First();
            packet.Type2 = data.Skip(9).Take(1).First();
            packet.Index = BitConverter.ToInt32(data.Skip(10).Take(4).ToArray(), 0);
            packet.Data = data.Skip(14).ToArray();

            return packet;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(this.FromAddress);
            bytes.AddRange(this.ToAddress);
            bytes.Add(this.Type1);
            bytes.Add(this.Type2);
            bytes.AddRange(BitConverter.GetBytes(this.Index));
            bytes.AddRange(this.Data);

            return bytes.ToArray();
        }
    }
}