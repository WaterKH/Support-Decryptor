using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KHUxDecrypt
{
    class BGADHeader
    {
        public short HeaderSize;
        public short NameSize;
        public short DataType;
        public short IsCompressed;
        public int DataSize;
        public int DecompressedSize;
    }
}
