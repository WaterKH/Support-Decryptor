using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace SupportDecrypt
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            string pngFile = "Medal_L_1012.png";
            ImageTranslating imageTranslator = new ImageTranslating();
            imageTranslator.TranslateImage(pngFile);
            Console.WriteLine("test");
            Console.Read();

            string file = "misc.mp4";
            string output = "KHUx";
            //string file = args[0];
            //string output = args[1];
            var bgadChunks = new List<BGAD>();
            var bgadDecompiler = new BGADDecompiler();

            //if (File.Exists("InvalidNames.txt"))
            //{
            //    File.Delete("InvalidNames.txt");
            //}

            var processor = new ProcessingThread<SupportFile>(x =>
            {
                try
                {
                    var fullName = output + "\\" + Encoding.ASCII.GetString(x.FileName);
                    if (fullName.Contains("lwf/"))
                    {
                        fullName = HandleLWF(fullName);
                    }
                    var path = Path.GetDirectoryName(fullName);
                    Directory.CreateDirectory( path);
                    if (!File.Exists(fullName))
                    {
                        File.WriteAllBytes(fullName, x.Data);
                    }
                }
                catch (Exception e)
                {
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

                    Console.WriteLine();
                    Console.WriteLine(e.Message);
                    throw new Exception(e.Message);
                }
                
            });

            bgadDecompiler.Decompile(file, processor);

            Console.WriteLine("Decompile Complete... Waiting on File Writes");

            processor.StopWhenEmpty();

            Console.WriteLine("Finished Writing... Program Complete");
            Console.WriteLine("Hit any key to exit program");
            Console.Read();
            
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

        
    }
}
