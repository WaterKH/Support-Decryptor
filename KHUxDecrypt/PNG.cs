using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KHUxDecrypt
{
    class PNG
    {
        #region PNG
        public byte[] Signature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        // Always 13 bytes
        #region IHDR Chunk
        public int Width { get; set; } // 4bytes
        public int Height { get; set; } // 4bytes
        public short BitDepth { get; set; } // 1byte
        public short ColorType { get; set; } // 1byte
        public short CompressionMethod { get; set; } // 1byte
        public short FilterMethod { get; set; } // 1byte
        public short InterlaceMethod { get; set; } // 1byte
        #endregion
        
        public Chunk[] chunks { get; set; }
        public int NumberOfChunks { get; set; }
        #endregion
    }
}
