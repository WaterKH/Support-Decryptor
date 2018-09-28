using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportDecrypt
{
    class BGAD
    {
        public string Signature;
        public short KeyType;
        public short Unk;
        public BGADHeader Header = new BGADHeader();
        public byte[] Name;
        public byte[] Data;
    }
}
