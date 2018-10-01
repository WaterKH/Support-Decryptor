using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KHUxDecrypt
{
    class BMP
    {
        public static string Signature = "BM";
        public static int SizeOfFile = 0;
        public static int ReservedBytes = 0;
        public static int StartAddress = 0x36;
        public static int HeaderSize = 0x28;
        public static int Width = 0;
        public static int Height = 0;
        public static short ColorPlanes = 0x01;
        public static short BitsPerPixel = 0x20;
        public static int Compression = 0;
        public static int ImageSize = 0;
        public static int HorizontalResolution = 0;
        public static int VerticalResolution = 0;
        public static int ColorPalette = 0;
        public static int ImportantColor = 0;
        
        public static byte[] Template(int width, int height)
        {
            //var bytes = new byte[StartAddress - 1];

            var bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes(Signature));
            bytes.AddRange(BitConverter.GetBytes(SizeOfFile));
            bytes.AddRange(BitConverter.GetBytes(ReservedBytes));
            bytes.AddRange(BitConverter.GetBytes(StartAddress));
            bytes.AddRange(BitConverter.GetBytes(HeaderSize));
            bytes.AddRange(BitConverter.GetBytes(width));
            bytes.AddRange(BitConverter.GetBytes(height));
            bytes.AddRange(BitConverter.GetBytes(ColorPlanes));
            bytes.AddRange(BitConverter.GetBytes(BitsPerPixel));
            bytes.AddRange(BitConverter.GetBytes(Compression));
            bytes.AddRange(BitConverter.GetBytes(ImageSize));
            bytes.AddRange(BitConverter.GetBytes(HorizontalResolution));
            bytes.AddRange(BitConverter.GetBytes(VerticalResolution));
            bytes.AddRange(BitConverter.GetBytes(ColorPalette));
            bytes.AddRange(BitConverter.GetBytes(ImportantColor));

            return bytes.ToArray();
        }

        public static decimal GetRowSize(int width)
        {
            decimal rowSize = Math.Floor((decimal)(BitsPerPixel * width + 31) / 32) * 4;

            return rowSize;
        }
    }
}
