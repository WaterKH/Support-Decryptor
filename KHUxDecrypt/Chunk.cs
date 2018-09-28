using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportDecrypt
{
    class Chunk
    {
        #region Header Lengths

        public int Length { get; set; } // uint32
        public string ChunkType { get; set; } // 4byte name
        public byte[] Data { get; set; } // Length filled with data
        public byte[] CyclicRedundancyCheck32 { get; set; } // 4byte
        #endregion
        
    }
}
