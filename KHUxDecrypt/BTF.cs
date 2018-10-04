using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KHUxDecrypt
{
    class BTF
    {
        public byte[] Signature = new byte[] { 0x89, 0x42, 0x54, 0x46 };
        public byte[] Unk1 = new byte[] { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public short HeaderSize { get; set; }
        public int Unk2 { get; set; }
        public byte[] HorizontalRatio { get; set; }
        public byte[] VerticalRatio { get; set; }
        public int Unk5 { get; set; }
        public byte[] Width { get; set; }
        public byte[] Height { get; set; }
        public int DataSize { get; set; }
        //public List<byte> Data = new List<byte>();
        public byte[] Data;
    }
}
