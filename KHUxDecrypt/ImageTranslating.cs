using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KHUxDecrypt
{
    class ImageTranslating
    {
        public byte[] TranslateImage(byte[] data)
        {
            using (var reader = new BinaryReader(new MemoryStream(data)))
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

                try
                {
                    BTF.Data = Utilities.DecompressBytes(BTF.Data, 0);

                    var rowSize = (int) BMP.GetRowSize(Utilities.ByteToInt(BTF.Width));
                    var pixelArraySize = rowSize * Math.Abs(Utilities.ByteToInt(BTF.Height));

                    byte[] colorChunk = new byte[4];
                    var imageData = new List<byte>();

                    for (int i = (BTF.Data.Length - 1) - rowSize; i >= 0; i -= rowSize)
                    {
                        var temp = BTF.Data.ToList().GetRange(i, rowSize);

                        for (int j = 0; j < temp.Count; j += 4)
                        {
                            imageData.AddRange(new byte[] {temp[j + 2], temp[j + 1], temp[j], temp[j + 3]});
                        }
                    }

                    var returnImageBMP = BMP.Template(Utilities.ByteToInt(BTF.Width), Utilities.ByteToInt(BTF.Height))
                        .ToList();
                    returnImageBMP.AddRange(imageData);

                    return returnImageBMP.ToArray();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Warning: No Compression used here... For future use. Probably not BMP Format.");

                    var returnImageBMP = BMP.Template(Utilities.ByteToInt(BTF.Width), Utilities.ByteToInt(BTF.Height))
                        .ToList();
                    returnImageBMP.AddRange(BTF.Data);

                    return returnImageBMP.ToArray();
                }
            }
        }
    }
}
