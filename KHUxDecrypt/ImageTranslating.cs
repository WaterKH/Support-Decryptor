using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportDecrypt
{
    class ImageTranslating
    {
        public void TranslateImage(string file)
        {
            using (var reader = new BinaryReader(new FileStream(file, FileMode.Open)))
            {
                var BTF = new BTF()
                {
                    Signature = reader.ReadBytes(4),
                    Unk1 = reader.ReadBytes(12),
                    HeaderSize = Utilities.ByteToShort(reader.ReadBytes(2)),
                    Unk2 = Utilities.ByteToInt(reader.ReadBytes(4)),
                    Unk3 = Utilities.ByteToShort(reader.ReadBytes(2)),
                    Unk4 = Utilities.ByteToShort(reader.ReadBytes(2)),
                    Unk5 = Utilities.ByteToInt(reader.ReadBytes(4)),
                    Width = reader.ReadBytes(2),
                    Height = reader.ReadBytes(2),
                    DataSize = Utilities.ByteToInt(reader.ReadBytes(4))
                };

                
                BTF.Data = reader.ReadBytes(BTF.DataSize);
                
                BTF.Data = Utilities.DecompressBytes(BTF.Data, 0);
                
                byte[] colorChunk = new byte[4];
                byte[] reversedData = BTF.Data;
                var runningData = new List<byte>();

                // Initially reverse it to get the flipped image
                reversedData.Reverse();

                for (int i = 0; i < reversedData.Length; i += 4)
                {
                    // Assume 32 bit?
                    colorChunk = reversedData.ToList().GetRange(i, 4).ToArray();
                    
                    // Change the red and blue channels here
                    var t = colorChunk[0];
                    colorChunk[0] = colorChunk[2];
                    colorChunk[2] = t;

                    // Add to our data, but reverse it so that when we reverse the entire thing again, we'll be in the correct color channels as well as right ways up
                    runningData.AddRange(colorChunk.Reverse());
            
                }

                // Final flip
                runningData.Reverse();
                using (var stream = new FileStream("test.bmp", FileMode.OpenOrCreate))
                {
                    var bmpTemplate = BMP.Template((int)BitConverter.ToInt16(BTF.Width, 0), (int)BitConverter.ToInt16(BTF.Height, 0));

                    stream.Write(bmpTemplate, 0, bmpTemplate.Length);
                    stream.Write(runningData.ToArray(), 0, runningData.Count);
                }
            }
        }
    }
}
