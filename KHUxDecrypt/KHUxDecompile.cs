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

        public List<BGAD> Decompile(string file, string output)
        {
            var bgadChunks = new List<BGAD>();
            var noCompression = new byte[] {0x00, 0x00};
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
                        var decryptedData =
                            khuxDecrypt.Decrypt(bgad.Data, bgad.Header.DataSize, bgad.Header.NameSize);

                        if (bgad.Header.IsCompressed != 0)
                        {
                            decryptedData = Utilities.DecompressBytes(decryptedData, bgad.Header.DecompressedSize);
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

            return bgadChunks;
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
                else if (fullName.Substring(fullName.Length - 4, 4).Equals(".png") || Utilities.SignatureMatch(file.Data))
                {
                    file.Data = imageTranslator.TranslateImage(file.Data);
                }

                var path = Path.GetDirectoryName(fullName);
                Directory.CreateDirectory(path);

                if (!File.Exists(fullName))
                {
                    File.WriteAllBytes(fullName, file.Data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
                //using (var writer = new StreamWriter("InvalidNames.txt", true))
                //{
                //    writer.WriteLine("Decrypted");
                //    foreach (var f in x.FileName)
                //    {
                //        writer.Write(f + " ");
                //    }
                //    writer.WriteLine();
                //    writer.WriteLine("Before Decryption");
                //    foreach (var f in x.PreDecryptedFileName)
                //    {
                //        writer.Write(f + " ");
                //    }
                //    writer.WriteLine();
                //}
                var path = output + "\\InvalidFileNames\\";
                var fileToWrite = "File_" + imageCounter + ".txt";
                Directory.CreateDirectory(path);

                if (!File.Exists(path + fileToWrite))
                {
                    File.WriteAllBytes(path + fileToWrite, file.Data);
                }

                ++imageCounter;
            }
        }
    }
}
