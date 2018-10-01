using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KHUxDecrypt
{
    class KHUxFile
    {
        public byte[] PreDecryptedFileName { get; set; }
        public byte[] FileName { get; set; }
        public byte[] Data { get; set; }
    }
}
