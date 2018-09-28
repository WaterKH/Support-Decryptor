using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace SupportDecrypt
{
    class DecompilePNG
    {
        public int IHDRChunkLength = 13;
        public bool Decompile(string file)
        {
            Console.WriteLine("Starting Decompile...");
            PNG png = new PNG();
            using (var reader = new BinaryReader(new FileStream(file, FileMode.Open)))
            {
                var beginning = reader.ReadBytes(png.Signature.Length);
                if (!ByteArrayEquals(beginning, png.Signature))
                {
                    return false;
                }

                Console.WriteLine("Populate Chunk");
                var ihdrChunk = PopulateChunk(reader);

                png = ParseIHDR(ihdrChunk);

                var dataChunk = PopulateChunk(reader);

                List<byte[]> runningDataChunks = new List<byte[]>
                {
                    dataChunk.Data
                };
                
                do
                {
                    dataChunk = PopulateChunk(reader);
                    runningDataChunks.Add(dataChunk.Data);

                } while (dataChunk.ChunkType != "IEND");
               

                // TODO check first 2 bytes of first data chunk to see compression used
                if (runningDataChunks[0][0] == 0x78 && runningDataChunks[0][1] == 0x9C)
                {
                    Console.WriteLine("Compression is Default");
                }

                Console.WriteLine("Finished");
            }
            return true;
        }

        public Chunk PopulateChunk(BinaryReader reader)
        {
            var chunk = new Chunk
            {
                Length = this.uInt32ToInt(reader.ReadBytes(4)),
                ChunkType = Encoding.ASCII.GetString(reader.ReadBytes(4))
            };

            chunk.Data = reader.ReadBytes(chunk.Length);
            chunk.CyclicRedundancyCheck32 = reader.ReadBytes(4);

            return chunk;
        }
        
        public PNG ParseIHDR(Chunk ihdrChunk)
        {
            var png = new PNG();

            if (ihdrChunk.Length != IHDRChunkLength)
            {
                return null;
            }

            var tempData = ihdrChunk.Data;

            png.Width = uInt32ToInt(tempData.ToList().GetRange(0, 4).ToArray());
            if (png.Width <= 0)
            {
                return null;
            }
            
            png.Height = uInt32ToInt(tempData.ToList().GetRange(4, 4).ToArray());
            if (png.Height <= 0)
            {
                return null;
            }

            png.BitDepth = tempData[8];
            png.ColorType = tempData[9];
            png.CompressionMethod = tempData[10];
            png.FilterMethod = tempData[11];
            png.InterlaceMethod = tempData[12];

            return png;
        }

        public byte[] CompressBytes(byte[] bytesToCompress)
        {
            byte[] compressedBytes;
            using (var stream = new MemoryStream())
            {
                using (var compressor = new ZlibStream(stream, CompressionMode.Compress, CompressionLevel.Default))
                {
                    compressor.Write(bytesToCompress, 0, bytesToCompress.Length);
                }

                compressedBytes = stream.ToArray();
            }

            return compressedBytes;
        }

        public byte[] DecompressBytes(byte[] bytesToDecompress)
        {
            byte[] decompressedBytes;
            using (var stream = new MemoryStream())
            {
                using (var compressor = new ZlibStream(stream, CompressionMode.Decompress, CompressionLevel.Default))
                {
                    compressor.Write(bytesToDecompress, 0, bytesToDecompress.Length);
                }

                decompressedBytes = stream.ToArray();
            }

            return decompressedBytes;
        }

        //https://stackoverflow.com/questions/1389570/c-sharp-byte-array-comparison
        public bool ByteArrayEquals(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null)
            {
                Console.WriteLine("Byte array null");
                return false;
            }

            if (b1.Length != b2.Length)
            {
                Console.WriteLine("Lengths do not match");
                return false;
            }
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    Console.WriteLine(b1[i] + " " + b2[i]);
                    return false;
                }
            }
            return true;
        }

        public int uInt32ToInt(byte[] data)
        {
            //Array.Reverse(data);
            return data[3] +
                   data[2] * 256 +
                   data[1] * 256 * 256 +
                   data[0] * 256 * 256 * 256;
        }
    }
}
