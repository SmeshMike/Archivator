using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivating
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            StringAnalizis sa = new StringAnalizis("aaaddbbc");
            sa.CountLetters();
            foreach (var a in sa.SortedLetterCount)
                Console.WriteLine(a);



            Console.ReadLine();
        }
    }
}
