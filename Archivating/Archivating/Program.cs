using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Archivating
{
    class Program
    {

        public static void StringProb(string s)
        {
            int length = s.Length;
            char[] chr = s.ToCharArray();
            int[] chr_num = new int[length];
            for (int i = 0; i < length; i++)
            {
                chr_num[i] = 1;
                for (int j = i + 1; j < length - 1; j++)
                {
                    while (chr[i] == chr[j])               
                    {
                        for (int k = j; k < length - j; k++)
                        {
                            chr[k - 1] = chr[k];
                        }
                        Array.Clear(chr, length - 1, 1);
                        length--;
                    }
                }
                
            }
            string s1 = new string(chr);
            string s2 = string.Join("", chr_num);
            Console.WriteLine(s1, "  ", s2);
            
        }


        static void Main(string[] args)
        {
            string Str = File.ReadAllText(@"C:\SomeTXT.txt");
            double Str_L = Str.Length;
            StringProb(Str);
            Console.ReadLine();
        }
    }

   

}

