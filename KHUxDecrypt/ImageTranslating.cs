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
            using(var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
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
                    //DataSize = Utilities.ByteToInt(reader.ReadBytes(4))
                };
                Console.WriteLine(BTF.HeaderSize);
                if (BTF.HeaderSize != 8)
                {
                    var Unk6 = reader.ReadBytes(2);
                }

                BTF.DataSize = Utilities.ByteToInt(reader.ReadBytes(4));

                var returnImageBMP = BMP.Template(Utilities.ByteToInt(BTF.Width), Utilities.ByteToInt(BTF.Height))
                    .ToList();
                var rowSize = (int)BMP.GetRowSize(Utilities.ByteToInt(BTF.Width));
                var pixelArraySize = rowSize * Math.Abs(Utilities.ByteToInt(BTF.Height));

                if (BTF.DataSize < 0)
                    BTF.Data = reader.ReadBytes(pixelArraySize * 8);
                else
                    BTF.Data = reader.ReadBytes(BTF.DataSize);

                BTF.Data = Utilities.DecompressBytes(BTF.Data, 0);

                if (rowSize == 0)
                    return data;

                try
                {
                    for (int i = BTF.Data.Length - rowSize; i >= 0; i -= rowSize)
                    {
                        var tempData = BTF.Data.ToList().GetRange(i, rowSize);

                        var imageData = new List<byte>();

                        //for (int j = tempData.Length - rowSize; j >= 0; j -= rowSize)
                        //{
                        //    var temp = BTF.Data.ToList().GetRange(j, rowSize);
                            for (int k = 0; k < tempData.Count; k += 4)
                            {
                                imageData.AddRange(new byte[] {tempData[k + 2], tempData[k + 1], tempData[k], tempData[k + 3] });
                            }
                        //}
                        returnImageBMP.AddRange(imageData);
                    }
                    return returnImageBMP.ToArray();
                }
                catch (Exception e)
                {
                    throw;
                    //returnImageBMP.AddRange(BTF.Data);

                    //return returnImageBMP.ToArray();
                }
            }
        }
    }
}
