using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;

namespace KHUxDecrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = "misc.mp4";
            string output = "KHUx";
            //string file = args[0];
            //string output = args[1];
            var bgadChunks = new List<BGAD>();
            var bgadDecompiler = new KHUxDecompile();
            ImageTranslating imageTranslator = new ImageTranslating();
            
            bgadDecompiler.Decompile(file, output);
            
            Console.WriteLine("Finished Decompiling... Program Complete");
            Console.WriteLine("Hit any key to exit program");
            Console.Read();
            
        }
    }
}
