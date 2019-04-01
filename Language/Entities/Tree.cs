using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class Tree
    {
        public Tree ParentValue { get; set; }
        public List<Tree> ChildValues = new List<Tree>();
        public Lexemes Lexeme { get; set; }
        public string Value { get; set; }

        public void PrintPretty(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("└─");
                indent += "  ";
            }
            else
            {
                Console.Write("├─");
                indent += "| ";
            }
            Console.WriteLine(Value);

            for (int i = 0; i < ChildValues.Count; i++)
                ChildValues[i].PrintPretty(indent, i == ChildValues.Count - 1);
        }
    }
}
