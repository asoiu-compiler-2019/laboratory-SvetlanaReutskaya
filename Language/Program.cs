using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    class Program
    {
        static void Main(string[] args)
        {
            Interpretor parser = new Interpretor();
            parser.Run("../../../Language/MyCode.txt");


            Console.ReadLine();
        }
    }
}
