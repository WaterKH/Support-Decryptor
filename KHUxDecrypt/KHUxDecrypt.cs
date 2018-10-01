using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KHUxDecrypt
{
    class KHUxDecrypt
    {
        public byte[] Decrypt(byte[] data, int length, int key)
        {   
            for (int i = 0; i < data.Length; i += 4)
            {
                key = KhuxRandom(key);
                var tempKey = BitConverter.GetBytes(key);
                for (int j = 0; j < tempKey.Length; ++j)
                {
                    if (i + j == data.Length)
                        break;
                    data[i + j] ^= (byte) tempKey[j];
                }
            }

            return data;
        }

        public int KhuxRandom(int seed)
        {
            return 0x19660D * seed + 0x3C6EF35F; // Where do **these** values come from??
        }
    }
}
