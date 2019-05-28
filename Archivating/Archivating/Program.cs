using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivating
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StringAnalizis sa = new StringAnalizis();
            sa.CountLetters();
            foreach (var a in sa.Encrypt())
                Console.WriteLine(a.Key + "  " + a.Value);

            Console.ReadLine();
        }
    }
}
