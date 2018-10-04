using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace KHUxDecrypt
{
    class KHUxDecompile
    {
        KHUxDecrypt khuxDecrypt = new KHUxDecrypt();
        private int imageCounter = 0;
        private ImageTranslating imageTranslator = new ImageTranslating();

        public void Decompile(string file, string output)
        {
            var compareSignature = new byte[] {0x42, 0x47, 0x41, 0x44};

            using (var reader = new BinaryReader(new FileStream(file, FileMode.Open)))
            {
                byte[] signature = new byte[4];

                while ((reader.Read(signature, 0, 4)) > 0)
                {
                    if (Utilities.ByteArrayEquals(signature, compareSignature))
                    {
                        //Array.Reverse(signature); // I believe it is supposed to be read DAGB
                        var bgad = new BGAD()
                        {
                            Signature = Encoding.UTF8.GetString(signature),
                            KeyType = Utilities.ByteToShort(reader.ReadBytes(2)),
                            Unk = Utilities.ByteToShort(reader.ReadBytes(2)),
                            Header = new BGADHeader()
                            {
                                HeaderSize = Utilities.ByteToShort(reader.ReadBytes(2)),
                                NameSize = Utilities.ByteToShort(reader.ReadBytes(2)),
                                DataType = Utilities.ByteToShort(reader.ReadBytes(2)),
                                IsCompressed = Utilities.ByteToShort(reader.ReadBytes(2)),
                                DataSize = Utilities.ByteToInt(reader.ReadBytes(4)),
                                DecompressedSize = Utilities.ByteToInt(reader.ReadBytes(4))
                            }
                        };

                        bgad.Name = reader.ReadBytes(bgad.Header.NameSize);
                        var preDecryption = new List<byte>();
                        foreach (var k in bgad.Name)
                            preDecryption.Add(k);

                        var decryptedName =
                            khuxDecrypt.Decrypt(bgad.Name, bgad.Header.NameSize, bgad.Header.DataSize);
                        
                        bgad.Data = reader.ReadBytes(bgad.Header.DataSize);

                        if (File.Exists(output + "\\" + Encoding.ASCII.GetString(decryptedName)))
                            continue;

                        var decryptedData =
                            khuxDecrypt.Decrypt(bgad.Data, bgad.Header.DataSize, bgad.Header.NameSize);

                        if (!Encoding.ASCII.GetString(decryptedName).Contains("audio"))
                        {
                            if (bgad.Header.IsCompressed != 0)
                            {
                                decryptedData = Utilities.DecompressBytes(decryptedData, bgad.Header.DecompressedSize);
                            }
                        }

                        var khFile = new KHUxFile()
                        {
                            PreDecryptedFileName = preDecryption.ToArray(),
                            FileName = decryptedName,
                            Data = decryptedData
                        };

                        WriteToFile(khFile, output);
                    }
                }
            }
        }

        public void WriteToFile(KHUxFile file, string output)
        {
            try
            {
                var fullName = output + "\\" + Encoding.ASCII.GetString(file.FileName);
                
                if (fullName.Contains("lwf/"))
                {
                    fullName = Utilities.HandleLWF(fullName);
                }
                
                if (!File.Exists(fullName))
                {
                    if (fullName.Substring(fullName.Length - 4, 4).Equals(".png") || Utilities.SignatureMatch(file.Data))
                    {
                        file.Data = imageTranslator.TranslateImage(file.Data).ToArray();
                    }
                
                    var path = Path.GetDirectoryName(fullName);
                    Directory.CreateDirectory(path);
                    File.WriteAllBytes(fullName, file.Data);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine();
                //Console.WriteLine(e.Message);
                var path = output + "\\InvalidFileNames\\";
                var fileNameToWrite = "File_" + imageCounter + "Name.txt";

                Directory.CreateDirectory(path);

                var fileName = new List<byte>();
                fileName.AddRange(file.PreDecryptedFileName);
                fileName.AddRange(new byte[] { 0x5c, 0x6e, 0x5c, 0x72, 0x5c, 0x6e, 0x5c, 0x72 });
                fileName.AddRange(file.FileName);
                fileName.AddRange(new byte[] { 0x5c, 0x6e, 0x5c, 0x72, 0x5c, 0x6e, 0x5c, 0x72});

                var fileData = new List<byte>();
                fileData.AddRange(file.Data);

                if (!File.Exists(path + fileNameToWrite))
                {
                    File.WriteAllBytes(path + fileNameToWrite, fileName.ToArray());
                }

                var fileDataToWrite = "File_" + imageCounter + "Data";

                if (file.Data.Length > 8)
                {
                    if (Utilities.ByteArrayEquals(fileData.GetRange(0, 2).ToArray(), new byte[] {0x42, 0x4D}))
                    {
                        fileDataToWrite += ".png";
                    }
                    else if (Utilities.ByteArrayEquals(fileData.GetRange(0, 3).ToArray(),
                        new byte[] {0x41, 0x4B, 0x42}))
                    {
                        fileDataToWrite += ".akb";
                    }
                    else if (Utilities.ByteArrayEquals(fileData.GetRange(0, 3).ToArray(),
                        new byte[] {0x4C, 0x57, 0x46}))
                    {
                        fileDataToWrite += ".lwf";
                    }
                    else if (fileData[0] == 0x7B)
                    {
                        fileDataToWrite += ".json";
                    }
                    else if (Utilities.ByteArrayEquals(fileData.GetRange(0, 8).ToArray(),
                        new byte[] { 0xEF, 0xBB, 0xBF, 0x3C, 0x3F, 0x78, 0x6D, 0x6C }))
                    {
                        fileDataToWrite += ".plist";
                    }
                    else
                    {
                        fileDataToWrite += ".txt";
                    }
                }

                if (!File.Exists(path + fileDataToWrite))
                {
                    File.WriteAllBytes(path + fileDataToWrite, fileData.ToArray());
                }

                ++imageCounter;
            }
        }
    }
}
