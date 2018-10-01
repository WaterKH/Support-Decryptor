using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace KHUxDecrypt
{
    class Utilities
    {
        public static byte[] DecompressBytes(byte[] bytesToDecompress, int decompressedSize)
        {
            byte[] decompressedBytes = new byte[decompressedSize];

            var compressionLevel = CompressionLevel.Default;
            if (bytesToDecompress[0] == 0x78)
            {
                if (bytesToDecompress[1] == 0x01)
                {
                    // No Compression/ Low
                    // 1
                    compressionLevel = CompressionLevel.Level1;
                }
                else if (bytesToDecompress[1] == 0x5E)
                {
                    // Low
                    // 2 - 5
                    compressionLevel = CompressionLevel.Level2;
                }
                else if (bytesToDecompress[1] == 0x9C)
                {
                    // Default
                    // 6
                    compressionLevel = CompressionLevel.Level6;
                }
                else if (bytesToDecompress[1] == 0xDA)
                {
                    // Best
                    // 7 - 9
                    compressionLevel = CompressionLevel.Level9;
                }
            }
            try
            {
                using (var stream = new MemoryStream(bytesToDecompress))
                {
                    using (var decompressor = new ZlibStream(stream, CompressionMode.Decompress, compressionLevel))
                    {
                        if (decompressedSize != 0)
                        {
                            decompressor.Read(decompressedBytes, 0, decompressedSize);
                        }
                        else
                        {
                            using (var outputStream = new MemoryStream())
                            {
                                decompressor.CopyTo(outputStream);
                                decompressedBytes = outputStream.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                decompressedBytes = bytesToDecompress;
            }

            return decompressedBytes;
        }


        //https://stackoverflow.com/questions/1389570/c-sharp-byte-array-comparison
        public static bool ByteArrayEquals(byte[] b1, byte[] b2)
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

        public static int ByteToInt(byte[] data)
        {
            int result = 0;

            for (int i = 0; i < data.Length; ++i)
            {
                result += data[i] * (int)(Math.Pow(256, i));
            }
            return result;
        }

        public static short ByteToShort(byte[] data)
        {
            short result = 0;

            for (int i = 0; i < data.Length; ++i)
            {
                result += (short)(data[i] * Math.Pow(256, i));
            }
            return result;
        }

        public static bool IsHexDigit(char c)
        {
            if (char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                return true;

            return false;
        }

        //https://stackoverflow.com/questions/5613279/c-sharp-hex-to-ascii
        public static string HandleLWF(string fullName)
        {
            StringBuilder sb = new StringBuilder();

            var fileName = Path.GetFileName(fullName);
            var path = Path.GetDirectoryName(fullName);

            var splitFileName = fileName.Split('_');

            if (splitFileName.Length == 1)
                return fullName;

            var hexString = splitFileName[splitFileName.Length - 1];
            hexString = hexString.Remove(hexString.Length - 4);

            if (hexString.Length <= 4 || hexString.Length % 2 != 0)
                return fullName;

            sb.Append(path + "\\");

            for (int i = 0; i < splitFileName.Length - 1; ++i)
            {
                sb.Append(splitFileName[i] + "_");
            }

            for (int i = 0; i < hexString.Length; i += 2)
            {
                if (i + 1 < hexString.Length)
                {
                    string hs = hexString.Substring(i, 2);
                    if (Utilities.IsHexDigit(hs[0]) && Utilities.IsHexDigit(hs[1]))
                    {
                        sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
                    }
                    else
                    {
                        return fullName;
                    }
                }
                else
                {
                    sb.Append(hexString.Substring(i));
                }
            }

            sb.Append(fileName.Substring(fileName.Length - 4));

            return sb.ToString();
        }

        public static bool SignatureMatch(byte[] data)
        {
            if (data.Length < 4)
                return false;

            var temp = data.ToList().GetRange(0, 4);

            if (temp[0] == 0x89 && temp[1] == 0x42 && temp[2] == 0x54 && temp[3] == 0x46)
            {
                return true;
            }

            return false;
        }
    }
}
