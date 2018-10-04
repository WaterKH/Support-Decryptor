using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

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
            var bgadDecompiler = new KHUxDecompile();
            
            bgadDecompiler.Decompile(file, output);
            
            Console.WriteLine("Finished Decompiling... Program Complete");
            Console.WriteLine("Hit any key to exit program");
            Console.Read();
            //var t = 1529206523;
            //var s = 2119926748;
            //var u = 1855304407;

            //var mod = 2147483647;
            ////var a = 1;
            ////var c = 590720225;

            //var counter = new int[] { -1, -1 };
            //var found = false;

            //for (int a = 1; a < mod; ++a)
            //{
            //    var c = (s - (t * a)) % mod;

            //        var x = (s * a + c) % mod;
            //        if(x == u)
            //        {
            //            Console.WriteLine("RESULTS: " + a + " " + c);

            //        }

            //}


            //Console.WriteLine("FINISHED");
            //Console.Read();
        }
    }
}
